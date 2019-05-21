msbuild Emitter.Projects/Emitter.Net40.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL;NO_TLS_1_1" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net45.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net451.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net452.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net46.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net461.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net462.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net47.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net471.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.Net472.csproj /t:Clean,Build /p:DefineConstants="FX;TRACE;SSL" /p:Configuration=Release
msbuild Emitter.Projects/Emitter.WinRT.csproj /t:Clean,Build /p:DefineConstants="TRACE;WINDOWS_APP,WINDOWS_PHONE_APP,SSL,WINRT" /p:Configuration=Release
dotnet restore Emitter
dotnet build Emitter -c Release -f netstandard1.5