using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BEITAQuick.Models;
using BEITAQuick.Services;

namespace BEITAQuick
{
    public partial class MainWindow : Window
    {
        private int currentStep = 0;
        private InstallConfig config;
        private InstallService installService;
        private readonly System.Collections.Generic.Dictionary<ContentItem, TextBox> pathTextBoxes = new System.Collections.Generic.Dictionary<ContentItem, TextBox>();

        public MainWindow()
        {
            InitializeComponent();
            if (!LoadConfiguration())
            {
                NextButton.IsEnabled = false;
                return;
            }
            BuildPathConfigUI();
            UpdateUI();
        }

        private bool LoadConfiguration()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                config = ConfigReader.LoadConfig(basePath);

                // 设置窗口标题
                this.Title = config.ApplicationName + " - 安装向导";

                // 设置欢迎页面文本
                WelcomeTitle.Text = $"欢迎安装 {config.ApplicationName}";
                WelcomeMessage.Text = $"此向导将引导您完成 {config.ApplicationName} 的安装过程。";

                // 设置完成页面文本
                CompleteMessage.Text = $"{config.ApplicationName} 已成功安装！";

                if (!string.IsNullOrEmpty(config.LicensePath))
                {
                    LicenseText.Text = File.ReadAllText(config.LicensePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载配置失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void BuildPathConfigUI()
        {
            PathConfigPanel.Children.Clear();
            pathTextBoxes.Clear();

            foreach (var item in config.ContentItems)
            {
                // 创建每个组件的配置面板
                var itemPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

                // 组件名称标签
                var label = new TextBlock
                {
                    Text = $"组件: {item.Name}",
                    FontFamily = (System.Windows.Media.FontFamily)FindResource("AeroFont"),
                    FontSize = 18,
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 0, 0, 8)
                };
                itemPanel.Children.Add(label);

                // 路径输入区域
                var pathGrid = new Grid();
                pathGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                pathGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // 文本框
                var textBox = new TextBox
                {
                    Text = item.TargetPath,
                    Style = (Style)FindResource("PathTextBoxStyle"),
                    Margin = new Thickness(0, 0, 10, 0)
                };
                Grid.SetColumn(textBox, 0);
                pathGrid.Children.Add(textBox);

                // 浏览按钮
                var browseButton = new Button
                {
                    Style = (Style)FindResource("BrowseButtonStyle"),
                    Tag = item
                };
                browseButton.Click += BrowseButton_Click;
                Grid.SetColumn(browseButton, 1);
                pathGrid.Children.Add(browseButton);

                itemPanel.Children.Add(pathGrid);

                // 显示将要安装的文件夹
                var infoText = new TextBlock
                {
                    Text = $"将安装到: {Path.Combine(item.TargetPath, item.Name)}",
                    FontFamily = (System.Windows.Media.FontFamily)FindResource("AeroFont"),
                    FontSize = 14,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Margin = new Thickness(0, 5, 0, 0)
                };
                itemPanel.Children.Add(infoText);

                PathConfigPanel.Children.Add(itemPanel);

                // 保存文本框引用
                pathTextBoxes[item] = textBox;

                // 监听文本框变化以更新信息文本
                textBox.TextChanged += (s, e) =>
                {
                    try
                    {
                        infoText.Text = $"将安装到: {Path.Combine(textBox.Text, item.Name)}";
                    }
                    catch (ArgumentException)
                    {
                        infoText.Text = "请输入有效的安装路径。";
                    }
                };
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = (ContentItem)button.Tag;

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = $"选择 {item.Name} 的安装位置",
                ShowNewFolderButton = true,
                SelectedPath = pathTextBoxes[item].Text
            })
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    pathTextBoxes[item].Text = dialog.SelectedPath;
            }
        }

        private void UpdateUI()
        {
            WelcomePage.Visibility = currentStep == 0 ? Visibility.Visible : Visibility.Collapsed;
            LicensePage.Visibility = currentStep == 1 ? Visibility.Visible : Visibility.Collapsed;
            CustomPathPage.Visibility = currentStep == 2 ? Visibility.Visible : Visibility.Collapsed;
            InstallPage.Visibility = currentStep == 3 ? Visibility.Visible : Visibility.Collapsed;
            CompletePage.Visibility = currentStep == 4 ? Visibility.Visible : Visibility.Collapsed;

            BackButton.Visibility = currentStep > 0 && currentStep < 3 ? Visibility.Visible : Visibility.Collapsed;
            NextButton.Visibility = currentStep == 3 ? Visibility.Collapsed : Visibility.Visible;
            CancelButton.IsEnabled = currentStep != 3;

            if (currentStep == 0)
            {
                NextButton.Content = "下一步";
                NextButton.IsEnabled = true;
            }
            else if (currentStep == 1)
            {
                NextButton.Content = "下一步";
                NextButton.IsEnabled = AcceptLicense.IsChecked == true;
            }
            else if (currentStep == 2)
            {
                NextButton.Content = "安装";
                NextButton.IsEnabled = true;
            }
            else if (currentStep == 3)
            {
                InstallProgress.Value = 0;
            }
            else if (currentStep == 4)
            {
                NextButton.Content = "完成";
                NextButton.IsEnabled = true;
                CancelButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep == 0)
            {
                // 从欢迎页到许可协议页（如果有）或路径配置页
                if (!string.IsNullOrEmpty(config.LicensePath))
                {
                    currentStep = 1;
                }
                else
                {
                    currentStep = 2;
                }
            }
            else if (currentStep == 1)
            {
                // 从许可协议页到路径配置页
                currentStep = 2;
            }
            else if (currentStep == 2)
            {
                // 从路径配置页到安装页
                // 更新配置中的目标路径
                foreach (var item in config.ContentItems)
                {
                    item.TargetPath = pathTextBoxes[item].Text;
                }
                currentStep = 3;
                UpdateUI();
                await StartInstallation();
            }
            else if (currentStep == 4)
            {
                Application.Current.Shutdown();
            }

            UpdateUI();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep > 0)
            {
                if (currentStep == 2 && string.IsNullOrEmpty(config.LicensePath))
                {
                    currentStep = 0;
                }
                else
                {
                    currentStep--;
                }
                UpdateUI();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要取消安装吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void AcceptLicense_Changed(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private async Task StartInstallation()
        {
            installService = new InstallService();
            installService.ProgressChanged += InstallService_ProgressChanged;

            try
            {
                await Task.Run(() => installService.Install(config));
                currentStep = 4;
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"安装失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                currentStep = 2;
                UpdateUI();
            }
        }

        private void InstallService_ProgressChanged(object sender, ProgressEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                InstallStatusText.Text = e.Message;
                InstallDetailText.Text = e.CurrentFile != null ? $"正在处理: {e.CurrentFile}" : "";
                InstallProgress.Value = (double)e.CurrentItem / e.TotalItems * 100;
            });
        }
    }
}
