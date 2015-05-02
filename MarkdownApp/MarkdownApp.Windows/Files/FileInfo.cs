using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownApp
{
    public class FileInfo : IDataItem
    {
        [JsonProperty("full_path")]
        public string FullPath { get; set; }

        public FileInfo(String fullPath)
        {
            FullPath = fullPath;
        }

        [JsonIgnore]
        public string UniqueId { get { return FullPath.GetHashCode() + ""; } }
        [JsonIgnore]
        public string Title { get { return System.IO.Path.GetFileName(FullPath); } }
        [JsonIgnore]
        public string Subtitle { get { return FullPath; } }
        [JsonIgnore]
        public string Description { get { return ""; } }
        [JsonIgnore]
        public string ImagePath { get { return ""; } }
        [JsonIgnore]
        public string Content { get { return ""; } }
        [JsonIgnore]
        public string BackgroundColor { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
