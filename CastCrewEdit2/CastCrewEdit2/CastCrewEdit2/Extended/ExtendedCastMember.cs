namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

using DVDProfilerXML.Version400;
using System.Xml.Serialization;

public class ExtendedCastMember : CastMember
{
    [XmlAttribute]
    public string ImdbLink;
}