using BasicApp.Common;
using Core.Common;
using Core.Markdown;
using MarkdownApp.Languages;
using System;
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

        public MarkdownEditPage()
        {
            this.InitializeComponent();
            editor.SyntaxLanguage = new TextEditor.Languages.PythonSyntaxLanguage();
            //Log.Error("Hello!");
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a text file.
            Windows.Storage.Pickers.FileOpenPicker open = new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            LanguageSupport.AddLanguageSupport(open);

            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                //IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

                // load the file into the editor
                editor.TextLF = await FileIO.ReadTextAsync(file);
            }
        }

        protected override void LoadState(LoadStateEventArgs e)
        {
            FileInfo fileInfo = e.NavigationParameter as FileInfo;

            if (fileInfo != null)
            {
                Log.Error("Hello, " + fileInfo.FullPath);
            }
            else
            {
                Log.Error("Name is required.  Go back and enter a name.");
            }

            editor.TextLF = fileInfo.ReadText().Result;
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //if (((ApplicationView.Value != ApplicationViewState.Snapped) ||
            //      ApplicationView.TryUnsnap()))
            //{

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            LanguageSupport.AddLanguageSupport(savePicker);

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we 
                // finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);

                // open the file
                // IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);

                await FileIO.WriteTextAsync(file, editor.TextLF);

                // Let Windows know that we're finished changing the file so the 
                // other app can update the remote version of the file.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox = new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }
            }
        }

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
