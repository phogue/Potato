mkdir "..\..\builds\Packages\"
nuget pack .\Myrcon.Procon.nuspec -version 2.0.1 -build -OutputDirectory ..\..\builds\Packages\
nuget pack .\Myrcon.Procon.Core.nuspec -version 2.0.1 -build -OutputDirectory ..\..\builds\Packages\
nuget pack .\Myrcon.Procon.Tools.nuspec -version 2.0.1 -build -OutputDirectory ..\..\builds\Packages\
nuget pack .\Myrcon.Procon.Shared.nuspec -version 2.0.1 -build -OutputDirectory ..\..\builds\Packages\