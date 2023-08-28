echo ##################################################
cd
echo %1
echo %2
echo ##################################################

set cce2Path=..\CastCrewEdit2\CastCrewEdit2\bin\%1\%2\net472
IF NOT EXIST %cce2Path% ( 
  md %cce2Path%
)
xcopy.exe /y bin\%1\%2\net472\CompareProfilerXMLAndCastCrewEdit2Cache.* %cce2Path%