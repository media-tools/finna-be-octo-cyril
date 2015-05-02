using Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownApp.Storage
{
    public static class Files
    {
        private static string KEY_FILES = "test_files";

        public static List<FileInfo> RecentFiles { get { return InternalConfig.RecentFiles; } }

        private static FilesConfig InternalConfig = new FilesConfig();

        public static void Load()
        {
            if (Settings.CurrentContainer.Values.ContainsKey(KEY_FILES))
            {
                string content = Settings.CurrentContainer.Values[KEY_FILES] as string;
                InternalConfig = PortableConfigHelper.ReadConfig<FilesConfig>(content: ref content);
                Settings.CurrentContainer.Values[KEY_FILES] = content;
            }
            else
            {
                InternalConfig = new FilesConfig();
                Settings.CurrentContainer.Values[KEY_FILES] = PortableConfigHelper.WriteConfig<FilesConfig>(InternalConfig);
            }
        }

        public static void Save()
        {
            Settings.CurrentContainer.Values[KEY_FILES] = PortableConfigHelper.WriteConfig<FilesConfig>(InternalConfig);
        }

        private class FilesConfig
        {
            [JsonProperty("recent_files")]
            public List<FileInfo> RecentFiles = new List<FileInfo>();
        }
    }
}
