using System;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace DoenaSoft.DVDProfiler.EditIMDbToDVDProfilerCrewRoleTransformation
{
    [Serializable()]
    public class Settings
    {
        public SizableForm MainForm;

        public String CurrentVersion;
    }   
}