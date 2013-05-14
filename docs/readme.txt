1. NuGet packages restoring

Open new *.csproj file and add some configuration lines:

  <!-- NuGet Restore Packages Tweaks -->
  <PropertyGroup>
    <SolutionDir Condition="$(SolutionDir) == '' Or $(SolutionDir) == '*Undefined*'">$(MSBuildProjectDirectory)\..\</SolutionDir>
    <RestorePackages>true</RestorePackages>
    <PackagesDir>$(SolutionDir)lib\packages</PackagesDir>
    <NuGetExePath>$(SolutionDir)tools\NuGet.exe</NuGetExePath>
  </PropertyGroup>

and after import tag of Microsoft.CSharp.targets this line:

  <Import Project="$(SolutionDir)build\NuGet.targets" />

