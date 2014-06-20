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

using Zen.Barcode;
using System.IO;
using System.Drawing.Imaging;
using Graphics = System.Drawing.Graphics;
using Bitmap = System.Drawing.Bitmap;
using DrawImage = System.Drawing.Image;
using System.Drawing.Drawing2D;
using System.Printing;
using SUT.PrintEngine;
using SUT.PrintEngine.Utils;

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
        private BitmapImage bitmap = null;

        public MainWindow()
        {

            InitializeComponent();

            renderBarcode("Tonitta Sauls");

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

        private Bitmap resizeBitmap(Bitmap bitmap, Size size)
        {
            Bitmap b = new Bitmap(640, 480);
            Graphics gfx = Graphics.FromImage(b);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            return b;
        }
        private DrawImage resizeImage(DrawImage imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((DrawImage)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (DrawImage)b;
        }

        private void renderBarcode(string textData)
        {
            //create barcode image
            CodeQrBarcodeDraw barcode = BarcodeDrawFactory.CodeQr;
            //draw barcode image
            DrawImage image = barcode.Draw(textData, 20);
            //create memory stream for barcode conversion
            MemoryStream ms = new MemoryStream();
            //save barcode image to memory stream as PNG
            image.Save(ms, ImageFormat.Png);
            //---debug render to file
            //FileStream fStream = new FileStream("barcode.png", FileMode.Create);
            //image.Save(fStream, ImageFormat.Png);
            //set memory stream position back to the beginning
            ms.Position = 0;
            //create bitmap image
            bitmap = new BitmapImage();
            //prep bitmap for memory stream
            bitmap.BeginInit();
            //set memory stream
            bitmap.StreamSource = ms;
            //end stream prep
            bitmap.EndInit();

            //use bitmap as image source            
            imgBarcode.Source = bitmap;
        }

        private void renderBarcode(string textData, PrintCapabilities printCaps)
        {
            //create barcode image
            CodeQrBarcodeDraw barcode = BarcodeDrawFactory.CodeQr;
            //draw barcode image
            DrawImage image = barcode.Draw(textData, 2);

            image = resizeImage(image, new Size(printCaps.PageImageableArea.ExtentWidth, printCaps.PageImageableArea.ExtentHeight));

            //create memory stream for barcode conversion
            MemoryStream ms = new MemoryStream();
            //save barcode image to memory stream as PNG
            image.Save(ms, ImageFormat.Png);
            //set memory stream position back to the beginning
            ms.Position = 0;
            //create bitmap image
            bitmap = new BitmapImage();
            //prep bitmap for memory stream
            bitmap.BeginInit();
            //set memory stream
            bitmap.StreamSource = ms;
            //end stream prep
            bitmap.EndInit();

            //use bitmap as image source            
            imgBarcode.Source = bitmap;
        }
        
        private DrawingVisual getDrawingVisual(BitmapImage bitmap)
        {
            var vis = new DrawingVisual();
            var dc = vis.RenderOpen();
            dc.DrawImage(bitmap, new Rect (0, 0, bitmap.Width, bitmap.Height));
            dc.Close();

            return vis;
        }

        private void printBarcode(FrameworkElement visual, string description)
        {
            PrintDialog pDiag = new PrintDialog();

            //Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            //{
            if (pDiag.ShowDialog() == true)
            {
                //get selected printer capabilities
                PrintCapabilities printCaps = pDiag.PrintQueue.GetPrintCapabilities(pDiag.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(printCaps.PageImageableArea.ExtentWidth / visual.ActualWidth, printCaps.PageImageableArea.ExtentHeight / visual.ActualHeight);
                //double scale = Math.Min(pDiag.PrintableAreaWidth / this.ActualWidth, pDiag.PrintableAreaHeight / this.ActualHeight);

                //Transform the Visual to scale
                visual.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                Size sz = new Size(printCaps.PageImageableArea.ExtentWidth, printCaps.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                visual.Measure(sz);
                visual.Arrange(new Rect(new Point(printCaps.PageImageableArea.OriginWidth, printCaps.PageImageableArea.OriginHeight), sz));

                pDiag.PrintVisual(visual, description);
            }
            //}));
        }

        private void printBarcode(BitmapImage bitmap, string description)
        {
            PrintDialog pDiag = new PrintDialog();

            //Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            //{
            if (pDiag.ShowDialog() == true)            
            {
                //get selected printer capabilities
                PrintCapabilities printCaps = pDiag.PrintQueue.GetPrintCapabilities(pDiag.PrintTicket);
                //renderBarcode("Tonitta Sauls", printCaps);

                DrawingVisual visual = getDrawingVisual(bitmap);
                //bitmap = addBitmapPadding(bitmap);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(printCaps.PageImageableArea.ExtentWidth / visual.ContentBounds.Width, printCaps.PageImageableArea.ExtentHeight / visual.ContentBounds.Height);                

                //Transform the Visual to scale
                visual.Transform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                //Size sz = new Size(printCaps.PageImageableArea.ExtentWidth, printCaps.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                //visual.Measure(sz);
                //visual.Arrange(new Rect(new Point(printCaps.PageImageableArea.OriginWidth, printCaps.PageImageableArea.OriginHeight), sz));

                //pDiag.PrintVisual(visual, description);
                var visualSize = new Size(visual.ContentBounds.Width, visual.ContentBounds.Height);
                var printControl = PrintControlFactory.Create(visualSize, visual);
                printControl.ShowPrintPreview();

            }
            //}));
        }

        private BitmapImage addBitmapPadding(BitmapImage bitmap)
        {
            return null;
        }

        public void doEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(exitFrames), frame);
            Dispatcher.PushFrame(frame);
        }

        public object exitFrames(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

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

        private void txtScan_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            printBarcode(bitmap, "Tonitta Sauls");
        }

        private void forceRepaint()
        {
            Dispatcher.Invoke(
              DispatcherPriority.Render,
              (Action)delegate()
              { this.ParentLayoutInvalidated(this); });
        }

    } //End class MainWindow
} //End namespace Barcode
