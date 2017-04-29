using System;
using System.Xml.Serialization;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version390;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    [XmlRoot("CrewInformation")]
    public partial class ExtendedCrewInformation : CrewInformation
    {
        [XmlAttribute]
        public String ImdbLink;
    }
}