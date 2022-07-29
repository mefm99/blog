using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.ViewModels
{
    public class EditProfileVM
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "الاسم مطلوب")]
        [MaxLength(20, ErrorMessage = "الاسم كبير للغاية")]
        [MinLength(5, ErrorMessage = "الاسم قصير جداً")]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [MaxLength(20, ErrorMessage = "اسم المستخدم كبير للغاية")]
        [MinLength(5, ErrorMessage = "اسم المستخدم قصير جداً")]
        public string Username { get; set; }
        [MaxLength(20, ErrorMessage = "كلمة المرور كبير للغاية")]
        [MinLength(5, ErrorMessage = "كلمة المرور قصير جداً")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور غير متطابقتان")]
        public string ConifrmPassword { get; set; }
        public IFormFile ImageFile { get; set; }
    }

}
