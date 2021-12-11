namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System.Xml.Serialization;
    using DVDProfilerXML.Version400;

    public class ExtendedCastMember : CastMember
    {
        [XmlAttribute]
        public string ImdbLink;
    }
}