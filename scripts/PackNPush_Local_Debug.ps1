#param([String]$file="*.csproj")

# sumo local folder - debug
Remove-Item *.nupkg
nuget.exe pack -Build -Symbols -OutputDirectory "." -Verbosity quiet -properties Configuration=Debug
nuget.exe push *.nupkg -Source "Local"

Remove-Item *.nupkg