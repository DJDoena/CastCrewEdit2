set cce2Path=..\..\..\..\CastCrewEdit2\CastCrewEdit2\bin\%1\%2\CastCrewEdit2
IF NOT EXIST %cce2Path% ( 
  md %cce2Path%
)
xcopy.exe /y CheckForDuplicatesInCastCrewEdit2Cache.* %cce2Path%
set readMePath=%cce2Path%\ReadMe
IF NOT EXIST %readMePath% ( 
  md %readMePath%
)
xcopy.exe /y CheckForDuplicates_*.txt %readMePath% 