using System;
using System.Collections.Generic;
using System.Drawing;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew
{
    internal interface ILog
    {
        Int32 Length { get; }

        void AppendParagraph(String text
            , Color color);

        void Clear();

        String CreateMultiplePersonOutput(IEnumerable<PersonInfo> persons);

        String CreatePersonOutput(PersonInfo person);

        String CreatePersonLinkHtml(String personLink);

        void FromString(String log);

        void Save(String fileName);
    }
}