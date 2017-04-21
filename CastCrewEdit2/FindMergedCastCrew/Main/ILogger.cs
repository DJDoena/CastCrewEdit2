using System;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

internal interface ILogger
{
    void LogChange(PersonInfo oldPerson
        , PersonInfo newPerson);

    void LogChange(PersonInfo oldPerson
        , String newLink);

    void LogException(Exception exception, PersonInfo person);
}