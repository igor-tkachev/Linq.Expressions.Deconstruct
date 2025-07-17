cd /d "%~dp0"
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal

