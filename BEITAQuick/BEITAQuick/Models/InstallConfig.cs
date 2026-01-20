using System.Collections.Generic;

namespace BEITAQuick.Models
{
    public class InstallConfig
    {
        public string ApplicationName { get; set; } = "安装向导";
        public string LicensePath { get; set; }
        public List<ContentItem> ContentItems { get; set; } = new List<ContentItem>();
    }

    public class ContentItem
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
    }
}
