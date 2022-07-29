using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Models
{
    public class User
    {
        public long Id { get; set; }
        public string? Fullname { get; set; }
        public string? Username { get; set; }
        public string? Passwordhash { get; set; }
        public byte[]? Image { get; set; }
        public string? PasswordSalt { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

}
