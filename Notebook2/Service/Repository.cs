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
        public async Task<string> login(LoginViewModel user)
        {
            string response="1";

            if (user != null)
            {

                

                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = string.Format("Select * FROM tbl_Users WHERE password = '{0}' and Email='{1}'", "dd", user.Login);
                    cmd.Connection = conn;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    await Task.Run(() => da.Fill(dt));

                    //if (dt.Rows.Count > 0)
                    //{
                    //    response.Data = JsonConvert.SerializeObject(dt);
                    //    response.resultCode = 200;
                    //}
                    //else
                    //{
                    //    response.message = "User Not Found!";
                    //    response.resultCode = 500;
                    //}


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
                    string query = 
                        string.Format("select * from [passwords] where [login]='{0}'", user.Login);
                    int usersCount;
                    SqlCommand cmd = new SqlCommand(query,conn);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        usersCount = Convert.ToInt32(cmd.ExecuteScalar());
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        response.resultCode = 500;
                        response.message = "Какая-то ошибка";
                        return response;
                    }
                    finally { conn.Close(); }
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
                    query = string.Format("Insert INTO [users]([name],[surname]) " +
                        "VALUES('{0}','{1}')", user.Name, user.Surname);
                    cmd = new SqlCommand(query, conn);
                    int iduser;
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
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
                            cmd.ExecuteNonQuery();
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
