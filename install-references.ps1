$SilksongPath = "C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight Silksong"

Set-Location $PSScriptRoot
New-Item -ItemType Directory -Path foreign-references -Force

Copy-Item -Force "$SilksongPath\Hollow Knight Silksong_Data\Managed\Assembly-CSharp.dll" foreign-references
Copy-Item -Force "$SilksongPath\Hollow Knight Silksong_Data\Managed\Assembly-CSharp-firstpass.dll" foreign-references
Copy-Item -Force "$SilksongPath\Hollow Knight Silksong_Data\Managed\UnityEngine.dll" foreign-references
Copy-Item -Force "$SilksongPath\Hollow Knight Silksong_Data\Managed\UnityEngine.UI.dll" foreign-references
Copy-Item -Force "$SilksongPath\Hollow Knight Silksong_Data\Managed\UnityEngine.InputLegacyModule.dll" foreign-references
Copy-Item -Force "$SilksongPath\Hollow Knight Silksong_Data\Managed\UnityEngine.CoreModule.dll" foreign-references
Copy-Item -Force "$SilksongPath\BepInEx\core\BepInEx.dll" foreign-references
Copy-Item -Force "$SilksongPath\BepInEx\core\0Harmony.dll" foreign-references
