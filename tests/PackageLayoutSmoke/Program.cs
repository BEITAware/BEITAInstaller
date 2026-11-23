using System;
using System.IO;
using BEITAQuick.Models;
using BEITAQuick.Services;

var temp = Directory.CreateTempSubdirectory("BEITAQuick-");
try
{
    foreach (var layout in new[] { "direct", "nested" })
    {
        var launcher = Path.Combine(temp.FullName, layout);
        var payload = layout == "direct" ? launcher : Path.Combine(launcher, "App");
        var output = Path.Combine(temp.FullName, layout + "-installed");
        string files = Path.Combine(payload, "CONTENT_1", "CONTENT", "Component");
        Directory.CreateDirectory(files);
        File.WriteAllText(Path.Combine(payload, "config.ini"), "Sample");
        File.WriteAllText(Path.Combine(payload, "LICENSE.txt"), "License");
        File.WriteAllText(Path.Combine(payload, "CONTENT_1", "config.ini"), output);
        File.WriteAllText(Path.Combine(files, "asset.txt"), layout);

        var config = ConfigReader.LoadConfig(launcher);
        if (config.ApplicationName != "Sample" || config.LicensePath != Path.Combine(payload, "LICENSE.txt"))
            throw new Exception("Application metadata did not follow the content layout.");
        new InstallService().Install(config);
        if (File.ReadAllText(Path.Combine(output, "Component", "asset.txt")) != layout)
            throw new Exception("Component was not installed.");
    }
    var empty = Directory.CreateDirectory(Path.Combine(temp.FullName, "empty"));
    ExpectInvalid(() => ConfigReader.LoadConfig(empty.FullName));
    ExpectInvalid(() => new InstallService().Install(new InstallConfig()));
    string broken = Path.Combine(temp.FullName, "direct", "CONTENT_2");
    Directory.CreateDirectory(Path.Combine(broken, "CONTENT"));
    File.WriteAllText(Path.Combine(broken, "config.ini"), " ");
    ExpectInvalid(() => ConfigReader.LoadConfig(Path.Combine(temp.FullName, "direct")));
    Console.WriteLine("Direct and nested installations and invalid-package checks passed.");
}
finally
{
    temp.Delete(recursive: true);
}

static void ExpectInvalid(Action action)
{
    try { action(); }
    catch (InvalidDataException) { return; }
    throw new Exception("Expected the invalid package to be rejected.");
}
