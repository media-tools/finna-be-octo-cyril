using Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MarkdownApp
{
    public class FileInfo : IDataItem
    {
        private bool printErrors;

        [JsonProperty("full_path")]
        public string FullPath { get; set; }

        [JsonIgnore]
        public IStorageFile StorageFile { get; private set; }

        [JsonIgnore]
        public bool PrintErrors { get; set; }

        [JsonIgnore]
        public bool IsValid { get; private set; }

        [JsonIgnore]
        public bool IsFullPathSupported { get; private set; }

        public FileInfo()
        {
            PrintErrors = false;
        }

        public FileInfo(IStorageFile storageFile, bool printErrors)
        {
            StorageFile = storageFile;
            PrintErrors = printErrors;
        }

        public FileInfo(string fullPath, bool printErrors)
        {
            FullPath = fullPath;
            PrintErrors = printErrors;
        }

        public async Task Check()
        {
            if (StorageFile == null)
            {
                try
                {
                    StorageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(FullPath);
                }
                catch (Exception e)
                {
                    if (PrintErrors)
                    {
                        Log.Error("Check:StorageFile: ", e);
                    }
                    else
                    {
                        Log.Debug("Check:StorageFile: ", e);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(FullPath))
            {
                FullPath = StorageFile.Path;
            }

            IsFullPathSupported = FullPath != null;
            IsValid = StorageFile != null;

        }

        public async Task<string> ReadText()
        {
            try
            {
                var text = await FileIO.ReadTextAsync(StorageFile);
                return text;
            }
            catch (Exception e)
            {
                if (PrintErrors)
                {
                    Log.Error(e);
                }
                else
                {
                    Log.Debug(e);
                }
                return null;
            }
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
            return this.FullPath;
        }
    }
}
