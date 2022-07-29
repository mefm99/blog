using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [MaxLength(25, ErrorMessage = "اسم المستخدم كبير للغاية")]
        public string Username { get; set; }
        [Required(ErrorMessage = "كلمة السر مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
