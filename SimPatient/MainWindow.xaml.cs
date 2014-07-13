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
using System.Windows.Markup;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Zen.Barcode;
using System.IO;
using System.Drawing.Imaging;
using Graphics = System.Drawing.Graphics;
using Bitmap = System.Drawing.Bitmap;
using DrawImage = System.Drawing.Image;
using System.Drawing.Drawing2D;
using RectangleF = System.Drawing.RectangleF;
using GraphicsUnit = System.Drawing.GraphicsUnit;
using System.Printing;
using SUT.PrintEngine;
using SUT.PrintEngine.Utils;
using System.Globalization;
using MediaLinearGradientBrush = System.Windows.Media.LinearGradientBrush;

using Color = System.Drawing.Color;
using RotateFlipType = System.Drawing.RotateFlipType;
using DrawFont = System.Drawing.Font;
using DrawBrush = System.Drawing.Brush;
using DrawBrushes = System.Drawing.Brushes;

using BarcodeLib;
using MySql.Data;
using SimPatient.DataModel;

namespace SimPatient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private BitmapImage bitmap = null;
        private Barcode barcode;
        private UserControl currentControl = null;
        public MainWindow()
        {
            InitializeComponent();
            string barcodeText = "Taylor, Hunter 8/23/2013";
            //renderBarcode(barcodeText);
            
            barcode = new Barcode
            {
                IncludeLabel = true,
                Alignment = AlignmentPositions.CENTER,
                Width = 300,
                Height = 40,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            //---add control
            //currentControl = new SimulationPoolControl { ActionMode = ActionMode.SelectionMode };
            currentControl = new LoginControl();
            bottomGrid.Children.Add(currentControl);

            //DockPanel.SetDock(currentControl, Dock.Bottom);
            //dockPanel.Children.Add(currentControl);

            //DrawImage img = barcode.Encode(TYPE.CODE128B, "MR123456");
            //Bitmap b = new Bitmap(350, 150);
            //Graphics g = Graphics.FromImage(b);
            ////g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            //g.DrawString("Name: Taylor, Hunter John Example 12\nDOB: 08/23/2013 Gender: Male\nPhysician: Dr. Doesn't Matter", new DrawFont("Verdana", 10f), DrawBrushes.Black, new RectangleF(40, 55, 350, 100));
            //g.DrawImage(img, new RectangleF(25, 105, img.Width, img.Height), new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            //bitmap = getBitmapImage(b);
            //imgBarcode.Source = bitmap;

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


            //txtMg.TextChanged += txtMg_TextChanged;
            //txtMg.GotFocus += (object sender, RoutedEventArgs e) => { selectAllText(sender as TextBox); };
            //txtPillCount.TextChanged += txtPillCount_TextChanged;
            //txtPillCount.GotFocus += (object sender, RoutedEventArgs e) => { selectAllText(sender as TextBox); };
            //txtRefillSpan.TextChanged += txtRefillSpan_TextChanged;
            //txtRefillSpan.GotFocus += (object sender, RoutedEventArgs e) => { selectAllText(sender as TextBox); };
            //txtTimeSpan.TextChanged += txtTimeSpan_TextChanged;
            //txtTimeSpan.GotFocus += (object sender, RoutedEventArgs e) => { selectAllText(sender as TextBox); };

        /*End MainWindow()*/}

        public Bitmap cropBitmap(Bitmap bmp)
        {

            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                System.Drawing.Color color;
                for (int i = 0; i < w; ++i)
                {
                    color = bmp.GetPixel(i, row);
                    if (color.A != 0/*color.R != 255 && color.G != 255 && color.B != 255*/)
                        return false;
                }
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                System.Drawing.Color color; ;
                for (int i = 0; i < h; ++i)
                {
                    color = bmp.GetPixel(col, i);
                    if (color.A != 0/*color.R != 255 && color.G != 255 && color.B != 255*/)
                        return false;
                }
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else break;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new RectangleF(0, 0, croppedWidth, croppedHeight),
                      new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      GraphicsUnit.Pixel);
                }
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }

        } //End cropBitmap()

        private Bitmap padBitmap(Bitmap bmp, double padScale)
        {
            int widthShim = (int)(bmp.Width * padScale);
            int heightShim = (int)(bmp.Height * padScale);
            int newWidth = bmp.Width + widthShim;
            int newHeight = bmp.Height + heightShim;

            var target = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(bmp,
                  new RectangleF(widthShim / 2, heightShim / 2, bmp.Width, bmp.Height),
                  new RectangleF(0, 0, bmp.Width, bmp.Height),
                  GraphicsUnit.Pixel);
            }

            return target;
        }

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

        //private void renderBarcode(string textData)
        //{
        //    //create barcode image
        //    CodeQrBarcodeDraw barcode = BarcodeDrawFactory.CodeQr;
        //    //draw barcode image
        //    DrawImage image = barcode.Draw(textData, 20);
        //    //create memory stream for barcode conversion
        //    MemoryStream ms = new MemoryStream();
        //    //save barcode image to memory stream as PNG
        //    image.Save(ms, ImageFormat.Png);
        //    //---debug render to file
        //    //FileStream fStream = new FileStream("barcode.png", FileMode.Create);
        //    //image.Save(fStream, ImageFormat.Png);
        //    //set memory stream position back to the beginning
        //    ms.Position = 0;
        //    //create bitmap image
        //    bitmap = new BitmapImage();
        //    //prep bitmap for memory stream
        //    bitmap.BeginInit();
        //    //set memory stream
        //    bitmap.StreamSource = ms;
        //    //end stream prep
        //    bitmap.EndInit();

        //    //use bitmap as image source            
        //    imgBarcode.Source = bitmap;
        //}

        private BitmapImage getBitmapImage(Bitmap bmp)
        {            
            //create memory stream for barcode conversion
            MemoryStream ms = new MemoryStream();
            //save barcode image to memory stream as PNG
            bmp.Save(ms, ImageFormat.Png);
            //set memory stream position back to the beginning
            ms.Position = 0;
            //create bitmap image
            BitmapImage bitmapImage = new BitmapImage();
            //prep bitmap for memory stream
            bitmapImage.BeginInit();
            //set memory stream
            bitmapImage.StreamSource = ms;
            //end stream prep
            bitmapImage.EndInit();
            //return BitmapImage
            return bitmapImage;
        }

        private Bitmap getBitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                //BitmapEncoder enc = new BmpBitmapEncoder();
                //enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                //enc.Save(outStream);
                saveAsPng(getImage(getDrawingVisual(bitmapImage)), outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        //private void renderBarcode(string textData, PrintCapabilities printCaps)
        //{
        //    //create barcode image
        //    CodeQrBarcodeDraw barcode = BarcodeDrawFactory.CodeQr;
        //    //draw barcode image
        //    DrawImage image = barcode.Draw(textData, 2);

        //    image = resizeImage(image, new Size(printCaps.PageImageableArea.ExtentWidth, printCaps.PageImageableArea.ExtentHeight));

        //    //create memory stream for barcode conversion
        //    MemoryStream ms = new MemoryStream();
        //    //save barcode image to memory stream as PNG
        //    image.Save(ms, ImageFormat.Png);
        //    //set memory stream position back to the beginning
        //    ms.Position = 0;
        //    //create bitmap image
        //    bitmap = new BitmapImage();
        //    //prep bitmap for memory stream
        //    bitmap.BeginInit();
        //    //set memory stream
        //    bitmap.StreamSource = ms;
        //    //end stream prep
        //    bitmap.EndInit();

        //    ms = new MemoryStream();
        //    saveAsPng(getImage(renderDrawingVisual(bitmap)), ms);
        //    bitmap = bitmapImageFromStream(ms);

        //    //use bitmap as image source            
        //    imgBarcode.Source = bitmap;
        //}

        /// <summary>
        /// Issue for BitmapSource returned and used as ImageSource for Image control.  Clipping occurs.
        /// </summary>
        private RenderTargetBitmap renderBitmapSource(DrawingVisual drawingVisual)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((Int32)Math.Ceiling(drawingVisual.ContentBounds.Width),
                                                            (Int32)Math.Ceiling(drawingVisual.ContentBounds.Height),
                                                            300, 300,
                                                            PixelFormats.Pbgra32);
            
            bmp.Render(drawingVisual);
            return bmp;
        }

        public RenderTargetBitmap getImage(DrawingVisual drawingVisual)
        {
            Size size = new Size(drawingVisual.ContentBounds.Width, drawingVisual.ContentBounds.Height);
            if (size.IsEmpty)
                return null;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            
            result.Render(drawingVisual);

            return result;
        }

        public void saveAsPng(RenderTargetBitmap src, Stream outputStream)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            encoder.Save(outputStream);
        }

        public void saveAsBmp(RenderTargetBitmap src, Stream outputStream)
        {
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            encoder.Save(outputStream);
        }


        private BitmapImage bitmapImageFromStream(Stream stream)
        {
            //create bitmap image
            BitmapImage bmp = new BitmapImage();
            //prep bitmap for memory stream
            bmp.BeginInit();
            //set memory stream
            bmp.StreamSource = stream;
            //end stream prep
            bmp.EndInit();
            //return BitmapImage
            return bmp;
        }

        private DrawingVisual getDrawingVisual(BitmapImage bitmap)
        {            
            var vis = new DrawingVisual();
            var dc = vis.RenderOpen();
            dc.DrawImage(bitmap, new Rect(0, 0, bitmap.Width, bitmap.Height));            
            dc.Close();

            return vis;
        }

        private DrawingVisual renderDrawingVisual(BitmapImage bitmap)
        {
            double fontSize = 18;
            var vis = new DrawingVisual();
            var dc = vis.RenderOpen();
            dc.DrawImage(bitmap, new Rect (0, 0, bitmap.Width, bitmap.Height));
            FormattedText fText = createFormattedText("Name: Taylor, Hunter", fontSize);
            double spacing = fText.Baseline + 5;
            dc.DrawText(fText, new Point(bitmap.Width + 10, 0));
            fText = createFormattedText("DOB: 8/23/2013", fontSize);
            dc.DrawText(fText, new Point(bitmap.Width + 10, spacing));
            fText = createFormattedText("Gender: Male", fontSize);
            dc.DrawText(fText, new Point(bitmap.Width + 10, spacing * 2));
            dc.Close();
            return vis;
        }

        private FormattedText createFormattedText(string text, double emSize)
        {
            FormattedText fText =  new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), emSize, Brushes.Black);
            return fText;
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
            PrintCapabilities printCaps = pDiag.PrintQueue.GetPrintCapabilities(pDiag.PrintTicket);
            if (File.Exists("print-settings.xml"))
            {
                FileStream fs = new FileStream("print-settings.xml", FileMode.Open);
                pDiag.PrintTicket = new PrintTicket(fs);
                fs.Close();
            }

            //renderBarcode("Taylor, Hunter", printCaps);
            DrawingVisual visual = getDrawingVisual(bitmap);
            //bitmap = addBitmapPadding(bitmap);
            //double scale = Math.Min(printCaps.PageImageableArea.ExtentWidth / visual.ContentBounds.Width, printCaps.PageImageableArea.ExtentHeight / visual.ContentBounds.Height);

            //Transform the Visual to scale
            //visual.Transform = new ScaleTransform(0.95, 0.95);

            //---update imgBarcode control
            //MemoryStream ms = new MemoryStream();
            //saveAsPng(renderBitmapSource(visual), ms);
            //bitmap = bitmapImageFromStream(ms);

            //crop rendered image
            //bitmap = getBitmapImage(padBitmap(cropBitmap(getBitmap(bitmap)), 0.05));
            
            var visualSize = new Size(visual.ContentBounds.Width, visual.ContentBounds.Height);
            //var printControl = PrintControlFactory.Create(visualSize, visual);
            //var printControl = PrintControlFactory.Create(imgBarcode);
            //printControl.ShowPrintPreview();
            

            //use bitmap as image source            
            //imgBarcode.Source = bitmap;

            //---save png file
            FileStream fStream = new FileStream("barcode-transformation.png", FileMode.Create);
            saveAsPng(getImage(getDrawingVisual(bitmap)), fStream);
            fStream.Close();

            //pDiag.PrintTicket.PageOrientation = PageOrientation.Landscape;
            //pDiag.PrintTicket.PageMediaSize = new PageMediaSize(2.13 * 96, 4 * 96);
            //pDiag.PrintTicket.PageResolution = new PageResolution(300, 300);

            if(pDiag.ShowDialog() == true)
                pDiag.PrintVisual(visual, description);

            //pDiag.PrintTicket.GetXmlStream();

            fStream = new FileStream("print-settings.xml", FileMode.Create);
            byte[] data = pDiag.PrintTicket.GetXmlStream().ToArray();
            fStream.Write(data, 0, data.Length);
            //pDiag.PrintTicket.SaveTo(fStream);
            fStream.Close();            

            //if (pDiag.ShowDialog() == true)
            //{
                //get selected printer capabilities
                //PrintCapabilities printCaps = pDiag.PrintQueue.GetPrintCapabilities(pDiag.PrintTicket);
                //renderBarcode("Tonitta Sauls", printCaps);

                //DrawingVisual visual = getDrawingVisual(bitmap);
                //bitmap = addBitmapPadding(bitmap);

                //get scale of the print wrt to screen of WPF visual
                //double scale = Math.Min(printCaps.PageImageableArea.ExtentWidth / visual.ContentBounds.Width, printCaps.PageImageableArea.ExtentHeight / visual.ContentBounds.Height);

                //Transform the Visual to scale
                //visual.Transform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                //Size sz = new Size(printCaps.PageImageableArea.ExtentWidth, printCaps.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                //visual.Measure(sz);
                //visual.Arrange(new Rect(new Point(printCaps.PageImageableArea.OriginWidth, printCaps.PageImageableArea.OriginHeight), sz));
                //pDiag.PrintVisual(visual, description);
                //FlowDocument doc = new FlowDocument();
                //pDiag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Flow Document version of imgBarcode");
                //var visualSize = new Size(visual.ContentBounds.Width, visual.ContentBounds.Height);
                //printControl = PrintControlFactory.Create(imgBarcode);
                //printControl.ShowPrintPreview();
            //}
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

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            printBarcode(bitmap, "Tonitta Sauls");
        }

        public FixedDocument createFixedDocument(double docWidthInches, double docHeightInches)
        {
            FixedDocument doc = new FixedDocument();
            Size size = new Size(96 * docWidthInches, 96 * docHeightInches);
            doc.DocumentPaginator.PageSize = size;

            PageContent page = new PageContent();
            FixedPage fixedPage = new FixedPage();
            fixedPage.Background = Brushes.White;
            fixedPage.Width = size.Width;
            fixedPage.Height = size.Height;

            ((IAddChild)page).AddChild(fixedPage);
            doc.Pages.Add(page);

            return doc;
        }

        private void forceRepaint()
        {
            Dispatcher.Invoke(
              DispatcherPriority.Render,
              (Action)delegate()
              { this.ParentLayoutInvalidated(this); });
        }

        private void mnuPatientEditor_Click(object sender, RoutedEventArgs e)
        {
            (new PatientEditorWindow()).ShowDialog();
        }

        private void mnuPreferences_Click(object sender, RoutedEventArgs e)
        {
            (new PreferencesWindow()).ShowDialog();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PreferencesWindow.loadPreferences();

            DBConnection dbCon = new DBConnection
            (
                Preferences.HostAddress,
                Preferences.PortAddress,
                Preferences.DatabaseName,
                Preferences.Username,
                Preferences.Password
            );

            Dispatcher.BeginInvoke(new Action(()=>
            {
            if (dbCon.OpenConnection())
            {
                MessageBox.Show("dbCon.OpenConnection() == true!!!");
                dbCon.CloseConnection();
            }
            else MessageBox.Show("dbCon.OpenConnection() == false!!!");
            }));
        }

    } //End class MainWindow

    public enum ActionMode
    {
        SelectMode, EditMode,
        SelectModeAdmin
    }

    #region Value Converters

    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : BaseConverter, IValueConverter
    {
        public InverseBooleanConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class DateTimeConverter : BaseConverter, IValueConverter
    {
        public DateTimeConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is DateTime) == false)
                throw new InvalidOperationException("The target must be a DateTime");

            DateTime dateTime = (DateTime)value;

            return dateTime.ToString("MM/dd/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class MedicalRecordNumberConverter : BaseConverter, IValueConverter
    {
        public MedicalRecordNumberConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is int) == false)
                throw new InvalidOperationException("The target must be a int");

            return "MR" + (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

#endregion Value Converters

} //End namespace SimPatient