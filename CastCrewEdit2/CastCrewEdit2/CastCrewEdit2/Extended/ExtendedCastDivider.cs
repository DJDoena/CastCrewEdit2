﻿using System;
using System.Xml.Serialization;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version400;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    public class ExtendedCastDivider : Divider
    {
        [XmlAttribute]
        public String ImdbLink;
    }
}