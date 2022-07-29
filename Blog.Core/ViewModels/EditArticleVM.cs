using Blog.Core.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.ViewModels
{
    public class EditArticleVM
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "عنوان المقالة مطلوب")]
        [MaxLength(150, ErrorMessage = "العنوان كبير للغاية")]
        [MinLength(10, ErrorMessage = "العنوان قصير جداً")]
        public string Title { get; set; }
        [Required(ErrorMessage = "عنوان المقالة التفصيلي مطلوب")]
        [MaxLength(150, ErrorMessage = "العنوان التفصيلي كبير للغاية")]
        [MinLength(10, ErrorMessage = "العنوان التفصيلي قصير جداً")]
        public string SubTitle { get; set; }
        [Required(ErrorMessage = "تفاصيل المقالة مطلوبة")]
        [MaxLength(10000, ErrorMessage = "المقالة طويلة للغاية")]
        [MinLength(20, ErrorMessage = "المقالة قصيرة جداً")]
        public string Details { get; set; }
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" }, CustomErrorMessage = "صيغة الملف غير مدعومة")]
        public IFormFile ImageFile { get; set; }
        public byte[] Image { get; set; }
        public string TitleInfo1 { get; set; }
        public string TitleInfo2 { get; set; }
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }

}
