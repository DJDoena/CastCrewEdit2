using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    internal sealed class ExtendedSerializer<T>
    {
        private readonly XmlSerializer XmlSerializer;

        private readonly Encoding Encoding;

        public ExtendedSerializer(Type[] additionalTypes
            , Encoding encoding)
        {
            XmlSerializer = new XmlSerializer(typeof(T), additionalTypes);

            Encoding = encoding;
        }

        public String ToString(T instance)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(ms, instance);

                String text = Encoding.GetString(ms.ToArray());

                return (text);
            }
        }

        private void Serialize(Stream stream
             , T instance)
        {
            using (XmlTextWriter xtw = new NoTypeAttributeXmlWriter(stream, Encoding))
            {
                xtw.Formatting = Formatting.Indented;

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                ns.Add(String.Empty, String.Empty);

                XmlSerializer.Serialize(xtw, instance, ns);
            }
        }
    }
}