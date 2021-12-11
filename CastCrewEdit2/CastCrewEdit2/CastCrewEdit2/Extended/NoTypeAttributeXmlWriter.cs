using System.IO;
using System.Text;
using System.Xml;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    internal sealed class NoTypeAttributeXmlWriter : XmlTextWriter
    {
        private bool SkipAttribute { get; set; }

        public NoTypeAttributeXmlWriter(Stream w, Encoding encoding) : base(w, encoding)
        { }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            if ((ns == "http://www.w3.org/2001/XMLSchema-instance") && (localName == "type"))
            {
                this.SkipAttribute = true;
            }
            else
            {
                base.WriteStartAttribute(prefix, localName, ns);
            }
        }

        public override void WriteString(string text)
        {
            if (!this.SkipAttribute)
            {
                base.WriteString(text);
            }
        }

        public override void WriteEndAttribute()
        {
            if (!this.SkipAttribute)
            {
                base.WriteEndAttribute();
            }

            this.SkipAttribute = false;
        }
    }
}