// Copyright (c) Adnan Umer. All rights reserved. Follow me @aztnan
// Email: aztnan@outlook.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TextEditor;
using TextEditor.Lexer;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace TextEditor.UI
{
    public sealed class SyntaxEditor : Control
    {
        public SyntaxEditor()
        {
            this.DefaultStyleKey = typeof(SyntaxEditor);

            LineNumberBlock = new TextBlock { Foreground = new SolidColorBrush(Color.FromArgb(255, 43, 145, 175)) };
            TextView = new RichEditBox();

            this.Loaded += (s, e) => { BindTextViewerScrollViewer(); };
        }

        #region Text View

        public static readonly DependencyProperty TextViewProperty =
            DependencyProperty.Register("TextView", typeof(RichEditBox), typeof(SyntaxEditor),
                                        new PropertyMetadata(null, OnTextViewPropertyChanged));

        private static void OnTextViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SyntaxEditor)d).OnTextViewChanged((RichEditBox)e.OldValue, (RichEditBox)e.NewValue);
        }

        public RichEditBox TextView
        {
            get { return (RichEditBox)GetValue(TextViewProperty); }
            set { SetValue(TextViewProperty, value); }
        }

        private void OnTextViewChanged(RichEditBox oldValue, RichEditBox newValue)
        {
            if (oldValue != null)
            {
                oldValue.TextChanged -= HandleTextViewTextChanged;
                oldValue.KeyUp -= HandleTextViewKeyUp;
            }

            if (newValue != null)
            {
                newValue.TextChanged += HandleTextViewTextChanged;
                newValue.KeyUp += HandleTextViewKeyUp;

                BindTextViewStyle();
                RefreshLineNumbers(); // (1)

                // newValue.Paste += RequestLineNumberRedraw;
            }
        }

        private void RequestLineNumberRedraw(object sender, object e)
        {
            RefreshLineNumbers();
        }

        void BindTextViewStyle()
        {
            TextView.SetBinding(RichEditBox.FontFamilyProperty, new Binding { Path = new PropertyPath("FontFamily"), Source = this });
            TextView.SetBinding(RichEditBox.FontSizeProperty, new Binding { Path = new PropertyPath("FontSize"), Source = this });

            BindTextViewerScrollViewer();
        }

        void BindTextViewerScrollViewer()
        {
            var g = VisualTreeHelper.GetChild(TextView, 0) as Grid;
            if (g != null)
            {
                int max = VisualTreeHelper.GetChildrenCount(g);
                for (int i = 0; i < max; i++)
                {
                    var ele = VisualTreeHelper.GetChild(g, i) as ScrollViewer;
                    if (ele != null && scrollViewer != null)
                    {
                        ele.ViewChanged += (s, e) =>
                        {
                            RefreshLineNumbers();
                            scrollViewer.ChangeView(ele.HorizontalOffset, ele.VerticalOffset, null, true);
                        };
                    }
                }
            }
        }

        #endregion

        #region Line Number

        public static readonly DependencyProperty LineNumberBlockProperty =
            DependencyProperty.Register("LineNumberBlock", typeof(TextBlock), typeof(SyntaxEditor), new PropertyMetadata(null));

        public TextBlock LineNumberBlock
        {
            get { return (TextBlock)GetValue(LineNumberBlockProperty); }
            set { SetValue(LineNumberBlockProperty, value); }
        }

        void RefreshLineNumbers()
        {
            var builder = new StringBuilder();
            string text = Text;

            int previousLineNumber = -1;
            int previousVerticalHeight = 0;
            int countMultiLines = 0;
            double betterLineHeight = -1;
            foreach (int position in GetPositions(text, "\r"))
            {
                var range = TextView.Document.GetRange(position, position + 1);
                Windows.Foundation.Rect rectangle;
                int hit;
                range.GetRect(PointOptions.NoHorizontalScroll, out rectangle, out hit);

                if (betterLineHeight == -1)
                    betterLineHeight = (double)rectangle.Height;

                int verticalHeight = (int)(rectangle.Top);
                int maxLineNumber = (int)Math.Round(verticalHeight / betterLineHeight);
                int minLineNumber = previousLineNumber + 1;

                if (maxLineNumber > 0)
                    betterLineHeight = (double)verticalHeight / (double)maxLineNumber;

                int printableLineNumber = minLineNumber + 1 - countMultiLines; // starts with 1 and excludes addtional line numbers from multi lines
                builder.AppendLine(printableLineNumber + "");
                //builder.AppendLine(printableLineNumber + " (" + verticalHeight + "/" + betterLineHeight + "="+ verticalHeight / betterLineHeight + "~"+ maxLineNumber + ")");
                for (int i = minLineNumber; i < maxLineNumber; i++)
                {
                    builder.AppendLine("");
                }

                previousLineNumber = maxLineNumber;
                previousVerticalHeight = verticalHeight;
                countMultiLines += maxLineNumber - minLineNumber;
            }

            // builder.AppendLine("n" + text.Count<char>(c => c == '\n'));
            // builder.AppendLine("r" + text.Count<char>(c => c == '\r'));

            //for (int i = 1; i <= stop; i++)
            //    builder.AppendLine(i.ToString());

            LineNumberBlock.Text = builder.ToString();
        }

        public static IEnumerable<int> GetPositions(string source, string searchString)
        {
            int len = searchString.Length;
            int start = -len;
            while (true)
            {
                start = source.IndexOf(searchString, start + len);
                if (start == -1)
                {
                    break;
                }
                else
                {
                    yield return start;
                }
            }
        }

        #endregion

        #region Syntax Language

        public static readonly DependencyProperty SyntaxLanguageProperty =
            DependencyProperty.Register("SyntaxLanguage", typeof(SyntaxLanguage), typeof(SyntaxEditor),
                                        new PropertyMetadata(null, OnSyntaxLanguagePropertyChanged));

        private static void OnSyntaxLanguagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SyntaxEditor)d).OnSyntaxLanguageChanged((SyntaxLanguage)e.NewValue);
        }

        public SyntaxLanguage SyntaxLanguage
        {
            get { return (SyntaxLanguage)GetValue(SyntaxLanguageProperty); }
            set { SetValue(SyntaxLanguageProperty, value); }
        }

        private void OnSyntaxLanguageChanged(SyntaxLanguage newValue)
        {
            if (newValue == null)
            {
                tokenizer = null;
                return;
            }

            if (newValue.HighlightColors == null)
                throw new ArgumentException("Grammer HightlightColrs must not be null");

            tokenizer = new Tokenizer(newValue.Grammer);
        }

        #endregion

        #region Highlighting

        Tokenizer tokenizer = null;

        int textLength = 0;
        private void HandleTextViewTextChanged(object sender, RoutedEventArgs e)
        {
            if (tokenizer == null) return;

            var editor = (RichEditBox)sender;
            string text = Text;

            if (text.Length == textLength) return;
            textLength = text.Length;

            editor.Document.GetRange(0, int.MaxValue).CharacterFormat.ForegroundColor = Colors.Black;

            var t = tokenizer.Tokenize(text);
            var highlightColors = SyntaxLanguage.HighlightColors;

            Color foregroundColor;
            while (t.MoveNext())
            {
                if (highlightColors.TryGetValue(t.Current.Type, out foregroundColor))
                {
                    editor.Document.GetRange(t.Current.StartIndex, t.Current.StartIndex + t.Current.Length).CharacterFormat.ForegroundColor = foregroundColor;
                }
            }
        }

        #endregion

        public string TextLF
        {
            get
            {
                return Text.Replace('\r', '\n');
            }
            set
            {
                Text = value.Replace('\n', '\r');
            }
        }

        public string Text
        {
            get
            {
                string text = string.Empty;

                if (TextView != null)
                    TextView.Document.GetText(TextGetOptions.None, out text);

                return text;
            }
            set
            {
                if (TextView != null)
                {
                    TextView.Document.SetText(TextSetOptions.None, value);
                    RefreshLineNumbers();
                }
            }
        }

        #region Indentation

        private void HandleTextViewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string text = Text;
                RefreshLineNumbers();

                var indentLevel = GetIndentLevel(ref text);
                e.Handled = true;
                if (indentLevel == 0) return;

                TextView.Document.Selection.SetText(TextSetOptions.None, new String(' ', indentLevel));
                var x = TextView.Document.Selection.StartPosition + indentLevel;
                TextView.Document.Selection.SetRange(x, x);
            }
            /*
            else if (TextView.Document.Selection.Length > 0 ||
                e.Key == Windows.System.VirtualKey.Back) {
                RefreshLineNumbers();
            }
            */
        }

        int GetIndentLevel(ref string text)
        {
            if (SyntaxLanguage.IndentationProvider == null ||
                TextView.Document.Selection.Length != 0)
                return 0;

            try
            {
                return SyntaxLanguage.IndentationProvider.GuessIndentLevel(text, TextView.Document.Selection.EndPosition);
            }
            catch (Exception)
            { }

            return 0;
        }

        #endregion

        ScrollViewer scrollViewer;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (TextView != null)
                BindTextViewStyle();

            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            Debug.Assert(scrollViewer != null);
        }
    }
}