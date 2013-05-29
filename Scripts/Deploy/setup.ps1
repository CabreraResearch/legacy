$scriptDir = $(Split-Path -parent $MyInvocation.MyCommand.Definition)

function Install-NeededFor {
param(
   [string] $packageName = ''
  ,[bool] $defaultAnswer = $true
)
  if ($packageName -eq '') {return $false}
  
  $yes = '6'
  $no = '7'
  $msgBoxTimeout='-1'
  $defaultAnswerDisplay = 'Yes'
  $buttonType = 0x4;
  if (!$defaultAnswer) { $defaultAnswerDisplay = 'No'; $buttonType= 0x104;}
  
  $answer = $msgBoxTimeout
  try {
    $timeout = 30
    $question = "Do you need to install $($packageName)? Defaults to `'$defaultAnswerDisplay`' after $timeout seconds"
    $msgBox = New-Object -ComObject WScript.Shell
    $answer = $msgBox.Popup($question, $timeout, "Install $packageName", $buttonType)
  }
  catch {
  }
  
  if ($answer -eq $yes -or ($answer -eq $msgBoxTimeout -and $defaultAnswer -eq $true)) {
    write-host "Installing $packageName"
    return $true
  }
  
  write-host "Not installing $packageName"
  return $false
}

function Install-Service {
param(
   [string] $serviceName = '',
   [string] $exe = ''
)
  if ($serviceName -eq '') {return $false}
  
  try {
    ## delete existing service
    # have to use WMI for much of this, native cmdlets are incomplete
    $service = Get-WmiObject -Class Win32_Service -Filter "Name = '$serviceName'"
    if ($service -ne $null) 
    { 
        $service | stop-service
        $service.Delete() | out-null 
    }

    ## run installutil
    # 'frameworkdir' env var apparently isn't present on Win2003...
    $installUtil = join-path $env:SystemRoot Microsoft.NET\Framework64\v4.0.30319\installutil.exe
    $projectName = 'Services'
    $srcDir = Join-Path $scriptDir "$($projectName)"
    $serviceExe = join-path $srcDir $exe
    $installUtilLog = join-path $scriptDir InstallUtil.log
    & $installUtil $serviceExe /logfile="$installUtilLog" | write-verbose

    $service = Get-WmiObject -Class Win32_Service -Filter "Name = '$serviceName'"
        
    # activate
    $service | set-service -startuptype Automatic -passthru # | start-service
    write-verbose "Successfully started service $($service.name)"
  }
  catch {
    write-host "Not installing " + $serviceName
  }
    
  return $true
}

#install chocolatey
Write-Host "Start: Setting up package manager."

iex ((new-object net.webclient).DownloadString("http://chocolatey.org/install.ps1")) 
cinstm PowerShell
cinstm PSWindowsUpdate

Write-Host "End: Setting up package manager."
Write-Host "-----------"

Write-Host "Start: Checking for/installing required frameworks"
if (Install-NeededFor '.NET Runtimes from 2.0 to 4.5 (These are required for web services dependent on .NET)' $true) {
    cinstm webpicommandline
    cinstm vcredist2008
    cinstm vcredist2010
    cinstm vcredist2005
    cinst netframework2  -source webpi
    cinstm Dogtail.DotNet3.5SP1
    cinstm DotNet4.0
    cinst NETFramework4Update402 -source webpi
    cinst NETFramework4Update402_KB2544514_Only -source webpi
    cinstm DotNet4.5
    cinst WindowsInstaller31 -source webpi
    cinst WindowsInstaller45 -source webpi
}

Write-Host "End: Checking for/installing required frameworks"
Write-Host "-----------"
Write-Host "Start: Checking for/installing IIS Items..."

if (Install-NeededFor 'IIS: Includes ASP.NET and IIS Modules' $true) {
    cwindowsfeatures IIS-WebServerRole
    cinstm MSAccess2010-redist
    cinst aspnetmvc -source webpi
    cinstm aspnetmvc.install
    #cinst MVC3Loc -source webpi
    #cinst MVCLoc -source webpi
    #cinst MVC4Vs2010_Loc -source webpi
    cinst ASPNET  -source webpi
    cinst ASPNET_REGIIS  -source webpi
    cinst DefaultDocument -source webpi
    cinst DynamicContentCompression -source webpi
    cinst HTTPRedirection -source webpi
    cinst IIS7_ExtensionLessURLs -source webpi
    cinst IISManagementConsole -source webpi
    cinst ISAPIExtensions -source webpi
    cinst ISAPIFilters -source webpi
    cinst NETExtensibility -source webpi
    cinst RequestFiltering -source webpi
    cinst StaticContent -source webpi
    cinst StaticContentCompression -source webpi
    cinst UrlRewrite2 -source webpi
    cinst WindowsAuthentication -source webpi
}

Write-Host "End: Checking for/installing IIS Items..."
Write-Host "-----------"
Write-Host "Start: Attempting IIS Auto-Configure"

if (Install-NeededFor 'Auto-Configure IIS App and AppPool for Nbt' $true) {
    $projectName = 'WebApp'
    $srcDir = Join-Path $scriptDir "$($projectName)"
    $networkSvc = 'NT AUTHORITY\NETWORK SERVICE'
    Write-Host "Setting folder permissions on `'$srcDir`' to 'Read' for user $networkSvc"
    $acl = Get-Acl $srcDir
    $acl.SetAccessRuleProtection($True, $True)
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule("$networkSvc","Read", "ContainerInherit, ObjectInherit", "None", "Allow");
    $acl.AddAccessRule($rule);
    Set-Acl $srcDir $acl

    Import-Module WebAdministration
    $appPoolPath = "IIS:\AppPools\$projectName"
    #$pool = new-object
    Write-Warning "You can safely ignore the next error if it occurs related to getting an app pool that doesn't exist"
    $pool = Get-Item $appPoolPath
    if ($pool -eq $null) {
      Write-Host "Creating the app pool `'$appPoolPath`'"
      $pool = New-Item $appPoolPath 
    }
    
    $pool.processModel.identityType = "NetworkService" 
    $pool | Set-Item
    Set-itemproperty $appPoolPath -Name "managedRuntimeVersion" -Value "v4.0"
    #Set-itemproperty $appPoolPath -Name "managedPipelineMode" -Value "Integrated"
    Start-WebAppPool "$projectName"
    Write-Host "Creating the site `'$projectName`' with appPool `'$projectName`'"
    New-WebApplication "$projectName" -Site "Default Web Site" -PhysicalPath $srcDir -ApplicationPool "$projectName" -Force
    
}
Write-Host "End: Attempting IIS Auto-Configure"
Write-Host "-----------"
Write-Host "Start: Install ChemSW Services"

if (Install-NeededFor 'Install ChemSW Services' $true) {
    $env:Path += ";%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319"
    Install-Service "ChemSW Log Service"  "cswlogservice.exe"
    Install-Service "ChemSW Schedule Service"  "nbtschedservice.exe"
}

Write-Host "End: Install ChemSW Services"
Write-Host "-----------"
Write-Host "Start: Update all packages"

if (Install-NeededFor 'Update all packages to latest version' $true) {
  cup
  Get-WUInstall -MicrosoftUpdate -IgnoreUserInput -WhatIf
}

Write-Host "End: Update all packages"
Write-Host "-----------"
Write-Host "Start: Checking for Oracle"

if (Install-NeededFor 'Do you need to install the Oracle Client' $true) {
  Write-Host "Install the 64-bit Oracle Database 11g Release 2 Client (11.2.0.1.0) for Microsoft Windows (x64) from http://www.oracle.com/technetwork/database/enterprise-edition/downloads/112010-win64soft-094461.html" 
  Start "http://www.oracle.com/technetwork/database/enterprise-edition/downloads/112010-win64soft-094461.html" 
}

Write-Host "End: Checking for Oracle"
Write-Host "-----------"
Write-Host "Start: Checking for Crystal Runtime"

if (Install-NeededFor 'Do you need to install the Crystal Report Runtime' $true) {
  Write-Host "Install the 64-bit Crystal Report Runtime MSI from http://scn.sap.com/docs/DOC-7824" 
  Start "http://scn.sap.com/docs/DOC-7824"
}

Write-Host "End: Checking for Crystal Runtime"
Write-Host "-----------"
Write-Host "-----------"
Write-Host "Setup Script Completed"

