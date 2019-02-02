#param([String]$file="*.csproj")

# sumo local folder - debug
Remove-Item *.nupkg
nuget.exe pack -Build -OutputDirectory "." -Verbosity quiet -properties Configuration=Release
nuget.exe push *.nupkg -Source "Local"

Remove-Item *.nupkg