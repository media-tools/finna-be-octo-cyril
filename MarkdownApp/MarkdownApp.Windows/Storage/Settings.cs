using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MarkdownApp.Storage
{
    internal class Settings
    {
        public static ApplicationDataContainer LocalSettings { get; private set; }

        static Settings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
            // StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        }
    }
}
