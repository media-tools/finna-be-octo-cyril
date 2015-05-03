using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace MarkdownApp.Storage
{
    public static class Settings
    {
        private static readonly string CONTAINER_NAME = "testContainer";

        public static ApplicationDataContainer LocalSettings { get; private set; }

        public static ApplicationDataContainer CurrentContainer { get; private set; }

        public static void Load()
        {
            PortableLogging.Enable();
            Log.LogHandler += (type, messageLines) =>
            {
                BlockingDebugConsole.WriteLine("fuck: " + type);
                if (type == Log.Type.ERROR || type == Log.Type.FATAL_ERROR || type == Log.Type._TEST)
                {
                    string message = string.Join("\n", messageLines);

                    MessageDialog dialog = new MessageDialog(message, type == Log.Type.FATAL_ERROR ? "Fatal Error" : "Error");
                    dialog.ShowAsync();
                }
            };


            LocalSettings = ApplicationData.Current.LocalSettings;
            // StorageFolder localFolder = ApplicationData.Current.LocalFolder;


            Windows.Storage.ApplicationDataContainer container = LocalSettings.CreateContainer(CONTAINER_NAME, Windows.Storage.ApplicationDataCreateDisposition.Always);

            if (LocalSettings.Containers.ContainsKey(CONTAINER_NAME))
            {
                CurrentContainer = container;
            }
            else
            {
                Log.FatalError("Unable to open isolated storage: ", CONTAINER_NAME);
            }
        }
    }
}
