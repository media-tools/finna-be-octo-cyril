using Core.Ink;
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

        public SerializedCanvas PreloadedInk { get; set; }

        public InkCanvas Instance
        {
            get
            {
                foreach (InkCanvas inkCanvas in InkCanvas.Instances)
                {
                    if (inkCanvas.PageNumber == PageNumber)
                    {
                        return inkCanvas;
                    }
                }
                return null;
            }
        }

        public InkPage(int pageNumber)
        {
            PageNumber = pageNumber;
        }
    }
}
