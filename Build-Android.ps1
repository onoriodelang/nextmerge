param(
    [string]$UnityEditor = 'C:/Program Files/Unity/Hub/Editor/2021.3.18f1/Editor/Unity.exe'
)
$ErrorActionPreference = 'Stop'
if (!(Test-Path -LiteralPath $UnityEditor)) {
    throw "Unity editor not found at $UnityEditor. Install Unity 2021.3.18f1 with Android Build Support, SDK/NDK and OpenJDK, or pass -UnityEditor with its path."
}
$output = Join-Path $PSScriptRoot 'Builds/Android'
New-Item -ItemType Directory -Force -Path $output | Out-Null
$env:NEON_APK_PATH = Join-Path $output 'NeonCyberNinja.apk'
$arguments = @('-batchmode', '-nographics', '-projectPath', ('"' + $PSScriptRoot + '"'), '-buildTarget', 'Android', '-executeMethod', 'NeonAndroidBuild.Build', '-logFile', ('"' + (Join-Path $output 'build.log') + '"'))
$process = Start-Process -FilePath $UnityEditor -ArgumentList $arguments -WindowStyle Hidden -PassThru -Wait
if ($process.ExitCode -ne 0) { throw "Unity exited with code $($process.ExitCode). See Builds/Android/build.log." }
if (!(Test-Path -LiteralPath $env:NEON_APK_PATH)) { throw 'Unity exited without producing the APK.' }
Get-Item -LiteralPath $env:NEON_APK_PATH
