$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\bin\Release\Emitter.Net45\Emitter.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\Emitter.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\Emitter.compiled.nuspec

& $root\Deploy\NuGet.exe pack $root\Emitter.compiled.nuspec
