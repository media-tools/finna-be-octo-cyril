using MarkdownApp.Files;
using System;
using System.Collections.Generic;
using System.IO;
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
            new SupportedLanguage("Plain Text", "#b163a3", FileType.TEXT, new PythonSyntaxLanguage(), ".txt"),
            new SupportedLanguage("Python", "#f47d44", FileType.TEXT, new PythonSyntaxLanguage(), ".py"),
            new SupportedLanguage("Note", "#e46764", FileType.INK, null, ".note"),
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
                lang.AddLanguageSupport(savePicker);
            }
        }

        public static IEnumerable<NewFileItem> GetItemsNewFile()
        {
            foreach (SupportedLanguage lang in SupportedLanguages)
            {
                yield return new NewFileItem(lang);
            }
        }

        public static SupportedLanguage FindByExtension(string fullPath)
        {
            if (fullPath != null)
            {
                string extension = Path.GetExtension(fullPath);
                foreach (SupportedLanguage lang in SupportedLanguages)
                {
                    if (lang.Extensions.Any(e => e == extension))
                    {
                        return lang;
                    }
                }
            }
            return null;
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

        public void AddLanguageSupport(FileSavePicker savePicker)
        {
            savePicker.FileTypeChoices.Add(Title, Extensions);
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
