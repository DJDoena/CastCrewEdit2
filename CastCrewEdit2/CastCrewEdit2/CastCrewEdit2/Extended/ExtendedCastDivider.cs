namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System.Xml.Serialization;
    using DVDProfilerXML.Version400;

    public class ExtendedCastDivider : Divider
    {
        [XmlAttribute]
        public string ImdbLink;
    }
}