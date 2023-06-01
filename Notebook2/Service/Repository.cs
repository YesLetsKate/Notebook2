using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Notebook2.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Notebook2.Service
{
    public class Repository
    {
        string connectionstring =
            "Server=LAPTOP-VRJCKSUB;User ID = sa;Password = AutodeskVault@26200; " +
            "Initial Catalog=Notebook;Persist Security Info=true; " +
            "Trusted_Connection=True; ";

        public static string CreateSHA256(string password)
        {
            using (SHA256 sha1 = SHA256.Create())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sha_password = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sha_password.Append(b.ToString("x2"));
                }
                return sha_password.ToString();
            }
        }
        private int CheckLogin(string login)
        {
            //Проверка логина
            int usersCount;
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                string query =
                    string.Format("select iduser from [passwords] where [login]='{0}'", login);

                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    usersCount = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
                catch (Exception ex)
                {
                    usersCount = 500;
                    return usersCount;
                }
                finally { conn.Close(); }
            }
            return usersCount;
        }

        public async Task<ResponseModel> LoginData(LoginViewModel user)
        {
            ResponseModel response = new ResponseModel();

            if (user != null)
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    //Проверка логина
                    int usercount = CheckLogin(user.Login);
                    switch (usercount)
                    {
                        case 0:
                            response.resultCode = 100;
                            response.message = "Не правильный логин или пароль";
                            return response;
                        case > 0:
                            response.resultCode = 100;
                            response.message = "Логин свободен";
                            break;
                    }

                    //Получение соли
                    string query = string.Format("select salt from [passwords] where login='{0}'", user.Login);
                    SqlCommand cmd = new SqlCommand(query, conn);
                    string salt;
                    try
                    {
                        conn.Open();
                        salt = cmd.ExecuteScalar().ToString();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        response.message = "Ошибка какая-то";
                        response.resultCode = 500;
                        return response;
                    }
                    finally { conn.Close(); }

                    string hash_password = CreateSHA256(user.Password + salt);


                    query = string.Format("Select * FROM [passwords],[users] " +
                        "WHERE [password] = '{0}' " +
                        "and [login]='{1}' and [users].iduser = [passwords].iduser",
                        hash_password, user.Login);
                    cmd = new SqlCommand(query, conn);

                    try
                    {
                        conn.Open();
                        await cmd.ExecuteNonQueryAsync();

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        await Task.Run(() => da.Fill(dt));
                        conn.Close();

                        if (dt.Rows.Count > 0)
                        {
                            response.Data = JsonConvert.SerializeObject(dt);
                            response.message = "Вход успешно произдведен";
                            response.resultCode = 800;
                        }
                        else
                        {
                            response.message = "Не правильный логин или пароль";
                            response.resultCode = 101;
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.message = "Ошибка какая-то";
                        response.resultCode = 500;
                        return response;
                    }
                    finally { conn.Close(); }
                }
            }
            return response;
        }

        public static string CreateSalt()
        {
            string alf = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN" +
                "OPQRSTUVWXYZ1234567890~!@\"#№$%^?&*_<>+-";
            string salt = "";
            Random rand = new Random();
            int n = 13;
            for (int i = 0; i < n; i++)
            {
                salt = salt + alf[rand.Next(alf.Length)];
            }
            return salt;
        }

        public async Task<ResponseModel> RegisterData(RegisterViewModel user)
        {
            ResponseModel response = new ResponseModel();
            if (user != null)
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    //Проверка логина
                    int usersCount = CheckLogin(user.Login);
                    if (usersCount > 1)
                    {
                        response.resultCode = 101;
                        response.message = "Логин занят";
                        return response;
                    }
                    else
                    {
                        response.resultCode = 100;
                        response.message = "Логин свободен";
                    }

                    //Вставка данных пользователя
                    string query = string.Format("Insert INTO [users]([name],[surname]) " +
                        "VALUES('{0}','{1}')", user.Name, user.Surname);
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int iduser;
                    try
                    {
                        conn.Open();
                        var result = await cmd.ExecuteNonQueryAsync();

                        if (result == 1) //row changes in the database - successfull
                        {
                            response.message = "Регистор Ок!";
                            response.resultCode = 200;
                        }
                        else
                        {
                            response.message = "Ошибка какая-то";
                            response.resultCode = 500;
                            return response;
                        }

                        //Получение iduser
                        query = string.Format("SELECT SCOPE_IDENTITY()");
                        cmd = new SqlCommand(query, conn);

                        try
                        {
                            //cmd.ExecuteNonQuery();
                            iduser = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                        }
                        catch (Exception ex)
                        {
                            response.resultCode = 500;
                            response.message = "Какая-то ошибка";
                            return response;
                        }
                        finally { conn.Close(); }
                    }
                    catch (Exception ex)
                    {
                        response.resultCode = 500;
                        response.message = "Какая-то ошибка";
                        conn.Close();
                        return response;
                    }




                    string salt = CreateSalt();//Генерация соли
                    string hashpass = CreateSHA256(user.Password + salt);//получение хэша пароль+соль

                    query = string.Format("insert into [passwords]([iduser],[login]," +
                        "[password],[salt]) values({0},'{1}','{2}','{3}');",
                        iduser, user.Login, hashpass, salt);
                    cmd = new SqlCommand(query, conn);
                    try
                    {
                        conn.Open();
                        var result = await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                        if (result == 1) //row changes in the database - successfull
                        {
                            response.message = "Регистрация прошла успешно";
                            response.resultCode = 201;
                        }
                        else
                        {
                            response.message = "Какая-то ошибка";
                            response.resultCode = 500;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.resultCode = 500;
                        response.message = "Какая-то ошибка";
                        return response;
                    }
                    finally { conn.Close(); }
                }
            }
            return response;
        }

    }
}
