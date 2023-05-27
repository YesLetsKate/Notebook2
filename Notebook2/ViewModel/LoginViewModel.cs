using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Notebook2.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        [RegularExpression("^[a-zA-Z0-9_.]{4,}$", 
            ErrorMessage = "Логин должен иметь длинну 4 симолов.\n" +
            "Разрешенные символы: (латинские буквы) (цифры) _ -")]
        public string Login { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9!@#$%^&*()_+{}\\[\\]:;\\\"'<>,.?/\\\\|\\-=`~]{6,}$",
            ErrorMessage = "Пароль должен иметь длинну 6 симолов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
