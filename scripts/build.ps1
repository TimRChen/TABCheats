param(
  [string]$GameExe = "Z:\zombie_game\zombit_army_game\YiWanJiangShiJunTuan v1.0.14\TheyAreBillions.exe"
)
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$refs   = Join-Path $root "refs"
$src    = Join-Path $root "src\TABCheats.cs"
$dist   = Join-Path $root "dist"
New-Item -ItemType Directory -Force -Path $dist | Out-Null

$harmony = Join-Path $refs "0Harmony.dll"
$dx      = Join-Path $refs "DXVision.dll"
if (-not (Test-Path $harmony)) { Write-Host "Missing refs/0Harmony.dll - run scripts/extract-refs.ps1 first"; exit 1 }
if (-not (Test-Path $dx))      { Write-Host "Missing refs/DXVision.dll - run scripts/extract-refs.ps1 first"; exit 1 }

$csc = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path $csc)) { $csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe' }

$out = Join-Path $dist 'TABCheats.dll'
$cargs = @(
  '-nologo','-target:library',
  '-out:' + $out,
  '-r:System.dll','-r:System.Core.dll','-r:System.Drawing.dll',
  '-r:' + $GameExe,
  '-r:' + $harmony,
  '-r:' + $dx,
  $src
)
& $csc $cargs
Write-Host "Built $out"
