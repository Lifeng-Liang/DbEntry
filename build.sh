#!/bin/bash


xbuild /property:Configuration=Release src/DbEntry.Net.sln

if [ $? -eq 0 ]
then

cp src/Leafing.Core/bin/Release/Leafing.Core.dll bin
cp src/Leafing.Data/bin/Release/Leafing.Data.dll bin
cp src/Leafing.Data.Oracle8/bin/Release/Leafing.Data.Oracle8.dll bin

cp src/Leafing.CodeGen/bin/Release/Leafing.CodeGen.exe bin
cp src/Leafing.CodeGen/bin/Release/Leafing.CodeGen.exe.config bin
cp src/Leafing.Extra/bin/Release/Leafing.Extra.dll bin
cp src/Leafing.Membership/bin/Release/Leafing.Membership.dll bin
cp src/Leafing.Membership/bin/Release/Leafing.Membership.dll bin
cp src/Leafing.Web/bin/Release/Leafing.Web.dll bin

fi
