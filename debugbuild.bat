msbuild /property:Configuration=Debug src\DbEntryBase.sln

copy src\Resources\nunit.framework.dll src\Leafing.Processor\bin\Debug\
copy src\Resources\Mono.Cecil.dll src\Leafing.Processor\bin\Debug\
copy src\Resources\Mono.Cecil.Pdb.dll src\Leafing.Processor\bin\Debug\
copy src\Resources\Mono.Cecil.Mdb.dll src\Leafing.Processor\bin\Debug\

msbuild /property:Configuration=Debug src\DbEntry.Net.sln
