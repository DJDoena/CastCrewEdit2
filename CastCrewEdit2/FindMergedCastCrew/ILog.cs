using System.Collections.Generic;
using System.Drawing;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew
{
    internal interface ILog
    {
        int Length { get; }

        void AppendParagraph(string text
            , Color color);

        void Clear();

        string CreateMultiplePersonOutput(IEnumerable<PersonInfo> persons);

        string CreatePersonOutput(PersonInfo person);

        string CreatePersonLinkHtml(string personLink);

        void FromString(string log);

        void Save(string fileName);
    }
}