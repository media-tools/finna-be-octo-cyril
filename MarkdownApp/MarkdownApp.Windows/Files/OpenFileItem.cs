using MarkdownApp.Languages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MarkdownApp.Files
{
    public class OpenFileItem : IDataItem
    {
        public OpenFileItem()
        {
        }

        [JsonIgnore]
        public string UniqueId { get { return "open"; } }
        [JsonIgnore]
        public string Title { get { return "Open File"; } }
        [JsonIgnore]
        public string DisplayName { get { return Title; } }
        [JsonIgnore]
        public string Subtitle { get { return ""; } }
        [JsonIgnore]
        public string Description { get { return ""; } }
        [JsonIgnore]
        public string ImagePath { get { return ""; } }
        [JsonIgnore]
        public string Content { get { return ""; } }
        [JsonIgnore]
        public string BackgroundColor { get { return "#83b5dd"; } }

        public override string ToString()
        {
            return this.UniqueId;
        }
    }
}
