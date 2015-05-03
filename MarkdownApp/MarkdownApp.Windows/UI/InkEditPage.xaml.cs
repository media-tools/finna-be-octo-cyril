using BasicApp.Common;
using Core.Common;
using MarkdownApp.Languages;
using MarkdownApp.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MarkdownApp.UI
{
    public sealed partial class InkEditPage : BasicPage
    {
        public RecentFile CurrentFile;

        public InkEditPage()
        {
            this.InitializeComponent();
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await SaveFile();
        }

        private async void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            if (CurrentFile.Language != null)
                CurrentFile.Language.AddLanguageSupport(savePicker);
            else
                LanguageSupport.AddLanguageSupport(savePicker);

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CurrentFile = await FileStorage.RegisterFile(storageFile: file);
                await SaveFile();
            }
        }


        protected async override Task LoadState(LoadStateEventArgs e)
        {
            CurrentFile = e.NavigationParameter as RecentFile;
            await LoadFile();
        }

        private async Task LoadFile()
        {
            if (CurrentFile != null && CurrentFile.IsValid)
            {
                Log.Toast("File has been read successfully: " + CurrentFile.DisplayName);

                // load the file
                string content = await FileIO.ReadTextAsync(CurrentFile.StorageFile);
            }
            else
            {
                Log.Error("The file is not valid.");
            }
        }

        private async Task SaveFile()
        {
            // Prevent updates to the remote version of the file until we 
            // finish making changes and call CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(CurrentFile.StorageFile);

            string content = "";
            await FileIO.WriteTextAsync(CurrentFile.StorageFile, content);

            // Let Windows know that we're finished changing the file so the 
            // other app can update the remote version of the file.
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(CurrentFile.StorageFile);
            if (status != FileUpdateStatus.Complete)
            {
                Log.FatalError("File couldn't be saved: ", CurrentFile.FullPath);
            }
            else
            {
                Log.Toast("File has been saved: " + CurrentFile.DisplayName);
            }
        }

    }
}
