Set-Location $PSScriptRoot

$zipFile = "Cheat Pack 4.zip"
$buildDll = "Cheat Pack 4/bin/Release/Cheat Pack 4.dll"

$tempDir = Join-Path $env:TEMP ([System.IO.Path]::GetRandomFileName())
if (Test-Path $tempDir) {
    Remove-Item -Recurse -Force $tempDir
}
$targetDir = Join-Path $tempDir "BepInEx/plugins/Cheat Pack 4"
New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
Copy-Item $buildDll $targetDir -Force

if (Test-Path $zipFile) {
    Remove-Item $zipFile -Force
}
Compress-Archive -Path "$tempDir/*" -DestinationPath $zipFile
Remove-Item -Recurse -Force $tempDir
