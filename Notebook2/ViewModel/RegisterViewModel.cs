using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Notebook2.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Display(Name = "Имя")]
        [RegularExpression("^[А-Яа-я0-9_.]{1,}$",
            ErrorMessage = "Имя должно написана на кирилице\n" +
            "Разрешенные символы: (кирилица) (цифры) -")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression("^[А-Яа-я0-9_.]{1,}$",
            ErrorMessage = "Фамилия должена написана на кирилице\n" +
            "Разрешенные символы: (кирилица) (цифры) -")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Display(Name = "Логин")]
        [RegularExpression("^[a-zA-Z0-9_.]{4,}$",
            ErrorMessage = "Логин должен иметь длинну 4 симолов.\n" +
            "Разрешенные символы: (латинские буквы) (цифры) _ -")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Display(Name = "Пароль")]
        [RegularExpression("^[a-zA-Z0-9!@#$%^&*()_+{}\\[\\]:;\\\"'<>,.?/\\\\|\\-=`~]{6,}$",
            ErrorMessage = "Пароль должен иметь длинну 6 симолов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }
}
