using BasicApp.Common;
using Core.Common;
using MarkdownApp.Ink;
using MarkdownApp.Languages;
using MarkdownApp.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class InkEditPage : BasicEditPage
    {
        private readonly InkCanvas2 inkCanvas;

        public InkEditPage()
        {
            this.InitializeComponent();
            //inkCanvas = new InkCanvas2(MainCanvas);
            
            
        }

        protected async override Task LoadState(LoadStateEventArgs e)
        {
            await base.LoadState(e);
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]

            var pages = new ObservableCollection<InkPage>();
            for (int i = 1; i <= 10; ++i)
            {
                pages.Add(new InkPage(i));
            }
            this.DefaultViewModel["Pages"] = pages;
            //GridViewPages.ItemsSource = pages;

            Log._Test("abc");






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

        protected async override Task<string> GetContent()
        {
            return "";
        }

        protected async override Task SetContent(string content)
        {
        }
    }
}
