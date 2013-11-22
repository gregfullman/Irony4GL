using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Irony.Parsing;

namespace Informix4GLLanguage.Intellisense
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("Informix4GL")]
    [Name("Informix4GL completion")]
    internal class Informix4GLCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new Informix4GLCompletionSource(this, textBuffer);
        }

    }

    internal class Informix4GLCompletionSource : ICompletionSource
    {
        private Informix4GLCompletionSourceProvider m_sourceProvider;
        private ITextBuffer m_textBuffer;
        private List<Completion> m_compList;
        private ParseTree m_parseTree;

        public Informix4GLCompletionSource(Informix4GLCompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
        {
            m_sourceProvider = sourceProvider;
            m_textBuffer = textBuffer;
        }

        private void GetLocalScopeVariables(ICompletionSession session)
        {
            // get the caret position as an absolute character position within the text
            int caretPosition = session.TextView.Caret.Position.BufferPosition.Position;

            // get the token closest to the caret position
            Token current = null, backup = null, selected = null;
            foreach (var token in m_parseTree.Tokens)
            {
                current = token;
                if (token.Location.Position < caretPosition)
                    backup = current;
                else
                    break;
            }
            if(current != null && backup != null)
            {
                int backDelta = Math.Abs(caretPosition - backup.Location.Position);
                int currDelta = Math.Abs(caretPosition - current.Location.Position);
                if (backDelta < currDelta)
                    selected = backup;
                else
                    selected = current;

                // we now have the closest token to the caret. Let's determine the scope
                //int i = 0;
                // TODO: much more work to do here...
            }
        }

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            m_parseTree = Informix4GLFactory.Parse(m_textBuffer.CurrentSnapshot.GetText());

            GetLocalScopeVariables(session);

            List<string> strList = new List<string>();
            // TODO: need to work on way to get completion text
            strList.Add("addition");
            strList.Add("adaptation");
            strList.Add("subtraction");
            strList.Add("summation");
            m_compList = new List<Completion>();
            foreach (string str in strList)
                m_compList.Add(new Completion(str, str, str, null, null));

            completionSets.Add(new CompletionSet(
                "Tokens",    //the non-localized title of the tab
                "Tokens",    //the display title of the tab
                FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                    session),
                m_compList,
                null));
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = m_sourceProvider.NavigatorService.GetTextStructureNavigator(m_textBuffer);
            TextExtent extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }
    }
}
