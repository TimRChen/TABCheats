param(
  [string]$GameExe = "Z:\zombie_game\zombit_army_game\YiWanJiangShiJunTuan v1.0.14\TheyAreBillions.exe"
)
# This script lives in <repo>/scripts/. refs/ sits next to it; src/ and dist/ live in the repo root.
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition }
$root = Split-Path -Parent $scriptDir
$refs = Join-Path $scriptDir "refs"
$src  = Join-Path $root "src\TABCheats.cs"
$dist = Join-Path $root "dist"
New-Item -ItemType Directory -Force -Path $dist | Out-Null

$harmony = Join-Path $refs "0Harmony.dll"
$dx      = Join-Path $refs "DXVision.dll"
if (-not (Test-Path -LiteralPath $harmony)) { Write-Host "Missing scripts/refs/0Harmony.dll - run scripts/extract-refs.ps1 first"; exit 1 }
if (-not (Test-Path -LiteralPath $dx))      { Write-Host "Missing scripts/refs/DXVision.dll - run scripts/extract-refs.ps1 first"; exit 1 }
if (-not (Test-Path -LiteralPath $src))     { Write-Host "Missing src/TABCheats.cs"; exit 1 }
if (-not (Test-Path -LiteralPath $GameExe)) { Write-Host "Missing game exe: $GameExe"; exit 1 }

$csc = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $csc)) { $csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe' }

$out = Join-Path $dist 'TABCheats.dll'
# The game folder name contains spaces ("YiWanJiangShiJunTuan v1.0.14") and csc re-splits its
# command line on spaces, so the quotes have to be part of each argument string.
$cargs = @(
  '-nologo', '-target:library', '-optimize+',
  ('-out:"' + $out + '"'),
  '-r:System.dll', '-r:System.Core.dll', '-r:System.Drawing.dll',
  ('-r:"' + $GameExe + '"'),
  ('-r:"' + $harmony + '"'),
  ('-r:"' + $dx + '"'),
  ('"' + $src + '"')
)
& $csc $cargs
if ($LASTEXITCODE -ne 0) { Write-Host "BUILD FAILED (exit $LASTEXITCODE)"; exit $LASTEXITCODE }
Write-Host "Built $out ($((Get-Item -LiteralPath $out).Length) bytes)"
