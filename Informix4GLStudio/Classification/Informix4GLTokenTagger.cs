using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Informix4GLGrammar;

namespace Informix4GLLanguage.Classification
{
    //[Export(typeof(ITaggerProvider))]
    //[ContentType("Lua")]
    //[TagType(typeof(LuaTokenTag))]
    //internal sealed class LuaTerminalTagProvider : ITaggerProvider
    //{
    //    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
    //    {
    //        return new LuaTerminalTagger(buffer) as ITagger<T>;
    //    }
    //}

    //internal sealed class LuaTerminalTagger : ITagger<LuaTokenTag>
    //{
    //    ITextBuffer _buffer;
    //    char[] _separators;
    //    ITagAggregator<LuaCommentTag> _aggregator;
    //    HashSet<string> _keywords = new HashSet<string>();
    //    HashSet<string> _symbols = new HashSet<string>();

    //    internal LuaTerminalTagger(ITextBuffer buffer,
    //                                ITagAggregator<LuaCommentTag> luaTagAggregator)
    //    {
    //        _buffer = buffer;
    //        _aggregator = luaTagAggregator;

    //        //Add standard Lua keywords here
    //        _keywords.Add("function");
    //        _keywords.Add("local");
    //        _keywords.Add("do");
    //        _keywords.Add("end");
    //        _keywords.Add("while");
    //        _keywords.Add("repeat");
    //        _keywords.Add("until");
    //        _keywords.Add("if");
    //        _keywords.Add("then");
    //        _keywords.Add("elseif");
    //        _keywords.Add("else");
    //        _keywords.Add("for");
    //        _keywords.Add("in");
    //        _keywords.Add("function");
    //        _keywords.Add("return");
    //        _keywords.Add("break");
    //        _keywords.Add("nil");
    //        _keywords.Add("false");
    //        _keywords.Add("true");
    //        _keywords.Add("not");
    //        _keywords.Add("and");
    //        _keywords.Add("or");

    //        //operators
    //        //. : { } ( ) = + <= >= ~=
    //        //- + * / [ ] , ; % ^ ==
    //        //# .. ... < >

    //        //Tokenizing text is performed in this order:
    //        //comment
    //        //string
    //        //numbers
    //        //symbols
    //        //keywords
    //        //identifiers

    //    }

    //    public event EventHandler<SnapshotSpanEventArgs> TagsChanged
    //    {
    //        add { }
    //        remove { }
    //    }

    //    public IEnumerable<ITagSpan<LuaTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    //    {
    //        var commentTags = _aggregator.GetTags(spans);

    //        foreach (SnapshotSpan curSpan in spans)
    //        {
    //            ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
    //            int curLoc = containingLine.Start.Position;
    //            string[] tokens = containingLine.GetText().ToLower().Split(' ');

    //            foreach (var token in tokens)
    //            {
    //                if (_keywords.Contains(token))
    //                {
    //                    var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, token.Length));
    //                    if (tokenSpan.IntersectsWith(curSpan))
    //                        yield return new TagSpan<LuaTokenTag>(tokenSpan,
    //                                                              new LuaTokenTag(LuaTokenTypes.Keyword));
    //                }

    //                //add an extra char location because of the space
    //                curLoc += token.Length + 1;
    //            }
    //        }
    //    }
    //}

}
