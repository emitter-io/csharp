Write-Host "Installing .NET MicroFramework 4.3 ..."
$msiPath = "$($env:USERPROFILE)\MicroFrameworkSDK43.MSI"
(New-Object Net.WebClient).DownloadFile('https://s3.amazonaws.com/cdn.misakai.com/www-lib/MicroFramework4.3.msi', $msiPath)
cmd /c start /wait msiexec /i $msiPath /quiet
Write-Host "Installed" -ForegroundColor green

Write-Host "Installing .NET MicroFramework 4.4 ..."
$msiPath = "$($env:USERPROFILE)\MicroFrameworkSDK44.MSI"
(New-Object Net.WebClient).DownloadFile('https://s3.amazonaws.com/cdn.misakai.com/www-lib/MicroFramework4.4.msi', $msiPath)
cmd /c start /wait msiexec /i $msiPath /quiet
Write-Host "Installed" -ForegroundColor green