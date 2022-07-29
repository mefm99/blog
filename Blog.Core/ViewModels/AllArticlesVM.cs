using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.ViewModels
{
    public class AllArticlesVM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public DateTime? AddedDate { get; set; }
        public byte[] Image { get; set; }

    }

}
