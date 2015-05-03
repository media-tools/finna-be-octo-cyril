using BasicApp.Common;
using Core.Common;
using Core.Markdown;
using MarkdownApp.Languages;
using MarkdownApp.Storage;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace MarkdownApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class MarkdownEditPage : BasicPage
    {
        public RecentFile CurrentFile;

        public MarkdownEditPage()
        {
            this.InitializeComponent();
            editor.SyntaxLanguage = new TextEditor.Languages.PythonSyntaxLanguage();
            //Log.Error("Hello!");
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

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    new CoreDispatcherPriority(), () =>
                    {

                        ///RtfConversion.ToHtml(editor.Document);
                    });
        }

        private async void OutputView_Loaded(object sender, RoutedEventArgs e)
        {
            string markdownCode = editor.TextLF;
            string html = await Markdown.MarkdownToHTML(markdownCode: markdownCode);
            OutputView.NavigateToString(html);
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
                Log._Test("File has been read successfully: " + CurrentFile.FullPath);

                // load the file into the editor
                editor.TextLF = await FileIO.ReadTextAsync(CurrentFile.StorageFile);
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

            // open the file
            // IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);

            await FileIO.WriteTextAsync(CurrentFile.StorageFile, editor.TextLF);

            // Let Windows know that we're finished changing the file so the 
            // other app can update the remote version of the file.
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(CurrentFile.StorageFile);
            if (status != FileUpdateStatus.Complete)
            {
                Log.FatalError("File couldn't be saved: ", CurrentFile.FullPath);
            }
        }

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
