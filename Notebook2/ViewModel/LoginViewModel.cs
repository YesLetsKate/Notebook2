using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Notebook2.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", 
            ErrorMessage = "E-mail id is not valid")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", 
            ErrorMessage = "E-mail id is not valid")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
