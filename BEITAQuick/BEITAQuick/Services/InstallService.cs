using System;
using System.IO;
using BEITAQuick.Models;

namespace BEITAQuick.Services
{
    public class InstallService
    {
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public void Install(InstallConfig config)
        {
            int totalItems = config.ContentItems.Count;
            int currentItem = 0;

            foreach (var item in config.ContentItems)
            {
                currentItem++;
                OnProgressChanged(new ProgressEventArgs
                {
                    CurrentItem = currentItem,
                    TotalItems = totalItems,
                    CurrentFile = item.Name,
                    Message = $"正在安装 {item.Name}..."
                });

                CopyDirectory(item.SourcePath, Path.Combine(item.TargetPath, item.Name));
            }

            OnProgressChanged(new ProgressEventArgs
            {
                CurrentItem = totalItems,
                TotalItems = totalItems,
                Message = "安装完成！"
            });
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(targetDir, fileName), true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(dir);
                CopyDirectory(dir, Path.Combine(targetDir, dirName));
            }
        }

        protected virtual void OnProgressChanged(ProgressEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public int CurrentItem { get; set; }
        public int TotalItems { get; set; }
        public string CurrentFile { get; set; }
        public string Message { get; set; }
    }
}
