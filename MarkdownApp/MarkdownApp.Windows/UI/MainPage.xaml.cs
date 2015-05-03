using BasicApp.Common;
using Core.Common;
using MarkdownApp.Files;
using MarkdownApp.Languages;
using MarkdownApp.Storage;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace MarkdownApp.UI
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class MainPage : BasicPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override Task LoadState(LoadStateEventArgs e)
        {
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]

            var newFiles = new ObservableCollection<IDataItem>();
            newFiles.Add(new OpenFileItem());
            foreach (var item in LanguageSupport.GetItemsNewFile())
            {
                newFiles.Add(item);
            }
            this.DefaultViewModel["NewFiles"] = newFiles;


            var recentFiles = new ObservableCollection<IDataItem>();
            // recentFiles.Add(new RecentFile(fullPath: "C:/test/abc.txt", printErrors: true));
            foreach (RecentFile file in FileStorage.RecentFiles)
            {
                file.PrintErrors = true;
                await file.Check();
                recentFiles.Add(file);
            }
            this.DefaultViewModel["RecentFiles"] = recentFiles;
        }

        public async Task OpenFile(RecentFile info)
        {
            if (info != null)
            {
                await info.Check();
            }

            if (info != null && info.IsValid)
            {
                if (info.Language.FileType == FileType.TEXT)
                Frame.Navigate(typeof(MarkdownEditPage), info);
                else if (info.Language.FileType == FileType.INK)
                        Frame.Navigate(typeof(InkEditPage), info);
                else Log.FatalError("Unknown file type.");
            }
            else
            {
                Log.FatalError("The file is not valid.");
            }
        }

        public async Task OpenFile(FileActivatedEventArgs e)
        {
            RecentFile file = await FileStorage.RegisterFile(e);
            // Log._Test(info);
            await OpenFile(file);
        }

        private async Task PickFile()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            LanguageSupport.AddLanguageSupport(openPicker);

            Windows.Storage.StorageFile storageFile = await openPicker.PickSingleFileAsync();
            if (storageFile != null)
            {
                RecentFile file = await FileStorage.RegisterFile(storageFile: storageFile);
                await OpenFile(file);
            }
        }

        private async Task NewFile(NewFileItem item)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            if (item != null)
                item.Language.AddLanguageSupport(savePicker);
            else
                LanguageSupport.AddLanguageSupport(savePicker);

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            StorageFile storageFile = await savePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                RecentFile file = await FileStorage.RegisterFile(storageFile: storageFile);
                await OpenFile(file);
            }
        }

        private async void GridViewNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is OpenFileItem)
            {
                await PickFile();
            }
            else if (e.ClickedItem is NewFileItem)
            {
                await NewFile(e.ClickedItem as NewFileItem);
            }
        }

        private async void GridViewRecent_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Log._Test(e.ClickedItem);
            await OpenFile(e.ClickedItem as RecentFile);
        }
    }
}
