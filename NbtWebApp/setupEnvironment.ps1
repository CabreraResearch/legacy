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
    $timeout = 10
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
if (Install-NeededFor 'chocolatey [Required for the rest of this script to work]') {
  iex ((new-object net.webclient).DownloadString("http://chocolatey.org/install.ps1")) 
}

if (Install-NeededFor 'autosave' $false) {
  cinstm nodejs.install
  $nodePath = Join-Path $env:programfiles 'nodejs'
   $is64bit = (Get-WmiObject Win32_Processor).AddressWidth -eq 64
  if ($is64bit) {$nodePath = Join-Path ${env:ProgramFiles(x86)} 'nodejs'}
  $env:Path = "$($env:Path);$nodePath"
  npm install -g autosave
  
  Write-Host 'You still need to enable experimental packages in Chrome and install the Chrome Extension'
  Write-Host 'Details at https://github.com/NV/chrome-devtools-autosave#readme'
}

if (Install-NeededFor 'Grunt' $false) {
  cinstm  nodejs.install
  cinstm  PhantomJS
  $nodePath = Join-Path $env:programfiles 'nodejs'
   $is64bit = (Get-WmiObject Win32_Processor).AddressWidth -eq 64
  if ($is64bit) {$nodePath = Join-Path ${env:ProgramFiles(x86)} 'nodejs'}
  $env:Path = "$($env:Path);$nodePath"
  npm uninstall -g grunt
  npm install -g grunt-cli
  
  Write-Host 'Details at http://gruntjs.com/getting-started'
}

Write-Host "Checking for/installing required frameworks"
if (Install-NeededFor '.NET Runtimes up to 4.5' $false) {
    cinstm  netframework2 -source webpi
    cinstm  NETFramework35 -source webpi
    cinstm  NETFramework4 -source webpi
    cinstm  NETFramework4Update402 -source webpi
    cinstm  NETFramework4Update402_KB2544514_Only -source webpi
	cinstm  DotNet4.5
    cinstm  WindowsInstaller31 -source webpi
    cinstm WindowsInstaller45 -source webpi
}

Write-Host "Checking for/installing required compilers"
if (Install-NeededFor 'C++ Libraries' $false) {
    cinstm  vcredist2005
    cinstm  vcredist2008
    cinstm  vcredist2010
}

if (Install-NeededFor 'Tortoise' $false) {
    cinstm  tortoisehg
}

Write-Host "Checking for/installing PowerShell"
if (Install-NeededFor 'PowerShell 3.0' $false) {
    cinstm  PowerShell
    cinstm  PowerGUI
	cinstm  pscx
}

Write-Host "Checking for/installing Visual Studio Items..."
if (Install-NeededFor 'VS2012 Premium' $false) {
 cinstm  VisualStudio2012Premium
 cinstm  resharper
 cinstm  aspnetmvc
 cinstm  MVCLoc -source webpi
}

if (Install-NeededFor 'VS2010 Full Edition SP1' $false) {
 cinstm  VS2010SP1Pack -source webpi
 cinstm  resharper
 cinstm  aspnetmvc
 cinstm  MVCLoc -source webpi
}

Write-Host "Finished checking for/installing Visual Studio Items."

Write-Host "Checking for/installing Other language support"
if (Install-NeededFor 'Perl' $false) {
 cinstm  ActivePerl
}
if (Install-NeededFor 'Python' $false) {
 cinstm  python 
 cinstm  easy.install
}
if (Install-NeededFor 'Java' $false) {
 chocolatey uninstall javaruntime
 cinst javaruntime
}
Write-Host "Finished checking for/installing Other language support"

Write-Host "Checking for/installing IIS Items..."
if (Install-NeededFor 'IIS' $false) {
  cinstm  ASPNET -source webpi
  cinstm  ASPNET_REGIIS -source webpi
  cinstm  DefaultDocument -source webpi
  cinstm  DynamicContentCompression -source webpi
  cinstm  HTTPRedirection -source webpi
  cinstm  IIS7_ExtensionLessURLs -source webpi
  cinstm  IISExpress -source webpi
  cinstm  IISExpress_7_5 -source webpi
  cinstm  IISManagementConsole -source webpi
  cinstm  ISAPIExtensions -source webpi
  cinstm  ISAPIFilters -source webpi
  cinstm  NETExtensibility -source webpi
  cinstm  RequestFiltering -source webpi
  cinstm  StaticContent -source webpi
  cinstm  StaticContentCompression -source webpi
  cinstm  UrlRewrite2 -source webpi
  cinstm  WindowsAuthentication -source webpi
  cinstm  NodeJSExt -source webpi
}

Write-Host "Checking for/installing Project NPM..."
if (Install-NeededFor 'This Web App Project NPM package' $false) {
	$nodePath = Join-Path $env:programfiles 'nodejs'
   $is64bit = (Get-WmiObject Win32_Processor).AddressWidth -eq 64
  if ($is64bit) {$nodePath = Join-Path ${env:ProgramFiles(x86)} 'nodejs'}
  $env:Path = "$($env:Path);$nodePath" 
 npm install
  
  Write-Host 'This step has worked inconsistenly for some people. If needed, cd into the project folder and execute `npm install`'
  Write-Host 'You still need to open the project folder in a command prompt and execute `grunt.cmd build:{mode}` to build the project.'
}

$projectName = 'Nbt'
$srcDir = Join-Path $scriptDir "$($projectName)"
if (Install-NeededFor 'Auto-Configure IIS App and AppPool for Nbt' $false) {
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