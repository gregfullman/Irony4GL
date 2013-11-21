using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Informix4GLGrammar;
using System.Text.RegularExpressions;

namespace Informix4GLLanguage.Classification
{
    //[Export(typeof(ITaggerProvider))]
    //[ContentType("Lua")]
    //[TagType(typeof(LuaCommentTag))]
    //internal sealed class LuaCommentTagProvider : ITaggerProvider
    //{
    //    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
    //    {
    //        //create a single tagger for each buffer.
    //        return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(
    //            "LuaCommentTagger", () => new LuaCommentTagger(buffer) as ITagger<T>);
    //    }
    //}

    //internal sealed class LuaCommentTagger : ITagger<LuaCommentTag>
    //{
    //    ITextBuffer _buffer;
    //    ITextSnapshot _snapshot;
    //    List<SnapshotSpan> _commentSpans = new List<SnapshotSpan>();

    //    internal LuaCommentTagger(ITextBuffer buffer)
    //    {
    //        _buffer = buffer;
    //        _snapshot = buffer.CurrentSnapshot;
    //        ReParse();

    //        _buffer.Changed += BufferChanged;   
    //    }

    //    public event EventHandler<SnapshotSpanEventArgs> TagsChanged
    //    {
    //        add { }
    //        remove { }
    //    }

    //    public IEnumerable<ITagSpan<LuaCommentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    //    {
    //        if (_commentSpans.Count == 0)
    //            yield break;

    //        var commentTag = new LuaCommentTag();
    //        foreach (var span in _commentSpans)
    //        {
    //            yield return new TagSpan<LuaCommentTag>(span, commentTag);
    //        }
    //    }


    //    private void ReParse()
    //    {
    //        ITextSnapshot newSnapshot = _buffer.CurrentSnapshot;
    //        string text = newSnapshot.GetText();

    //        var newComments = new List<SnapshotSpan>();

    //        int offset = 0;
    //        int startIndex = -1;
    //        do
    //        {
    //            startIndex = text.IndexOf("--", offset);
    //            if (startIndex > -1)
    //            {
    //                int length = 2;
    //                int commentLevel = 0;

    //                //Test for long comment or line comment
    //                if (text[startIndex + 2] == '[')
    //                {
    //                    string commentText = text.Substring(source.PreviewPosition + StartSymbol.Length);
    //                    var match = Regex.Match(commentText, @"^\[(=*)\[");
    //                    if (match.Value != string.Empty)
    //                    {
    //                        commentLevel = (byte)(match.Groups[1].Value.Length + 1);
    //                    }
    //                }
    //                else
    //                {
    //                    var endLine = text.IndexOf('\n', startIndex);
    //                    length = endLine - startIndex;
    //                }

    //                newComments.Add(new SnapshotSpan(newSnapshot, startIndex, length));
    //                offset += length;
    //            }

    //        } while (startIndex > -1);


    //        var startIndex = text.IndexOf("--");

    //        bool finished = (startIndex > -1);
    //        while (!finished)
    //        {


    //            var startIndex = text.IndexOf("--");
    //            if (startIndex == -1)
    //                finished = true;
    //        }


    //        var startLineNumber = newSnapShot.Start.GetContainingLine().LineNumber;
    //        var endLineNumber = curSpan.End.GetContainingLine().LineNumber;

    //        for (int lineNumber = startLineNumber; lineNumber <= endLineNumber; lineNumber++)
    //        {
    //            var line = curSpan.Snapshot.GetLineFromLineNumber(lineNumber);
    //            var lineText = line.GetText();
    //            if (lineText != String.Empty)
    //            {
    //                var startIndex = lineText.IndexOf("--");
    //                if (startIndex > -1)
    //                {
    //                    var startPosition = line.Start.Position + startIndex;
    //                    var endPosition = line.End.Position;
    //                    var tokenSpan = new SnapshotSpan(line.Snapshot, new Span(startPosition, endPosition - startPosition));
    //                    if (tokenSpan.IntersectsWith(curSpan))
    //                        yield return new TagSpan<LuaCommentTag>(tokenSpan, commentTag);
    //                }
    //            }
    //        }

    //        UpdateTags(newSnapshot, newComments);
    //    }

    //    private void UpdateTags(ITextSnapshot newSnapshot, List<Irony.Parsing.Token> newErrors)
    //    {
    //        //Determine the changed span. This sends a changed event that contains the new spans, plus any spans that may have been removed from the old regions.
    //        List<Span> oldSpans =
    //            new List<Span>(this.errorTokens.Select(r => r.AsSnapshotSpan(this.snapshot).TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive).Span));
    //        List<Span> newSpans = new List<Span>(newErrors.Select(r => r.AsSnapshotSpan(newSnapshot).Span));
    //        NormalizedSpanCollection oldSpanCollection = new NormalizedSpanCollection(oldSpans);
    //        NormalizedSpanCollection newSpanCollection = new NormalizedSpanCollection(newSpans);
    //        //the changed regions are regions that appear in one set or the other, but not both.
    //        NormalizedSpanCollection removed = NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);
    //        int changeStart = int.MaxValue;
    //        int changeEnd = -1;

    //        if (removed.Count > 0)
    //        {
    //            changeStart = removed[0].Start;
    //            changeEnd = removed[removed.Count - 1].End;
    //        }

    //        if (newSpans.Count > 0)
    //        {
    //            changeStart = Math.Min(changeStart, newSpans[0].Start);
    //            changeEnd = Math.Max(changeEnd, newSpans[newSpans.Count - 1].End);
    //        }

    //        this.snapshot = newSnapshot;
    //        this.errorTokens = newErrors;

    //        if (changeStart <= changeEnd)
    //            if (this.TagsChanged != null)
    //                this.TagsChanged(this, new SnapshotSpanEventArgs(
    //                    new SnapshotSpan(this.snapshot, Span.FromBounds(changeStart, changeEnd))));
    //    }

    //    private void BufferChanged(object sender, TextContentChangedEventArgs e)
    //    {
    //        // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
    //        if (e.After != _buffer.CurrentSnapshot)
    //            return;

    //        this.ReParse();
    //    }
    //}

    //internal class LuaCommentTag : ITag
    //{
    //    public LuaCommentTag(){}
    //}

    ////internal static class SyntaxErrorInfoExtensions
    ////{
    ////    public static SnapshotSpan AsSnapshotSpan(this Irony.Parsing.Token token, ITextSnapshot snapshot)
    ////    {
    ////        return new SnapshotSpan(snapshot, token.Location.Position, Math.Max(token.Length, 1));
    ////    }
    ////}
}
