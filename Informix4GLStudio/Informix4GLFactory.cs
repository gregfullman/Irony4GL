using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Informix4GLLanguage
{
    public class Informix4GLFactory
    {
        private static Informix4GLGrammar.Informix4GLGrammar _grammar;
        private static Irony.Parsing.Parser _parser;
        private static string _currentText;
        private static ParseTree _currentParseTree;

        private Informix4GLFactory() { }

        public static Informix4GLGrammar.Informix4GLGrammar Grammar
        {
            get
            {
                if (_grammar == null)
                {
                    _grammar = new Informix4GLGrammar.Informix4GLGrammar();
                }
                return _grammar;
            }
        }

        public static Irony.Parsing.Parser Parser
        {
            get
            {
                if (_parser == null)
                {
                    _parser = new Irony.Parsing.Parser(Grammar);
                }
                return _parser;
            }
        }

        public static ParseTree Parse(string text)
        {
            if (string.Compare(_currentText, text) != 0)
            {
                _currentText = text;
                _currentParseTree = _parser.Parse(text);
            }
            return _currentParseTree;
        }
    }
}
