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
        }

        protected async override Task BeforeLoadFile(LoadStateEventArgs e)
        {
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
            var pages = new ObservableCollection<InkPage>();
            this.DefaultViewModel["Pages"] = pages;

            InkCanvas.Instances.Clear();
        }

        protected async override Task<string> GetContent()
        {
            SerializedInkCollection result = new SerializedInkCollection();
            foreach (InkPage page in this.DefaultViewModel["Pages"] as ObservableCollection<InkPage>)
            {
                if (page.Instance != null)
                {
                    SerializedInk ink = page.Instance.Save();
                    if (ink.Strokes.Count > 0)
                    {
                        result.Add(ink);
                        Log._Test("Page ", page.PageNumber, " has ", ink.Strokes.Count, " strokes!");
                    }
                }
            }
            return PortableConfigHelper.WriteConfig(result);
        }

        protected async override Task SetContent(string content)
        {
            SerializedInkCollection collection = PortableConfigHelper.ReadConfig<SerializedInkCollection>(content: ref content);
            int p = 1;
            foreach (SerializedInk ink in collection.Pages)
            {
                InkPage inkPage = new InkPage(p++);
                inkPage.PreloadedInk = ink;
                (this.DefaultViewModel["Pages"] as ObservableCollection<InkPage>).Add(inkPage);
            }

            for (int q = 0; q < 5; q++)
            {
                InkPage inkPage = new InkPage(p + q++);
                (this.DefaultViewModel["Pages"] as ObservableCollection<InkPage>).Add(inkPage);
            }
        }
    }
}
