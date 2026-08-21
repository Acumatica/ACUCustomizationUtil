$ErrorActionPreference = 'Stop'

$programFile = Join-Path $PSScriptRoot "src/ACUCustomizationUtil/Program.cs"
$docsPath = Join-Path $PSScriptRoot "doc"

# Generate new version from current date/time (yy.MM.dd.HHmm)
$version = Get-Date -Format "yy.MM.dd.HHmm"
Write-Host "New version: $version"

# Bump AssemblyVersion in Program.cs
$programContent = Get-Content -Raw -Path $programFile
if ($programContent -notmatch '\[assembly:\s*AssemblyVersion\("\d{2}\.\d{2}\.\d{2}\.\d{4}"\)\]') {
    Write-Error "AssemblyVersion attribute not found or invalid format"
    exit 1
}
$programContent = $programContent -replace '(\[assembly:\s*AssemblyVersion\(")\d{2}\.\d{2}\.\d{2}\.\d{4}("\)\])', "`${1}$version`${2}"
Set-Content -Path $programFile -Value $programContent -NoNewline
Write-Host "Updated: $programFile"

# Update docs/VERSION
Set-Content -Path "$docsPath/VERSION" -Value $version
Write-Host "Written to docs/VERSION"

# Update README
$readmeFile = Join-Path $PSScriptRoot "README.md"
$readmeContent = Get-Content -Raw -Path $readmeFile
$readmeUpdated = ($readmeContent -replace '\b\d{2}\.\d{2}\.\d{2}\.\d{4}\b', $version).TrimEnd() + [Environment]::NewLine
Set-Content -Path $readmeFile -Value $readmeUpdated -NoNewline
Write-Host "Updated: $readmeFile"

# Update all other Markdown files
Get-ChildItem -Path $docsPath -Recurse -Filter *.md | Where-Object {
    $_.Name -ne "VERSION" -and
    $_.Name -notmatch '^Release_Notes_\d{2}\.\d{2}\.\d{2}\.\d{4}\.md$'
} | ForEach-Object {
    $filePath = $_.FullName
    $content = Get-Content -Raw -Path $filePath

    # Update version in the file's content, normalize to single trailing newline
    $updated = ($content -replace '\b\d{2}\.\d{2}\.\d{2}\.\d{4}\b', $version).TrimEnd() + [Environment]::NewLine

    Set-Content -Path $filePath -Value $updated -NoNewline
    Write-Host "Updated: $filePath"
}
