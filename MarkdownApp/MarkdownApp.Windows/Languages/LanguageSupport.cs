using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor;
using TextEditor.Languages;
using Windows.Storage.Pickers;

namespace MarkdownApp.Languages
{
    public static class LanguageSupport
    {
        private static List<SupportedLanguage> SupportedLanguages = new List<SupportedLanguage>
        {
            new SupportedLanguage("Markdown", new PythonSyntaxLanguage(), ".md"),
            new SupportedLanguage("Python", new PythonSyntaxLanguage(), ".py"),
        };

        public static void AddLanguageSupport(FileOpenPicker openPicker)
        {
            foreach (SupportedLanguage lang in SupportedLanguages)
            {
                foreach (string extension in lang.Extensions)
                {
                    openPicker.FileTypeFilter.Add(extension);
                }
            }
        }

        public static void AddLanguageSupport(FileSavePicker savePicker)
        {
            foreach (SupportedLanguage lang in SupportedLanguages)
            {
                savePicker.FileTypeChoices.Add(lang.Title, lang.Extensions);
            }
        }
    }

    public class SupportedLanguage
    {
        public string Title { get; private set; }
        public SyntaxLanguage Syntax { get; private set; }
        public string[] Extensions { get; private set; }

        public SupportedLanguage(string title, SyntaxLanguage syntax, params string[] extensions)
        {
            Title = title;
            Syntax = syntax;
            Extensions = extensions;
        }
    }
}
