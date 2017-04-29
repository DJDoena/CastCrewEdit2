using System;
using System.Xml.Serialization;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version390;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    [XmlRoot("CastInformation")]
    public partial class ExtendedCastInformation : CastInformation
    {
        [XmlAttribute]
        public String ImdbLink;
    }
}