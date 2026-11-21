namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

using DVDProfilerXML.Version400;
using System.Xml.Serialization;

public class ExtendedCrewMember : CrewMember
{
    [XmlAttribute]
    public string ImdbLink;
}