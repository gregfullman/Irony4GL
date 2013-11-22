#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Irony.Parsing {

  public enum TokenFlags {
    IsIncomplete = 0x01,
  }

  public enum TokenCategory {
    Content,
    Outline, //newLine, indent, dedent
    Comment,
    Directive,
    Error,
  }

  public class TokenList : List<Token> {}
  public class TokenStack : Stack<Token> { }

  //Tokens are produced by scanner and fed to parser, optionally passing through Token filters in between. 
  public class Token  {
    public Terminal Terminal {get; private set;}
    public ParseTreeNode TreeNode { get; private set; }
    public KeyTerm KeyTerm;
    public Symbol Symbol { get; protected internal set; }
    public readonly SourceLocation Location; 
    public readonly string Text;
    
    public object Value;
    public string ValueString {
      get { return (Value == null ? string.Empty : Value.ToString()); }
    } 

    public object Details;
    public TokenFlags Flags; 
    public TokenEditorInfo EditorInfo;

    public Token(Terminal term, SourceLocation location, string text, object value)  {
      SetTerminal(term);
      this.KeyTerm = term as KeyTerm;
      Location = location;
      Text = text;
      Value = value; 
    }

    public void SetTerminal(Terminal terminal) {
      Terminal = terminal; 
      this.EditorInfo = Terminal.EditorInfo;  //set to term's EditorInfo by default
    }

    // provide a link back to the corresponding parse tree node
    public void SetTreeNode(ParseTreeNode node)
    {
        TreeNode = node;
    }

    public bool IsSet(TokenFlags flag) {
      return (Flags & flag) != 0;
    }
    public TokenCategory Category  {
      get { return Terminal.Category; }
    }

    public bool IsError() {
      return Category == TokenCategory.Error;
    }

    public int Length {
      get { return Text == null ? 0 : Text.Length; }
    }

    public Token OtherBrace {  //matching opening/closing brace
      get { return _otherBrace; }
    } Token _otherBrace;

    public static void LinkMatchingBraces(Token openingBrace, Token closingBrace) {
      openingBrace._otherBrace = closingBrace;
      closingBrace._otherBrace = openingBrace;
    }

    public short ScannerState; //Scanner state after producing token 

    [System.Diagnostics.DebuggerStepThrough]
    public override string ToString() {
      return Terminal.TokenToString(this);
    }//method

  }//class

  //Some terminals may need to return a bunch of tokens in one call to TryMatch; MultiToken is a container for these tokens
  public class MultiToken : Token {
    public TokenList ChildTokens;
    public MultiToken(Terminal term, SourceLocation location, TokenList childTokens) : base(term, location, string.Empty, null) {
      ChildTokens = childTokens;
    }
  }//class

}//namespace
