using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Irony4GLClassifier
{
    internal class Informix4GLClassificationDefinitions
    {

        #region Format definition
        /// <summary>
        /// Defines an editor format for the Irony4GLClassifier type that has a purple background
        /// and is underlined.
        /// </summary>
        [Name("Irony4GLClassifier"), Export]
        internal ClassificationTypeDefinition Informix4GLClassificationType { get; set; } 

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Irony4GLClassifier")]
        [Name("Irony4GLClassifierFormatDefinition")]
        [UserVisible(true)] //this should be visible to the end user
        [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
        internal sealed class Irony4GLClassifierFormat : ClassificationFormatDefinition
        {
            /// <summary>
            /// Defines the visual format for the "Irony4GLClassifier" classification type
            /// </summary>
            public Irony4GLClassifierFormat()
            {
                this.DisplayName = "Irony4GLClassifier"; //human readable version of the name
                this.BackgroundColor = Colors.BlueViolet;
                this.TextDecorations = System.Windows.TextDecorations.Underline;
            }
        }
        #endregion //Format definition
    }
}
