netz -s -z -so ../bin/Release/DeepSkyDad.AF3.ControlPanel.exe ../bin/Release/CliWrap.dll ../bin/Release/Syroot.KnownFolders.dll
xcopy "../bin/Release/Avrdude" "../bin/Release/DeepSkyDad.AF3.ControlPanel.exe.netz/Avrdude" /e /i /h
cd "../bin/Release/DeepSkyDad.AF3.ControlPanel.exe.netz"
del "AF3 Control Panel.exe"
rename "DeepSkyDad.AF3.ControlPanel.exe" "AF3 Control Panel.exe"
