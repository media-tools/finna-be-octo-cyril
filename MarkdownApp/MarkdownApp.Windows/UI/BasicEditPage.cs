using BasicApp.Common;
using Core.Common;
using MarkdownApp.Languages;
using MarkdownApp.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;

namespace MarkdownApp.UI
{
    public abstract class BasicEditPage : BasicPage
    {
        public RecentFile CurrentFile;

        public BasicEditPage()
        {
        }

        protected async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null);
        }

        protected async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SaveFile();
            }
            catch (Exception ex)
            {
                Log.FatalError(ex.Message);
            }
        }

        protected async void SaveAsButton_Click(object sender, RoutedEventArgs e)
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

        protected async sealed override Task LoadState(LoadStateEventArgs e)
        {
            await base.LoadState(e);

            BeforeLoadFile(e);

            CurrentFile = e.NavigationParameter as RecentFile;
            await LoadFile();
        }

        protected abstract Task BeforeLoadFile(LoadStateEventArgs e);

        protected abstract Task<string> GetContent();
        protected abstract Task SetContent(string content);

        private async Task LoadFile()
        {
            if (CurrentFile != null && CurrentFile.IsValid)
            {
                Log.Toast("File has been read successfully: " + CurrentFile.DisplayName);

                // load the file
                string content = await FileIO.ReadTextAsync(CurrentFile.StorageFile);
                await SetContent(content: content);
            }
            else
            {
                Log.Error("The file is not valid.");
            }
        }

        private async Task SaveFile()
        {
            string content = await GetContent();

            // Prevent updates to the remote version of the file until we 
            // finish making changes and call CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(CurrentFile.StorageFile);

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
