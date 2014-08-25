!include WinMessages.nsh
;--------------------------------
; Basic
Name "DbEntry(Leafing Framwork)"
OutFile "Leafing.Setup.exe"

InstallDir $PROGRAMFILES\LeafingStudio\DbEntry4
InstallDirRegKey HKLM "Software\DbEntry4" "Install_Dir"
LicenseText "DbEntry under license Leafing Library Public License 1.0 and uses mono.cecil which is under Mono.Cecil License."
LicenseData "src\License.txt"
; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------
; Pages
Page license
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
; The stuff to install
Section "DbEntry4 (required)"
  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  File "src\License.txt"
  File "src\Resources\cecil_LICENSE.txt"
  File "bin\DbEntryClassLibrary.vsix"
  File "bin\Leafing.CodeGen.exe"
  File "bin\Leafing.CodeGen.exe.config"
  File "bin\Leafing.Core.dll"
  File "bin\Leafing.Data.dll"
  File "bin\Leafing.Data.Oracle8.dll"
  File "bin\Leafing.Extra.dll"
  File "bin\Leafing.MSBuild.dll"
  File "bin\Leafing.Processor.exe"
  File "bin\Leafing.Processor.exe.config"
  File "bin\Leafing.Web.dll"
  File "bin\Mono.Cecil.dll"
  File "bin\Mono.Cecil.Mdb.dll"
  File "bin\Mono.Cecil.Pdb.dll"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\DbEntry4 "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "SYSTEM\CurrentControlSet\Control\Session Manager\Environment" "DbEntryPath" "$INSTDIR"
  SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment"
  ExecShell "open" "$INSTDIR\DbEntryClassLibrary.vsix"

  ; write uninstall strings
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DbEntry4" "DisplayName" "DbEntry4 (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DbEntry4" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteUninstaller "uninstall.exe"
SectionEnd

;--------------------------------
; Uninstaller
Section "Uninstall"
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DbEntry4"
  DeleteRegKey HKLM SOFTWARE\DbEntry4
  DeleteRegValue HKLM "SYSTEM\CurrentControlSet\Control\Session Manager\Environment" "DbEntryPath"
  SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment"

  ; Remove files and uninstaller
  Delete $INSTDIR\License.txt
  Delete $INSTDIR\cecil_LICENSE.txt
  Delete $INSTDIR\DbEntryClassLibrary.vsix
  Delete $INSTDIR\Leafing.CodeGen.exe
  Delete $INSTDIR\Leafing.CodeGen.exe.config
  Delete $INSTDIR\Leafing.Core.dll
  Delete $INSTDIR\Leafing.Data.dll
  Delete $INSTDIR\Leafing.Data.Oracle8.dll
  Delete $INSTDIR\Leafing.Extra.dll
  Delete $INSTDIR\Leafing.MSBuild.dll
  Delete $INSTDIR\Leafing.MSBuild.InstallState
  Delete $INSTDIR\Leafing.Processor.exe
  Delete $INSTDIR\Leafing.Processor.exe.config
  Delete $INSTDIR\Leafing.Web.dll
  Delete $INSTDIR\Mono.Cecil.dll
  Delete $INSTDIR\Mono.Cecil.Mdb.dll
  Delete $INSTDIR\Mono.Cecil.Pdb.dll
  Delete $INSTDIR\uninstall.exe

  RMDir "$INSTDIR"
SectionEnd
