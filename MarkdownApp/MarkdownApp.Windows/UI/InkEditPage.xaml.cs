using BasicApp.Common;
using Core.Common;
using MarkdownApp.Ink;
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
    public sealed partial class InkEditPage : BasicEditPage
    {
        private readonly InkCanvas2 inkCanvas;

        public InkEditPage()
        {
            this.InitializeComponent();
            //inkCanvas = new InkCanvas2(MainCanvas);
            
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
