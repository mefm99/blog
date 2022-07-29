using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Models
{
    public class Article
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Details { get; set; }
        public byte[]? Image { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UserId { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsShow { get; set; }
        public int? Counter { get; set; }
        public User? User { get; set; }
    }

}
