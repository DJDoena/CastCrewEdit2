using System;
using System.IO;
using System.Text;
using System.Xml;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    internal sealed class NoTypeAttributeXmlWriter : XmlTextWriter
    {
        private Boolean SkipAttribute { get; set; }

        public NoTypeAttributeXmlWriter(Stream w, Encoding encoding)
            : base(w, encoding)
        { }

        public override void WriteStartAttribute(String prefix
            , String localName
            , String ns)
        {
            if ((ns == "http://www.w3.org/2001/XMLSchema-instance") && (localName == "type"))
            {
                SkipAttribute = true;
            }
            else
            {
                base.WriteStartAttribute(prefix, localName, ns);
            }
        }

        public override void WriteString(String text)
        {
            if (SkipAttribute == false)
            {
                base.WriteString(text);
            }
        }

        public override void WriteEndAttribute()
        {
            if (SkipAttribute == false)
            {
                base.WriteEndAttribute();
            }

            SkipAttribute = false;
        }
    }
}