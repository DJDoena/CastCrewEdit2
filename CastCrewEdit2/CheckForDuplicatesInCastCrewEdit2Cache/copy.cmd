xcopy.exe /y CheckForDuplicatesInCastCrewEdit2Cache.* ..\..\..\..\CastCrewEdit2\CastCrewEdit2\bin\%1\CastCrewEdit2
set readMePath=..\..\..\..\CastCrewEdit2\CastCrewEdit2\bin\%1\CastCrewEdit2\ReadMe
IF NOT EXIST %readMePath% ( 
  md %readMePath%
)
xcopy.exe /y CheckForDuplicates_*.txt ..\..\..\..\CastCrewEdit2\CastCrewEdit2\bin\%1\CastCrewEdit2\ReadMe