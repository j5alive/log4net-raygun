#tool "nuget:?package=NUnit.Runners&version=2.6.4"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var solution = "./log4net.Raygun.sln";

//////////////////////////////////////////////////////////////////////
// BUILD TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    MSBuild(solution, settings =>
        settings.WithTarget("Clean")
				.SetConfiguration("Debug"));
    MSBuild(solution, settings =>
        settings.WithTarget("Clean")
				.SetConfiguration("Release 4.0"));
    MSBuild(solution, settings =>
        settings.WithTarget("Clean")
				.SetConfiguration("Release 4.5"));
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
	MSBuild(solution, settings =>
        settings.SetConfiguration("Release 4.5"));
		
	MSBuild(".\\log4net.Raygun.WebApi\\log4net.Raygun.WebApi.csproj", settings =>
        settings.SetConfiguration("Release 4.5"));
	MSBuild(".\\log4net.Raygun\\log4net.Raygun.csproj", settings =>
        settings.SetConfiguration("Release 4.0"));
	MSBuild(".\\log4net.Raygun\\log4net.Raygun.csproj", settings =>
        settings.SetConfiguration("Release 4.5"));
});

Task("PreparePackages")
	.IsDependentOn("Run-Unit-Tests")
	.Does(() =>
{
    MSBuild(solution, settings =>
        settings.SetConfiguration("Release 4.0"));

	MSBuild(".\\log4net.Raygun.WebApi\\log4net.Raygun.WebApi.csproj", settings =>
        settings.SetConfiguration("Release 4.5")
				.WithTarget("Build")
				.WithTarget("Package"));

	var webApiPackage = GetFiles("./log4net.Raygun.WebApi/NuGet/*.nupkg");
	CopyFiles(webApiPackage, "output");

	MSBuild(".\\log4net.Raygun\\log4net.Raygun.csproj", settings =>
        settings.SetConfiguration("Release 4.0"));
	MSBuild(".\\log4net.Raygun\\log4net.Raygun.csproj", settings =>
        settings.SetConfiguration("Release 4.5")
				.WithTarget("Build")
				.WithTarget("Package"));
				
	var otherPackages = GetFiles("./log4net.Raygun/NuGet/*.nupkg");
	CopyFiles(otherPackages, "output");
});

//////////////////////////////////////////////////////////////////////
// TESTING TARGETS
//////////////////////////////////////////////////////////////////////

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit("**/bin/Release/net45/*Tests.dll", new NUnitSettings {
        NoResults = true
        });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("PreparePackages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
