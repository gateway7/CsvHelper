rmdir /S /Q NuGet

dotnet pack src\CsvHelper --configuration release --output ..\..\..\..\NuGet

rem rmdir /q/s ..\..\NuGet\CsvHelper

rem nuget add src\CsvHelper\NuGet\CsvHelper.3.0.1.nupkg -source ..\..\NuGet

pause
