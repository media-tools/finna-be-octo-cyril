using BasicApp.Common;
using Core.Common;
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
            var items = new ObservableCollection<FileInfo>();

            var test = new FileInfo(fullPath: "C:/test/abc.txt", printErrors: true);
            items.Add(test);

            foreach (FileInfo fileInfo in Files.RecentFiles)
            {
                items.Add(fileInfo);
            }
            
            this.DefaultViewModel["Items"] = items;
        }

        public async Task OpenFile(FileActivatedEventArgs e)
        {
            FileInfo info = await Files.RegisterFile(e);
            Log._Test(info);
            if (info != null)
            {
                OpenFile(info);
            }
        }

        public void OpenFile(FileInfo info)
        {
            if (info != null && info.IsValid)
            {
                Frame.Navigate(typeof(MarkdownEditPage), info);
            }
            else
            {
                Log.FatalError("The file is not valid.");
            }
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Log._Test(e.ClickedItem);
            OpenFile(e.ClickedItem as FileInfo);
        }
    }
}
