namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

using DVDProfilerXML.Version400;
using System.Xml.Serialization;

[XmlRoot("CastInformation")]
public partial class ExtendedCastInformation : CastInformation
{
    [XmlAttribute]
    public string ImdbLink;
}