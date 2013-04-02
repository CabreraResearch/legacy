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

#install chocolatey
if (Install-NeededFor 'Chocolatey: apt-get for Windows [Required for the rest of this script to work]') {
  iex ((new-object net.webclient).DownloadString("http://chocolatey.org/install.ps1")) 
}

Write-Host "Start: Checking for/installing required frameworks"
if (Install-NeededFor '.NET Runtimes from 2.0 to 4.5 (These are required under the hood for any NPM packages dependent on .NET, such as Phantom)' $false) {
    cwebpi netframework2 
    cwebpi NETFramework35
    cwebpi NETFramework4 
    cwebpi NETFramework4Update402
    cwebpi NETFramework4Update402_KB2544514_Only
	cinstm DotNet4.5
    cwebpi WindowsInstaller31 
    cwebpi WindowsInstaller45 
}

if (Install-NeededFor 'C++ Libraries (2005 - 2010) also required for some Node packages' $false) {
    cinstm vcredist2005
    cinstm vcredist2008
    cinstm vcredist2010
}

if (Install-NeededFor 'NodeJS and PhantomJS (Required for compiling the Project' $true) {
  cinstm nodejs.install
  cinstm PhantomJS
}

Write-Host "End: Checking for/installing required frameworks"
Write-Host "-----------"
Write-Host "Start: Checking for/installing NPM packages"

if (Install-NeededFor 'Grunt: Installs the CLI and AutoSave globally' $true) {
  $nodePath = Join-Path $env:programfiles 'nodejs'
   $is64bit = (Get-WmiObject Win32_Processor).AddressWidth -eq 64
  if ($is64bit) {$nodePath = Join-Path ${env:ProgramFiles(x86)} 'nodejs'}
  $env:Path = "$($env:Path);$nodePath"
  npm uninstall -g grunt
  npm install -g grunt-cli
  
  Write-Host 'Details at http://gruntjs.com/getting-started'
  
  npm install -g autosave
  
  Write-Host 'You still need to enable experimental packages in Chrome and install the Chrome Extension'
  Write-Host 'Details at https://github.com/NV/chrome-devtools-autosave#readme'
}

Write-Host "End: Checking for/installing NPM packages"
Write-Host "-----------"
Write-Host "Start: Checking for/installing Core developer tools..."

if (Install-NeededFor 'TortoiseHg: Installs latest Tortoise and VisualHg' $false) {
    cinstm tortoisehg
	cinstm VisualHG
}

if (Install-NeededFor 'PowerShell 3.0 (the thing running this script, plus an IDE and common extensions' $false) {
    cinstm PowerShell
    cinstm PowerGUI
	cinstm pscx
}

if (Install-NeededFor 'Visual Studio 2012 (Premium)' $false) {
 cinstm VisualStudio2012Premium
}

if (Install-NeededFor 'Visual Studio 2010 (Full Edition) SP1' $false) {
 cwebpi VS2010SP1Pack
}

if (Install-NeededFor 'ReSharper (for 2010 and 2012)' $false) {
 cinstm resharper
}

if (Install-NeededFor 'Visual Studio MVC (2010 and 2012)' $false) {
 cinstm aspnetmvc
 cwebpi MVCLoc
 cwebpi MVC4Vs2010_Loc
}

if (Install-NeededFor 'Google Chrome Canary (Best browser for development)' $false) {
 cinst GoogleChrome.Canary
}

if (Install-NeededFor 'Supported Browsers: Chrome, FF, Safari, IE9' $false) {
 cinst GoogleChrome
 cinst Firefox
 cinst safari
 cinst ie9
}

Write-Host "End: Checking for/installing Core developer tools..."
Write-Host "-----------"
Write-Host "Start: Checking for/installing Other language support"

if (Install-NeededFor 'Perl (ActivePerl)' $false) {
 cinstm ActivePerl
}
if (Install-NeededFor 'Python and Easy' $false) {
 cinstm python 
 cinstm easy.install
}
if (Install-NeededFor 'Java Runtime' $false) {
 cinst javaruntime
}

Write-Host "End: checking for/installing Other language support"
Write-Host "-----------"
Write-Host "Start: Checking for/installing IIS Items..."

if (Install-NeededFor 'IIS: Includes ASP.NET and IIS Modules' $false) {
  cwebpi ASPNET 
  cwebpi ASPNET_REGIIS 
  cwebpi DefaultDocument 
  cwebpi DynamicContentCompression 
  cwebpi HTTPRedirection 
  cwebpi IIS7_ExtensionLessURLs 
  cwebpi IISExpress 
  cwebpi IISExpress_7_5 
  cwebpi IISManagementConsole 
  cwebpi ISAPIExtensions 
  cwebpi ISAPIFilters 
  cwebpi NETExtensibility 
  cwebpi RequestFiltering 
  cwebpi StaticContent 
  cwebpi StaticContentCompression 
  cwebpi UrlRewrite2 
  cwebpi WindowsAuthentication 
}

Write-Host "End: Checking for/installing IIS Items..."
Write-Host "-----------"
Write-Host "Start: Checking for/installing Project NPM..."

if (Install-NeededFor 'This Web App Project NPM package' $false) {
	$nodePath = Join-Path $env:programfiles 'nodejs'
   $is64bit = (Get-WmiObject Win32_Processor).AddressWidth -eq 64
  if ($is64bit) {$nodePath = Join-Path ${env:ProgramFiles(x86)} 'nodejs'}
  $env:Path = "$($env:Path);$nodePath" 
 npm install
  
  Write-Host 'This step has worked inconsistenly for some people. If needed, cd into the project folder and execute `npm install`'
  Write-Host 'You still need to open the project folder in a command prompt and execute `grunt.cmd build:{mode}` to build the project.'
}

Write-Host "End: Checking for/installing Project NPM..."
Write-Host "-----------"
Write-Host "Start: Attempting IIS Auto-Configure"

if (Install-NeededFor 'Auto-Configure IIS App and AppPool for Nbt' $false) {
	$projectName = 'Nbt'
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
  
  Write-Host 'You still need to open Visual Studio and build the application one time prior to going to the site in a browser.'
}

Write-Host "End: Attempting IIS Auto-Configure"
Write-Host "-----------"
Write-Host "Start: Purely optional developer tools"

if (Install-NeededFor 'Install NotePad++ (The old-and-busted Notepad of choice)' $false) {
  cinstm notepadplusplus
}

if (Install-NeededFor 'Install SublimeText2 (The new hotness Notepad of choice' $false) {
  cinstm sublimetext2
  cinst EthanBrown.SublimeText2.EditorPackages
  cinst EthanBrown.SublimeText2.WebPackages
  cinst EthanBrown.SublimeText2.UtilPackages
}
  
if (Install-NeededFor 'Install Emacs (If thats the kind of thing you are into)' $false) {
  cinstm Emacs
}
  
if (Install-NeededFor 'Install SysInternals (Complete SysInternals suite added to PATH)' $false) {
  cinstm sysinternals
}

if (Install-NeededFor 'Install GNU Windows Utils (Collection of nix tools for Windows)' $false) {
  cinstm GnuWin
}

if (Install-NeededFor 'Install 7-zip (File zipper/unzipper)' $false) {
  cinstm 7zip
}

if (Install-NeededFor 'Install FileZilla (FTP client)' $false) {
  cinstm filezilla
}

Write-Host "End: Purely optional developer tools"
Write-Host "-----------"
Write-Host "Start: Update all packages?"

if (Install-NeededFor 'Update all packages to latest version?' $false) {
  cup
}

Write-Host "End: Update all packages?"
Write-Host "-----------"
Write-Host "Setup Script Completed"