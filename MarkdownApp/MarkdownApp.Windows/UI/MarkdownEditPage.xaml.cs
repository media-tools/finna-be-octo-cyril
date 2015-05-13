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

namespace MarkdownApp.UI
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class MarkdownEditPage : BasicEditPage
    {
        public MarkdownEditPage()
        {
            this.InitializeComponent();
            editor.SyntaxLanguage = new TextEditor.Languages.PythonSyntaxLanguage();
            //Log.Error("Hello!");
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

        protected async override Task BeforeLoadFile(LoadStateEventArgs e)
        {
        }

        protected async override Task<string> GetContent()
        {
            return editor.TextLF;
        }

        protected async override Task SetContent(string content)
        {
            // load the file into the editor
            editor.TextLF = content;
        }

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
