using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Schema.Common
{
    public class CodeBuilder
    {
        private StringBuilder sb = new StringBuilder();
        private int CharactersToIndent { get; set; }
        public int IndentLength { get; set; } = 2;

        public CodeBuilder Append(string text)
        {
            sb.Append(text);
            return this;
        }

        public CodeBuilder Indent()
        {
            CharactersToIndent += 1;
            return this;
        }

        public CodeBuilder Indent(int charactersToIndent)
        {
            CharactersToIndent += charactersToIndent;
            return this;
        }

        public CodeBuilder Outdent()
        {
            CharactersToIndent -= 1;
            return this;
        }

        public CodeBuilder Outdent(int charactersToIndent)
        {
            CharactersToIndent -= charactersToIndent;
            return this;
        }


        public CodeBuilder AppendLine(string text)
        {
            var indent = Math.Max(CharactersToIndent * IndentLength, 0);
            for (int i = 0; i < indent; i++)
                sb.Append(" ");

            sb.AppendLine(text);
            return this;
        }

        public CodeBuilder EndLine(string text)
        {
            sb.AppendLine(text);
            return this;
        }

        public CodeBuilder StartLine()
        {
            var indent = Math.Max(CharactersToIndent * IndentLength, 0);
            for (int i = 0; i < indent; i++)
                sb.Append(" ");

            return this;
        }

        public CodeBuilder StartLine(string text)
        {
            var indent = Math.Max(CharactersToIndent * IndentLength, 0);
            for (int i = 0; i < indent; i++)
                sb.Append(" ");

            sb.Append(text);

            return this;
        }

        public CodeBuilder EndLine()
        {
            sb.AppendLine("");
            return this;
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        public CodeBuilder AppendDelimited(string delimiter, IEnumerable<string> strings)
        {
            var text = string.Join(delimiter, strings);
            sb.Append(text);
            return this;
        }


        public CodeBuilder AppendDelimited<T>(string delimiter, IEnumerable<T> items, Func<T, String> expression)
        {
            var strings = from T item in items
                          select expression(item);
            var text = string.Join(delimiter, strings);
            sb.Append(text);
            return this;
        }

        public CodeBuilder AppendLineDelimited(string delimiter, IEnumerable<string> strings)
        {
            if (strings == null)
                return this;

            var stringList = strings.ToList();

            if (stringList.Count == 1)
            {
                AppendLine(stringList[0]);
                return this;
            }

            for (var i = 0; i < stringList.Count - 1; i++ )
                AppendLine($"{stringList[i]}{delimiter}");

            AppendLine(stringList.Last());
            return this;
        }

        public CodeBuilder AppendLineDelimited<T>(string delimiter, IEnumerable<T> items, Func<T, String> expression)
        {
            var strings = items.Select(expression);
            return AppendLineDelimited(delimiter, strings);
        }
    }
}
