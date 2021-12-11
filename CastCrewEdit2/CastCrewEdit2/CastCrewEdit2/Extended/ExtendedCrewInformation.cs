namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System.Xml.Serialization;
    using DVDProfilerXML.Version400;

    [XmlRoot("CrewInformation")]
    public partial class ExtendedCrewInformation : CrewInformation
    {
        [XmlAttribute]
        public string ImdbLink;
    }
}