using System;
using System.IO;
using BEITAQuick.Models;

namespace BEITAQuick.Services
{
    public class ConfigReader
    {
        public static InstallConfig LoadConfig(string basePath)
        {
            var config = new InstallConfig();
            var contentDirs = Directory.GetDirectories(basePath, "CONTENT_*");
            if (contentDirs.Length == 0 && Directory.Exists(Path.Combine(basePath, "App")))
            {
                basePath = Path.Combine(basePath, "App");
                contentDirs = Directory.GetDirectories(basePath, "CONTENT_*");
            }
            if (contentDirs.Length == 0)
                throw new InvalidDataException("未找到 CONTENT_* 安装内容。请将整个安装包解压到同一个文件夹。");

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
            foreach (var contentDir in contentDirs)
            {
                var configFile = Path.Combine(contentDir, "config.ini");
                var targetPath = File.ReadAllText(configFile).Trim();
                if (string.IsNullOrWhiteSpace(targetPath))
                    throw new InvalidDataException("安装路径为空: " + configFile);

                var contentPath = Path.Combine(contentDir, "CONTENT");
                var folders = Directory.GetDirectories(contentPath);
                if (folders.Length == 0)
                    throw new InvalidDataException("未找到要安装的组件: " + contentPath);

                foreach (var folder in folders)
                {
                    config.ContentItems.Add(new ContentItem
                    {
                        Name = Path.GetFileName(folder),
                        SourcePath = folder,
                        TargetPath = targetPath
                    });
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
