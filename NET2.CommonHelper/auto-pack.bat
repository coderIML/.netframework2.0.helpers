nuget setapikey oy2ah26eu43kiyahpjxqdq7g56bug5le5oh5vouih3vztu
nuget pack CommonHelper.csproj -Build -Properties owners="arison";description="just only for .net framework 2.0 programs!if your programs run on .net framework runtimes,you can use it ";id="dotnet.framework.2.0.commonhelper" -Version 1.0.0
nuget push CommonHelper.1.0.0.nupkg -Source https://www.nuget.org/api/v2/package
