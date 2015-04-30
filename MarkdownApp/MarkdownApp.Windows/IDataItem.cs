using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownApp
{
    interface IDataItem
    {
        string UniqueId { get; }
        string Title { get; }
        string Subtitle { get; }
        string Description { get; }
        string ImagePath { get; }
        string Content { get; }
    }
}
