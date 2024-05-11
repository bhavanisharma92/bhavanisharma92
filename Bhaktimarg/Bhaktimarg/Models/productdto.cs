using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bhaktimarg.Models
{
    public class blogdto
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }

        public string MetaTag { get; set; }
        public Boolean IsActive { get; set; }

    }

    public class productdetailsdto
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }

        public Boolean IsActive { get; set; }
        public string Dates { get; set; }
        public string Description { get; set; }
        public string MetaTag { get; set; }
    }
    public class BlogListdto
    {
        public decimal Moreid { get; set; }
        public BlogListdto()
        {
            this.Blogs = new List<productdetailsdto>();
        }
        public List<productdetailsdto> Blogs { get; set; }
    }
}