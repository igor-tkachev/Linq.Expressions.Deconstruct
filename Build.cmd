cd /d "%~dp0"
"%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" Linq.Expressions.Deconstruct.sln /p:Configuration=Release /t:Restore;Rebuild /v:m
"%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" Linq.Expressions.Deconstruct.sln /p:Configuration=Debug   /t:Restore;Rebuild /v:m
