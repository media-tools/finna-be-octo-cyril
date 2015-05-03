using Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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

                foreach (FileInfo file in InternalConfig.RecentFiles)
                {
                    file.Check().Wait();
                }
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

        public static async Task<FileInfo> RegisterFile(Windows.ApplicationModel.Activation.FileActivatedEventArgs e)
        {
            foreach (IStorageItem storageItem in e.Files)
            {
                if (storageItem is IStorageFile)
                {
                    //Log._Test(storageItem.Path);
                    FileInfo file = new FileInfo(storageFile: storageItem as IStorageFile, printErrors: true);
                    await file.Check();
                    return file;
                    if (file.IsValid)
                    {
                        if (file.IsFullPathSupported)
                        {
                            if (!RecentFiles.Any(rf => rf.FullPath == file.FullPath))
                            {
                                RecentFiles.Add(file);
                            }
                        }
                        return file;
                    }
                }
                else
                {
                    Log.FatalError("Not a file: ", storageItem);
                }
            }
            return null;
        }
    }
}
