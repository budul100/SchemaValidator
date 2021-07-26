@echo off

SET CONFIGURATION=Release

SET PackageName=SchemaValidator
SET BuildDir=.\%PackageName%
SET ScriptsDir=.\_Scripts
SET NuGetDir=.

SET ProjectPaths='%BuildDir%\%PackageName%.csproj'

echo.
echo ##### Create Library #####
echo.

CHOICE /C mb /N /M "Shall the [b]uild (x.x._X_.0) or the [m]inor version (x._X_.0.0) be increased?"
SET VERSIONSELECTION=%ERRORLEVEL%
echo.

if /i "%VERSIONSELECTION%" == "1" (
	echo.
	echo Update minor version
	echo.

	powershell "%ScriptsDir%\Update_VersionMinor.ps1 -projectPaths %ProjectPaths%"
)

GOTO BUILD

:BUILD

echo.
echo Clean solution
echo.

powershell "%ScriptsDir%\CleanFolders.ps1 -baseDir %CD%"

echo.
echo Build solution
echo.

dotnet build "%BuildDir%\%PackageName%.csproj" --configuration %CONFIGURATION%

echo.
echo Test solution
echo.

dotnet test %PackageName%.sln --logger:"console;verbosity=detailed"

if not %ERRORLEVEL% == 0 goto BUILDEND

echo.
echo Copy NuGet packages
echo.

del "%NuGetDir%\*.nupkg"
for /R %cd% %%f in (*.nupkg) do copy %%f %NuGetDir%\

echo.
echo Update build version
echo.

powershell "%ScriptsDir%\Update_VersionBuild.ps1 -projectPaths %ProjectPaths%"
goto BUILDEND

:BUILDEND

echo.
PAUSE
