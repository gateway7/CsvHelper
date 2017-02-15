rmdir /S /Q NuGet

dotnet pack src\CsvHelper --configuration release --output ..\..\..\..\NuGet

rem nuget add src\CsvHelper\NuGet\CsvHelper.3.0.1.nupkg -source ..\..\NuGet

pause
