@echo off
set flashersourcepath=../MicroFlasher.Wpf/Bin/Debug
set flashertargetpath=release/flasher

rd release /S /Q
md release

cd release
md flasher
cd ..

xcopy /y "%flashersourcepath%\flasher.exe" "%flashertargetpath%\*"
xcopy /y "%flashersourcepath%\flasher.exe.config" "%flashertargetpath%\*"
xcopy /y "%flashersourcepath%\atmega.dll" "%flashertargetpath%\*"
xcopy /y "%flashersourcepath%\MicroFlasher.dll" "%flashertargetpath%\*"
xcopy /y "%flashersourcepath%\devices.xml" "%flashertargetpath%\*"

pause