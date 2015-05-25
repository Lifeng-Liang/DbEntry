
msbuild /property:Configuration=Release src\DbEntryBase.sln

copy src\Leafing.MSBuild\bin\Release\Leafing.MSBuild.dll bin /y
copy src\Leafing.Core\bin\Release\Leafing.Core.dll bin /y
copy src\Leafing.Data\bin\Release\Leafing.Data.dll bin /y
copy src\Leafing.Data.Oracle8\bin\Release\Leafing.Data.Oracle8.dll bin /y

copy src\Leafing.Processor\bin\Release\Leafing.Processor.exe bin /y
copy src\Leafing.Processor\bin\Release\Leafing.Processor.exe.config bin /y
copy src\Resources\Mono.Cecil.dll bin /y
copy src\Resources\Mono.Cecil.Pdb.dll bin /y
copy src\Resources\Mono.Cecil.Mdb.dll bin /y

msbuild /property:Configuration=Release src\DbEntry.Net.sln

copy src\Leafing.CodeGen\bin\Release\Leafing.CodeGen.exe bin /y
copy src\Leafing.CodeGen\bin\Release\Leafing.CodeGen.exe.config bin /y
copy src\Leafing.Extra\bin\Release\Leafing.Extra.dll bin /y
copy src\Leafing.Membership\bin\Release\Leafing.Membership.dll bin /y
copy src\Leafing.Membership\bin\Release\Leafing.Membership.dll bin /y
copy src\Leafing.Web\bin\Release\Leafing.Web.dll bin /y
