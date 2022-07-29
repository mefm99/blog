using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Models
{
    public class ArticleInfo
    {
        public long Id { get; set; }
        public long ArticleId { get; set; }
        public string? Title { get; set; }
        public string? Property { get; set; }
        public string? Value { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShow { get; set; }
    }

}
