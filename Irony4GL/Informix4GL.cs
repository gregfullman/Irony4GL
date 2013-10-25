using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using System.Globalization;

namespace Irony.Samples.Informix4GL
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
            // Use C#'s string literal for right now
            StringLiteral StringLiteral = Informix4GLTerminalFactory.CreateString("StringLiteral");

            // Use C#'s char literal for right now
            StringLiteral CharLiteral = TerminalFactory.CreateCSharpChar("CharLiteral");

            // Use C#'s number for right now
            NumberLiteral Number = TerminalFactory.CreateCSharpNumber("Number");

            // Use C#'s identifier for right now
            IdentifierTerminal Identifier = Informix4GLTerminalFactory.CreateIdentifier("Identifier");

            // Symbols
            KeyTerm plus = ToTerm("+", "plus");
            KeyTerm minus = ToTerm("-", "minus");
            KeyTerm div = ToTerm("/", "div");
            KeyTerm colon = ToTerm(":", "colon");
            KeyTerm semi = ToTerm(";", "semi");
            KeyTerm singleEqual = ToTerm("=");
            KeyTerm doubleEqual = ToTerm("==");
            KeyTerm nequal = ToTerm("!=", "nequal");
            KeyTerm le = ToTerm("<=", "le");
            KeyTerm lt = ToTerm("<", "lt");
            KeyTerm ge = ToTerm(">=", "ge");
            KeyTerm gt = ToTerm(">", "gt");
            NonTerminal semi_opt = new NonTerminal("semi?");
            semi_opt.Rule = Empty | semi;
            KeyTerm dot = ToTerm(".", "dot");
            KeyTerm comma = ToTerm(",", "comma");
            KeyTerm star = ToTerm("*", "star");
            NonTerminal comma_opt = new NonTerminal("comma_opt", Empty | comma);
            NonTerminal commas_opt = new NonTerminal("commas_opt");
            commas_opt.Rule = MakeStarRule(commas_opt, null, comma);
            KeyTerm qmark = ToTerm("?", "qmark");
            NonTerminal qmark_opt = new NonTerminal("qmark_opt", Empty | qmark);
            KeyTerm Lbr = ToTerm("[");
            KeyTerm Rbr = ToTerm("]");
            KeyTerm LCbr = ToTerm("{");
            KeyTerm RCbr = ToTerm("}");
            KeyTerm Lpar = ToTerm("(");
            KeyTerm Rpar = ToTerm(")");
            KeyTerm AtSym = ToTerm("@");
            KeyTerm tgoto = ToTerm("goto");
            KeyTerm not = ToTerm("not");
            //KeyTerm yld = ToTerm("yield");

            // Handle comments
            CommentTerminal Comment = new CommentTerminal("Comment", "#", "\r", "\n", "\u2085", "\u2028", "\u2029");
            NonGrammarTerminals.Add(Comment);

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
            var ifCondition = new NonTerminal("ifCondition");
            var oneOrMoreIfCondition2s = new NonTerminal("oneOrMoreIfCondition2s");
            var ifLogicalTerm = new NonTerminal("ifLogicalTerm");
            var oneOrMoreIfLogicalTerms = new NonTerminal("oneOrMoreIfLogicalTerms");
            var ifLogicalFactor = new NonTerminal("ifLogicalFactor");
            var oneOrMoreIfLogicalFactors = new NonTerminal("oneOrMoreIfLogicalFactors");

            var simpleExpression = new NonTerminal("simpleExpression");
            var term = new NonTerminal("term");
            var oneOrMoreTerms = new NonTerminal("oneOrMoreTerms");
            var addingOperator = new NonTerminal("addingOperator");
            var oneOrMoreFactors = new NonTerminal("oneOrMoreFactors");
            var multiplyingOperator = new NonTerminal("multiplyingOperator");
            var factor = new NonTerminal("factor");
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

            /************************************************************************************************************/
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

            includeDefinition.Rule = ToTerm("&") + "include" + StringLiteral;
            includeDefinitions.Rule = MakeStarRule(includeDefinitions, null, includeDefinition);

            deferStatementOrCodeBlock.Rule = deferStatement | codeBlock;    // derived
            
            mainStatements.Rule = MakeStarRule(mainStatements, deferStatementOrCodeBlock);
            mainBlock.Rule = "main" + typeDeclarations + mainStatements + "end" + "main";
            deferStatement.Rule = "defer" + (ToTerm("interrupt") | "quit");

            reportOrFunctionDefinition.Rule = reportDefinition | functionDefinition;    // derived
            functionOrReportDefinitions.Rule = MakeStarRule(functionOrReportDefinitions, reportOrFunctionDefinition);

            returnStatement.Rule = "return" + variableOrConstantList.Q();

            functionDefinition.Rule = "function" +
                                      functionIdentifier +
                                      parameterList +
                                      typeDeclarations +
                                      codeBlock.Q() +
                                      "end" + "function";

            parameterGroup.Rule = MakePlusRule(parameterGroup, comma, Identifier);

            // TODO: not sure about this grammar rule. It allows parameters to be seperated by spaces instead of commas...
            zeroOrMoreParametersGroups.Rule = MakeStarRule(zeroOrMoreParametersGroups, parameterGroup); // derived
            parameterList.Rule = Empty | (Lpar + zeroOrMoreParametersGroups + Rpar);

            globalDeclaration.Rule = "globals" + (StringLiteral | (typeDeclarations + "end" + "globals"));

            oneOrMoreVariableDeclarations.Rule = MakePlusRule(oneOrMoreVariableDeclarations, comma, variableDeclaration);
            typeDeclaration.Rule = "define" + oneOrMoreVariableDeclarations;
            typeDeclarations.Rule = MakeStarRule(typeDeclarations, typeDeclaration);

            typeDefinition.Rule = "type" + Identifier + type;
            typeDefinitions.Rule = MakeStarRule(typeDefinitions, typeDefinition);

            oneOrMoreConstantIdentifiers.Rule = MakePlusRule(oneOrMoreConstantIdentifiers, comma, constantIdentifier);
            constantIdentifierAndTypePair.Rule = oneOrMoreConstantIdentifiers + type;
            oneOrMoreConstantIdentifierAndTypePairs.Rule = MakePlusRule(oneOrMoreConstantIdentifierAndTypePairs, comma, constantIdentifierAndTypePair);
            variableDeclaration.Rule = oneOrMoreConstantIdentifierAndTypePairs;

            type.Rule = typeIdentifier | indirectType | largeType | structuredType;
            indirectType.Rule = "like" + tableIdentifier + dot + Identifier;
            typeIdentifier.Rule = charType | numberType | timeType | classChain;
            largeType.Rule = ToTerm("text") | "byte";
            sign.Rule = plus | minus;
            numericConstant.Rule = Number | (sign + Number);

            /*builtInClassType.Rule = uiClassType | baseClassType | omClassType;
            uiClassType.Rule = "ui" + dot + (ToTerm("window") | "form" | "dialog" | "combobox" | "dragdrop");
            baseClassType.Rule = "base" + dot + (ToTerm("channel") | "stringbuffer" | "stringtokenizer" | "typeinfo" | "messageserver");
            omClassType.Rule = "om" + dot + (ToTerm("domnode") | "nodelist" | "saxattributes" | "saxdocumenthandler" | "xmlreader" | "xmlwriter");*/

            numberType.Rule = ToTerm("bigint") | "integer" | "int" | "smallint" | "real" | "smallfloat" |
                              ((ToTerm("decimal") | "dec" | "numeric" | "money") + ((Lpar + numericConstant + ("," + numericConstant).Q() + Rpar) | Empty)) |
                              ((ToTerm("float") | "double") + ((Lpar + numericConstant + Rpar) | Empty));
            charType.Rule = ((ToTerm("varchar") | "nvarchar") + Lpar + numericConstant + ("," + numericConstant).Q() + Rpar) |
                            ((ToTerm("char") | "nchar" | "character") + ((Lpar + numericConstant + Rpar) | Empty)) |
                            "string";
            timeType.Rule = "date" | ("datetime" + datetimeQualifier) | ("interval" + intervalQualifier);
            datetimeQualifier.Rule = (ToTerm("year") + "to" + yearQualifier) |
                                     (ToTerm("month") + "to" + monthQualifier) |
                                     (ToTerm("day") + "to" + dayQualifier) |
                                     (ToTerm("hour") + "to" + hourQualifier) |
                                     (ToTerm("minute") + "to" + minuteQualifier) |
                                     (ToTerm("second") + "to" + secondQualifier) |
                                     (ToTerm("fraction") + "to" + fractionQualifier);
            intervalQualifier.Rule = ("year" + (Lpar + numericConstant + Rpar).Q() + "to" + yearQualifier) |
                                     ("month" + (Lpar + numericConstant + Rpar).Q() + "to" + monthQualifier) |
                                     ("day" + (Lpar + numericConstant + Rpar).Q() + "to" + dayQualifier) |
                                     ("hour" + (Lpar + numericConstant + Rpar).Q() + "to" + hourQualifier) |
                                     ("minute" + (Lpar + numericConstant + Rpar).Q() + "to" + minuteQualifier) |
                                     ("second" + (Lpar + numericConstant + Rpar).Q() + "to" + secondQualifier) |
                                     (ToTerm("fraction") + "to" + fractionQualifier);
            unitType.Rule = yearQualifier;
            yearQualifier.Rule = "year" | monthQualifier;
            monthQualifier.Rule = "month" | dayQualifier;
            dayQualifier.Rule = "day" | hourQualifier;
            hourQualifier.Rule = "hour" | minuteQualifier;
            minuteQualifier.Rule = "minute" | secondQualifier;
            secondQualifier.Rule = "second" | fractionQualifier;
            fractionQualifier.Rule = "fraction" + (Lpar + numericConstant + Rpar).Q();

            structuredType.Rule = recordType | arrayType | dynArrayType;
            recordType.Rule = "record" + ((oneOrMoreVariableDeclarations + "end" + "record") | ("like" + tableIdentifier + dot + star));
            arrayIndexer.Rule = Lbr + numericConstant + ((comma + numericConstant) | (comma + numericConstant + comma + numericConstant)).Q() + Rbr;
            arrayType.Rule = "array" + arrayIndexer + "of" + (recordType | typeIdentifier | largeType);
            dynArrayType.Rule = ToTerm("dynamic") + "array" + ("with" + numericConstant + "dimensions").Q() + "of" +
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

            runStatement.Rule = "run" + (variable | StringLiteral) +
                                ((ToTerm("in") + "form" + "mode") | (ToTerm("in") + "line" + "mode")).Q() +
                                ((ToTerm("without") + "waiting") | ("returning" + variable)).Q();
            oneOrMoreExpressions.Rule = MakePlusRule(oneOrMoreExpressions, comma, expression);
            assignmentStatement.Rule = "let" + variable + singleEqual + oneOrMoreExpressions;
            oneOrMoreVariables.Rule = MakePlusRule(oneOrMoreVariables, comma, variable);
            oneOrMoreActualParameters.Rule = MakePlusRule(oneOrMoreActualParameters, comma, actualParameter);
            procedureStatement.Rule = "call" + procedureIdentifier +
                                      ((Lpar + oneOrMoreActualParameters.Q() + Rpar) | Empty) +
                                      ("returning" + oneOrMoreVariables).Q();
            procedureIdentifier.Rule = functionIdentifier;
            actualParameter.Rule = star | expression;
            gotoStatement.Rule = "goto" + colon.Q() + label;

            oneOrMoreLogicalTerms.Rule = MakePlusRule(oneOrMoreLogicalTerms, ToTerm("or"), logicalTerm);
            condition.Rule = ToTerm("true") | "false" | oneOrMoreLogicalTerms;
            logicalTerm.Rule = MakePlusRule(oneOrMoreLogicalFactors, ToTerm("and"), logicalFactor);
            logicalFactor.Rule = (sqlExpression + not.Q() + "in" + expressionSet) |
                                 (sqlExpression + not.Q() + "like" + sqlExpression + StringLiteral) |
                                 (sqlExpression + not.Q() + "between" + sqlExpression + "and" + sqlExpression) |
                                 (sqlExpression + "is" + not.Q() + "null") |
                                 (quantifiedFactor) |
                                 (Lpar + condition + Rpar) |
                                 (sqlExpression + relationalOperator + sqlExpression);
            quantifiedFactor.Rule = (sqlExpression + relationalOperator + (ToTerm("all") | "any").Q() + subquery) |
                                    (not.Q() + "exists" + subquery) |
                                    subquery;
            expressionSet.Rule = sqlExpression | subquery;
            subquery.Rule = Lpar + sqlSelectStatement + Rpar;
            sqlExpression.Rule = MakePlusRule(sqlExpression, (plus | minus), sqlTerm);
            sqlAlias.Rule = ToTerm("as").Q() + Identifier;
            sqlTerm.Rule = MakePlusRule(sqlTerm, (sqlMultiply | "/"), sqlFactor);
            sqlMultiply.Rule = star;
            sqlFactor.Rule = MakePlusRule(sqlFactor, ToTerm("||"), sqlFactor2);
            oneOrMoreSqlExpressions.Rule = MakePlusRule(oneOrMoreSqlExpressions, comma, sqlExpression);
            sqlFactor2.Rule = (sqlVariable + ("units" + unitType).Q()) |
                              (sqlLiteral + ("units" + unitType).Q()) |
                              (groupFunction + Lpar + (star | "all" | "distinct").Q() + oneOrMoreSqlExpressions.Q() + Rpar) |
                              ((sqlFunction + Lpar + oneOrMoreSqlExpressions + Rpar)) |
                              (((plus | minus) + sqlExpression)) |
                              ((Lpar + sqlExpression + Rpar)) |
                              sqlExpressionList;

            sqlExpressionList.Rule = Lpar + sqlExpression + comma + oneOrMoreSqlExpressions + Rpar;
            unsignedConstant.Rule = Number | StringLiteral | constantIdentifier | "null";

            sqlLiteral.Rule = unsignedConstant | StringLiteral | "null" | "false" | "true";
            sqlVariable.Rule = columnsTableId;
            sqlFunction.Rule = numberFunction | charFunction | dateFunction | otherFunction;
            dateFunction.Rule = ToTerm("year") | "date" | "day" | "month";
            numberFunction.Rule = "mod";
            charFunction.Rule = "length";
            groupFunction.Rule = ToTerm("avg") | "count" | "max" | "min" | "sum";
            otherFunction.Rule = ToTerm("decode") | "nvl" | constantIdentifier;
            relationalOperator.Rule = singleEqual | doubleEqual | nequal | le | lt | ge | gt | (not.Q() + "matches") | "like";

            oneOrMoreIfCondition2s.Rule = MakePlusRule(oneOrMoreIfCondition2s, relationalOperator, oneOrMoreIfLogicalTerms);
            ifCondition.Rule = ToTerm("true") | "false" | oneOrMoreIfCondition2s;

            oneOrMoreIfLogicalTerms.Rule = MakePlusRule(oneOrMoreIfLogicalTerms, ToTerm("or"), ifLogicalTerm);
            ifLogicalTerm.Rule = MakePlusRule(ifLogicalTerm, ToTerm("and"), ifLogicalFactor);
            clippedUsing.Rule = "clipped" | ("using" + StringLiteral);
            zeroOrMoreClippedUsings.Rule = MakeStarRule(zeroOrMoreClippedUsings, null, clippedUsing);
            expression.Rule = (simpleExpression + zeroOrMoreClippedUsings);// | ifCondition;
            ifLogicalFactor.Rule = (simpleExpression + "is" + not.Q() + "null") |
                                   (not + ifCondition) |
                                   (Lpar + ifCondition + Rpar) |
                                   simpleExpression;
            oneOrMoreTerms.Rule = MakePlusRule(oneOrMoreTerms, addingOperator, term);
            simpleExpression.Rule = sign.Q() + oneOrMoreTerms;
            addingOperator.Rule = plus | minus;

            term.Rule = MakePlusRule(term, multiplyingOperator, factor);
            multiplyingOperator.Rule = star | div | "mod";
            factor.Rule = ((ToTerm("group").Q() + 
                                (functionIdentifier | variable | constant) +                // all of these conflict
                                (Lpar + oneOrMoreActualParameters.Q() + Rpar).Q()
                            ) |
                            (Lpar + (expression | ifCondition) + Rpar) |
                            (not + factor)
                          ) +
                          ("units" + unitType).Q();
            
            classChain.Rule = MakePlusRule(classChain, dot, Identifier);

            functionIdentifier.Rule = ToTerm("day") | "year" | "month" | "column" |
                                      "sum" | "avg" | "min" | "max" | "extend" | "date" | "infield" |
                                      "prepare" | constantIdentifier | classChain;

            constantIdentifier.Rule = ToTerm("accept") | "ascii" | "count" | "current" | "false" | "first" | "found" | "group" |
                                      "hide" | "index" | "int_flag" | "interrupt" | "last" | "length" | "lineno" | "mdy" | "no" |
                                      "not" | "notfound" | "null" | "pageno" | "real" | "size" | "sql" | "status" | "temp" | "time" |
                                      "today" | "true" | "user" | "wait" | "weekday" | "work" | Identifier;

            constant.Rule = numericConstant | constantIdentifier | (sign + Identifier) | Identifier | StringLiteral;

            variable.Rule = entireVariable | componentVariable;
            entireVariable.Rule = variableIdentifier;
            variableIdentifier.Rule = constantIdentifier;
            indexingVariable.Rule = Lbr + oneOrMoreExpressions + Rbr;
            componentVariable.Rule = (recordVariable + indexingVariable.Q()) + 
                                     ((dot + star) | (dot + componentVariable + ((ToTerm("through") | "thru") + componentVariable).Q())).Q();
            recordVariable.Rule = constantIdentifier;

            fieldIdentifier.Rule = constantIdentifier;
            structuredStatement.Rule = conditionalStatement | repetetiveStatement;
            conditionalStatement.Rule = ifStatement | caseStatement;
            ifStatement.Rule = "if" + ifCondition + "then" + codeBlock.Q() + ("else" + codeBlock.Q()).Q() + "end" + "if";
            repetetiveStatement.Rule = whileStatement | forEachStatement | forStatement;
            whileStatement.Rule = "while" + ifCondition + codeBlock.Q() + "end" + "while";
            forStatement.Rule = "for" + controlVariable + singleEqual + forList + ("step" + numericConstant).Q() +
                                codeBlock.Q() + "end" + "for";
            forList.Rule = initialValue + "to" + finalValue;
            controlVariable.Rule = Identifier;
            initialValue.Rule = expression;
            finalValue.Rule = expression;

            forEachStatement.Rule = "foreach" + Identifier + ("using" + variableList).Q() + ("into" + variableList).Q() +
                                    (ToTerm("with") + "reoptimization").Q() + codeBlock.Q() + "end" + "foreach";
            variableList.Rule = oneOrMoreVariables;
            variableOrConstantList.Rule = oneOrMoreExpressions;

            whenExpression.Rule = "when" + expression + codeBlock.Q();
            whenIf.Rule = "when" + ifCondition + codeBlock;
            zeroOrMoreWhenExpressions.Rule = MakeStarRule(zeroOrMoreWhenExpressions, null, whenExpression);
            zeroOrMoreWhenIfs.Rule = MakeStarRule(zeroOrMoreWhenIfs, null, whenIf);
            caseStatement.Rule = ("case" + expression + zeroOrMoreWhenExpressions +
                                    ("otherwise" + codeBlock.Q()).Q() + "end" + "case") |
                                 ("case" + zeroOrMoreWhenIfs + ("otherwise" + codeBlock).Q() + "end" + "case");

            otherFGLStatement.Rule = otherProgramFlowStatement |
                                     otherStorageStatement |
                                     reportStatement |
                                     screenStatement;
            otherProgramFlowStatement.Rule = runStatement |
                                             gotoStatement |
                                             ("sleep" + expression) |
                                             exitStatements |
                                             continueStatements |
                                             returnStatement;
            exitTypes.Rule = ToTerm("foreach") | "for" | "case" | "construct" | "display" | "input" | "menu" | "report" | "while";
            exitStatements.Rule = ("exit" + exitTypes) | (ToTerm("exit") + "program" + ((Lpar + expression + Rpar) | expression).Q());
            continueStatements.Rule = "continue" + exitTypes;
            otherStorageStatement.Rule = (ToTerm("allocate") + "array" + Identifier + arrayIndexer) |
                                         ("locate" + variableList + "in" + ("memory" | ("file" + (variable | StringLiteral).Q()))) |
                                         (ToTerm("deallocate") + "array" + Identifier) |
                                         (ToTerm("resize") + "array" + Identifier + arrayIndexer) |
                                         ("free" + oneOrMoreVariables) |
                                         ("initialize" + oneOrMoreVariables + ((ToTerm("to") + "null") | ("like" + oneOrMoreExpressions))) |
                                         ("validate" + oneOrMoreVariables + "like" + oneOrMoreExpressions);
            
            printExpressionItem.Rule = ("column" + expression) | "pageno" | "lineno" |
                                       ("byte" + variable) | ("text" + variable) |
                                       (expression + (ToTerm("space") | "spaces").Q() + ("wordwrap" + (ToTerm("right") + "margin" + numericConstant).Q()).Q());
            printExpressionList.Rule = MakePlusRule(printExpressionList, comma, printExpressionItem);
            
            reportStatement.Rule = (ToTerm("start") + "report" + constantIdentifier +
                                        ("to" + (expression | ("pipe" + expression) | "printer")).Q() +
                                        ("with" + zeroOrMoreReportDimensionSpecifiers).Q()) |
                                   (ToTerm("terminate") + "report" + constantIdentifier) |
                                   (ToTerm("finish") + "report" + constantIdentifier) |
                                   ("pause" + StringLiteral.Q()) |
                                   ("need" + expression + "lines") |
                                   ("print" + ((printExpressionList.Q() + semi.Q()) | ("file" | StringLiteral)).Q()) |
                                   ("skip" + ((expression + (ToTerm("line") | "lines")) | (ToTerm("to") + "top" + "of" + "page"))) |
                                   (ToTerm("output") + "to" + "report" + constantIdentifier + Lpar + oneOrMoreExpressions.Q() + Rpar);
            reportDimensionSpecifier.Rule = ((ToTerm("left") | "right" | "top" | "bottom") + "margin" + numericConstant) |
                                            (ToTerm("page") + "length" + numericConstant) |
                                            (ToTerm("top") + "of" + "page" + StringLiteral);
            zeroOrMoreReportDimensionSpecifiers.Rule = MakeStarRule(zeroOrMoreReportDimensionSpecifiers, null, reportDimensionSpecifier);

            thruNotation.Rule = ((ToTerm("through") | "thru") + ("same" + dot).Q() + Identifier).Q();
            fieldName.Rule = Identifier |
                             (((Identifier + (Lbr + numericConstant + Rbr).Q()) + dot).Q() + Identifier) |
                             ((Identifier + (Lbr + numericConstant + Rbr).Q()) + dot + (star | (Identifier + thruNotation)));
            fieldList.Rule = oneOrMoreExpressions;
            keyList.Rule = oneOrMoreExpressions;
            constructEvents.Rule = ((ToTerm("before") | "after") + ("construct" | ("field" + fieldList))) |
                                   (ToTerm("on") + "key" + Lpar + keyList + Rpar);
            
            attribute.Rule = oneOrMoreSpecialAttributes;

            specialAttribute.Rule = displayAttribute | controlAttribute;
            displayAttribute.Rule = ToTerm("black") | "blue" | "cyan" | "green" | "magenta" | "red" |
                                    "white" | "yellow" | "bold" | "dim" | "normal" | "invisible" | "reverse" | "blink" | "underline";
            controlAttribute.Rule = ("name" + singleEqual + StringLiteral) |
                                    ("help" + singleEqual + numericConstant) |
                                    (ToTerm("without") + "defaults" + (singleEqual + numericConstant).Q()) |
                                    (ToTerm("field") + "order" + "form") |
                                    ("unbuffered" + (singleEqual + numericConstant).Q()) |
                                    ("cancel" + (singleEqual + numericConstant).Q()) |
                                    ("accept" + (singleEqual + numericConstant).Q());


            oneOrMoreSpecialAttributes.Rule = MakePlusRule(oneOrMoreSpecialAttributes, comma, specialAttribute);
            attributeList.Rule = (ToTerm("attribute") | "attributes") + Lpar + attribute + Rpar;
            constructGroupStatement.Rule = constructEvents + oneOrMoreCodeBlocks;
            oneOrMoreConstructGroupStatements.Rule = MakePlusRule(oneOrMoreConstructGroupStatements, null, constructGroupStatement);
            oneOrMoreCodeBlocks.Rule = MakePlusRule(oneOrMoreCodeBlocks, null, codeBlock);
            constructStatement.Rule = "construct" +
                                      ((ToTerm("by") + "name" + variable + "on" + columnsList) |
                                       (variable + "on" + columnsList) |
                                       ("from" + fieldList)) +
                                      attributeList.Q() +
                                      ("help" + numericConstant).Q() +
                                      (oneOrMoreConstructGroupStatements + "end" + "construct").Q();

            displayArrayStatement.Rule = ToTerm("display") + "array" + expression + "to" + expression +
                                         attributeList.Q() + zeroOrMoreDisplayEvents + (ToTerm("end") + "display").Q();
            zeroOrMoreDisplayEvents.Rule = MakeStarRule(zeroOrMoreDisplayEvents, null, displayEvents);
            displayInsideStatement.Rule = (ToTerm("continue") | "exit") + "display";
            displayEvents.Rule = ToTerm("on") + "key" + Lpar + keyList + Rpar + oneOrMoreCodeBlocks;
            displayStatement.Rule = "display" +
                                    ((ToTerm("by") + "name" + oneOrMoreExpressions) |
                                     (("to" + fieldList) | ("at" + expression + comma + expression)).Q()) +
                                    attributeList.Q();
            errorStatement.Rule = "error" + oneOrMoreExpressions + attributeList.Q();
            messageStatement.Rule = "message" + oneOrMoreExpressions + attributeList.Q();
            promptStatement.Rule = "prompt" + oneOrMoreExpressions + attributeList.Q() +
                                   "for" + ToTerm("char").Q() + variable +
                                   ("help" + numericConstant).Q() +
                                   attributeList.Q() +
                                   (zeroOrMoreKeyListCodeBlocks + "end" + "prompt").Q();
            keyListCodeBlock.Rule = ToTerm("on") + "key" + Lpar + keyList + Rpar + codeBlock.Q();
            zeroOrMoreKeyListCodeBlocks.Rule = MakeStarRule(zeroOrMoreKeyListCodeBlocks, null, keyListCodeBlock);

            inputEvents.Rule = ((ToTerm("before") | "after") +
                                (ToTerm("input") | "row" | "insert" | "delete")) |
                               ((ToTerm("before") | "after") + "field" + fieldList) |
                               (ToTerm("on") + "key" + Lpar + keyList + Rpar) |
                               (ToTerm("on") + "change" + fieldList) |
                               ("on" + (ToTerm("idle") | "action") + Identifier);
            
            inputOrConstructInsideStatement.Rule = (ToTerm("next") + "field" + (fieldName | "next" | "previous")) |
                                        ((ToTerm("continue") | "exit") + (ToTerm("input") | "construct"));
            
            inputGroupStatement.Rule = inputEvents + zeroOrMoreCodeBlocks;
            zeroOrMoreCodeBlocks.Rule = MakeStarRule(zeroOrMoreCodeBlocks, null, codeBlock);
            inputStatement.Rule = "input" +
                                  ((ToTerm("by") + "name" + oneOrMoreExpressions +
                                    (ToTerm("without") + "defaults").Q()) |
                                   (oneOrMoreExpressions + (ToTerm("without") + "defaults").Q() + "from" + fieldList)) +
                                  attributeList.Q() +
                                  ("help" + numericConstant).Q() +
                                  (oneOrMoreInputGroupStatements + includeDefinitions.Q() + "end" + "input").Q();
            oneOrMoreInputGroupStatements.Rule = MakePlusRule(oneOrMoreInputGroupStatements, null, inputGroupStatement);
            inputArrayStatement.Rule = ToTerm("input") + "array" + expression +
                                       (ToTerm("without") + "defaults").Q() + "from" + oneOrMoreExpressions +
                                       ("help" + numericConstant).Q() +
                                       attributeList.Q() +
                                       (oneOrMoreInputGroupStatements + includeDefinitions.Q() + ToTerm("end") + "input").Q();

            menuEvents.Rule = (ToTerm("before") + "menu") |
                              ("command" +
                                (("key" + Lpar + keyList + Rpar).Q() +
                                 expression + expression.Q() + ("help" + numericConstant).Q())) |
                              ("on" + (ToTerm("idle") | "action") + Identifier);
            additionalExpression.Rule = comma + expression;
            zeroOrMoreAdditionalExpressions.Rule = MakeStarRule(zeroOrMoreAdditionalExpressions, null, additionalExpression);
            menuInsideStatement.Rule = ((ToTerm("next") | "show" | "hide") + "option" + (expression | "all") + zeroOrMoreAdditionalExpressions) |
                                       ((ToTerm("continue") | "exit") + "menu");
            menuGroupStatement.Rule = menuEvents + codeBlock.Q();
            zeroOrMoreMenuGroupStatements.Rule = MakeStarRule(zeroOrMoreMenuGroupStatements, null, menuGroupStatement);
            menuStatement.Rule = "menu" + expression + zeroOrMoreMenuGroupStatements + includeDefinitions.Q() + "end" + "menu";
            reservedLinePosition.Rule = ("first" + (plus + numericConstant).Q()) |
                                        numericConstant |
                                        ("last" + (minus + numericConstant).Q());
            specialWindowAttribute.Rule = (ToTerm("black") | "blue" | "cyan" | "green" | "magenta" | "red" | "white" |
                                            "yellow" | "bold" | "dim" | "normal" | "invisible") |
                                          "reverse" | "border" |
                                          ((ToTerm("prompt") | "form" | "menu" | "message") + "line" + reservedLinePosition) |
                                          (ToTerm("comment") + "line" + (reservedLinePosition | "off"));
            windowAttribute.Rule = oneOrMoreSpecialWindowAttributes;
            oneOrMoreSpecialWindowAttributes.Rule = MakePlusRule(oneOrMoreSpecialWindowAttributes, comma, specialWindowAttribute);
            windowAttributeList.Rule = (ToTerm("attribute") | "attributes") + Lpar + windowAttribute + Rpar;

            optionStatement.Rule = ((ToTerm("message") | "prompt" | "menu" | "comment" | "error" | "form") + "line" + expression) |
                                   ((ToTerm("insert") | "delete" | "next" | "previous" | "accept" | "help") + "key" + expression) |
                                   ("input" + (ToTerm("wrap") | (ToTerm("no") + "wrap"))) |
                                   (ToTerm("help") + "file" + expression) |
                                   ((ToTerm("input") | "display") + attributeList) |
                                   (ToTerm("sql") + "interrupt" + (ToTerm("on") | "off")) |
                                   (ToTerm("field") + "order" + (ToTerm("constrained") | "unconstrained"));
            optionsStatement.Rule = (ToTerm("option") | "options") + oneOrMoreOptionsStatements;
            oneOrMoreOptionsStatements.Rule = MakePlusRule(oneOrMoreOptionsStatements, comma, optionStatement);

            screenStatement.Rule = ("clear" + ("form" | ("window" + Identifier) | (ToTerm("window").Q() + "screen") | fieldList)) |
                                   (ToTerm("close") + "window" + Identifier) |
                                   (ToTerm("close") + "form" + Identifier) |
                                   constructStatement |
                                   (ToTerm("current") + "window" + "is" + ("screen" | Identifier)) |
                                   displayStatement |
                                   displayArrayStatement |
                                   (ToTerm("display") + "form" + Identifier + attributeList.Q()) |
                                   errorStatement |
                                   messageStatement |
                                   promptStatement |
                                   inputStatement |
                                   inputArrayStatement |
                                   menuStatement |
                                   (ToTerm("open") + "form" + expression + "from" + expression) |
                                   (ToTerm("open") + "window" + expression + "at" + expression + "from" + expression +
                                        ((ToTerm("with") + "form" + expression) | ("with" + expression + "rows" + "comma" + expression + "columns"))
                                    + windowAttributeList.Q()) |
                                   optionsStatement |
                                   ("scroll" + oneOrMoreFieldLists + (ToTerm("up") | "down") + ("by" + numericConstant).Q());
            oneOrMoreFieldLists.Rule = MakePlusRule(oneOrMoreFieldLists, comma, fieldList);

            sqlStatements.Rule = cursorManipulationStatement |
                                 dataDefinitionStatement |
                                 dataManipulationStatement |
                                 dynamicManagementStatement |
                                 queryOptimizationStatement |
                                 dataIntegrityStatement |
                                 clientServerStatement;

            cursorManipulationStatement.Rule = ("close" + cursorName) |
                                               ("declare" + cursorName +
                                                    ("cursor" + (ToTerm("with") + "hold").Q() + "for" +
                                                        ((sqlSelectStatement + (ToTerm("for") + "update" + ("of" + columnsList).Q()).Q()) |
                                                         sqlInsertStatement |
                                                         statementId)
                                                    ) |
                                                    (ToTerm("scroll") + "cursor" + (ToTerm("with") + "hold").Q() + "for" +
                                                        (sqlSelectStatement | statementId))
                                                ) |
                                                ("fetch" +
                                                    ("next" | (ToTerm("previous") | "prior") | "first" | "last" | "current" |
                                                        ("relative" + expression) | ("absolute" + expression)).Q() +
                                                    cursorName + ("into" + variableList).Q()) |
                                                ("flush" + cursorName) |
                                                ("open" + cursorName + ("using" + variableOrConstantList).Q()) |
                                                ("put" + cursorName + ("from" + variableOrConstantList).Q());
            columnsList.Rule = MakeStarRule(columnsList, comma, columnsTableId);
            statementId.Rule = constantIdentifier;
            cursorName.Rule = Identifier;
            dataType.Rule = type;
            columnItem.Rule = constantIdentifier +
                                (dataType | ((ToTerm("byte") | "text") + ("in" + ("table" | constantIdentifier)).Q()) + (ToTerm("not") + "null").Q()) |
                              ("unique" + Lpar + oneOrMoreConstantIdentifiers.Q() + Rpar + ("constraint" + constantIdentifier).Q());
            oneOrMoreColumnItems.Rule = MakePlusRule(oneOrMoreColumnItems, comma, columnItem);
            dataDefinitionStatement.Rule = (ToTerm("drop") + "table" + constantIdentifier) |
                                           ("create" + ToTerm("temp").Q() + "table" + constantIdentifier +
                                                Lpar + oneOrMoreColumnItems + Rpar +
                                                (ToTerm("with") + "no" + "log").Q() +
                                                (ToTerm("in") + constantIdentifier).Q() +
                                                (ToTerm("extent") + "size" + numericConstant).Q() +
                                                (ToTerm("next") + "size" + numericConstant).Q() +
                                                (ToTerm("lock") + "mode" + Lpar + (ToTerm("page") | "row") + Rpar).Q()) |
                                           ("create" + ToTerm("unique").Q() + ToTerm("cluster").Q() +
                                                "index" + constantIdentifier + "on" + constantIdentifier +
                                                Lpar + oneOrMoreConstantIdentifiersWithAscDesc + Rpar) |
                                           (ToTerm("drop") + "index" + constantIdentifier);
            constantIdentifierWithAscDesc.Rule = constantIdentifier + (ToTerm("asc") | "desc").Q();
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
            headSelectStatement.Rule = "select" + ("all" | (ToTerm("distinct") | "unique")).Q() + selectList;
            tableIdentifier.Rule = ((constantIdentifier + AtSym + constantIdentifier + colon + constantIdentifier) |
                                        (constantIdentifier + colon + constantIdentifier) |
                                        constantIdentifier);     // TODO: might still be wrong, but it's a better attempt at removing reduce-reduce conflict
            
            fromTable.Rule = ToTerm("outer").Q() + tableIdentifier + sqlAlias.Q();
            tableExpression.Rule = simpleSelectStatement;
            fromTableExpression.Rule = fromTable | (Lpar + tableExpression + Rpar + sqlAlias.Q());
            oneOrMoreFromTableExpressions.Rule = MakePlusRule(oneOrMoreFromTableExpressions, comma, fromTableExpression);
            fromSelectStatement.Rule = "from" + oneOrMoreFromTableExpressions;
            aliasName.Rule = Identifier;

            mainSelectStatement.Rule = headSelectStatement + ("into" + variableList).Q() +
                                       fromSelectStatement + whereStatement.Q() +
                                       groupByStatement.Q() + havingStatement.Q() +
                                       unionSelectStatement.Q() + orderbyStatement.Q() +
                                       (ToTerm("into") + "temp" + Identifier).Q() +
                                       (ToTerm("with") + "no" + "log").Q();
            unionSelectStatement.Rule = "union" + ToTerm("all").Q() + simpleSelectStatement;
            simpleSelectStatement.Rule = headSelectStatement +
                                         fromSelectStatement +
                                         whereStatement.Q() +
                                         groupByStatement.Q() +
                                         havingStatement.Q() +
                                         unionSelectStatement.Q();
            whereStatement.Rule = "where" + condition;
            groupByStatement.Rule = ToTerm("group") + "by" + variableOrConstantList;
            havingStatement.Rule = "having" + condition;
            orderbyColumn.Rule = expression + (ToTerm("asc") | "desc").Q();
            orderbyStatement.Rule = ToTerm("order") + "by" + oneOrMoreOrderByColumns;
            oneOrMoreOrderByColumns.Rule = MakePlusRule(oneOrMoreOrderByColumns, comma, orderbyColumn);
            sqlLoadStatement.Rule = ToTerm("load") + "from" + (variable | StringLiteral) +
                                    ("delimiter" + (variable | StringLiteral)).Q() +
                                    ((ToTerm("insert") + "into" + tableIdentifier + (Lpar + columnsList + Rpar).Q()) |
                                        sqlInsertStatement);
            sqlUnLoadStatement.Rule = ToTerm("unload") + "to" + (variable | StringLiteral) +
                                      ("delimiter" + (variable | StringLiteral)).Q() +
                                      sqlSelectStatement;
            sqlInsertStatement.Rule = ToTerm("insert") + "into" + tableIdentifier + (Lpar + columnsList + Rpar).Q() +
                                      (("values" + Lpar + oneOrMoreExpressions + Rpar) | simpleSelectStatement);
            sqlUpdateStatement.Rule = "update" + tableIdentifier + "set" +
                                      (oneOrMoreColumnsTableIdEqualExpressions |
                                            ((Lpar + columnsList + Rpar) | ((aliasName + dot).Q() + star)) +
                                            singleEqual + ((Lpar + oneOrMoreExpressions + Rpar) | ((aliasName + dot).Q() + star))) +
                                      ("where" + (condition | ("current" + "of" + cursorName))).Q();
            columnsTableIdEqualExpression.Rule = columnsTableId + singleEqual + expression;
            oneOrMoreColumnsTableIdEqualExpressions.Rule = MakePlusRule(oneOrMoreColumnsTableIdEqualExpressions, comma, columnsTableIdEqualExpression);
            sqlDeleteStatement.Rule = ToTerm("delete") + "from" + tableIdentifier +
                                      ("where" + (condition | ("current" + "of" + cursorName))).Q();
            dynamicManagementStatement.Rule = ("prepare" + cursorName + "from" + expression) |
                                              ("execute" + cursorName + ("using" + variableList).Q()) |
                                              ("free" + (cursorName | statementId)) |
                                              (ToTerm("lock") + "table" + expression + "in" + (ToTerm("share") | "exclusive") + "mode");
            queryOptimizationStatement.Rule = (ToTerm("update") + "statistics" + ("for" + "table" + tableIdentifier).Q()) |
                                              (ToTerm("set") + "lock" + "mode" + "to" + (("wait" + ToTerm("seconds").Q()) | "not" + "wait")) |      // TODO: not sure if that's right...
                                              (ToTerm("set") + "explain" + (ToTerm("on") | "off")) |
                                              (ToTerm("set") + "isolation" + "to" +
                                                ((ToTerm("cursor") + "stability") | ((ToTerm("dirty") | "committed" | "repeatable") + "read"))) |
                                              ("set" + ToTerm("buffered").Q() + "log");
            databaseDeclaration.Rule = "database" + constantIdentifier + (AtSym + constantIdentifier).Q() + ToTerm("exclusive").Q() + semi.Q();
            clientServerStatement.Rule = ToTerm("close") + "database";
            dataIntegrityStatement.Rule = wheneverStatement |
                                          ((ToTerm("begin") | "commit" | "rollback") + "work");
            wheneverStatement.Rule = "whenever" + wheneverType + wheneverFlow;
            wheneverType.Rule = (ToTerm("not") + "found") |
                                (ToTerm("any").Q() + (ToTerm("sqlerror") | "error")) |
                                (ToTerm("sqlwarning") | "warning");
            wheneverFlow.Rule = "continue" | ToTerm("stop") | ("call" + Identifier) |
                                (((ToTerm("go") + "to") | ToTerm("goto")) + colon.Q() + Identifier);

            reportDefinition.Rule = "report" + Identifier + parameterList + typeDeclarations.Q() +
                                    outputReport.Q() + ("order" + ToTerm("external").Q() + "by" + variableList).Q() +
                                    formatReport.Q() + "end" + "report";
            outputReport.Rule = "output" + (ToTerm("report") + "to" + (StringLiteral | ("pipe" + StringLiteral) | "printer")).Q() +
                                zeroOrMoreReportDimensionSpecifiers;
            formatReport.Rule = "format" + ((ToTerm("every") + "row") | oneOrMoreReportCodeBlocks);
            oneOrMoreReportCodeBlocks.Rule = MakePlusRule(oneOrMoreReportCodeBlocks, null, reportCodeBlock);
            reportCodeBlock.Rule = ((ToTerm("first").Q() + "page" + "header") |
                                    (ToTerm("page") + "trailer") |
                                    (ToTerm("on") + ((ToTerm("every") + "row") | (ToTerm("last") + "row"))) |
                                    ((ToTerm("before") | "after") + "group" + "of" + variable))
                                   + codeBlock;
        }
    }
}
