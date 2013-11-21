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
        List<Irony.Parsing.Token> errorTokens = new List<Irony.Parsing.Token>();
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

            List<Irony.Parsing.Token> currentErrors = this.errorTokens;
            ITextSnapshot currentSnapshot = spans[0].Snapshot;
            SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;
            
            foreach (var error in currentErrors)
            {
                if (error.Location.Line <= endLineNumber && error.Location.Line >= startLineNumber)
                {
                    int length = Math.Max(error.Length, 1);
                    length = Math.Min(length, 100);
                    length = Math.Min(currentSnapshot.Length - error.Location.Position - 1, length);

                    var line = currentSnapshot.GetLineFromLineNumber(error.Location.Line);
                    var startPosition = line.Start.Position + error.Location.Column;
                    yield return new TagSpan<Informix4GLErrorTag>(
                        new SnapshotSpan(currentSnapshot, startPosition, length),
                        new Informix4GLErrorTag(error.ValueString));
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
            foreach (var token in parseTree.Tokens)
            {
                if (token.IsError())
                {
                    errorTokens.Add(token);
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
