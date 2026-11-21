namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

using DVDProfilerXML.Version400;
using System.Xml.Serialization;

[XmlRoot("CrewInformation")]
public partial class ExtendedCrewInformation : CrewInformation
{
    [XmlAttribute]
    public string ImdbLink;
}