%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\Net35Essentials/Net35Essentials.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\Net35Essentials\src\Net35Essentials\bin\Release" ".\Dependency Builds\Net35Essentials\DLLs\"

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" .\Testity.Unity3D.Events/Testity.Unity3D.Events.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\Testity.Unity3D.Events\src\Testity.Unity3D.Events\bin\Release" ".\Dependency Builds\Testity.Unity3D.Events\DLLs\"
xcopy  /R /E /Y /q ".\Testity.Unity3D.Events\src\Testity.Unity3D.Events.Editor\bin\Release" ".\Dependency Builds\Testity.Unity3D.Events\DLLs\"

PAUSE
