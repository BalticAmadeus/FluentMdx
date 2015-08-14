del *.nupkg
del bin/Release/*.*
msbuild BalticAmadeus.FluentMdx.csproj /t:Rebuild /property:Configuration=Release
nuget pack BalticAmadeus.FluentMdx.csproj -Prop Configuration=Release
nuget setApiKey 7b084f3a-997f-4ef0-ac0a-e0b1b042a09f  
nuget push *.nupkg
