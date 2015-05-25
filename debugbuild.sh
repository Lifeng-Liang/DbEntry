#!/bin/bash


xbuild /property:Configuration=Debug src/DbEntryBase.sln

if [ $? -eq 0 ]
then

cp src/Resources/nunit.framework.dll src/Leafing.Processor/bin/Debug/
cp src/Resources/Mono.Cecil.dll src/Leafing.Processor/bin/Debug/
cp src/Resources/Mono.Cecil.Pdb.dll src/Leafing.Processor/bin/Debug/
cp src/Resources/Mono.Cecil.Mdb.dll src/Leafing.Processor/bin/Debug/

xbuild /property:Configuration=Debug src/DbEntry.Net.sln

if [ $? -eq 0 ]
then

echo Done
#cp src/Leafing.Web/bin/Debug/Leafing.Web.dll src/Leafing.Processor/bin/Debug/

fi
fi
