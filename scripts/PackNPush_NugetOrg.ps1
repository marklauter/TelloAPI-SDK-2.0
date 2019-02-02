#param([String]$file="*.csproj")

#nuget.org - release
Remove-Item *.nupkg
nuget.exe pack -Build -OutputDirectory "." -Verbosity quiet -properties Configuration=Release
nuget.exe push *.nupkg -Source "nuget.org"

Remove-Item *.nupkg