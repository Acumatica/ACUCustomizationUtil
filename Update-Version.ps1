# Update-Version.ps1
$ErrorActionPreference = 'Stop'

$programFile = Join-Path $PSScriptRoot "src/ACUCustomizationUtil/Program.cs"
$docsPath = Join-Path $PSScriptRoot "doc"

# Читаем строку с версией
$programContent = Get-Content -Raw -Path $programFile

# Ищем строго 4-компонентную версию: XX.XX.XX.XXXX
if ($programContent -match '\[assembly:\s*AssemblyVersion\("(\d{2}\.\d{2}\.\d{2}\.\d{4})"\)\]') {
    $version = $matches[1]
    Write-Host "Found version: $version"
} else {
    Write-Error "AssemblyVersion not found or invalid format"
    exit 1
}

# Обновляем docs/VERSION
Set-Content -Path "$docsPath/VERSION" -Value $version
Write-Host "Written to docs/VERSION"

# Обновляем все .md-файлы, кроме VERSION
Get-ChildItem -Path $docsPath -Recurse -Filter *.md | Where-Object {
    $_.Name -ne "VERSION" -and
    $_.Name -notmatch '^Release_Notes_\d{2}\.\d{2}\.\d{2}\.\d{4}\.md$'
} | ForEach-Object {
    $filePath = $_.FullName
    $content = Get-Content -Raw -Path $filePath

    # Заменяем старую версию (предыдущую) на новую
    $updated = $content -replace '\b\d{2}\.\d{2}\.\d{2}\.\d{4}\b', $version

    Set-Content -Path $filePath -Value $updated
    Write-Host "Updated: $filePath"
}
