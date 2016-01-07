
MSBuild.exe Emitter.sln /p:Configuration=Release

.\Deploy\NuGet.exe pack Emitter.nuspec -OutputDirectory ".\Deploy"
