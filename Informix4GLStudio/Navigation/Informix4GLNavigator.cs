using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.NavigateTo.Interfaces;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Informix4GLLanguage.Navigation
{
    [Export(typeof(INavigateToItemProvider))]
    [ContentType("Informix4GL")]
    internal sealed class NavigationTaggerProvider : INavigateToItemProvider
    {
        #region INavigateToItemProvider Members

        public void StartSearch(INavigateToCallback callback, string searchValue)
        {
            throw new NotImplementedException();
        }

        public void StopSearch()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

 
}
