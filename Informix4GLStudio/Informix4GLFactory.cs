using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Informix4GLLanguage
{
    public delegate void TreeNodeAnalyzer(ParseTreeNode node);

    public class Informix4GLFactory
    {
        private static Informix4GLGrammar.Informix4GLGrammar _grammar;
        private static Irony.Parsing.Parser _parser;
        private static string _currentText;
        private static ParseTree _currentParseTree;
        private static ParseTree _lastGoodParseTree;

        public static ParseTree LastGoodParseTree
        {
            get
            {
                return _lastGoodParseTree;
            }
        }

        private static List<string> _globalVariables;
        private static Dictionary<string, List<string>> _functionVariables;

        public static List<string> GlobalVariables
        {
            get
            {
                if (_globalVariables == null)
                {
                    _globalVariables = new List<string>();
                }
                return _globalVariables;
            }
        }

        public static Dictionary<string, List<string>> FunctionVariables
        {
            get
            {
                if (_functionVariables == null)
                {
                    _functionVariables = new Dictionary<string, List<string>>();
                }
                return _functionVariables;
            }
        }

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

        private static void ClearState()
        {
            GlobalVariables.Clear();
            FunctionVariables.Clear();
        }

        public static ParseTree Parse(string text)
        {
            if (string.Compare(_currentText, text) != 0)
            {
                _currentText = text;
                if (_currentParseTree != null && _currentParseTree.Root != null)
                {
                    _lastGoodParseTree = _currentParseTree;
                }
                _currentParseTree = _parser.Parse(text);
                if (_currentParseTree.Root != null)
                {
                    ClearState();
                    // We can now walk the parse tree and do some simple semantic analysis
                    // Such as:
                    // 1) Collect global variables
                    // 2) Collect type definitions
                    // 3) Collect a list of local functions and their scope variables
                    AnalyzeParseTree(_currentParseTree.Root, new List<KeyValuePair<string, TreeNodeAnalyzer>>()
                        {
                            new KeyValuePair<string, TreeNodeAnalyzer>("globalDeclaration", GlobalsAnalyzer),
                            new KeyValuePair<string, TreeNodeAnalyzer>("mainBlock", MainBlockAnalyzer),
                            new KeyValuePair<string, TreeNodeAnalyzer>("functionOrReportDefinitions", FunctionOrReportDefsAnalyzer)
                        });
                }
            }
            return _currentParseTree;
        }

        private static void AnalyzeParseTree(ParseTreeNode root, List<KeyValuePair<string, TreeNodeAnalyzer>> functionList)
        {
            ParseTreeNode desiredNode;
            foreach (var child in _currentParseTree.Root.ChildNodes)
            {
                foreach (var analyzer in functionList)
                {
                    desiredNode = GetDesiredNode(analyzer.Key, child);
                    if (desiredNode != null)
                    {
                        analyzer.Value(desiredNode);
                        break;
                    }
                }
            }
        }

        private static void GlobalsAnalyzer(ParseTreeNode node)
        {
            node = node.LastChild;
            node = node.FirstChild;
            // node is now typeDeclarations or StringLiteral
            if (node.Term.Name == "StringLiteral")
            {
                // TODO: fetch the globals from the file specified
            }
            else
            {
                ParseTreeNode nodeToUse;
                foreach (var typeDeclaration in node.ChildNodes)
                {
                    int j = 0;
                    nodeToUse = typeDeclaration.ChildNodes[1].FirstChild.FirstChild;
                    foreach (var identAndTypePair in nodeToUse.ChildNodes)
                    {
                        // TODO: get the type

                        foreach (var constIdent in identAndTypePair.FirstChild.ChildNodes)
                        {
                            if (constIdent.FirstChild != null)
                                GlobalVariables.Add(constIdent.FirstChild.Token.Text);
                            else
                                GlobalVariables.Add(constIdent.Token.Text);
                        }

                    }
                }
            }
        }

        private static void FunctionOrReportDefsAnalyzer(ParseTreeNode node)
        {
            List<string> functionVariables = new List<string>();
            string functionName = null;
            foreach (var functionOrReportDef in node.ChildNodes)
            {
                if (functionOrReportDef.FirstChild.Term.Name == "functionDefinition")
                {
                    // get the function's name
                    ParseTreeNode tempNode = functionOrReportDef.FirstChild.ChildNodes[1];
                    Token tempToken;
                    while ((tempToken = tempNode.Token) == null)
                        tempNode = tempNode.FirstChild;
                    functionName = tempToken.Text;

                    // gather the function's arguments (will be stored as variables)
                    ParseTreeNode args = functionOrReportDef.FirstChild.ChildNodes[2].ChildNodes[1];
                    if (args.Token == null)
                    {
                        // we have arguments
                        int j = 0;
                        if (args.FirstChild != null)
                        {
                            // TODO: supposedly the language supports having params not seperated by commas
                            // called parameter groups. I've never seen this before, so not handling it right now.
                            foreach (var param in args.FirstChild.ChildNodes)
                            {
                                if (param.Token != null)
                                {
                                    functionVariables.Add(param.Token.ValueString);
                                }
                            }
                        }
                    }

                    // get the function's variables
                    ParseTreeNode nodeToUse;
                    foreach (var typeDeclaration in functionOrReportDef.FirstChild.ChildNodes[3].ChildNodes)
                    {
                        int j = 0;
                        nodeToUse = typeDeclaration.ChildNodes[1].FirstChild.FirstChild;
                        foreach (var identAndTypePair in nodeToUse.ChildNodes)
                        {
                            // TODO: get the type

                            foreach (var constIdent in identAndTypePair.FirstChild.ChildNodes)
                            {
                                if (constIdent.FirstChild != null)
                                    functionVariables.Add(constIdent.FirstChild.Token.Text);
                                else
                                    functionVariables.Add(constIdent.Token.Text);
                            }

                        }
                    }
                }
                else
                {
                    // report definition
                    // get the report's name
                    ParseTreeNode tempNode = functionOrReportDef.FirstChild.ChildNodes[1];
                    Token tempToken;
                    while ((tempToken = tempNode.Token) == null)
                        tempNode = tempNode.FirstChild;
                    functionName = tempToken.Text;

                    // gather the function's arguments (will be stored as variables)
                    ParseTreeNode args = functionOrReportDef.FirstChild.ChildNodes[2].ChildNodes[1];
                    if (args.Token == null)
                    {
                        // we have arguments
                        int j = 0;
                        if (args.FirstChild != null)
                        {
                            // TODO: supposedly the language supports having params not seperated by commas
                            // called parameter groups. I've never seen this before, so not handling it right now.
                            foreach (var param in args.FirstChild.ChildNodes)
                            {
                                if (param.Token != null)
                                {
                                    functionVariables.Add(param.Token.ValueString);
                                }
                            }
                        }
                    }

                    // get the function's variables
                    ParseTreeNode nodeToUse;
                    foreach (var typeDeclaration in functionOrReportDef.FirstChild.ChildNodes[3].FirstChild.FirstChild.ChildNodes)
                    {
                        int j = 0;
                        nodeToUse = typeDeclaration.ChildNodes[1].FirstChild.FirstChild;
                        foreach (var identAndTypePair in nodeToUse.ChildNodes)
                        {
                            // TODO: get the type

                            foreach (var constIdent in identAndTypePair.FirstChild.ChildNodes)
                            {
                                if (constIdent.FirstChild != null)
                                    functionVariables.Add(constIdent.FirstChild.Token.Text);
                                else
                                    functionVariables.Add(constIdent.Token.Text);
                            }

                        }
                    }
                }
            }

            if (functionName != null && !FunctionVariables.ContainsKey(functionName))
            {
                FunctionVariables.Add(functionName, functionVariables);
            }
        }

        private static void FunctionAnalyzer(ParseTreeNode node)
        {

        }

        private static void MainBlockAnalyzer(ParseTreeNode node)
        {
            node = node.ChildNodes[1];

            ParseTreeNode nodeToUse;
            List<string> mainVariables = new List<string>();
            foreach (var typeDeclaration in node.ChildNodes)
            {
                int j = 0;
                nodeToUse = typeDeclaration.ChildNodes[1].FirstChild.FirstChild;
                foreach (var identAndTypePair in nodeToUse.ChildNodes)
                {
                    // TODO: get the type

                    foreach (var constIdent in identAndTypePair.FirstChild.ChildNodes)
                    {
                        if (constIdent.FirstChild != null)
                            mainVariables.Add(constIdent.FirstChild.Token.Text);
                        else
                            mainVariables.Add(constIdent.Token.Text);
                    }

                }
            }
            FunctionVariables.Add("main", mainVariables);
        }

        private static void ReportAnalyzer(ParseTreeNode node)
        {

        }

        private static ParseTreeNode GetDesiredNode(string name, ParseTreeNode node)
        {
            // This algorithm for getting down to the correct
            while (node.Term.Name.StartsWith("Unnamed") ||
                  !node.Term.Name.StartsWith(name))
            {
                node = node.FirstChild;
                if (node == null)
                    return null;
            }

            if (node.Term.Name.EndsWith("?"))
            {
                node = node.FirstChild;
            }

            return node;
        }

        public static string GetParentFunctionName(ParseTreeNode root, ParseTreeNode node)
        {
            String functionName = null;
            Stack<ParseTreeNode> nodeQueue = new Stack<ParseTreeNode>();
            nodeQueue.Push(root);

            ParseTreeNode temp;
            ParseTreeNode functionDef = null;
            bool ending = false;
            while (nodeQueue.Count > 0)
            {
                temp = nodeQueue.Pop();
                if (temp.ChildNodes != null && temp.ChildNodes.Count > 0)
                {
                    for (int i = temp.ChildNodes.Count - 1; i >= 0; i--)
                        nodeQueue.Push(temp.ChildNodes[i]);
                }

                if (temp.Term.Name == "mainBlock" ||
                        temp.Term.Name == "functionDefinition" ||
                        temp.Term.Name == "reportDefinition")
                {
                    functionDef = temp;
                }

                if (temp.Token != null)
                {
                    if (functionDef != null)
                    {
                        if (temp.Token.ValueString == "end")
                            ending = true;
                        else
                        {
                            if (ending)
                            {
                                switch (temp.Token.ValueString)
                                {
                                    case "main":
                                        if (functionDef.Term.Name == "mainBlock")
                                            functionDef = null;
                                        break;
                                    case "function":
                                        if (functionDef.Term.Name == "functionDefinition")
                                            functionDef = null;
                                        break;
                                    case "report":
                                        if (functionDef.Term.Name == "reportDefinition")
                                            functionDef = null;
                                        break;
                                }
                                ending = false;
                            }
                        }
                    }
                    if (temp.Token.Location.Position >= node.Token.Location.Position)
                    {
                        break;
                    }
                }
            }

            if (functionDef != null)
            {
                if (functionDef.Term.Name == "mainBlock")
                {
                    functionName = "main";
                }
                else
                {
                    int i = 0;
                    if (functionDef.Term.Name == "functionDefinition")
                        functionName = functionDef.ChildNodes[1].FirstChild.FirstChild.Token.ValueString;
                    else
                        functionName = functionDef.ChildNodes[1].Token.ValueString;
                }
            }
            return functionName;
        }
    }
}
