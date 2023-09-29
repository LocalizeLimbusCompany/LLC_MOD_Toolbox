$Path = "LLC_MOD_Toolbox"
$folderPath = $env:USERPROFILE
Remove-Item $Path/LimbusCompanyPath.txt
Remove-Item $Path/Logs

$folderPath/.nuget/packages/7z.net/1.0.3/build/7za.exe a -t7z "./LLC_MOD_Toolbox.7z" "LLC_MOD_Toolbox/" -mx=9 -ms
$folderPath/.nuget/packages/7z.net/1.0.3/build/7za.exe a -t7z "./LocalizeLimbusModInstaller.7z" "LLC_MOD_Toolbox/" -mx=9 -ms

Write-Host "Press Enter to exit..."
$null = Read-Host