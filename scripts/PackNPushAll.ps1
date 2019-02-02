param([string]$target = "debug")
# debug, release, nuget
echo "push target: " $target

[string]$scriptName = "PackNPush_Local_Debug.ps1"
if($target -eq "release")
{
    $scriptName = "PackNPush_Local_Release.ps1"
}
if($target -eq "nuget")
{
    $scriptName = "PackNPush_NugetOrg.ps1"
}
echo "script: " $scriptName

echo "pack n push >>>>>>>>>>>>>>>>>>>>>>>>>>>>>"
Set-Location .\src\Tello.Conroller\
[string]$location = Get-Location
echo "location: " $location
Invoke-Expression $scriptName
Set-Location ..
Set-Location ..
[string]$location = Get-Location
echo "location: " $location
echo "pack n push <<<<<<<<<<<<<<<<<<<<<<<<<<<<<"

