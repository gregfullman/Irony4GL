using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Informix4GLLanguage.Classification
{
    internal static class OrdinaryClassificationDefinition
    {
        #region Type definition

        /// <summary>
        /// Defines the "ordinary" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("keyword")]
        internal static ClassificationTypeDefinition Informix4GLKeyWord = null;

        #endregion
    }
}
