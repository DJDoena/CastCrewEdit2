using System;
using System.Xml.Serialization;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version390;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    public class ExtendedCastDivider : Divider
    {
        [XmlAttribute]
        public String ImdbLink;
    }
}