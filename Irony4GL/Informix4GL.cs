using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Irony.Samples.Informix4GL
{
    [Language("4GL", "1.0", "Informix 4GL Grammar")]
    public class Informix4GLGrammar : Grammar
    {
        public Informix4GLGrammar()
        {
            // Use C#'s string literal for right now
            StringLiteral StringLiteral = TerminalFactory.CreateCSharpString("StringLiteral");

            // Use C#'s char literal for right now
            StringLiteral CharLiteral = TerminalFactory.CreateCSharpChar("CharLiteral");

            // Use C#'s number for right now
            NumberLiteral Number = TerminalFactory.CreateCSharpNumber("Number");

            // Use C#'s identifier for right now
            IdentifierTerminal Identifier = TerminalFactory.CreateCSharpIdentifier("Identifier");

            // Symbols
            KeyTerm plus = ToTerm("+", "plus");
            KeyTerm minus = ToTerm("-", "minus");
            KeyTerm colon = ToTerm(":", "colon");
            KeyTerm semi = ToTerm(";", "semi");
            KeyTerm equal = ToTerm("=", "equal");
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
            KeyTerm tgoto = ToTerm("goto");
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
            var constructInsideStatement = new NonTerminal("constructInsideStatement");
            var displayInsideStatement = new NonTerminal("displayInsideStatement");
            var inputInsideStatement = new NonTerminal("inputInsideStatement");

            var runStatement = new NonTerminal("runStatement");
            var variable = new NonTerminal("variable");
            var expression = new NonTerminal("expression");
            var procedureIdentifier = new NonTerminal("procedureIdentifier");
            var actualParameter = new NonTerminal("actualParameter");
            var oneOrMoreExpressions = new NonTerminal("oneOrMoreExpressions");
            var oneOrMoreActualParameters = new NonTerminal("oneOrMoreActualParameters");
            var oneOrMoreVariables = new NonTerminal("oneOrMoreVariables");
            var gotoStatement = new NonTerminal("gotoStatement");

            /************************************************************************************************************/
            // initialize the root
            Root = compilation_unit;

            constantIdentifier.Rule = ToTerm("accept") | "ascii" | "count" | "current" | "false" | "first" | "found" | "group" |
                                      "hide" | "index" | "int_flag" | "interrupt" | "last" | "length" | "lineno" | "mdy" | "no" |
                                      "not" | "notfound" | "null" | "pageno" | "real" | "size" | "sql" | "status" | "temp" | "time" |
                                      "today" | "true" | "user" | "wait" | "weekday" | "work" | Identifier;

            // Rules
            compilation_unit.Rule = databaseDeclaration.Q() +
                                    globalDeclaration.Q() +
                                    typeDeclarations +
                                    mainBlock.Q() +
                                    functionOrReportDefinitions;

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

            oneOrMoreConstantIdentifiers.Rule = MakePlusRule(oneOrMoreConstantIdentifiers, comma, constantIdentifier);
            constantIdentifierAndTypePair.Rule = constantIdentifier + type;
            oneOrMoreConstantIdentifierAndTypePairs.Rule = MakePlusRule(oneOrMoreConstantIdentifierAndTypePairs, comma, constantIdentifierAndTypePair);
            variableDeclaration.Rule = (oneOrMoreConstantIdentifiers + type) | oneOrMoreConstantIdentifierAndTypePairs;

            type.Rule = typeIdentifier | indirectType | largeType | structuredType;
            indirectType.Rule = "like" + tableIdentifier + dot + Identifier;
            typeIdentifier.Rule = charType | numberType | timeType;
            largeType.Rule = ToTerm("text") | "byte";
            sign.Rule = plus | minus;
            numericConstant.Rule = Number | (sign + Number);
            numberType.Rule = ToTerm("bigint") | "integer" | "int" | "smallint" | "real" | "smallfloat" |
                              ((ToTerm("decimal") | "dec" | "numeric" | "money") + ((Lpar + numericConstant + ("," + numericConstant).Q() + Rpar) | Empty)) |
                              ((ToTerm("float") | "double") + ((Lpar + numericConstant + Rpar) | Empty));
            charType.Rule = ((ToTerm("varchar") | "nvarchar") + Lpar + numericConstant + ("," + numericConstant).Q() + Rpar) |
                            ((ToTerm("char") | "nchar" | "character") + ((Lpar + numericConstant + Rpar) | Empty));
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
            dynArrayType.Rule = ToTerm("dynamic") + "array" + "with" + numericConstant + "dimensions" + "of" +
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
                                   constructInsideStatement |
                                   displayInsideStatement |
                                   inputInsideStatement;

            runStatement.Rule = "run" + (variable | StringLiteral) +
                                ((ToTerm("in") + "form" + "mode") | (ToTerm("in") + "line" + "mode")).Q() +
                                ((ToTerm("without") + "waiting") | ("returning" + variable)).Q();
            oneOrMoreExpressions.Rule = MakePlusRule(oneOrMoreExpressions, comma, expression);
            assignmentStatement.Rule = "let" + variable + equal + oneOrMoreExpressions;
            oneOrMoreVariables.Rule = MakePlusRule(oneOrMoreVariables, comma, variable);
            oneOrMoreActualParameters.Rule = MakePlusRule(oneOrMoreActualParameters, comma, actualParameter);
            procedureStatement.Rule = "call" + procedureIdentifier +
                                      ((Lpar + oneOrMoreActualParameters.Q() + Rpar) | Empty) +
                                      ("returning" + oneOrMoreVariables).Q();
            procedureIdentifier.Rule = functionIdentifier;
            actualParameter.Rule = star | expression;
            gotoStatement.Rule = "goto" + colon.Q() + label;

            

            // dummy for test
            databaseDeclaration.Rule = "database";
            reportDefinition.Rule = "reportDef";
            variableOrConstantList.Rule = "varConstList";
            functionIdentifier.Rule = "functionId";
            tableIdentifier.Rule = "tableId";
            structuredStatement.Rule = "structuredStatement";
            sqlStatements.Rule = "sqlStatements";
            otherFGLStatement.Rule = "otherFGLStatement";
            menuInsideStatement.Rule = "menuInsideStatement";
            constructInsideStatement.Rule = "constructInsideStatement";
            displayInsideStatement.Rule = "displayInsideStatement";
            inputInsideStatement.Rule = "inputInsideStatement";

            variable.Rule = "variable";
            expression.Rule = "expression";
        }
    }
}
