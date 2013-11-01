using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Irony4GLClassifier
{
    public sealed class Irony4GLContentTypeDefinition
    {
        public const string ContentType = "Informix 4GL";

        /// <summary>
        /// Exports the Informix4GL content type
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(Irony4GLContentTypeDefinition.ContentType)]
        [BaseDefinition("code")]
        public ContentTypeDefinition Informix4GLContentType { get; set; }

        /// <summary>
        /// Exports the Informix4GL file extension
        /// </summary>
        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(Irony4GLContentTypeDefinition.ContentType)]
        [FileExtension(".4gl")]
        public FileExtensionToContentTypeDefinition Informix4GLExtension { get; set; }
    }
}
