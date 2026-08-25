param(
  [string]$GameExe = "Z:\zombie_game\zombit_army_game\YiWanJiangShiJunTuan v1.0.14\TheyAreBillions.exe"
)
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$refs = Join-Path $root "refs"
New-Item -ItemType Directory -Force -Path $refs | Out-Null

$asm = [System.Reflection.Assembly]::LoadFrom($GameExe)
$names = @('costura.0harmony.dll.compressed','costura.dxvision.dll.compressed')
$outs  = @('0Harmony.dll','DXVision.dll')

for ($i = 0; $i -lt $names.Count; $i++) {
  $stream = $asm.GetManifestResourceStream($names[$i])
  if ($null -eq $stream) {
    Write-Host "Missing resource: $($names[$i])"
    continue
  }
  $ds = New-Object System.IO.Compression.DeflateStream($stream, [System.IO.Compression.CompressionMode]::Decompress)
  $outPath = Join-Path $refs $outs[$i]
  $file = [System.IO.File]::Create($outPath)
  try {
    $ds.CopyTo($file)
  } finally {
    $file.Close()
    $ds.Close()
    $stream.Close()
  }
  Write-Host "Extracted $outPath"
}
Write-Host "Done."
