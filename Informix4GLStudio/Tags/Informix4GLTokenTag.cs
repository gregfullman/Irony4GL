using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;

namespace Informix4GLLanguage
{
    public class Informix4GLTokenTag : ITag
    {
        public Informix4GLTokenTypes type { get; private set; }
        public string Text { get; private set; }

        public Informix4GLTokenTag(Informix4GLTokenTypes type, string text)
        {
            this.type = type;
            this.Text = text;
        }

        public Informix4GLTokenTag(Informix4GLTokenTypes type)
        {
            this.type = type;
            this.Text = string.Empty;
        }
    }
}
