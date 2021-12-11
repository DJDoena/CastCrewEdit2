namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System.Xml.Serialization;
    using DVDProfilerXML.Version400;

    public class ExtendedCrewMember : CrewMember
    {
        [XmlAttribute]
        public string ImdbLink;
    }
}