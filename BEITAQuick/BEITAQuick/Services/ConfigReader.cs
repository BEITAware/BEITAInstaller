using System;
using System.Collections.Generic;
using System.IO;
using BEITAQuick.Models;

namespace BEITAQuick.Services
{
    public class ConfigReader
    {
        public static InstallConfig LoadConfig(string basePath)
        {
            var config = new InstallConfig();

            // 读取应用程序名称配置
            var appConfigFile = Path.Combine(basePath, "config.ini");
            if (File.Exists(appConfigFile))
            {
                var appName = File.ReadAllText(appConfigFile).Trim();
                if (!string.IsNullOrEmpty(appName))
                {
                    config.ApplicationName = appName;
                }
            }

            // 读取内容配置
            var contentDirs = Directory.GetDirectories(basePath, "CONTENT_*");

            foreach (var contentDir in contentDirs)
            {
                var configFile = Path.Combine(contentDir, "config.ini");
                if (File.Exists(configFile))
                {
                    var targetPath = File.ReadAllText(configFile).Trim();
                    var contentPath = Path.Combine(contentDir, "CONTENT");
                    
                    if (Directory.Exists(contentPath))
                    {
                        foreach (var folder in Directory.GetDirectories(contentPath))
                        {
                            config.ContentItems.Add(new ContentItem
                            {
                                Name = Path.GetFileName(folder),
                                SourcePath = folder,
                                TargetPath = targetPath
                            });
                        }
                    }
                }
            }

            var licensePath = Path.Combine(basePath, "LICENSE.txt");
            if (File.Exists(licensePath))
            {
                config.LicensePath = licensePath;
            }

            return config;
        }
    }
}
