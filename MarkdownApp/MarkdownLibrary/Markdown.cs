using CommonMark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownLibrary
{
    public static class Markdown
    {
        public static async Task<string> MarkdownToHTML(string markdownCode) {
            string htmlCode = null;
            await Task.Run(() =>
            {
                CommonMarkSettings settings = CommonMarkSettings.Default;
                settings.OutputFormat = OutputFormat.Html;
                settings.AdditionalFeatures = CommonMarkAdditionalFeatures.All;
                htmlCode = CommonMarkConverter.Convert(source: markdownCode, settings: settings);
            });
            return htmlCode;
        }
    }
}
