using BasicApp.Common;
using Core.Common;
using MarkdownApp.Files;
using MarkdownApp.Languages;
using MarkdownApp.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace MarkdownApp
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
            foreach (var item in LanguageSupport.GetItemsNewFile())
            {
                newFiles.Add(item);
            }
            this.DefaultViewModel["NewFiles"] = newFiles;


            var recentFiles = new ObservableCollection<RecentFile>();
            recentFiles.Add(new RecentFile(fullPath: "C:/test/abc.txt", printErrors: true));
            foreach (RecentFile file in FileStorage.RecentFiles)
            {
                file.PrintErrors = true;
                recentFiles.Add(file);
            }
            this.DefaultViewModel["RecentFiles"] = recentFiles;
        }

        public async Task PickFile()
        {
            // Open a text file.
            Windows.Storage.Pickers.FileOpenPicker open = new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            LanguageSupport.AddLanguageSupport(open);

            Windows.Storage.StorageFile storageFile = await open.PickSingleFileAsync();
            if (storageFile != null)
            {
                RecentFile file = new RecentFile(storageFile: storageFile, printErrors: true);
                await OpenFile(file);
            }
        }

        public async Task OpenFile(FileActivatedEventArgs e)
        {
            RecentFile info = await FileStorage.RegisterFile(e);
            // Log._Test(info);
            await OpenFile(info);
        }

        public async Task OpenFile(RecentFile info)
        {
            if (info != null)
            {
                await info.Check();
            }

            if (info != null && info.IsValid)
            {
                Frame.Navigate(typeof(MarkdownEditPage), info);
            }
            else
            {
                Log.FatalError("The file is not valid.");
            }
        }

        private async void GridViewRecent_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Log._Test(e.ClickedItem);
            await OpenFile(e.ClickedItem as RecentFile);
        }

        private async void GridViewNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Log._Test(e.ClickedItem);
            await OpenFile(e.ClickedItem as RecentFile);
        }
    }
}
