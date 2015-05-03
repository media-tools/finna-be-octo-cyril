using MarkdownApp.Files;
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
            new SupportedLanguage("Markdown", "#ffd600", FileType.TEXT, new PythonSyntaxLanguage(), ".md"),
            new SupportedLanguage("Python", "#f47d44", FileType.TEXT, new PythonSyntaxLanguage(), ".py"),
            new SupportedLanguage("Note", "#e46764", FileType.INK, null, ".notes"),
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

        public static IEnumerable<NewFile> GetItemsNewFile()
        {
            foreach (SupportedLanguage lang in SupportedLanguages)
            {
                yield return new NewFile(lang);
            }
        }
    }

    public class SupportedLanguage
    {
        public string Title { get; private set; }
        public SyntaxLanguage Syntax { get; private set; }
        public string Color { get; private set; }
        public string[] Extensions { get; private set; }
        public FileType FileType { get; private set; }

        public SupportedLanguage(string title, string color, FileType fileType, SyntaxLanguage syntax, params string[] extensions)
        {
            Title = title;
            Syntax = syntax;
            Color = color;
            Extensions = extensions;
            FileType = fileType;
        }
    }
}
