using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Adornments;
using System.Threading;

namespace Informix4GLLanguage.Classification
{
    internal struct Informix4GLError
    {
        public Informix4GLError(Irony.Parsing.Token token, Irony.Parsing.ParserMessage message)
        {
            Token = token;
            Message = message;
        }

        public Irony.Parsing.Token Token;
        public Irony.Parsing.ParserMessage Message;
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(Informix4GLErrorTag))]
    [ContentType("Informix4GL")]
    internal sealed class Informix4GLSyntaxErrorTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            //create a single tagger for each buffer.
            return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(
                "Informix4GLSyntaxErrorTagger", () => new Informix4GLSyntaxErrorTagger(buffer) as ITagger<T>);
        }
    }

    internal sealed class Informix4GLSyntaxErrorTagger : ITagger<Informix4GLErrorTag>, IDisposable
    {
        ITextBuffer buffer;
        ITextSnapshot snapshot;
        List<Informix4GLError> errorTokens = new List<Informix4GLError>();
        Timer delayTimer;
       
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public Informix4GLSyntaxErrorTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
            this.snapshot = buffer.CurrentSnapshot;
            this.ReParse();
            this.buffer.Changed += BufferChanged;
        }

        public IEnumerable<ITagSpan<Informix4GLErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0 || this.errorTokens.Count == 0)
                yield break;

            List<Informix4GLError> currentErrors = this.errorTokens;
            ITextSnapshot currentSnapshot = spans[0].Snapshot;
            SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;
            
            foreach (var error in currentErrors)
            {
                if (error.Token != null)
                {
                    if (error.Token.Location.Line <= endLineNumber && error.Token.Location.Line >= startLineNumber)
                    {
                        int length = Math.Max(error.Token.Length, 1);
                        length = Math.Min(length, 100);
                        length = Math.Min(currentSnapshot.Length - error.Token.Location.Position - 1, length);
                        if (length < 0)
                        {
                            length = 0;
                        }

                        var line = currentSnapshot.GetLineFromLineNumber(error.Token.Location.Line);
                        var startPosition = line.Start.Position + error.Token.Location.Column;
                        var errorMsg = error.Message != null ? error.Message.Message : error.Token.ValueString;
                        yield return new TagSpan<Informix4GLErrorTag>(
                            new SnapshotSpan(currentSnapshot, startPosition, length),
                            new Informix4GLErrorTag(errorMsg));
                    }
                }
            }
        }

        private void ReParse()
        {
            int previousCount = errorTokens.Count;
            errorTokens.Clear();

            ITextSnapshot newSnapshot = this.buffer.CurrentSnapshot;
            string text = newSnapshot.GetText();

            var parser = Informix4GLFactory.Parser;
            var newErrors = new List<Irony.Parsing.Token>();
            var parseTree = parser.Parse(text);

            // TODO: need a better way to do this...
            // First want to look at the parser's error messages, and associate a token with them
            foreach (var err in parseTree.ParserMessages)
            {
                if (parseTree.Tokens.Any(x => x.Location.Line == err.Location.Line))
                {
                    errorTokens.Add(new Informix4GLError(parseTree.Tokens.First(x => x.Location.Line == err.Location.Line), err));
                }
                else
                {
                    // look up one line (or more) until we find a token to assign this error to
                    int i = err.Location.Line - 1;
                    while (i >= 0)
                    {
                        if (parseTree.Tokens.Any(x => x.Location.Line == i))
                        {
                            errorTokens.Add(new Informix4GLError(parseTree.Tokens.First(x => x.Location.Line == i), err));
                            break;
                        }
                    }
                    if (i < 0)
                    {
                        errorTokens.Add(new Informix4GLError(null, err));
                    }
                }
            }

            // Get the tokens as well
            foreach (var token in parseTree.Tokens)
            {
                if (token.IsError())
                {
                    errorTokens.Add(new Informix4GLError(token, null));
                }
            }

            if (previousCount != 0 || errorTokens.Count != 0)
            {
                if (this.TagsChanged != null)
                    this.TagsChanged(this, new SnapshotSpanEventArgs(
                        new SnapshotSpan(this.snapshot, 0, this.snapshot.Length)));
            }
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != buffer.CurrentSnapshot)
                return;

            if (delayTimer != null)
                delayTimer.Dispose();

            delayTimer = new Timer(o => this.ReParse(), null, 500, Timeout.Infinite);
        }

        #region IDisposable Members

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                if(delayTimer != null)
                    delayTimer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    internal class Informix4GLErrorTag : ErrorTag
    {
        public Informix4GLErrorTag() : base(PredefinedErrorTypeNames.SyntaxError) { }

        public Informix4GLErrorTag(string message) : base(PredefinedErrorTypeNames.SyntaxError, message) { }
    }

    internal static class SyntaxErrorInfoExtensions
    {
        public static SnapshotSpan AsSnapshotSpan(this Irony.Parsing.Token token, ITextSnapshot snapshot)
        {
            return new SnapshotSpan(snapshot, token.Location.Position, Math.Max(token.Length, 1));
        }
    }
}
