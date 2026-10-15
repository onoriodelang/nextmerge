$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$backup = Join-Path $root '.reskin-backup'
$utf8 = New-Object System.Text.UTF8Encoding($false)
$changed = New-Object System.Collections.Generic.List[string]
function Save-Asset([string]$path, [string]$content) {
    $relative = $path.Substring($root.Length + 1)
    $copy = Join-Path $backup $relative
    if (!(Test-Path -LiteralPath $copy)) {
        New-Item -ItemType Directory -Force -Path (Split-Path $copy) | Out-Null
        Copy-Item -LiteralPath $path -Destination $copy
    }
    [IO.File]::WriteAllText($path, $content, $utf8)
    $changed.Add($relative)
}
function Color([string]$hex) {
    $v = @(); for ($i=0; $i -lt 6; $i+=2) { $v += ([Convert]::ToInt32($hex.Substring($i,2),16)/255).ToString('0.######',[Globalization.CultureInfo]::InvariantCulture) }
    return "{r: $($v[0]), g: $($v[1]), b: $($v[2]), a: 1}"
}
$tiers = @('21D4FD','43F0BA','8085FF','DC5CFF','FFB648','FF427D')
$materials = Get-ChildItem (Join-Path $root 'Assets/Content') -Recurse -Filter *.mat
foreach ($mat in $materials) {
    $name = $mat.BaseName
    $base = $null; $energy = '21D4FD'; $grid = 0
    if ($name -match '^(floor|table|merge)_mat') { $base='14243D'; $grid=1 }
    elseif ($name -match '^(walls|fon|pillar|column|trash|lock)|^enviroment') { $base='202D49'; $energy='727CFF' }
    elseif ($name -match '^(axe|kunai|shuriken|cyrcle|saw)') {
        $tier = 0
        if ($name -match '(?:mat_|spec)([1-6])') { $tier=[int]$Matches[1]-1 }
        $energy=$tiers[$tier]; $base=$energy
    }
    elseif ($name -match '^hand_clothes') { $base='101F36' }
    elseif ($name -match '^char_1_(boss|pistol|rocket).*mat_1$') { $base='5D285A'; $energy='FF427D' }
    elseif ($name -match '^char_1_gg_mat_1$') { $base='126F85' }
    if (!$base) { continue }
    $text = [IO.File]::ReadAllText($mat.FullName)
    $text = [regex]::Replace($text,'m_Shader: \{[^\r\n]+\}', 'm_Shader: {fileID: 4800000, guid: b42574e75e6d46d8a8d46cc875f48af1, type: 3}')
    $text = [regex]::Replace($text,'m_ShaderKeywords:[^\r\n]*','m_ShaderKeywords: ')
    $text = [regex]::Replace($text,'(?m)^    - _Color:.*$', ('    - _Color: '+(Color $base)))
    if ($text -notmatch '    - _NeonColor:') { $text=$text.Replace('    m_Colors:', "    m_Colors:`n    - _NeonColor: $(Color $energy)") }
    if ($text -notmatch '    - _Grid:') { $text=$text.Replace('    m_Floats:', "    m_Floats:`n    - _Grid: $grid`n    - _GridScale: 0.65`n    - _RimPower: 3") }
    Save-Asset $mat.FullName $text
}
$tileColors = @{DefaultView='153B56'; LockedView='273047'; OnHoverView='8085FF'; OnMergeAvailableView='43F0BA'; OnUnitPickedView='21D4FD'}
Get-ChildItem (Join-Path $root 'Assets/Modules/MergeMechanic/Materials') -Filter *.mat | ForEach-Object {
    if ($tileColors.ContainsKey($_.BaseName)) {
        $text=[IO.File]::ReadAllText($_.FullName)
        $text=[regex]::Replace($text,'(?m)^    - _Color:.*$', ('    - _Color: '+(Color $tileColors[$_.BaseName])))
        Save-Asset $_.FullName $text
    }
}
foreach ($scene in @('ProdGame','ProdLoad')) {
    $path=Join-Path $root "Assets/Scenes/$scene.unity"
    $text=[IO.File]::ReadAllText($path)
    foreach ($key in @('m_FogColor','m_BackGroundColor')) { $text=[regex]::Replace($text,"${key}: \{[^\r\n]+\}","${key}: $(Color '090F24')") }
    $text=[regex]::Replace($text,'m_SkyboxMaterial: \{[^\r\n]+\}','m_SkyboxMaterial: {fileID: 0}')
    $text=[regex]::Replace($text,'m_ClearFlags: 1','m_ClearFlags: 2')
    Save-Asset $path $text
}
$copyText = @{'TAP TO START'='TAP TO JACK IN';'LOOSE'='SIGNAL LOST';'WIN'='MISSION CLEAR';'GAMEPLAY'='NEON OPS'}
Get-ChildItem (Join-Path $root 'Assets/Prefabs/UI') -Recurse -Filter *.prefab | ForEach-Object {
    $text=[IO.File]::ReadAllText($_.FullName); $original=$text
    foreach ($key in $copyText.Keys) { $text=$text.Replace("m_text: $key`r", "m_text: $($copyText[$key])`r"); $text=$text.Replace("m_text: $key`n", "m_text: $($copyText[$key])`n") }
    # Tint colored UI graphics; preserve white atlas sprites and transparent animation endpoints.
    $text=[regex]::Replace($text,'m_Color: \{r: ([\d.]+), g: ([\d.]+), b: ([\d.]+), a: 1\}', {
        param($m)
        $r=[double]::Parse($m.Groups[1].Value,[Globalization.CultureInfo]::InvariantCulture)
        $g=[double]::Parse($m.Groups[2].Value,[Globalization.CultureInfo]::InvariantCulture)
        $b=[double]::Parse($m.Groups[3].Value,[Globalization.CultureInfo]::InvariantCulture)
        if ([Math]::Max($r,[Math]::Max($g,$b))-[Math]::Min($r,[Math]::Min($g,$b)) -lt 0.15) { return $m.Value }
        if ($r -gt $g -and $r -gt $b) { return "m_Color: $(Color 'FF427D')" }
        return "m_Color: $(Color '21D4FD')"
    })
    if ($text -ne $original) { Save-Asset $_.FullName $text }
}
$changed | ConvertTo-Json | Set-Content (Join-Path $root 'NeonReskin-files.json')
Write-Output "Updated $($changed.Count) assets. Originals preserved in .reskin-backup."
