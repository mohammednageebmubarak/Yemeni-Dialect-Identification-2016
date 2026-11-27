param([Parameter(Mandatory=$true)][string]$WekaJar,[string]$DependencyDirectory='')
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$jar=(Resolve-Path $WekaJar).Path
$out=Join-Path $root 'WEKA\build'; New-Item -ItemType Directory -Force $out | Out-Null
$cp=$jar
if($DependencyDirectory){ $dep=(Resolve-Path $DependencyDirectory).Path; $cp="$cp;$dep\*" }
& javac -encoding UTF-8 -cp $cp -d $out (Join-Path $root 'WEKA\DialectEngine.java')
if($LASTEXITCODE -ne 0){throw 'WEKA Java compilation failed'}
$env:DID_WEKA_CP="$out;$cp"
[Environment]::SetEnvironmentVariable('DID_WEKA_CP',$env:DID_WEKA_CP,'User')
Write-Host 'DID_WEKA_CP configured. Restart Visual Studio if it is open.'
