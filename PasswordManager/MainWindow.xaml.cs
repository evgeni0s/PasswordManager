using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using log4net.Config;
using log4net;
using System.Reflection;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon MyNotifyIcon;
        private bool actualClose;
        private MainWindowViewModel mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();


            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;
            MyNotifyIcon = new NotifyIcon();
            //using (var str = new Stream())
            //var bitmap = new Bitmap(LocalResources.Tray_icon.);
            IntPtr pIcon = LocalResources.key2.Handle;
            Icon ico = System.Drawing.Icon.FromHandle(pIcon);
            MyNotifyIcon.Icon = ico;
            MyNotifyIcon.MouseDoubleClick += MyNotifyIcon_MouseDoubleClick;
            MyNotifyIcon.MouseClick += MyNotifyIcon_MouseClick;
            InitializeLogging();
        }

        private void InitializeLogging()
        {
            Console.SetOut(new ControlWriter(LogTextBox));

            // connecting config file for log4net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        private void MyNotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = (ContextMenu)this.FindResource("NotifierContextMenu");
                contextMenu.IsOpen = true;
            }
        }

        private void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void Menu_Settings(object sender, RoutedEventArgs e)
        {

        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.BalloonTipTitle = "Minimize Successful";
                MyNotifyIcon.BalloonTipText = "Minimized the app";
                MyNotifyIcon.ShowBalloonTip(400);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (actualClose)
            {
                return;
            }
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }

        private void Menu_Close(object sender, RoutedEventArgs e)
        {
            actualClose = true;
            Close();
        }

        private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            mainWindowViewModel.OnPasswordKeyDown(sender, e);
        }

        private void LogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogTextBox.ScrollToEnd();
        }
    }
}
