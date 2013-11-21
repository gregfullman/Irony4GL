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
using Informix4GLGrammar;
using System.Threading;

namespace Informix4GLLanguage.Classification
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("Informix4GL")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class Informix4GLClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name("Informix4GL")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition Informix4GLContentType = null;

        [Export]
        [FileExtension(".4gl")]
        [ContentType("Informix4GL")]
        internal static FileExtensionToContentTypeDefinition Informix4GLFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new Informix4GLClassifier(buffer, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    /// <summary>
    /// This is effectively the replacement to the LineScanner from 2008.
    /// This class must handle very quick processing times during GetTags() 
    /// as it is called very frequently!
    /// </summary>
    internal sealed class Informix4GLClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        Irony.Parsing.Parser _parser;

        IDictionary<Irony.Parsing.TokenType, ClassificationTag> _4glTags;
        ClassificationTag _commentTag;


        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        Dictionary<int, int> _lineStates = new Dictionary<int, int>();

        internal Informix4GLClassifier(ITextBuffer buffer,
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;

            _parser = Informix4GLFactory.Parser;
            _parser.Context.Mode = Irony.Parsing.ParseMode.VsLineScan;

            _4glTags = new Dictionary<Irony.Parsing.TokenType, ClassificationTag>();
            _4glTags[Irony.Parsing.TokenType.Text]          = BuildTag(typeService, PredefinedClassificationTypeNames.Character);
            _4glTags[Irony.Parsing.TokenType.Keyword]       = BuildTag(typeService, PredefinedClassificationTypeNames.Keyword);
            _4glTags[Irony.Parsing.TokenType.Identifier]    = BuildTag(typeService, PredefinedClassificationTypeNames.Identifier);
            _4glTags[Irony.Parsing.TokenType.String]        = BuildTag(typeService, PredefinedClassificationTypeNames.String);
            _4glTags[Irony.Parsing.TokenType.Literal]       = BuildTag(typeService, PredefinedClassificationTypeNames.Literal);
            _4glTags[Irony.Parsing.TokenType.Operator]      = BuildTag(typeService, PredefinedClassificationTypeNames.Operator);
            _4glTags[Irony.Parsing.TokenType.LineComment]   = BuildTag(typeService, PredefinedClassificationTypeNames.Comment);
            _4glTags[Irony.Parsing.TokenType.Comment]       = BuildTag(typeService, PredefinedClassificationTypeNames.Comment);

            _commentTag = BuildTag(typeService, PredefinedClassificationTypeNames.Comment);

            InitializeLineStates(_buffer.CurrentSnapshot);
        }

        /// <summary>
        /// In the context of a classification tagger, this is called initially w/ spans for all
        /// content in the file.
        /// It is called immediately after the user modifies text given the span of text that was modified.
        /// It is also called for all lines that are newly visible due to scrolling.
        /// This function gets called ALOT.  Keep processing times to a minimal and try to only handle 1 line at a
        /// time.
        /// </summary>
        /// <param name="spans"></param>
        /// <returns></returns>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            var snapShot = spans[0].Snapshot;
            foreach (var span in spans)
            {
                var startLine = span.Start.GetContainingLine();
                var endLine = span.End.GetContainingLine();

                var startLineNumber = startLine.LineNumber;
                var endLineNumber = endLine.LineNumber;

                for (int i = startLineNumber; i <= endLineNumber; i++)
                {
                    var line = spans[0].Snapshot.GetLineFromLineNumber(i);
                    _parser.Scanner.VsSetSource(line.GetText(), 0);

                    int state = 0;
                    _lineStates.TryGetValue(i, out state);

                    var token = _parser.Scanner.VsReadToken(ref state);
                    while (token != null)
                    {
                        if (token.Category == Irony.Parsing.TokenCategory.Content)
                        {
                            if (token.EditorInfo != null)
                            {
                                ClassificationTag tag;
                                if (_4glTags.TryGetValue(token.EditorInfo.Type, out tag))
                                {

                                    var location = new SnapshotSpan(snapShot, line.Start.Position + token.Location.Position, token.Length);
                                    yield return new TagSpan<ClassificationTag>(location, tag);
                                    
                                }
                            }
                        }
                        else if (token.Category == Irony.Parsing.TokenCategory.Comment)
                        {

                            var location = new SnapshotSpan(snapShot, line.Start.Position + token.Location.Position, token.Length);
                            yield return new TagSpan<ClassificationTag>(location, _commentTag);
                        }

                        token = _parser.Scanner.VsReadToken(ref state);
                    }

                    int oldState = 0;
                    _lineStates.TryGetValue(i + 1, out oldState);
                     _lineStates[i + 1] = state;
                    
                    //We're going into overtime, process new tags and send the event that these spans need updating!
                    if (oldState != state)
                    {
                        var lineNumber = endLineNumber;
                        while (oldState != state && lineNumber < snapShot.LineCount)
                        {
                            lineNumber++;
                            var dummyToken = _parser.Scanner.VsReadToken(ref state);
                            while (dummyToken != null)
                            {
                                dummyToken = _parser.Scanner.VsReadToken(ref state);
                            }

                            _lineStates.TryGetValue(lineNumber + 1, out oldState);
                            _lineStates[lineNumber + 1] = state;
                        }

                        if (lineNumber >= snapShot.LineCount)
                            lineNumber = snapShot.LineCount - 1;

                        var lastLine = snapShot.GetLineFromLineNumber(lineNumber);
                        if (lastLine != null && this.TagsChanged != null)
                        {
                            int length = lastLine.End.Position - endLine.End.Position;
                            var snapShotSpan = new SnapshotSpan(snapShot, endLine.End.Position, length);
                            this.TagsChanged(this, new SnapshotSpanEventArgs(snapShotSpan));
                        }
                    }            
                }
            }
        }

        private ClassificationTag BuildTag(IClassificationTypeRegistryService typeService, string type)
        {
            return new ClassificationTag(typeService.GetClassificationType(type));
        }

        /// <summary>
        /// Initializes the line states based on the snapshot.
        /// </summary>
        private void InitializeLineStates(ITextSnapshot snapShot)
        {
            _lineStates[0] = 0;
            foreach (var line in snapShot.Lines)
            {
                int state = 0;
                _parser.Scanner.VsSetSource(line.GetText(), 0);

                var dummyToken = _parser.Scanner.VsReadToken(ref state);
                while (dummyToken != null)
                {
                    dummyToken = _parser.Scanner.VsReadToken(ref state);
                }

                _lineStates[line.LineNumber + 1] = state;
            }
        }
    }
}
