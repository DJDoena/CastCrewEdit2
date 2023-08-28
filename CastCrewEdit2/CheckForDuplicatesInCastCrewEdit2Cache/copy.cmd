echo ##################################################
cd
echo %1
echo %2
echo ##################################################

set cce2Path=..\CastCrewEdit2\CastCrewEdit2\bin\%1\%2\net472
IF NOT EXIST %cce2Path% ( 
  md %cce2Path%
)
xcopy.exe /y bin\%1\%2\net472\CheckForDuplicatesInCastCrewEdit2Cache.* %cce2Path%
set readMePath=%cce2Path%\ReadMe
IF NOT EXIST %readMePath% ( 
  md %readMePath%
)
xcopy.exe /y bin\%1\%2\net472\CheckForDuplicates_*.txt %readMePath% 

:end