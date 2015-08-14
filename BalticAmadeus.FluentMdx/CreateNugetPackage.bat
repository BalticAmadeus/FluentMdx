del *.nupkg
del bin/Release/*.*
msbuild BalticAmadeus.FluentMdx.csproj /t:Rebuild /property:Configuration=Release
nuget pack BalticAmadeus.FluentMdx.csproj -Prop Configuration=Release

