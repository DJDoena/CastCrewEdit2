namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    internal sealed class ExtendedSerializer<T>
    {
        private readonly XmlSerializer _serializer;

        private readonly Encoding _encoding;

        public ExtendedSerializer(Type[] additionalTypes, Encoding encoding)
        {
            _serializer = new XmlSerializer(typeof(T), additionalTypes);

            _encoding = encoding;
        }

        public string ToString(T instance)
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, instance);

                var text = _encoding.GetString(ms.ToArray());

                return text;
            }
        }

        private void Serialize(Stream stream, T instance)
        {
            using (var xtw = new NoTypeAttributeXmlWriter(stream, _encoding))
            {
                xtw.Formatting = Formatting.Indented;

                var ns = new XmlSerializerNamespaces();

                ns.Add(string.Empty, string.Empty);

                _serializer.Serialize(xtw, instance, ns);
            }
        }
    }
}