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

        private Token GetPrecedingToken(ICompletionSession session)
        {
            Token retToken = null;

            // get the caret position as an absolute character position within the text
            int caretPosition = session.TextView.Caret.Position.BufferPosition.Position;

            // get the token closest to the caret position
            int lastNodePos = m_parseTree.Tokens.FindLastIndex(x => x.Location.Position < caretPosition);
            if (lastNodePos > 0)
            {
                retToken = m_parseTree.Tokens[lastNodePos - 1];
            }
            return retToken;
        }

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            List<string> strList = new List<string>() { " " };
            m_parseTree = Informix4GLFactory.Parse(m_textBuffer.CurrentSnapshot.GetText());

            // Need to make sure we don't do any completion in certain circumstances
            // TODO: need to define more circumstances where this is true, and
            // also handle stuff like defines, where it's not immediately obvious we're in a define
            Token previousToken = GetPrecedingToken(session);
             if (previousToken != null)
            {
                string containingFunction = null;
                if (m_parseTree.Root != null)
                {
                    containingFunction = Informix4GLFactory.GetParentFunctionName(m_parseTree.Root, previousToken.TreeNode);
                }
                else if (Informix4GLFactory.LastGoodParseTree != null && Informix4GLFactory.LastGoodParseTree.Root != null)
                {
                    containingFunction = Informix4GLFactory.GetParentFunctionName(Informix4GLFactory.LastGoodParseTree.Root, previousToken.TreeNode);
                }

                if (containingFunction != null)
                {
                    if (Informix4GLFactory.FunctionVariables.ContainsKey(containingFunction))
                    {
                        strList.AddRange(Informix4GLFactory.FunctionVariables[containingFunction]);
                    }
                }
                if (previousToken.Text != "function")
                {
                    // TODO: need to work on way to get completion text
                    strList.AddRange(Informix4GLFactory.GlobalVariables);
                }
            }
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
