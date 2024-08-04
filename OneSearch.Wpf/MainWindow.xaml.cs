using OneSearch.Extensibility.Core.Data;
using OneSearch.Extensibility.Core.Log;
using OneSearch.Extensibility.Core.Services;
using OneSearch.Plugin.OneNote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OneSearch.Wpf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var services = new SimpleServiceCollection();
            Configure(services);
            var provider = services.BuildServiceProvider();

            var sw = new Stopwatch();
            sw.Start();
            var plugin = provider.GetService<IOneNotePlugin>();
            plugin.Execute();
            sw.Stop();
            Console.WriteLine("ElapsedTime : " + sw.ElapsedMilliseconds + " ms");

            Console.WriteLine("finished.");
            //while (true)
            //{
            //    ;
            //}
        }

        private static void Configure(IServiceCollection services)
        {
            services.AddOneSearch();
            services.AddOneNotePlugin();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleWindowState();
            }
            else
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void ToggleWindowState()
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            ListBoxResult?.Items?.Add("aaa");

            // ListBoxResult.Items.Add(((TextBox)sender).Text);
        }
    }

    public static class TestExtensions
    {
        public static void AddOneSearch(this IServiceCollection services)
        {
            services.Add(typeof(ITraceLogger<>), typeof(SimpleTraceLogger<>), ServiceLifeTime.Singleton);
            services.Add(typeof(ITraceLoggerFactory), typeof(TraceLoggerFactory), ServiceLifeTime.Singleton);

            services.Add<AppSettings>((provider) => {
                    return AppSettings.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\OneSearch\\app.txt");
                }, ServiceLifeTime.Singleton);
            services.MapDataSrouce<AppSettings, AppSettingSectionA>();
        }
    }

}
