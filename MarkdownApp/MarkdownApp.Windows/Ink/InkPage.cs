using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownApp.Ink
{
    public class InkPage
    {
        public int PageNumber { get; set; }

        public InkCanvas Instance { get; set; }

        public InkPage(int pageNumber)
        {
            PageNumber = pageNumber;
        }
    }
}
