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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using TextEditor.Lexer;

namespace TextEditor.Languages
{
    public class PythonGrammer : IGrammer
    {
        public PythonGrammer()
        {
            Rules = new GrammerRule[]
            {
                new GrammerRule(TokenType.Comment, new Regex("^(#.*)")), // Comment
                new GrammerRule(TokenType.WhiteSpace, new Regex("^\\s")), // Whitespace
                //new GrammerRule(TokenType.Operator, new Regex("^(and|or|not|is)\\b")), // Word Operator
                new GrammerRule(TokenType.Operator, new Regex("^[\\+\\-\\*/%&|\\^~<>!]")), // Single Char Operator
                new GrammerRule(TokenType.Operator, new Regex("^((==)|(!=)|(<=)|(>=)|(<>)|(<<)|(>>)|(//)|(\\*\\*))")), // Double Char Operator
                new GrammerRule(TokenType.Delimiter, new Regex("^[\\(\\)\\[\\]\\{\\}@,:`=;\\.]")), // Single Delimiter
                new GrammerRule(TokenType.Delimiter, new Regex("^((\\+=)|(\\-=)|(\\*=)|(%=)|(/=)|(&=)|(\\|=)|(\\^=))")), // Double Char Operator
                new GrammerRule(TokenType.Delimiter, new Regex("^((//=)|(>>=)|(<<=)|(\\*\\*=))")), // Triple Delimiter

                new GrammerRule(TokenType.Identifier, new Regex("^[_A-Za-z][_A-Za-z0-9]*")), // Identifier

                new GrammerRule(TokenType.String, new Regex("^((\"\"\"(.*)\"\"\")|('''(.)*'''))", RegexOptions.IgnoreCase | RegexOptions.Multiline)),
                new GrammerRule(TokenType.String, new Regex("^((@'(?:[^']|'')*'|'(?:\\.|[^\\']|)*('|\\b))|(@\"(?:[^\"]|\"\")*\"|\"(?:\\.|[^\\\"])*(\"|\\b)))", RegexOptions.IgnoreCase | RegexOptions.Singleline)), // String Marker
            };

            Keywords = new string[]
            {
                "as", "assert", "break", "class", "continue", "def", "del", "elif", "else",
                "except", "finally", "for", "from", "global", "if", "import", "lambda",
                "pass", "raise", "return", "try", "while", "with", "yield", "in", "print",
                "and", "or", "not"
            };

            Builtins = new string[]
            {
                "ArithmeticError", "AssertionError", "AttributeError", "BaseException",
                "BufferError", "BytesWarning", "DeprecationWarning", "EOFError", "Ellipsis",
                "EnvironmentError", "Exception", "False", "FloatingPointError", "FutureWarning",
                "GeneratorExit", "IOError", "ImportError", "ImportWarning", "IndentationError",
                "IndexError", "KeyError", "KeyboardInterrupt", "LookupError", "MemoryError",
                "NameError", "None", "NotImplemented", "NotImplementedError", "OSError",
                "OverflowError", "PendingDeprecationWarning", "ReferenceError", "RuntimeError",
                "RuntimeWarning", "StandardError", "StopIteration", "SyntaxError",
                "SyntaxWarning", "SystemError", "SystemExit", "TabError", "True", "TypeError",
                "UnboundLocalError", "UnicodeDecodeError", "UnicodeEncodeError", "UnicodeError",
                "UnicodeTranslateError", "UnicodeWarning", "UserWarning", "ValueError", "Warning",
                "WindowsError", "ZeroDivisionError", "_", "__debug__", "__doc__", "__import__",
                "__name__", "__package__", "abs", "all", "any", "apply", "basestring", "bin",
                "bool", "buffer", "bytearray", "bytes", "callable", "chr", "classmethod",
                "cmp", "coerce", "complex", "copyright", "credits", "delattr",
                "dict", "divmod", "enumerate", "filter", "float",
                "format", "frozenset", "getattr", "hasattr",
                "hash", "hex", "id", "int", "intern", "isinstance", "issubclass",
                "iter", "len", "license", "list", "long", "map", "max", "memoryview",
                "min", "next", "object", "oct", "ord", "pow", "print", "property",
                "range", "raw_input", "reduce", "repr", "reversed",
                "round", "set", "setattr", "slice", "sorted", "staticmethod", "str", "sum",
                "super", "tuple", "type", "unichr", "unicode", "xrange", "zip"
            };


        }

        public IEnumerable<GrammerRule> Rules { get; private set; }

        public IEnumerable<string> Builtins { get; private set; }

        public IEnumerable<string> Keywords { get; private set; }
    }
}