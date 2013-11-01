using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Irony.Parsing;
using Irony.Samples.Informix4GL;

namespace Irony4GLClassifier
{

    #region Provider definition
    /// <summary>
    /// This class causes a classifier to be added to the set of classifiers. Since 
    /// the content type is set to "text", this classifier applies to all text files
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType(Irony4GLContentTypeDefinition.ContentType)]
    internal class Irony4GLClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return new Irony4GLClassifier(buffer, ClassificationRegistry);
        }
    }
    #endregion //provider def

    #region Classifier
    /// <summary>
    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
    /// </summary>
    internal class Irony4GLClassifier : IClassifier
    {
        //public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        private IClassificationTypeRegistryService classificationRegistryService;
        private ITextBuffer textBuffer;
        private Parser parser;
        private Scanner scanner;

        internal Irony4GLClassifier(ITextBuffer textBuffer, IClassificationTypeRegistryService registry)
        {
            this.textBuffer = textBuffer;
            this.classificationRegistryService = registry;
            parser = new Parser(new Informix4GLGrammar());
        }

        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </summary>
        /// <param name="trackingSpan">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            // TODO: 1) need to scan the provided span
            //       2) from the scan, get all tokens within the span
            //          a) then need to classify the token

            //create a list to hold the results
            List<ClassificationSpan> classifications = new List<ClassificationSpan>();

            //var tree = parser.ScanOnly(span.GetText(), "Source");
            var tree = parser.Parse(span.GetText());
            foreach (var token in tree.Tokens)
            {
                // TODO: need to send in tokens instead of null...obviously
                classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)),
                                                               GetClassificationType(null)));
            }
            return classifications;
        }

        private IClassificationType GetClassificationType(Token token)
        {
            return this.classificationRegistryService.GetClassificationType("Irony4GLClassifier");
        }

#pragma warning disable 67
        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the classification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
    }
    #endregion //Classifier
}
