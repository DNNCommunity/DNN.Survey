#addin nuget:?package=SharpZipLib&version=1.4.2
#addin nuget:?package=Cake.Compression&version=0.2.6

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = "";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
.Does(()=>{
	CleanDirectory("./Install");
	CleanDirectory("./Install/Package");
	CleanDirectory("./Install/Package/bin");
	CleanDirectory("./Install/Package/Providers/DataProviders/SqlDataProvider");
	CleanDirectory("./Install/Package/Documentation");
});

Task("GetInfo")
.Does(() => {
	version = XmlPeek("./DNN_Survey.dnn", "dotnetnuke/packages/package[1]/@version");
	Information("Version is: " + version);
});

Task("ReleasePackage")
.IsDependentOn("Clean")
.IsDependentOn("GetInfo")
.Does(() => {	

	// GET THE RESOURCE FILES
	var resourceFiles = GetFiles("./*.ascx");
	resourceFiles.Add(GetFiles("./App_LocalResources/*.resx"));
	resourceFiles.Add(GetFiles("./Controls/*.ascx"));
	resourceFiles.Add(GetFiles("./**/*.css"));
	resourceFiles.Add(GetFiles("./*.txt"));
	resourceFiles.Add(GetFiles("./images/*"));
	resourceFiles.Add(GetFiles("./js/*.js"));
	foreach (var file in resourceFiles)
	{
		Information("Zipping resource file: " + file);
	}
	Zip("./", "./Install/Package/Resources.zip", resourceFiles);

	// GET THE SQL SCRIPTS
	var sqlScripts = GetFiles("./Providers/DataProviders/SqlDataProvider/*.SqlDataProvider");
	CopyFiles(sqlScripts, "./Install/Package/Providers/DataProviders/SqlDataProvider");

	// COPY THE BINARIES
	CopyFile("../../../bin/Dnn.Modules.Survey.dll", "./Install/Package/bin/DNN.Modules.Survey.dll");

	// COPY THE SYMBOLS
	var symbols = GetFiles("../../../bin/DNN.Modules.Survey.pdb");
	Zip("../../../bin", "./Install/Package/Symbols.zip", symbols);

	// COPY RELEASE NOTES AND LICENSE
	CopyFiles("./Documentation/*.html", "./Install/Package/Documentation");

	// COPY THE MANIFEST
	CopyFiles("./*.dnn", "./Install/Package");

	// ZIP THE PACKAGE
	Zip("./Install/Package", "./Install/" + "Dnn.Modules.Survey_" + version + "_Install.zip");
	
	// CLEANUP TEMP FOLDER
	DeleteDirectory("./Install/Package", new DeleteDirectorySettings{
		Recursive = true,
		Force = true
	});
});

Task("Default")
.IsDependentOn("ReleasePackage");

RunTarget(target);