$Path = "LLC_MOD_Toolbox"
Remove-Item $Path/LimbusCompanyPath.txt

C:\Users\Administrator\.nuget\packages\7z.net\1.0.3\build\7za.exe a -t7z "./LLC_MOD_Toolbox.7z" "LLC_MOD_Toolbox/" -mx=9 -ms