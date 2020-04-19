set cce2Path=..\..\..\..\CastCrewEdit2\CastCrewEdit2\bin\%1\%2\CastCrewEdit2
IF NOT EXIST %cce2Path% ( 
  md %cce2Path%
)
xcopy.exe /y CompareProfilerXMLAndCastCrewEdit2Cache.* %cce2Path%