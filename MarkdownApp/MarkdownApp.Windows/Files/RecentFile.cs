using Core.Common;
using MarkdownApp.Languages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace MarkdownApp
{
    public class RecentFile : IDataItem
    {
        [JsonProperty("full_path")]
        public string FullPath { get; set; }

        [JsonProperty("future_token")]
        public string Token { get; set; }

        [JsonIgnore]
        public IStorageFile StorageFile { get; private set; }

        [JsonIgnore]
        public bool PrintErrors { get; set; }

        [JsonIgnore]
        public bool IsValid { get; private set; }

        [JsonIgnore]
        public bool IsFullPathSupported { get; private set; }

        [JsonIgnore]
        public SupportedLanguage Language { get; private set; }

        public RecentFile()
        {
            PrintErrors = false;
        }

        public RecentFile(IStorageFile storageFile, bool printErrors)
        {
            StorageFile = storageFile;
            Token = StorageApplicationPermissions.FutureAccessList.Add(file: storageFile);
            PrintErrors = printErrors;
        }

        public RecentFile(string fullPath, bool printErrors)
        {
            FullPath = fullPath;
            PrintErrors = printErrors;
        }

        public async Task Check()
        {
            if (StorageFile == null)
            {
                if (string.IsNullOrWhiteSpace(Token))
                {
                    Log.Error("File has no token!");
                }
                else
                {
                    try
                    {
                        StorageFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token: Token);
                        // StorageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(FullPath);
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
            }

            if (string.IsNullOrWhiteSpace(FullPath))
            {
                if (StorageFile != null)
                {
                    FullPath = StorageFile.Path;
                }
                else
                {
                    Log.Error("Bug! Both StorageFile and FullPath are null!");
                }
            }

            IsFullPathSupported = FullPath != null;
            IsValid = StorageFile != null && Token != null;

            Language = LanguageSupport.FindByExtension(fullPath: FullPath);
            if (Language != null)
            {
                BackgroundColor = Language.Color;
            }
            else {
                BackgroundColor = "#ffffffff";
            }
            //Log._Test(BackgroundColor);
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
        public string DisplayName { get { return FullPath.Count(c => c == '/' || c == '\\') <= 3 ? FullPath : Path.Combine("...", Path.Combine(FullPath.Split('\\', '/').TakeLast(3).ToArray())); } }
        [JsonIgnore]
        public string Subtitle { get { return DisplayName; } }
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
