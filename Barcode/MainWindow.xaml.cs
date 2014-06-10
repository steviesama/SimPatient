using System;
using System.Collections.Generic;
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

using WinForms = System.Windows.Forms;
using System.Windows.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Barcode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private double pillCount = 0.0;
        private double pillMg = 0.0;
        private double refillSpan = 0.0;
        private double timeSpan = 0.0;

        public MainWindow()
        {

            InitializeComponent();

            //StringBuilder str = new StringBuilder();

            //for (int i = 1; i < 10; i++)
            //    str.AppendLine(string.Format("{0:#,0.#############}", Math.Pow((double)i, 7)));

            //MessageBox.Show(str.ToString());

            //try
            //{
            //    var test = JsonConvert.DeserializeObject<Dictionary<string, string>>("{\"stuff\":\"C.S. Taylor, Jr.\"}");
            //    MessageBox.Show("test['stuff'] == " + test["stuff"]);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, ex.Source);
            //}

            txtMg.TextChanged += txtMg_TextChanged;
            txtMg.GotFocus += (object sender, RoutedEventArgs e) => { selectAllText(sender as TextBox); };
            txtPillCount.TextChanged += txtPillCount_TextChanged;
            txtPillCount.GotFocus += (object sender, RoutedEventArgs e) => { selectAllText(sender as TextBox); };
            txtRefillSpan.TextChanged += txtRefillSpan_TextChanged;
            txtRefillSpan.GotFocus += (object sender, RoutedEventArgs e) => { selectAllText(sender as TextBox); };
            txtTimeSpan.TextChanged += txtTimeSpan_TextChanged;
            txtTimeSpan.GotFocus += (object sender, RoutedEventArgs e) => { selectAllText(sender as TextBox); };

        /*End MainWindow()*/}

        void txtTimeSpan_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(txtTimeSpan.Text, out timeSpan);
            calcPillsPerTimeSpan();
        }

        void txtRefillSpan_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(txtRefillSpan.Text, out refillSpan);
            calcPillsPerTimeSpan();
        }

        void txtPillCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(txtPillCount.Text, out pillCount);
            calcPillsPerTimeSpan();
        }

        private void txtMg_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(txtMg.Text, out pillMg);
            calcPillsPerTimeSpan();
        }

        private void calcPillsPerTimeSpan()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                double refillIntervals = (timeSpan / refillSpan);
                double pillsPerTimeSpan = refillIntervals * pillCount;
                txtPillsPerTimeSpan.Text = string.Format("{0:n}", pillsPerTimeSpan);
            }));
        }

        public void selectAllText(TextBox textBox)
        {
            textBox.SelectAll();
        }

    } //End class MainWindow
} //End namespace Barcode
