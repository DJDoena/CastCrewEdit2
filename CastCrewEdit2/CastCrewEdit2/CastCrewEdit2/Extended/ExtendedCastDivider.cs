namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

using DVDProfilerXML.Version400;
using System.Xml.Serialization;

public class ExtendedCastDivider : Divider
{
    [XmlAttribute]
    public string ImdbLink;
}