using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Informix4GLLanguage
{
    public class Informix4GLFactory
    {
        private static Informix4GLGrammar.Informix4GLGrammar _grammar;
        private static Irony.Parsing.Parser _parser;

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
    }
}
