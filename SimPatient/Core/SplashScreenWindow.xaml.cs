using System.Windows;

using System.Diagnostics;

namespace SimPatient
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        /// <summary>
        /// Constructs the SplashScreenWindow and sets its owner
        /// to the MainWindow instance before it is displayed.
        /// </summary>
        public SplashScreenWindow()
        {
            InitializeComponent();
            Owner = MainWindow.Instance;
        }

        /// <summary>
        /// Close button Click event handler.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Help hyperlink Click event handler.
        /// </summary>
        private void helpHyperlink_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            Process.Start(currentDirectory + "/Help/help.html");
        }
    }
}
