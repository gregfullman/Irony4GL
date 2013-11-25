using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using System.Globalization;

namespace Informix4GLGrammar
{
    public static class Informix4GLTerminalFactory
    {
        public static IdentifierTerminal CreateIdentifier(string name)
        {
            IdentifierTerminal id = new IdentifierTerminal(name, IdOptions.AllowsEscapes | IdOptions.CanStartWithEscape);
            //id.AddPrefix("@", IdOptions.IsNotKeyword);
            //From spec:
            //Start char is "_" or letter-character, which is a Unicode character of classes Lu, Ll, Lt, Lm, Lo, or Nl 
            id.StartCharCategories.AddRange(new UnicodeCategory[] {
         UnicodeCategory.UppercaseLetter, //Ul
         UnicodeCategory.LowercaseLetter, //Ll
         UnicodeCategory.TitlecaseLetter, //Lt
         UnicodeCategory.ModifierLetter,  //Lm
         UnicodeCategory.OtherLetter,     //Lo
         UnicodeCategory.LetterNumber     //Nl
      });
            //Internal chars
            /* From spec:
            identifier-part-character: letter-character | decimal-digit-character | connecting-character |  combining-character |
                formatting-character
      */
            id.CharCategories.AddRange(id.StartCharCategories); //letter-character categories
            id.CharCategories.AddRange(new UnicodeCategory[] {
        UnicodeCategory.DecimalDigitNumber, //Nd
        UnicodeCategory.ConnectorPunctuation, //Pc
        UnicodeCategory.SpacingCombiningMark, //Mc
        UnicodeCategory.NonSpacingMark,       //Mn
        UnicodeCategory.Format                //Cf
      });
            //Chars to remove from final identifier
            id.CharsToRemoveCategories.Add(UnicodeCategory.Format);
            return id;
        }

        public static StringLiteral CreateString(string name)
        {
            StringLiteral term = new StringLiteral(name, "\"", StringOptions.AllowsAllEscapes);
            term.AddPrefix("@", StringOptions.NoEscapes | StringOptions.AllowsLineBreak | StringOptions.AllowsDoubledQuote);
            term.AddStartEnd("\'", StringOptions.AllowsDoubledQuote | StringOptions.AllowsLineBreak);
            return term;
        }
    }

    [Language("4GL", "1.0", "Informix 4GL Grammar")]
    public class Informix4GLGrammar : Grammar
    {
        

        public Informix4GLGrammar() : base(false)
        {
            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;

            // Use C#'s string literal for right now
            StringLiteral StringLiteral = Informix4GLTerminalFactory.CreateString("StringLiteral");

            // Use C#'s char literal for right now
            StringLiteral CharLiteral = TerminalFactory.CreateCSharpChar("CharLiteral");

            // Use C#'s number for right now
            NumberLiteral Number = TerminalFactory.CreateCSharpNumber("Number");

            // Use C#'s identifier for right now
            IdentifierTerminal Identifier = Informix4GLTerminalFactory.CreateIdentifier("Identifier");

            #region Symbols
            KeyTerm plus = ToTerm("+", "plus");
            KeyTerm minus = ToTerm("-", "minus");
            KeyTerm div = ToTerm("/", "div");
            KeyTerm colon = ToTerm(":", "colon");
            KeyTerm semi = ToTerm(";", "semi");
            KeyTerm singleEqual = ToTerm("=");
            KeyTerm doubleEqual = ToTerm("==");
            KeyTerm doubleOr = ToTerm("||");
            KeyTerm nequal = ToTerm("!=", "nequal");
            KeyTerm le = ToTerm("<=", "le");
            KeyTerm lt = ToTerm("<", "lt");
            KeyTerm ge = ToTerm(">=", "ge");
            KeyTerm gt = ToTerm(">", "gt");
            KeyTerm plusEqual = ToTerm("+=");
            KeyTerm dot = ToTerm(".", "dot");
            KeyTerm comma = ToTerm(",", "comma");
            KeyTerm star = ToTerm("*", "star");
            KeyTerm Lbr = ToTerm("[");
            KeyTerm Rbr = ToTerm("]");
            KeyTerm LCbr = ToTerm("{");
            KeyTerm RCbr = ToTerm("}");
            KeyTerm Lpar = ToTerm("(");
            KeyTerm Rpar = ToTerm(")");
            KeyTerm AtSym = ToTerm("@");
            KeyTerm AndSym = ToTerm("&");

            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");
            RegisterBracePair("[", "]");
            #endregion

            #region Comments
            // Handle comments
            CommentTerminal Comment = new CommentTerminal("Comment", "#", "\r", "\n", "\u2085", "\u2028", "\u2029");
            NonGrammarTerminals.Add(Comment);
            #endregion

            #region NonTerminals
            // Non-Terminals
            var constantIdentifier = new NonTerminal("constantIdentifier");
            var compilation_unit = new NonTerminal("compilation_unit");
            var databaseDeclaration = new NonTerminal("databaseDeclaration");
            var globalDeclaration = new NonTerminal("globalDeclaration");
            var typeDeclarations = new NonTerminal("typeDeclarations");
            var mainBlock = new NonTerminal("mainBlock");
            var functionOrReportDefinitions = new NonTerminal("functionOrReportDefinitions");

            var mainStatements = new NonTerminal("mainStatements");
            var deferStatementOrCodeBlock = new NonTerminal("deferStatementOrCodeBlock");
            var deferStatement = new NonTerminal("deferStatement");
            var codeBlock = new NonTerminal("codeBlock");

            var reportOrFunctionDefinition = new NonTerminal("reportOrFunctionDefinition");
            var reportDefinition = new NonTerminal("reportDefinition");
            var functionDefinition = new NonTerminal("functionDefinition");

            var returnStatement = new NonTerminal("returnStatement");
            var variableOrConstantList = new NonTerminal("variableOrConstantList");

            var functionIdentifier = new NonTerminal("functionIdentifier");
            
            var parameterList = new NonTerminal("parameterList");
            var parameterGroup = new NonTerminal("parameterGroup");
            var zeroOrMoreParametersGroups = new NonTerminal("zeroOrMoreParametersGroups");

            var typeDeclaration = new NonTerminal("typeDeclaration");
            var oneOrMoreVariableDeclarations = new NonTerminal("zeroOrMoreVariableDeclarations");
            var variableDeclaration = new NonTerminal("variableDeclaration");
            
            var type = new NonTerminal("type");
            var oneOrMoreConstantIdentifiers = new NonTerminal("oneOrMoreConstantIdentifiers");
            var constantIdentifierAndTypePair = new NonTerminal("constantIdentifierAndTypePair");
            var oneOrMoreConstantIdentifierAndTypePairs = new NonTerminal("oneOrMoreConstantIdentifierAndTypePairs");

            var typeIdentifier = new NonTerminal("typeIdentifier");
            var indirectType = new NonTerminal("indirectType");
            var largeType = new NonTerminal("largeType");
            var structuredType = new NonTerminal("structuredType");
            var tableIdentifier = new NonTerminal("tableIdentifier");
            var charType = new NonTerminal("charType");
            var numberType = new NonTerminal("numberType");
            var timeType = new NonTerminal("timeType");
            var numericConstant = new NonTerminal("numericConstant");
            var sign = new NonTerminal("sign");
            var datetimeQualifier = new NonTerminal("datetimeQualifier");
            var intervalQualifier = new NonTerminal("intervalQualifier");
            var yearQualifier = new NonTerminal("yearQualifier");
            var monthQualifier = new NonTerminal("monthQualifier");
            var dayQualifier = new NonTerminal("dayQualifier");
            var hourQualifier = new NonTerminal("hourQualifier");
            var minuteQualifier = new NonTerminal("minuteQualifier");
            var secondQualifier = new NonTerminal("secondQualifier");
            var fractionQualifier = new NonTerminal("fractionQualifier");
            var unitType = new NonTerminal("unitType");

            var recordType = new NonTerminal("recordType");
            var arrayType = new NonTerminal("arrayType");
            var dynArrayType = new NonTerminal("dynArrayType");
            var arrayIndexer = new NonTerminal("arrayIndexer");

            var statement = new NonTerminal("statement");
            var label = new NonTerminal("label");
            var unlabelledStatement = new NonTerminal("unlabelledStatement");
            var statementOrDbDeclaration = new NonTerminal("statementOrDbDeclaration");
            var simpleStatement = new NonTerminal("simpleStatement");
            var structuredStatement = new NonTerminal("structuredStatement");

            var assignmentStatement = new NonTerminal("assignmentStatement");
            var procedureStatement = new NonTerminal("procedureStatement");
            var sqlStatements = new NonTerminal("sqlStatements");
            var otherFGLStatement = new NonTerminal("otherFGLStatement");
            var menuInsideStatement = new NonTerminal("menuInsideStatement");
            var displayInsideStatement = new NonTerminal("displayInsideStatement");
            var inputOrConstructInsideStatement = new NonTerminal("inputOrConstructInsideStatement");

            var runStatement = new NonTerminal("runStatement");
            var variable = new NonTerminal("variable");
            var expression = new NonTerminal("expression");
            var procedureIdentifier = new NonTerminal("procedureIdentifier");
            var actualParameter = new NonTerminal("actualParameter");
            var oneOrMoreExpressions = new NonTerminal("oneOrMoreExpressions");
            var oneOrMoreActualParameters = new NonTerminal("oneOrMoreActualParameters");
            var oneOrMoreVariables = new NonTerminal("oneOrMoreVariables");
            var gotoStatement = new NonTerminal("gotoStatement");

            var condition = new NonTerminal("condition");
            var oneOrMoreLogicalTerms = new NonTerminal("oneOrMoreLogicalTerms");
            var logicalTerm = new NonTerminal("logicalTerm");
            var oneOrMoreLogicalFactors = new NonTerminal("oneOrMoreLogicalFactors");
            var logicalFactor = new NonTerminal("logicalFactor");
            var sqlExpression = new NonTerminal("sqlExpression");
            var expressionSet = new NonTerminal("expressionSet");
            var quantifiedFactor = new NonTerminal("quantifiedFactor");
            var relationalOperator = new NonTerminal("relationalOperator");
            var subquery = new NonTerminal("subquery");
            var sqlSelectStatement = new NonTerminal("sqlSelectStatement");
            var sqlTerm = new NonTerminal("sqlTerm");
            var sqlAlias = new NonTerminal("sqlAlias");
            var sqlFactor = new NonTerminal("sqlFactor");
            var sqlMultiply = new NonTerminal("sqlMultiply");
            var sqlFactor2 = new NonTerminal("sqlFactor2");
            var sqlLiteral = new NonTerminal("sqlLiteral");
            var sqlVariable = new NonTerminal("sqlVariable");
            var groupFunction = new NonTerminal("groupFunction");
            var sqlFunction = new NonTerminal("sqlFunction");
            var sqlExpressionList = new NonTerminal("sqlExpressionList");
            var oneOrMoreSqlExpressions = new NonTerminal("oneOrMoreSqlExpressions");
            var unsignedConstant = new NonTerminal("unsignedConstant");

            var numberFunction = new NonTerminal("numberFunction");
            var charFunction = new NonTerminal("charFunction");
            var dateFunction = new NonTerminal("dateFunction");
            var otherFunction = new NonTerminal("otherFunction");
            var columnsTableId = new NonTerminal("columnsTableId");
            //var ifCondition = new NonTerminal("ifCondition");
            //var oneOrMoreIfCondition2s = new NonTerminal("oneOrMoreIfCondition2s");
            //var ifLogicalTerm = new NonTerminal("ifLogicalTerm");
            //var oneOrMoreIfLogicalTerms = new NonTerminal("oneOrMoreIfLogicalTerms");
            //var ifLogicalFactor = new NonTerminal("ifLogicalFactor");
            //var oneOrMoreIfLogicalFactors = new NonTerminal("oneOrMoreIfLogicalFactors");

            var simpleExpression = new NonTerminal("simpleExpression");
            //var term = new NonTerminal("term");
            //var oneOrMoreTerms = new NonTerminal("oneOrMoreTerms");
            var addingOperator = new NonTerminal("addingOperator");
            var oneOrMoreFactors = new NonTerminal("oneOrMoreFactors");
            //var multiplyingOperator = new NonTerminal("multiplyingOperator");
            //var factor = new NonTerminal("factor");
            var constant = new NonTerminal("constant");

            var entireVariable = new NonTerminal("entireVariable");
            var componentVariable = new NonTerminal("componentVariable");
            var variableIdentifier = new NonTerminal("variableIdentifier");
            var indexingVariable = new NonTerminal("indexingVariable");
            var recordVariable = new NonTerminal("recordVariable");

            var fieldIdentifier = new NonTerminal("fieldIdentifier");
            var conditionalStatement = new NonTerminal("conditionalStatement");
            var repetetiveStatement = new NonTerminal("repetetiveStatement");
            var ifStatement = new NonTerminal("ifStatement");
            var caseStatement = new NonTerminal("caseStatement");
            var whileStatement = new NonTerminal("whileStatement");
            var forEachStatement = new NonTerminal("forEachStatement");
            var forStatement = new NonTerminal("forStatement");
            var controlVariable = new NonTerminal("controlVariable");
            var forList = new NonTerminal("forList");
            var initialValue = new NonTerminal("initialValue");
            var finalValue = new NonTerminal("finalValue");

            var variableList = new NonTerminal("variableList");
            var whenExpression = new NonTerminal("whenExpression");
            var whenIf = new NonTerminal("whenIf");
            var zeroOrMoreWhenExpressions = new NonTerminal("zeroOrMoreWhenExpressions");
            var zeroOrMoreWhenIfs = new NonTerminal("zeroOrMoreWhenIfs");

            var clippedUsing = new NonTerminal("clippedUsing");
            var zeroOrMoreClippedUsings = new NonTerminal("zeroOrMoreClippedUsings");

            var otherProgramFlowStatement = new NonTerminal("otherProgramFlowStatement");
            var otherStorageStatement = new NonTerminal("otherStorageStatement");
            var reportStatement = new NonTerminal("reportStatement");
            var screenStatement = new NonTerminal("screenStatement");
            var exitStatements = new NonTerminal("exitStatements");
            var continueStatements = new NonTerminal("continueStatements");
            var exitTypes = new NonTerminal("exitTypes");

            var printExpressionItem = new NonTerminal("printExpressionItem");
            var printExpressionList = new NonTerminal("printExpressionList");
            var reportDimensionSpecifier = new NonTerminal("reportDimensionSpecifier");
            var zeroOrMoreReportDimensionSpecifiers = new NonTerminal("zeroOrMoreReportDimensionSpecifiers");

            var thruNotation = new NonTerminal("thruNotation");
            var fieldName = new NonTerminal("fieldName");
            var fieldList = new NonTerminal("fieldList");
            var keyList = new NonTerminal("keyList");
            var constructEvents = new NonTerminal("constructEvents");
            var specialAttribute = new NonTerminal("specialAttribute");
            var displayAttribute = new NonTerminal("displayAttribute");
            var controlAttribute = new NonTerminal("controlAttribute");
            var oneOrMoreSpecialAttributes = new NonTerminal("attribute");
            var attribute = new NonTerminal("attribute");
            var attributeList = new NonTerminal("attributeList");
            var constructGroupStatement = new NonTerminal("constructGroupStatement");
            var oneOrMoreCodeBlocks = new NonTerminal("oneOrMoreCodeBlocks");
            var constructStatement = new NonTerminal("constructStatement");
            var oneOrMoreConstructGroupStatements = new NonTerminal("oneOrMoreConstructGroupStatements");
            var columnsList = new NonTerminal("columnsList");

            var displayArrayStatement = new NonTerminal("displayArrayStatement");
            var displayEvents = new NonTerminal("displayEvents");
            var zeroOrMoreDisplayEvents = new NonTerminal("zeroOrMoreDisplayEvents");
            var displayStatement = new NonTerminal("displayStatement");
            var errorStatement = new NonTerminal("errorStatement");
            var messageStatement = new NonTerminal("messageStatement");
            var promptStatement = new NonTerminal("promptStatement");
            var zeroOrMoreKeyListCodeBlocks = new NonTerminal("zeroOrMoreKeyListCodeBlocks");
            var keyListCodeBlock = new NonTerminal("keyListCodeBlock");

            var inputEvents = new NonTerminal("inputEvents");
            var inputGroupStatement = new NonTerminal("inputGroupStatement");
            var inputStatement = new NonTerminal("inputStatement");
            var inputArrayStatement = new NonTerminal("inputArrayStatement");
            var zeroOrMoreCodeBlocks = new NonTerminal("zeroOrMoreCodeBlocks");
            var oneOrMoreInputGroupStatements = new NonTerminal("oneOrMoreInputGroupStatements");

            var menuEvents = new NonTerminal("menuEvents");
            var menuGroupStatement = new NonTerminal("menuGroupStatement");
            var menuStatement = new NonTerminal("menuStatement");
            var zeroOrMoreMenuGroupStatements = new NonTerminal("zeroOrMoreMenuGroupStatements");
            var additionalExpression = new NonTerminal("additionalExpression");
            var zeroOrMoreAdditionalExpressions = new NonTerminal("zeroOrMoreAdditionalExpressions");

            var reservedLinePosition = new NonTerminal("reservedLinePosition");
            var specialWindowAttribute = new NonTerminal("specialWindowAttribute");
            var oneOrMoreSpecialWindowAttributes = new NonTerminal("oneOrMoreSpecialWindowAttributes");
            var windowAttribute = new NonTerminal("windowAttribute");
            var windowAttributeList = new NonTerminal("windowAttributeList");

            var optionStatement = new NonTerminal("optionStatement");
            var optionsStatement = new NonTerminal("optionsStatement");
            var oneOrMoreOptionsStatements = new NonTerminal("oneOrMoreOptionsStatements");
            var oneOrMoreFieldLists = new NonTerminal("oneOrMoreFieldLists");
            var cursorManipulationStatement = new NonTerminal("cursorManipulationStatement");
            var dataDefinitionStatement = new NonTerminal("dataDefinitionStatement");
            var dataManipulationStatement = new NonTerminal("dataManipulationStatement");
            var dynamicManagementStatement = new NonTerminal("dynamicManagementStatement");
            var queryOptimizationStatement = new NonTerminal("queryOptimizationStatement");
            var dataIntegrityStatement = new NonTerminal("dataIntegrityStatement");
            var clientServerStatement = new NonTerminal("clientServerStatement");

            var cursorName = new NonTerminal("cursorName");
            var sqlInsertStatement = new NonTerminal("sqlInsertStatement");
            var statementId = new NonTerminal("statementId");

            var dataType = new NonTerminal("dataType");
            var columnItem = new NonTerminal("columnItem");
            var oneOrMoreColumnItems = new NonTerminal("oneOrMoreColumnItems");
            var oneOrMoreConstantIdentifiersWithAscDesc = new NonTerminal("oneOrMoreConstantIdentifiersWithAscDesc");
            var constantIdentifierWithAscDesc = new NonTerminal("constantIdentifierWithAscDesc");

            var sqlDeleteStatement = new NonTerminal("sqlDeleteStatement");
            var sqlUpdateStatement = new NonTerminal("sqlUpdateStatement");
            var sqlLoadStatement = new NonTerminal("sqlLoadStatement");
            var sqlUnLoadStatement = new NonTerminal("sqlUnLoadStatement");
            var mainSelectStatement = new NonTerminal("mainSelectStatement");
            var selectList = new NonTerminal("selectList");
            var sqlExpressionsWithSqlAlias = new NonTerminal("sqlExpressionsWithSqlAlias");
            var headSelectStatement = new NonTerminal("headSelectStatement");
            var tableQualifier = new NonTerminal("tableQualifier");
            var fromTable = new NonTerminal("fromTable");
            var tableExpression = new NonTerminal("tableExpression");
            var simpleSelectStatement = new NonTerminal("simpleSelectStatement");
            var fromSelectStatement = new NonTerminal("fromSelectStatement");
            var oneOrMoreFromTableExpressions = new NonTerminal("oneOrMoreFromTableExpressions");
            var fromTableExpression = new NonTerminal("fromTableExpression");
            var aliasName = new NonTerminal("aliasName");
            var whereStatement = new NonTerminal("whereStatement");
            var groupByStatement = new NonTerminal("groupByStatement");
            var havingStatement = new NonTerminal("havingStatement");
            var unionSelectStatement = new NonTerminal("unionSelectStatement");
            var orderbyStatement = new NonTerminal("orderbyStatement");
            var orderbyColumn = new NonTerminal("orderbyColumn");
            var oneOrMoreOrderByColumns = new NonTerminal("oneOrMoreOrderByColumns");
            var oneOrMoreColumnsTableIdEqualExpressions = new NonTerminal("oneOrMoreColumnsTableIdEqualExpressions");
            var columnsTableIdEqualExpression = new NonTerminal("columnsTableIdEqualExpression");
            var wheneverStatement = new NonTerminal("wheneverStatement");
            var wheneverType = new NonTerminal("wheneverType");
            var wheneverFlow = new NonTerminal("wheneverFlow");

            var outputReport = new NonTerminal("outputReport");
            var formatReport = new NonTerminal("formatReport");
            var oneOrMoreReportCodeBlocks = new NonTerminal("oneOrMoreReportCodeBlocks");
            var reportCodeBlock = new NonTerminal("reportCodeBlock");

            var includeDefinitions = new NonTerminal("includeDefinitions");
            var includeDefinition = new NonTerminal("includeDefinition");
            var typeDefinition = new NonTerminal("typeDefinition");
            var typeDefinitions = new NonTerminal("typeDefinitions");

            //var builtInClassType = new NonTerminal("builtInClassType");
            //var uiClassType = new NonTerminal("uiClassType");
            //var baseClassType = new NonTerminal("baseClassType");
            //var omClassType = new NonTerminal("omClassType");
            var classChain = new NonTerminal("classChain");
            //var extensionLibraryTypes = new NonTerminal("extensionLibraryTypes");
            //var mathLibraryType = new NonTerminal("mathLibraryType");
            //var osLibraryType = new NonTerminal("osLibraryType");

            var primaryExpression = new NonTerminal("primaryExpression");
            var parenthesizedExpression = new NonTerminal("parenthesizedExpression");
            var binaryOperatorExpression = new NonTerminal("binaryOperatorExpression");
            var unaryExpression = new NonTerminal("unaryExpression");
            var literal = new NonTerminal("literal");
            var binaryOperator = new NonTerminal("binaryOperator");
            var unaryOperator = new NonTerminal("unaryOperator");
            var memberAccess = new NonTerminal("memberAccess");
            var memberAccessSegment = new NonTerminal("memberAccessSegment");
            var memberAccessSegmentOpt = new NonTerminal("memberAccessSegmentOpt");
            var argument = new NonTerminal("argument");
            var argumentList = new NonTerminal("argumentList");
            var argumentListOpt = new NonTerminal("argumentListOpt");
            var argumentListPar = new NonTerminal("argumentListPar");
            var expressionList = new NonTerminal("expressionList");
            #endregion

            #region Keywords
            var INCLUDE = Keyword("include");
            var MAIN = Keyword("main");
            var END = Keyword("end");
            var DEFER = Keyword("defer");
            var INTERRUPT = Keyword("interrupt");
            var QUIT = Keyword("quit");
            var RETURN = Keyword("return");
            var FUNCTION = Keyword("function");
            var GLOBALS = Keyword("globals");
            var DEFINE = Keyword("define");
            var TYPE = Keyword("type");
            var LIKE = Keyword("like");
            var TEXT = Keyword("text");
            var BYTE = Keyword("byte");
            var BIGINT = Keyword("bigint");
            var INTEGER = Keyword("integer");
            var INT = Keyword("int");
            var SMALLINT = Keyword("smallint");
            var REAL = Keyword("real");
            var SMALLFLOAT = Keyword("smallfloat");
            var DECIMAL = Keyword("decimal");
            var DEC = Keyword("dec");
            var NUMERIC = Keyword("numeric");
            var MONEY = Keyword("money");
            var FLOAT = Keyword("float");
            var DOUBLE = Keyword("double");
            var VARCHAR = Keyword("varchar");
            var NVARCHAR = Keyword("nvarchar");
            var CHAR = Keyword("char");
            var NCHAR = Keyword("nchar");
            var CHARACTER = Keyword("character");
            var STRING = Keyword("string");
            var DATE = Keyword("date");
            var DATETIME = Keyword("datetime");
            var INTERVAL = Keyword("interval");
            var YEAR = Keyword("year");
            var TO = Keyword("to");
            var MONTH = Keyword("month");
            var DAY = Keyword("day");
            var HOUR = Keyword("hour");
            var MINUTE = Keyword("minute");
            var SECOND = Keyword("second");
            var FRACTION = Keyword("fraction");
            var RECORD = Keyword("record");
            var ARRAY = Keyword("array");
            var OF = Keyword("of");
            var DYNAMIC = Keyword("dynamic");
            var WITH = Keyword("with");
            var DIMENSIONS = Keyword("dimensions");
            var RUN = Keyword("run");
            var IN = Keyword("in");
            var FORM = Keyword("form");
            var MODE = Keyword("mode");
            var LINE = Keyword("line");
            var WITHOUT = Keyword("without");
            var WAITING = Keyword("waiting");
            var RETURNING = Keyword("returning");
            var LET = Keyword("let");
            var CALL = Keyword("call");
            var GOTO = Keyword("goto");
            var TRUE = Keyword("true");
            var FALSE = Keyword("false");
            var OR = Keyword("or");
            var AND = Keyword("and");
            var BETWEEN = Keyword("between");
            var IS = Keyword("is");
            var NULL = Keyword("null");
            var ALL = Keyword("all");
            var ANY = Keyword("any");
            var EXISTS = Keyword("exists");
            var AS = Keyword("as");
            var NOT = Keyword("not");
            var UNITS = Keyword("units");
            var DISTINCT = Keyword("distinct");
            var MOD = Keyword("mod");
            var LENGTH = Keyword("length");
            var AVG = Keyword("avg");
            var COUNT = Keyword("count");
            var MAX = Keyword("max");
            var MIN = Keyword("min");
            var SUM = Keyword("sum");
            var DECODE = Keyword("decode");
            var NVL = Keyword("nvl");
            var MATCHES = Keyword("matches");
            var CLIPPED = Keyword("clipped");
            var USING = Keyword("using");
            var COLUMN = Keyword("column");
            var EXTEND = Keyword("extend");
            var INFIELD = Keyword("infield");
            var PREPARE = Keyword("prepare");
            var ACCEPT = Keyword("accept");
            var ASCII = Keyword("ascii");
            var CURRENT = Keyword("current");
            var FIRST = Keyword("first");
            var FOUND = Keyword("found");
            var GROUP = Keyword("group");
            var HIDE = Keyword("hide");
            var INDEX = Keyword("index");
            var INT_FLAG = Keyword("int_flag");
            var LAST = Keyword("last");
            var LINENO = Keyword("lineno");
            var MDY = Keyword("mdy");
            var NO = Keyword("no");
            var NOTFOUND = Keyword("notfound");
            var PAGENO = Keyword("pageno");
            var SIZE = Keyword("size");
            var SQL = Keyword("sql");
            var STATUS = Keyword("status");
            var TEMP = Keyword("temp");
            var TIME = Keyword("time");
            var TODAY = Keyword("today");
            var USER = Keyword("user");
            var WAIT = Keyword("wait");
            var WEEKDAY = Keyword("weekday");
            var WORK = Keyword("work");
            var THROUGH = Keyword("through");
            var THRU = Keyword("thru");
            var IF = Keyword("if");
            var THEN = Keyword("then");
            var ELSE = Keyword("else");
            var WHILE = Keyword("while");
            var FOR = Keyword("for");
            var STEP = Keyword("step");
            var FOREACH = Keyword("foreach");
            var INTO = Keyword("into");
            var REOPTIMIZATION = Keyword("reoptimization");
            var WHEN = Keyword("when");
            var CASE = Keyword("case");
            var OTHERWISE = Keyword("otherwise");
            var SLEEP = Keyword("sleep");
            var CONSTRUCT = Keyword("construct");
            var DISPLAY = Keyword("display");
            var INPUT = Keyword("input");
            var MENU = Keyword("menu");
            var REPORT = Keyword("report");
            var EXIT = Keyword("exit");
            var PROGRAM = Keyword("program");
            var CONTINUE = Keyword("continue");
            var ALLOCATE = Keyword("allocate");
            var LOCATE = Keyword("locate");
            var MEMORY = Keyword("memory");
            var FILE = Keyword("file");
            var DEALLOCATE = Keyword("deallocate");
            var RESIZE = Keyword("resize");
            var FREE = Keyword("free");
            var INITIALIZE = Keyword("initialize");
            var VALIDATE = Keyword("validate");
            var SPACE = Keyword("space");
            var SPACES = Keyword("spaces");
            var WORDWRAP = Keyword("wordwrap");
            var RIGHT = Keyword("right");
            var MARGIN = Keyword("margin");
            var START = Keyword("start");
            var PIPE = Keyword("pipe");
            var PRINTER = Keyword("printer");
            var TERMINATE = Keyword("terminate");
            var FINISH = Keyword("finish");
            var PAUSE = Keyword("pause");
            var NEED = Keyword("need");
            var LINES = Keyword("lines");
            var PRINT = Keyword("print");
            var SKIP = Keyword("skip");
            var TOP = Keyword("top");
            var PAGE = Keyword("page");
            var OUTPUT = Keyword("output");
            var LEFT = Keyword("left");
            var BOTTOM = Keyword("bottom");
            var SAME = Keyword("same");
            var BEFORE = Keyword("before");
            var AFTER = Keyword("after");
            var FIELD = Keyword("field");
            var ON = Keyword("on");
            var KEY = Keyword("key");
            var DEFAULTS = Keyword("defaults");
            var BLACK = Keyword("black");
            var BLUE = Keyword("blue");
            var CYAN = Keyword("cyan");
            var GREEN = Keyword("green");
            var MAGENTA = Keyword("magenta");
            var RED = Keyword("red");
            var WHITE = Keyword("white");
            var YELLOW = Keyword("yellow");
            var BOLD = Keyword("bold");
            var DIM = Keyword("dim");
            var NORMAL = Keyword("normal");
            var INVISIBLE = Keyword("invisible");
            var BLINK = Keyword("blink");
            var UNDERLINE = Keyword("underline");
            var NAME = Keyword("name");
            var HELP = Keyword("help");
            var REVERSE = Keyword("reverse");
            var ORDER = Keyword("order");
            var UNBUFFERED = Keyword("unbuffered");
            var CANCEL = Keyword("cancel");
            var ATTRIBUTE = Keyword("attribute");
            var ATTRIBUTES = Keyword("attributes");
            var BY = Keyword("by");
            var FROM = Keyword("from");
            var AT = Keyword("at");
            var ERROR = Keyword("error");
            var MESSAGE = Keyword("message");
            var PROMPT = Keyword("prompt");
            var ROW = Keyword("row");
            var INSERT = Keyword("insert");
            var DELETE = Keyword("delete");
            var CHANGE = Keyword("change");
            var IDLE = Keyword("idle");
            var ACTION = Keyword("action");
            var NEXT = Keyword("next");
            var PREVIOUS = Keyword("previous");
            var COMMAND = Keyword("command");
            var SHOW = Keyword("show");
            var OPTION = Keyword("option");
            var BORDER = Keyword("border");
            var COMMENT = Keyword("comment");
            var OFF = Keyword("off");
            var WRAP = Keyword("wrap");
            var CONSTRAINED = Keyword("constrained");
            var UNCONSTRAINED = Keyword("unconstrained");
            var OPTIONS = Keyword("options");
            var CLEAR = Keyword("clear");
            var WINDOW = Keyword("window");
            var SCREEN = Keyword("screen");
            var CLOSE = Keyword("close");
            var OPEN = Keyword("open");
            var ROWS = Keyword("rows");
            var COLUMNS = Keyword("columns");
            var SCROLL = Keyword("scroll");
            var UP = Keyword("up");
            var DOWN = Keyword("down");
            var DECLARE = Keyword("declare");
            var CURSOR = Keyword("cursor");
            var HOLD = Keyword("hold");
            var UPDATE = Keyword("update");
            var FETCH = Keyword("fetch");
            var PRIOR = Keyword("prior");
            var RELATIVE = Keyword("relative");
            var ABSOLUTE = Keyword("absolute");
            var FLUSH = Keyword("flush");
            var PUT = Keyword("put");
            var TABLE = Keyword("table");
            var UNIQUE = Keyword("unique");
            var CONSTRAINT = Keyword("constraint");
            var DROP = Keyword("drop");
            var CREATE = Keyword("create");
            var LOG = Keyword("log");
            var EXTENT = Keyword("extent");
            var LOCK = Keyword("lock");
            var CLUSTER = Keyword("cluster");
            var ASC = Keyword("asc");
            var DESC = Keyword("desc");
            var SELECT = Keyword("select");
            var OUTER = Keyword("outer");
            var UNION = Keyword("union");
            var WHERE = Keyword("where");
            var HAVING = Keyword("having");
            var LOAD = Keyword("load");
            var DELIMITER = Keyword("delimiter");
            var UNLOAD = Keyword("unload");
            var VALUES = Keyword("values");
            var SET = Keyword("set");
            var EXECUTE = Keyword("execute");
            var SHARE = Keyword("share");
            var EXCLUSIVE = Keyword("exclusive");
            var STATISTICS = Keyword("statistics");
            var SECONDS = Keyword("seconds");
            var EXPLAIN = Keyword("explain");
            var ISOLATION = Keyword("isolation");
            var STABILITY = Keyword("stability");
            var DIRTY = Keyword("dirty");
            var COMMITTED = Keyword("committed");
            var REPEATABLE = Keyword("repeatable");
            var READ = Keyword("read");
            var BUFFERED = Keyword("buffered");
            var DATABASE = Keyword("database");
            var BEGIN = Keyword("begin");
            var COMMIT = Keyword("commit");
            var ROLLBACK = Keyword("rollback");
            var WHENEVER = Keyword("whenever");
            var SQLERROR = Keyword("sqlerror");
            var SQLWARNING = Keyword("sqlwarning");
            var WARNING = Keyword("warning");
            var STOP = Keyword("stop");
            var GO = Keyword("go");
            var EXTERNAL = Keyword("external");
            var FORMAT = Keyword("format");
            var EVERY = Keyword("every");
            var HEADER = Keyword("header");
            var TRAILER = Keyword("trailer");

            #endregion

            // initialize the root
            Root = compilation_unit;

            

            // Rules
            compilation_unit.Rule = databaseDeclaration.Q() +
                                    includeDefinitions.Q() +
                                    globalDeclaration.Q() +
                                    typeDefinitions +
                                    typeDeclarations +
                                    mainBlock.Q() +
                                    functionOrReportDefinitions;

            includeDefinition.Rule = AndSym + INCLUDE + StringLiteral;
            includeDefinitions.Rule = MakeStarRule(includeDefinitions, null, includeDefinition);

            deferStatementOrCodeBlock.Rule = deferStatement | codeBlock;    // derived
            
            mainStatements.Rule = MakeStarRule(mainStatements, deferStatementOrCodeBlock);
            mainBlock.Rule = MAIN + typeDeclarations + mainStatements + END + MAIN;
            deferStatement.Rule = DEFER + (INTERRUPT | QUIT);

            reportOrFunctionDefinition.Rule = reportDefinition | functionDefinition;    // derived
            functionOrReportDefinitions.Rule = MakeStarRule(functionOrReportDefinitions, reportOrFunctionDefinition);

            returnStatement.Rule = RETURN + variableOrConstantList.Q();

            functionDefinition.Rule = FUNCTION +
                                      functionIdentifier +
                                      parameterList +
                                      typeDeclarations +
                                      codeBlock.Q() +
                                      END + FUNCTION;

            parameterGroup.Rule = MakePlusRule(parameterGroup, comma, Identifier);

            // TODO: not sure about this grammar rule. It allows parameters to be seperated by spaces instead of commas...
            zeroOrMoreParametersGroups.Rule = MakeStarRule(zeroOrMoreParametersGroups, parameterGroup); // derived
            parameterList.Rule = Empty | (Lpar + zeroOrMoreParametersGroups + Rpar);

            globalDeclaration.Rule = GLOBALS + (StringLiteral | (typeDeclarations + END + GLOBALS));

            oneOrMoreVariableDeclarations.Rule = MakePlusRule(oneOrMoreVariableDeclarations, comma, variableDeclaration);
            typeDeclaration.Rule = DEFINE + oneOrMoreVariableDeclarations;
            typeDeclarations.Rule = MakeStarRule(typeDeclarations, typeDeclaration);

            typeDefinition.Rule = TYPE + Identifier + type;
            typeDefinitions.Rule = MakeStarRule(typeDefinitions, typeDefinition);

            oneOrMoreConstantIdentifiers.Rule = MakePlusRule(oneOrMoreConstantIdentifiers, comma, constantIdentifier);
            constantIdentifierAndTypePair.Rule = oneOrMoreConstantIdentifiers + type;
            oneOrMoreConstantIdentifierAndTypePairs.Rule = MakePlusRule(oneOrMoreConstantIdentifierAndTypePairs, comma, constantIdentifierAndTypePair);
            variableDeclaration.Rule = oneOrMoreConstantIdentifierAndTypePairs;

            type.Rule = typeIdentifier | indirectType | largeType | structuredType;
            indirectType.Rule = LIKE + tableIdentifier + dot + Identifier;
            typeIdentifier.Rule = charType | numberType | timeType | classChain;
            largeType.Rule = TEXT | BYTE;
            sign.Rule = plus | minus;
            numericConstant.Rule = Number | (sign + Number);

            /*builtInClassType.Rule = uiClassType | baseClassType | omClassType;
            uiClassType.Rule = "ui" + dot + (ToTerm("window") | "form" | "dialog" | "combobox" | "dragdrop");
            baseClassType.Rule = "base" + dot + (ToTerm("channel") | "stringbuffer" | "stringtokenizer" | "typeinfo" | "messageserver");
            omClassType.Rule = "om" + dot + (ToTerm("domnode") | "nodelist" | "saxattributes" | "saxdocumenthandler" | "xmlreader" | "xmlwriter");*/

            numberType.Rule = BIGINT | INTEGER | INT | SMALLINT | REAL | SMALLFLOAT |
                              ((DECIMAL | DEC | NUMERIC | MONEY) + ((Lpar + numericConstant + (comma + numericConstant).Q() + Rpar) | Empty)) |
                              ((FLOAT | DOUBLE) + ((Lpar + numericConstant + Rpar) | Empty));
            charType.Rule = ((VARCHAR | NVARCHAR) + Lpar + numericConstant + (comma + numericConstant).Q() + Rpar) |
                            ((CHAR | NCHAR | CHARACTER) + ((Lpar + numericConstant + Rpar) | Empty)) |
                            STRING;
            timeType.Rule = DATE | (DATETIME + datetimeQualifier) | (INTERVAL + intervalQualifier);
            datetimeQualifier.Rule = (YEAR + TO + yearQualifier) |
                                     (MONTH + TO + monthQualifier) |
                                     (DAY + TO + dayQualifier) |
                                     (HOUR + TO + hourQualifier) |
                                     (MINUTE + TO + minuteQualifier) |
                                     (SECOND + TO + secondQualifier) |
                                     (FRACTION + TO + fractionQualifier);
            intervalQualifier.Rule = (YEAR + (Lpar + numericConstant + Rpar).Q() + TO + yearQualifier) |
                                     (MONTH + (Lpar + numericConstant + Rpar).Q() + TO + monthQualifier) |
                                     (DAY + (Lpar + numericConstant + Rpar).Q() + TO + dayQualifier) |
                                     (HOUR + (Lpar + numericConstant + Rpar).Q() + TO + hourQualifier) |
                                     (MINUTE + (Lpar + numericConstant + Rpar).Q() + TO + minuteQualifier) |
                                     (SECOND + (Lpar + numericConstant + Rpar).Q() + TO + secondQualifier) |
                                     (FRACTION + TO + fractionQualifier);
            unitType.Rule = yearQualifier;
            yearQualifier.Rule = YEAR | monthQualifier;
            monthQualifier.Rule = MONTH | dayQualifier;
            dayQualifier.Rule = DAY | hourQualifier;
            hourQualifier.Rule = HOUR | minuteQualifier;
            minuteQualifier.Rule = MINUTE| secondQualifier;
            secondQualifier.Rule = SECOND | fractionQualifier;
            fractionQualifier.Rule = FRACTION + (Lpar + numericConstant + Rpar).Q();

            structuredType.Rule = recordType | arrayType | dynArrayType;
            recordType.Rule = RECORD + ((oneOrMoreVariableDeclarations + END + RECORD) | (LIKE + tableIdentifier + dot + star));
            arrayIndexer.Rule = Lbr + expressionList + Rbr;
            expressionList.Rule = MakePlusRule(expressionList, comma, expression);


            arrayType.Rule = ARRAY + arrayIndexer + OF + (recordType | typeIdentifier | largeType);
            dynArrayType.Rule = DYNAMIC + ARRAY + (WITH + numericConstant + DIMENSIONS).Q() + OF +
                                (recordType | typeIdentifier);

            statement.Rule = (label + colon).Q() + unlabelledStatement;
            statementOrDbDeclaration.Rule = statement | databaseDeclaration;
            codeBlock.Rule = MakePlusRule(codeBlock, statementOrDbDeclaration);
            label.Rule = Identifier;
            unlabelledStatement.Rule = simpleStatement | structuredStatement;

            simpleStatement.Rule = assignmentStatement |
                                   procedureStatement |
                                   (sqlStatements + semi.Q()) |
                                   otherFGLStatement |
                                   menuInsideStatement |
                                   displayInsideStatement |
                                   inputOrConstructInsideStatement;

            runStatement.Rule = RUN + (variable | StringLiteral) +
                                ((IN + FORM + MODE) | (IN + LINE + MODE)).Q() +
                                ((WITHOUT + WAITING) | (RETURNING + variable)).Q();
            oneOrMoreExpressions.Rule = MakePlusRule(oneOrMoreExpressions, comma, expression);
            assignmentStatement.Rule = LET + variable + singleEqual + oneOrMoreExpressions;
            oneOrMoreVariables.Rule = MakePlusRule(oneOrMoreVariables, comma, variable);
            oneOrMoreActualParameters.Rule = MakePlusRule(oneOrMoreActualParameters, comma, actualParameter);
            procedureStatement.Rule = CALL + memberAccess + (RETURNING + oneOrMoreVariables).Q();
            procedureIdentifier.Rule = functionIdentifier;
            actualParameter.Rule = star | expression;
            gotoStatement.Rule = GOTO + colon.Q() + label;

            oneOrMoreLogicalTerms.Rule = MakePlusRule(oneOrMoreLogicalTerms, OR, logicalTerm);
            condition.Rule = TRUE | FALSE | oneOrMoreLogicalTerms;
            logicalTerm.Rule = MakePlusRule(oneOrMoreLogicalFactors, AND, logicalFactor);
            logicalFactor.Rule = (sqlExpression + NOT.Q() + IN + expressionSet) |
                                 (sqlExpression + NOT.Q() + LIKE + sqlExpression + StringLiteral) |
                                 (sqlExpression + NOT.Q() + BETWEEN + sqlExpression + AND + sqlExpression) |
                                 (sqlExpression + IS + NOT.Q() + NULL) |
                                 (quantifiedFactor) |
                                 (Lpar + condition + Rpar) |
                                 (sqlExpression + relationalOperator + sqlExpression);
            quantifiedFactor.Rule = (sqlExpression + relationalOperator + (ALL | ANY).Q() + subquery) |
                                    (NOT.Q() + EXISTS + subquery) |
                                    subquery;
            expressionSet.Rule = sqlExpression | subquery;
            subquery.Rule = Lpar + sqlSelectStatement + Rpar;
            sqlExpression.Rule = MakePlusRule(sqlExpression, (plus | minus), sqlTerm);
            sqlAlias.Rule = AS.Q() + Identifier;
            sqlTerm.Rule = MakePlusRule(sqlTerm, (sqlMultiply | div), sqlFactor);
            sqlMultiply.Rule = star;
            sqlFactor.Rule = MakePlusRule(sqlFactor, doubleOr, sqlFactor2);
            oneOrMoreSqlExpressions.Rule = MakePlusRule(oneOrMoreSqlExpressions, comma, sqlExpression);
            sqlFactor2.Rule = (sqlVariable + (UNITS + unitType).Q()) |
                              (sqlLiteral + (UNITS + unitType).Q()) |
                              (groupFunction + Lpar + (star | ALL | DISTINCT).Q() + oneOrMoreSqlExpressions.Q() + Rpar) |
                              ((sqlFunction + Lpar + oneOrMoreSqlExpressions + Rpar)) |
                              (((plus | minus) + sqlExpression)) |
                              ((Lpar + sqlExpression + Rpar)) |
                              sqlExpressionList;

            sqlExpressionList.Rule = Lpar + sqlExpression + comma + oneOrMoreSqlExpressions + Rpar;
            unsignedConstant.Rule = Number | StringLiteral | constantIdentifier | NULL;

            sqlLiteral.Rule = unsignedConstant | StringLiteral | NULL | FALSE | TRUE;
            sqlVariable.Rule = columnsTableId;
            sqlFunction.Rule = numberFunction | charFunction | dateFunction | otherFunction;
            dateFunction.Rule = YEAR | DATE | DAY | MONTH;
            numberFunction.Rule = MOD;
            charFunction.Rule = LENGTH;
            groupFunction.Rule = AVG | COUNT | MAX | MIN | SUM;
            otherFunction.Rule = DECODE | NVL | constantIdentifier;
            relationalOperator.Rule = singleEqual | doubleEqual | nequal | le | lt | ge | gt | (NOT.Q() + MATCHES) | LIKE;

            //oneOrMoreIfCondition2s.Rule = MakePlusRule(oneOrMoreIfCondition2s, relationalOperator, oneOrMoreIfLogicalTerms);
            //ifCondition.Rule = ToTerm("true") | "false" | oneOrMoreIfCondition2s;

            //oneOrMoreIfLogicalTerms.Rule = MakePlusRule(oneOrMoreIfLogicalTerms, ToTerm("or"), ifLogicalTerm);
            //ifLogicalTerm.Rule = MakePlusRule(ifLogicalTerm, ToTerm("and"), ifLogicalFactor);
            clippedUsing.Rule = CLIPPED | (USING + StringLiteral);
            zeroOrMoreClippedUsings.Rule = MakeStarRule(zeroOrMoreClippedUsings, null, clippedUsing);
            //expression.Rule = (simpleExpression + zeroOrMoreClippedUsings);// | ifCondition;
            //ifLogicalFactor.Rule = (simpleExpression + "is" + not.Q() + "null") |
            //                       (not + ifCondition) |
            //                       (Lpar + ifCondition + Rpar) |
            //                       simpleExpression;
            //oneOrMoreTerms.Rule = MakePlusRule(oneOrMoreTerms, addingOperator, term);
            //simpleExpression.Rule = sign.Q() + oneOrMoreTerms;
            //addingOperator.Rule = plus | minus;

            //term.Rule = MakePlusRule(term, multiplyingOperator, factor);
            //multiplyingOperator.Rule = star | div | "mod";
            //factor.Rule = ((ToTerm("group").Q() + 
            //                    (functionIdentifier | variable | constant) +                // all of these conflict
            //                    (Lpar + oneOrMoreActualParameters.Q() + Rpar).Q()
            //                ) |
            //                (Lpar + ifCondition + Rpar) |
            //                (Lpar + expression + Rpar) |
            //                (not + factor)
            //              ) +
            //              ("units" + unitType).Q();

            classChain.Rule = MakePlusRule(classChain, dot, functionIdentifier);

            functionIdentifier.Rule = DAY | YEAR | MONTH | COLUMN | CLEAR | CREATE |
                                      SUM | AVG | MIN | MAX | EXTEND | DATE | INFIELD |
                                      PREPARE | READ | CLOSE | constantIdentifier;

            constantIdentifier.Rule = ACCEPT | ASCII | COUNT | CURRENT | FALSE | FIRST | FOUND | GROUP |
                                      HIDE | INDEX | INT_FLAG | INTERRUPT | LAST | LENGTH | LINENO | MDY | NO |
                                      NOTFOUND | NULL | PAGENO | REAL | SIZE | SPACE | SQL | STATUS | TEMP | TIME |
                                      TODAY | TRUE | USER | WAIT | WEEKDAY | WORK | Identifier;
            
            literal.Rule = Number | StringLiteral | TRUE| FALSE | NULL;
            primaryExpression.Rule = literal |
                                     memberAccess |
                                     unaryExpression |
                                     parenthesizedExpression;
            parenthesizedExpression.Rule = Lpar + expression + Rpar;
            expression.Rule = (binaryOperatorExpression | primaryExpression) + zeroOrMoreClippedUsings;
            binaryOperatorExpression.Rule = expression + binaryOperator + expression;
            unaryExpression.Rule = unaryOperator + primaryExpression;
            binaryOperator.Rule = lt | doubleEqual | nequal | gt | le | ge | plus | minus | star | div | MOD
                  | singleEqual | plusEqual | AND | OR | IS | (IS + NOT);
            unaryOperator.Rule = plus | minus | NOT;
            memberAccess.Rule = functionIdentifier + memberAccessSegmentOpt;
            memberAccessSegmentOpt.Rule = MakeStarRule(memberAccessSegmentOpt, null, memberAccessSegment);
            memberAccessSegment.Rule = (dot + (functionIdentifier | star)) |
                                       arrayIndexer |
                                       argumentListPar;
            argumentListPar.Rule = Lpar + argumentListOpt + Rpar;
            argumentListOpt.Rule = Empty | argumentList;
            argumentList.Rule = MakePlusRule(argumentList, comma, argument);
            argument.Rule = expression;

            constant.Rule = numericConstant | constantIdentifier | (sign + Identifier) | Identifier | StringLiteral;

            variable.Rule = entireVariable | componentVariable;
            entireVariable.Rule = variableIdentifier;
            variableIdentifier.Rule = constantIdentifier;
            indexingVariable.Rule = Lbr + oneOrMoreExpressions + Rbr;
            componentVariable.Rule = (recordVariable + indexingVariable.Q()) + 
                                     ((dot + star) | (dot + componentVariable + ((THROUGH | THRU) + componentVariable).Q())).Q();
            recordVariable.Rule = constantIdentifier;

            fieldIdentifier.Rule = constantIdentifier;
            structuredStatement.Rule = conditionalStatement | repetetiveStatement;
            conditionalStatement.Rule = ifStatement | caseStatement;
            ifStatement.Rule = IF + expression + THEN + codeBlock.Q() + (ELSE + codeBlock.Q()).Q() + END + IF;
            repetetiveStatement.Rule = whileStatement | forEachStatement | forStatement;
            whileStatement.Rule = WHILE + expression + codeBlock.Q() + END + WHILE;
            forStatement.Rule = FOR + controlVariable + singleEqual + forList + (STEP + numericConstant).Q() +
                                codeBlock.Q() + END + FOR;
            forList.Rule = initialValue + TO + finalValue;
            controlVariable.Rule = Identifier;
            initialValue.Rule = expression;
            finalValue.Rule = expression;

            forEachStatement.Rule = FOREACH + Identifier + (USING + variableList).Q() + (INTO + variableList).Q() +
                                    (WITH + REOPTIMIZATION).Q() + codeBlock.Q() + END + FOREACH;
            variableList.Rule = oneOrMoreVariables;
            variableOrConstantList.Rule = oneOrMoreExpressions;

            whenExpression.Rule = WHEN + expression + codeBlock.Q();
            whenIf.Rule = WHEN + expression + codeBlock;
            zeroOrMoreWhenExpressions.Rule = MakeStarRule(zeroOrMoreWhenExpressions, null, whenExpression);
            zeroOrMoreWhenIfs.Rule = MakeStarRule(zeroOrMoreWhenIfs, null, whenIf);
            caseStatement.Rule = (CASE + expression + zeroOrMoreWhenExpressions +
                                    (OTHERWISE + codeBlock.Q()).Q() + END + CASE) |
                                 (CASE + zeroOrMoreWhenIfs + (OTHERWISE + codeBlock).Q() + END + CASE);

            otherFGLStatement.Rule = otherProgramFlowStatement |
                                     otherStorageStatement |
                                     reportStatement |
                                     screenStatement;
            otherProgramFlowStatement.Rule = runStatement |
                                             gotoStatement |
                                             (SLEEP + expression) |
                                             exitStatements |
                                             continueStatements |
                                             returnStatement;
            exitTypes.Rule = FOREACH | FOR | CASE | CONSTRUCT | DISPLAY | INPUT | MENU | REPORT | WHILE;
            exitStatements.Rule = (EXIT + exitTypes) | (EXIT + PROGRAM + ((Lpar + expression + Rpar) | expression).Q());
            continueStatements.Rule = CONTINUE + exitTypes;
            otherStorageStatement.Rule = (ALLOCATE + ARRAY + Identifier + arrayIndexer) |
                                         (LOCATE + variableList + IN + (MEMORY | (FILE + (variable | StringLiteral).Q()))) |
                                         (DEALLOCATE + ARRAY + Identifier) |
                                         (RESIZE + ARRAY + Identifier + arrayIndexer) |
                                         (FREE + oneOrMoreVariables) |
                                         (INITIALIZE + oneOrMoreVariables + ((TO + NULL) | (LIKE + oneOrMoreExpressions))) |
                                         (VALIDATE + oneOrMoreVariables + LIKE + oneOrMoreExpressions);
            
            printExpressionItem.Rule = (COLUMN + expression) | PAGENO | LINENO |
                                       (BYTE + variable) | (TEXT + variable) |
                                       (expression + (SPACE | SPACES).Q() + (WORDWRAP + (RIGHT + MARGIN + numericConstant).Q()).Q());
            printExpressionList.Rule = MakePlusRule(printExpressionList, comma, printExpressionItem);
            
            reportStatement.Rule = (START + REPORT + constantIdentifier +
                                        (TO + (expression | (PIPE + expression) | PRINTER)).Q() +
                                        (WITH + zeroOrMoreReportDimensionSpecifiers).Q()) |
                                   (TERMINATE + REPORT + constantIdentifier) |
                                   (FINISH + REPORT + constantIdentifier) |
                                   (PAUSE + StringLiteral.Q()) |
                                   (NEED + expression + LINES) |
                                   (PRINT + ((printExpressionList.Q() + semi.Q()) | (FILE | StringLiteral)).Q()) |
                                   (SKIP + ((expression + (LINE | LINES)) | (TO + TOP + OF + PAGE))) |
                                   (OUTPUT + TO + REPORT + constantIdentifier + Lpar + oneOrMoreExpressions.Q() + Rpar);
            reportDimensionSpecifier.Rule = ((LEFT | RIGHT | TOP | BOTTOM) + MARGIN + numericConstant) |
                                            (PAGE + LENGTH + numericConstant) |
                                            (TOP + OF + PAGE + StringLiteral);
            zeroOrMoreReportDimensionSpecifiers.Rule = MakeStarRule(zeroOrMoreReportDimensionSpecifiers, null, reportDimensionSpecifier);

            thruNotation.Rule = ((THROUGH | THRU) + (SAME + dot).Q() + Identifier).Q();
            fieldName.Rule = Identifier |
                             (((Identifier + (Lbr + numericConstant + Rbr).Q()) + dot).Q() + Identifier) |
                             ((Identifier + (Lbr + numericConstant + Rbr).Q()) + dot + (star | (Identifier + thruNotation)));
            fieldList.Rule = oneOrMoreExpressions;
            keyList.Rule = oneOrMoreExpressions;
            constructEvents.Rule = ((BEFORE | AFTER) + (CONSTRUCT | (FIELD + fieldList))) |
                                   (ON + KEY + Lpar + keyList + Rpar);
            
            attribute.Rule = oneOrMoreSpecialAttributes;

            specialAttribute.Rule = displayAttribute | controlAttribute;
            displayAttribute.Rule = BLACK | BLUE | CYAN | GREEN | MAGENTA | RED |
                                    WHITE | YELLOW | BOLD | DIM | NORMAL | INVISIBLE | REVERSE | BLINK | UNDERLINE;
            controlAttribute.Rule = (NAME + singleEqual + StringLiteral) |
                                    (HELP + singleEqual + numericConstant) |
                                    (WITHOUT + DEFAULTS + (singleEqual + numericConstant).Q()) |
                                    (FIELD + ORDER + FORM) |
                                    (UNBUFFERED + (singleEqual + numericConstant).Q()) |
                                    (CANCEL + (singleEqual + numericConstant).Q()) |
                                    (ACCEPT + (singleEqual + numericConstant).Q());


            oneOrMoreSpecialAttributes.Rule = MakePlusRule(oneOrMoreSpecialAttributes, comma, specialAttribute);
            attributeList.Rule = (ATTRIBUTE | ATTRIBUTES) + Lpar + attribute + Rpar;
            constructGroupStatement.Rule = constructEvents + oneOrMoreCodeBlocks;
            oneOrMoreConstructGroupStatements.Rule = MakePlusRule(oneOrMoreConstructGroupStatements, null, constructGroupStatement);
            oneOrMoreCodeBlocks.Rule = MakePlusRule(oneOrMoreCodeBlocks, null, codeBlock);
            constructStatement.Rule = CONSTRUCT +
                                      ((BY + NAME + variable + ON + columnsList) |
                                       (variable + ON + columnsList) |
                                       (FROM + fieldList)) +
                                      attributeList.Q() +
                                      (HELP + numericConstant).Q() +
                                      (oneOrMoreConstructGroupStatements + END + CONSTRUCT).Q();

            displayArrayStatement.Rule = DISPLAY + ARRAY + expression + TO + expression +
                                         attributeList.Q() + zeroOrMoreDisplayEvents + (END + DISPLAY).Q();
            zeroOrMoreDisplayEvents.Rule = MakeStarRule(zeroOrMoreDisplayEvents, null, displayEvents);
            displayInsideStatement.Rule = (CONTINUE | EXIT) + DISPLAY;
            displayEvents.Rule = ON + KEY + Lpar + keyList + Rpar + oneOrMoreCodeBlocks;
            displayStatement.Rule = DISPLAY +
                                    ((BY + NAME + oneOrMoreExpressions) |
                                     ((TO + fieldList) | (AT + expression + comma + expression)).Q()) +
                                    attributeList.Q();
            errorStatement.Rule = ERROR + oneOrMoreExpressions + attributeList.Q();
            messageStatement.Rule = MESSAGE + oneOrMoreExpressions + attributeList.Q();
            promptStatement.Rule = PROMPT + oneOrMoreExpressions + attributeList.Q() +
                                   FOR + CHAR.Q() + variable +
                                   (HELP + numericConstant).Q() +
                                   attributeList.Q() +
                                   (zeroOrMoreKeyListCodeBlocks + END + PROMPT).Q();
            keyListCodeBlock.Rule = ON + KEY + Lpar + keyList + Rpar + codeBlock.Q();
            zeroOrMoreKeyListCodeBlocks.Rule = MakeStarRule(zeroOrMoreKeyListCodeBlocks, null, keyListCodeBlock);

            inputEvents.Rule = ((BEFORE | AFTER) +
                                (INPUT | ROW | INSERT | DELETE)) |
                               ((BEFORE | AFTER) + FIELD + fieldList) |
                               (ON + KEY + Lpar + keyList + Rpar) |
                               (ON + CHANGE + fieldList) |
                               (ON + ((IDLE + Identifier) | (ACTION + (CANCEL | ACCEPT | CLOSE | HELP | Identifier))));
            
            inputOrConstructInsideStatement.Rule = (NEXT + FIELD + (fieldName | NEXT | PREVIOUS)) |
                                        ((CONTINUE | EXIT) + (INPUT | CONSTRUCT));
            
            inputGroupStatement.Rule = inputEvents + zeroOrMoreCodeBlocks;
            zeroOrMoreCodeBlocks.Rule = MakeStarRule(zeroOrMoreCodeBlocks, null, codeBlock);
            inputStatement.Rule = INPUT +
                                  ((BY + NAME + oneOrMoreExpressions +
                                    (WITHOUT + DEFAULTS + attributeList.Q()).Q()) |
                                   (oneOrMoreExpressions + (WITHOUT + DEFAULTS).Q() + FROM + fieldList)) +
                                  attributeList.Q() +
                                  (HELP + numericConstant).Q() +
                                  (oneOrMoreInputGroupStatements + includeDefinitions.Q() + END + INPUT).Q();
            oneOrMoreInputGroupStatements.Rule = MakePlusRule(oneOrMoreInputGroupStatements, null, inputGroupStatement);
            inputArrayStatement.Rule = INPUT + ARRAY + expression +
                                       (WITHOUT + DEFAULTS).Q() + FROM + oneOrMoreExpressions +
                                       (HELP + numericConstant).Q() +
                                       attributeList.Q() +
                                       (oneOrMoreInputGroupStatements + includeDefinitions.Q() + ToTerm("end") + "input").Q();

            menuEvents.Rule = (BEFORE + MENU) |
                              (COMMAND +
                                ((KEY + Lpar + keyList + Rpar).Q() +
                                 expression + expression.Q() + (HELP + numericConstant).Q())) |
                              (ON + ((IDLE + Identifier) | (ACTION + (CLOSE | HELP | OUTPUT | PRINT | UPDATE | Identifier))));
            additionalExpression.Rule = comma + expression;
            zeroOrMoreAdditionalExpressions.Rule = MakeStarRule(zeroOrMoreAdditionalExpressions, null, additionalExpression);
            menuInsideStatement.Rule = ((NEXT | SHOW | HIDE) + OPTION + (expression | ALL) + zeroOrMoreAdditionalExpressions) |
                                       ((CONTINUE | EXIT) + MENU);
            menuGroupStatement.Rule = menuEvents + codeBlock.Q();
            zeroOrMoreMenuGroupStatements.Rule = MakeStarRule(zeroOrMoreMenuGroupStatements, null, menuGroupStatement);
            menuStatement.Rule = MENU + expression + zeroOrMoreMenuGroupStatements + includeDefinitions.Q() + "end" + "menu";
            reservedLinePosition.Rule = (FIRST + (plus + numericConstant).Q()) |
                                        numericConstant |
                                        (LAST + (minus + numericConstant).Q());
            specialWindowAttribute.Rule = (BLACK | BLUE | CYAN | GREEN | MAGENTA | RED | WHITE |
                                            YELLOW | BOLD | DIM | NORMAL | INVISIBLE) |
                                          REVERSE | BORDER |
                                          ((PROMPT | FORM | MENU | MESSAGE) + LINE + reservedLinePosition) |
                                          (COMMENT + LINE + (reservedLinePosition | OFF));
            windowAttribute.Rule = oneOrMoreSpecialWindowAttributes;
            oneOrMoreSpecialWindowAttributes.Rule = MakePlusRule(oneOrMoreSpecialWindowAttributes, comma, specialWindowAttribute);
            windowAttributeList.Rule = (ATTRIBUTE | ATTRIBUTES) + Lpar + windowAttribute + Rpar;

            optionStatement.Rule = ((MESSAGE | PROMPT | MENU | COMMENT | ERROR | FORM) + LINE + expression) |
                                   ((INSERT | DELETE | NEXT | PREVIOUS | ACCEPT | HELP) + KEY + expression) |
                                   (INPUT + (WRAP | (NO + WRAP))) |
                                   (HELP + FILE + expression) |
                                   ((INPUT | DISPLAY) + attributeList) |
                                   (SQL + INTERRUPT + (ON | OFF)) |
                                   (FIELD + ORDER + (CONSTRAINED | UNCONSTRAINED));
            optionsStatement.Rule = (OPTION | OPTIONS) + oneOrMoreOptionsStatements;
            oneOrMoreOptionsStatements.Rule = MakePlusRule(oneOrMoreOptionsStatements, comma, optionStatement);

            screenStatement.Rule = (CLEAR + (FORM | (WINDOW + Identifier) | (WINDOW.Q() + SCREEN) | fieldList)) |
                                   (CLOSE + WINDOW + Identifier) |
                                   (CLOSE + FORM + Identifier) |
                                   constructStatement |
                                   (CURRENT + WINDOW + IS + (SCREEN | Identifier)) |
                                   displayStatement |
                                   displayArrayStatement |
                                   (DISPLAY + FORM + Identifier + attributeList.Q()) |
                                   errorStatement |
                                   messageStatement |
                                   promptStatement |
                                   inputStatement |
                                   inputArrayStatement |
                                   menuStatement |
                                   (OPEN + FORM + expression + FROM + expression) |
                                   (OPEN + WINDOW + expression + AT + expression + FROM + expression +
                                        ((WITH + FORM + expression) | (WITH + expression + ROWS + comma + expression + COLUMNS))
                                    + windowAttributeList.Q()) |
                                   optionsStatement |
                                   (SCROLL + oneOrMoreFieldLists + (UP | DOWN) + (BY + numericConstant).Q());
            oneOrMoreFieldLists.Rule = MakePlusRule(oneOrMoreFieldLists, comma, fieldList);

            sqlStatements.Rule = cursorManipulationStatement |
                                 dataDefinitionStatement |
                                 dataManipulationStatement |
                                 dynamicManagementStatement |
                                 queryOptimizationStatement |
                                 dataIntegrityStatement |
                                 clientServerStatement;

            cursorManipulationStatement.Rule = (CLOSE + cursorName) |
                                               (DECLARE + cursorName +
                                                    (CURSOR + (WITH + HOLD).Q() + FOR +
                                                        ((sqlSelectStatement + (FOR + UPDATE + (OF + columnsList).Q()).Q()) |
                                                         sqlInsertStatement |
                                                         statementId)
                                                    ) |
                                                    (SCROLL + CURSOR + (WITH + HOLD).Q() + FOR +
                                                        (sqlSelectStatement | statementId))
                                                ) |
                                                (FETCH +
                                                    (NEXT | (PREVIOUS | PRIOR) | FIRST | LAST | CURRENT |
                                                        (RELATIVE + expression) | (ABSOLUTE + expression)).Q() +
                                                    cursorName + (INTO + variableList).Q()) |
                                                (FLUSH + cursorName) |
                                                (OPEN + cursorName + (USING + variableOrConstantList).Q()) |
                                                (PUT + cursorName + (FROM + variableOrConstantList).Q());
            columnsList.Rule = MakeStarRule(columnsList, comma, columnsTableId);
            statementId.Rule = constantIdentifier;
            cursorName.Rule = Identifier;
            dataType.Rule = type;
            columnItem.Rule = constantIdentifier +
                                (dataType | ((BYTE | TEXT) + (IN + (TABLE | constantIdentifier)).Q()) + (NOT + NULL).Q()) |
                              (UNIQUE + Lpar + oneOrMoreConstantIdentifiers.Q() + Rpar + (CONSTRAINT + constantIdentifier).Q());
            oneOrMoreColumnItems.Rule = MakePlusRule(oneOrMoreColumnItems, comma, columnItem);
            dataDefinitionStatement.Rule = (DROP + TABLE + constantIdentifier) |
                                           (CREATE + TEMP.Q() + TABLE + constantIdentifier +
                                                Lpar + oneOrMoreColumnItems + Rpar +
                                                (WITH + NO + LOG).Q() +
                                                (IN + constantIdentifier).Q() +
                                                (EXTENT + SIZE + numericConstant).Q() +
                                                (NEXT + SIZE + numericConstant).Q() +
                                                (LOCK + MODE + Lpar + (PAGE | ROW) + Rpar).Q()) |
                                           (CREATE + UNIQUE.Q() + CLUSTER.Q() +
                                                INDEX + constantIdentifier + ON + constantIdentifier +
                                                Lpar + oneOrMoreConstantIdentifiersWithAscDesc + Rpar) |
                                           (DROP + INDEX + constantIdentifier);
            constantIdentifierWithAscDesc.Rule = constantIdentifier + (ASC | DESC).Q();
            oneOrMoreConstantIdentifiersWithAscDesc.Rule = MakePlusRule(oneOrMoreConstantIdentifiersWithAscDesc, comma, constantIdentifierWithAscDesc);

            dataManipulationStatement.Rule = sqlInsertStatement |
                                             sqlDeleteStatement |
                                             sqlSelectStatement |
                                             sqlInsertStatement |
                                             sqlUpdateStatement |
                                             sqlLoadStatement |
                                             sqlUnLoadStatement;
            sqlSelectStatement.Rule = mainSelectStatement;
            columnsTableId.Rule = star | (tableIdentifier + indexingVariable.Q() + ((dot + star) | (dot + columnsTableId)).Q());
            selectList.Rule = MakePlusRule(selectList, comma, sqlExpressionsWithSqlAlias);
            sqlExpressionsWithSqlAlias.Rule = sqlExpression + sqlAlias.Q();
            headSelectStatement.Rule = SELECT + (ALL | (DISTINCT | UNIQUE)).Q() + selectList;
            tableIdentifier.Rule = ((constantIdentifier + AtSym + constantIdentifier + colon + constantIdentifier) |
                                        (constantIdentifier + colon + constantIdentifier) |
                                        constantIdentifier);     // TODO: might still be wrong, but it's a better attempt at removing reduce-reduce conflict
            
            fromTable.Rule = OUTER.Q() + tableIdentifier + sqlAlias.Q();
            tableExpression.Rule = simpleSelectStatement;
            fromTableExpression.Rule = fromTable | (Lpar + tableExpression + Rpar + sqlAlias.Q());
            oneOrMoreFromTableExpressions.Rule = MakePlusRule(oneOrMoreFromTableExpressions, comma, fromTableExpression);
            fromSelectStatement.Rule = FROM + oneOrMoreFromTableExpressions;
            aliasName.Rule = Identifier;

            mainSelectStatement.Rule = headSelectStatement + (INTO + variableList).Q() +
                                       fromSelectStatement + whereStatement.Q() +
                                       groupByStatement.Q() + havingStatement.Q() +
                                       unionSelectStatement.Q() + orderbyStatement.Q() +
                                       (INTO + TEMP + Identifier).Q() +
                                       (WITH + NO + LOG).Q();
            unionSelectStatement.Rule = UNION + ALL.Q() + simpleSelectStatement;
            simpleSelectStatement.Rule = headSelectStatement +
                                         fromSelectStatement +
                                         whereStatement.Q() +
                                         groupByStatement.Q() +
                                         havingStatement.Q() +
                                         unionSelectStatement.Q();
            whereStatement.Rule = WHERE + condition;
            groupByStatement.Rule = GROUP + BY + variableOrConstantList;
            havingStatement.Rule = HAVING + condition;
            orderbyColumn.Rule = expression + (ASC | DESC).Q();
            orderbyStatement.Rule = ORDER + BY + oneOrMoreOrderByColumns;
            oneOrMoreOrderByColumns.Rule = MakePlusRule(oneOrMoreOrderByColumns, comma, orderbyColumn);
            sqlLoadStatement.Rule = LOAD + FROM + (variable | StringLiteral) +
                                    (DELIMITER + (variable | StringLiteral)).Q() +
                                    ((INSERT + INTO + tableIdentifier + (Lpar + columnsList + Rpar).Q()) |
                                        sqlInsertStatement);
            sqlUnLoadStatement.Rule = UNLOAD + TO + (variable | StringLiteral) +
                                      (DELIMITER + (variable | StringLiteral)).Q() +
                                      sqlSelectStatement;
            sqlInsertStatement.Rule = INSERT + INTO + tableIdentifier + (Lpar + columnsList + Rpar).Q() +
                                      ((VALUES + Lpar + oneOrMoreExpressions + Rpar) | simpleSelectStatement);
            sqlUpdateStatement.Rule = UPDATE + tableIdentifier + SET +
                                      (oneOrMoreColumnsTableIdEqualExpressions |
                                            ((Lpar + columnsList + Rpar) | ((aliasName + dot).Q() + star)) +
                                            singleEqual + ((Lpar + oneOrMoreExpressions + Rpar) | ((aliasName + dot).Q() + star))) +
                                      (WHERE + (condition | (CURRENT + OF + cursorName))).Q();
            columnsTableIdEqualExpression.Rule = columnsTableId + singleEqual + expression;
            oneOrMoreColumnsTableIdEqualExpressions.Rule = MakePlusRule(oneOrMoreColumnsTableIdEqualExpressions, comma, columnsTableIdEqualExpression);
            sqlDeleteStatement.Rule = DELETE + FROM + tableIdentifier +
                                      (WHERE + (condition | (CURRENT + OF + cursorName))).Q();
            dynamicManagementStatement.Rule = (PREPARE + cursorName + FROM + expression) |
                                              (EXECUTE + cursorName + (USING + variableList).Q()) |
                                              (FREE + (cursorName | statementId)) |
                                              (LOCK + TABLE + expression + IN + (SHARE | EXCLUSIVE) + MODE);
            queryOptimizationStatement.Rule = (UPDATE + STATISTICS + (FOR + TABLE + tableIdentifier).Q()) |
                                              (SET + LOCK + MODE + TO + ((WAIT + SECONDS.Q()) | NOT + WAIT)) |      // TODO: not sure if that's right...
                                              (SET + EXPLAIN + (ON | OFF)) |
                                              (SET + ISOLATION + TO +
                                                ((CURSOR + STABILITY) | ((DIRTY | COMMITTED | REPEATABLE) + READ))) |
                                              (SET + BUFFERED.Q() + LOG);
            databaseDeclaration.Rule = DATABASE + constantIdentifier + (AtSym + constantIdentifier).Q() + EXCLUSIVE.Q() + semi.Q();
            clientServerStatement.Rule = CLOSE + DATABASE;
            dataIntegrityStatement.Rule = wheneverStatement |
                                          ((BEGIN | COMMIT | ROLLBACK) + WORK);
            wheneverStatement.Rule = WHENEVER + wheneverType + wheneverFlow;
            wheneverType.Rule = (NOT + FOUND) |
                                (ANY.Q() + (SQLERROR | ERROR)) |
                                (SQLWARNING | WARNING);
            wheneverFlow.Rule = CONTINUE | STOP | (CALL + Identifier) |
                                (((GO + TO) | GOTO) + colon.Q() + Identifier);

            reportDefinition.Rule = REPORT + Identifier + parameterList + typeDeclarations.Q() +
                                    outputReport.Q() + (ORDER + EXTERNAL.Q() + BY + variableList).Q() +
                                    formatReport.Q() + END + REPORT;
            outputReport.Rule = OUTPUT + (REPORT + TO + (StringLiteral | (PIPE + StringLiteral) | PRINTER)).Q() +
                                zeroOrMoreReportDimensionSpecifiers;
            formatReport.Rule = FORMAT + ((EVERY + ROW) | oneOrMoreReportCodeBlocks);
            oneOrMoreReportCodeBlocks.Rule = MakePlusRule(oneOrMoreReportCodeBlocks, null, reportCodeBlock);
            reportCodeBlock.Rule = ((FIRST.Q() + PAGE + HEADER) |
                                    (PAGE + TRAILER) |
                                    (ON + ((EVERY + ROW) | (LAST + ROW))) |
                                    ((BEFORE | AFTER) + GROUP + OF + variable))
                                   + codeBlock;
        }

        public KeyTerm Keyword(string keyword)
        {
            var term = ToTerm(keyword);
            // term.SetOption(TermOptions.IsKeyword, true);
            // term.SetOption(TermOptions.IsReservedWord, true);

            this.MarkReservedWords(keyword);
            term.EditorInfo = new TokenEditorInfo(TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);

            return term;
        }

        public KeyTerm Operator(string op)
        {
            string opCased = this.CaseSensitive ? op : op.ToLower();
            var term = new KeyTerm(opCased, op);
            //term.SetOption(TermOptions.IsOperator, true);

            //term.EditorInfo = new TokenEditorInfo(TokenType.Operator, TokenColor.Operator, TokenTriggers.None);

            return term;
        }
    }
}
