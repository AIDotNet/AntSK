using System.ComponentModel.DataAnnotations;

namespace AntSK.Models
{
    public class LoginParamsType
    {
        [Required(ErrorMessage ="请填写 *{0}*")]
        [Display(Name = "账户")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请填写 *{0}*")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        public bool UnValid => string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password);

        public string Mobile { get; set; }

        public string Captcha { get; set; }

        public string LoginType { get; set; }

        public bool AutoLogin { get; set; }
    }
}