msbuild Emitter.Projects/Emitter.NetMf42.csproj /t:Clean,Build /p:DefineConstants="MF;MF_FRAMEWORK_VERSION_V4_2;SSL" /p:OutputPath=..\bin\Release\Emitter.NetMf42\ /p:Configuration=Release
msbuild Emitter.Projects/Emitter.NetMf43.csproj /t:Clean,Build /p:DefineConstants="MF;MF_FRAMEWORK_VERSION_V4_3;SSL" /p:OutputPath=..\bin\Release\Emitter.NetMf43\ /p:Configuration=Release
msbuild Emitter.Projects/Emitter.NetMf44.csproj /t:Clean,Build /p:DefineConstants="MF;MF_FRAMEWORK_VERSION_V4_3;SSL" /p:OutputPath=..\bin\Release\Emitter.NetMf44\ /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net20.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL;NO_TLS_1_1;FX20" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net30.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL;NO_TLS_1_1;FX20" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net35.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL;NO_TLS_1_1" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net40.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL;NO_TLS_1_1" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net45.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net451.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net452.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net46.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net461.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.WinRT.csproj /t:Clean,Build /p:DefineConstants="TRACE;WINDOWS_APP,WINDOWS_PHONE_APP,SSL,WINRT" /p:Configuration=Release
dotnet restore Emitter
dotnet publish Emitter -o bin\Release\Emitter.DnxCore50\ -c Release -f netstandard1.5