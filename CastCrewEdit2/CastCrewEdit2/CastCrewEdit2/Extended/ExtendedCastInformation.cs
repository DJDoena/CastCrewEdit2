namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System.Xml.Serialization;
    using DVDProfilerXML.Version400;

    [XmlRoot("CastInformation")]
    public partial class ExtendedCastInformation : CastInformation
    {
        [XmlAttribute]
        public string ImdbLink;
    }
}