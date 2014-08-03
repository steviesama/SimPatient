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

using MySql.Data;
using SimPatient.DataModel;

using mshtml;

namespace SimPatient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static MainWindow Instance { get; set; }
        public static UserAccount CurrentUser { get; set; }

        private UserControl currentControl = null;

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            //---add control            
            loadBottomGrid(LoginControl.Instance);

            this.Closing += MainWindow_Closing;

        /*End MainWindow()*/}

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Are you sure you wish to exit?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question))
                Application.Current.Shutdown();
            else e.Cancel = true;
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

        private void mnuPatientEditor_Click(object sender, RoutedEventArgs e)
        {
            (new PatientEditorWindow()).ShowDialog();
        }

        private void mnuPreferences_Click(object sender, RoutedEventArgs e)
        {
            PreferencesWindow.Instance.ShowDialog();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //make sure preferences has the main window as its owner
            PreferencesWindow.Instance.Owner = this;

            PreferencesWindow.loadPreferences();

            DBConnection dbCon = new DBConnection
            (
                Preferences.HostAddress,
                Preferences.PortAddress,
                Preferences.DatabaseName,
                Preferences.Username,
                Preferences.Password
            );

            (new SplashScreenWindow()).ShowDialog();
        }

        public void loadBottomGrid(UserControl userControl)
        {
            if (userControl == null) return;

            bottomGrid.Children.Remove(currentControl);
            currentControl = userControl;
            bottomGrid.Children.Add(currentControl);

            if(userControl is SimulationPoolControl)
                mnuEditors.Visibility = Visibility.Visible;
            else if (userControl is LoginControl)
                mnuEditors.Visibility = Visibility.Collapsed;
        }

        private void mnuLogout_Click(object sender, RoutedEventArgs e)
        {
            loadBottomGrid(LoginControl.Instance);
        }

    } //End class MainWindow

    public enum ActionMode
    {
        NewMode, SelectMode, EditMode
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
    public class FirstTimePeriodConverter : BaseConverter, IValueConverter
    {
        public FirstTimePeriodConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is MedicationDose) == false)
                throw new InvalidOperationException("The target must be a MedicationDose");

            MedicationDose dose = value as MedicationDose;
            if (dose.Schedule.ToUpper() == "PRN") return string.Empty;

            DateTime dateTime = dose.TimePeriod;

            TimeSpan beforeElevenPm = new TimeSpan(22, 59, 0);
            TimeSpan elevenPm = new TimeSpan(23, 0, 0);
            TimeSpan beforeSeven = new TimeSpan(6, 59, 0);
            TimeSpan seven = new TimeSpan(7, 0, 0);
            TimeSpan beforeThreePm = new TimeSpan(14, 59, 0);
            TimeSpan threePm = new TimeSpan(15, 0, 0);

            if (dateTime.TimeOfDay.CompareTo(elevenPm) >= 0 || dateTime.TimeOfDay.CompareTo(beforeSeven) <= 0)
                return dateTime.ToString("HHmm");

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class SecondTimePeriodConverter : BaseConverter, IValueConverter
    {
        public SecondTimePeriodConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is MedicationDose) == false)
                throw new InvalidOperationException("The target must be a MedicationDose");

            MedicationDose dose = value as MedicationDose;
            if (dose.Schedule.ToUpper() == "PRN") return string.Empty;

            DateTime dateTime = dose.TimePeriod;

            TimeSpan beforeElevenPm = new TimeSpan(22, 59, 0);
            TimeSpan elevenPm = new TimeSpan(23, 0, 0);
            TimeSpan beforeSeven = new TimeSpan(6, 59, 0);
            TimeSpan seven = new TimeSpan(7, 0, 0);
            TimeSpan beforeThreePm = new TimeSpan(14, 59, 0);
            TimeSpan threePm = new TimeSpan(15, 0, 0);

            if (dateTime.TimeOfDay.CompareTo(seven) >= 0 && dateTime.TimeOfDay.CompareTo(beforeThreePm) <= 0)
                return dateTime.ToString("HHmm");

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class ThirdTimePeriodConverter : BaseConverter, IValueConverter
    {
        public ThirdTimePeriodConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is MedicationDose) == false)
                throw new InvalidOperationException("The target must be a MedicationDose");

            MedicationDose dose = value as MedicationDose;
            if (dose.Schedule.ToUpper() == "PRN") return string.Empty;

            DateTime dateTime = dose.TimePeriod;

            TimeSpan beforeElevenPm = new TimeSpan(22, 59, 0);
            TimeSpan elevenPm = new TimeSpan(23, 0, 0);
            TimeSpan beforeSeven = new TimeSpan(6, 59, 0);
            TimeSpan seven = new TimeSpan(7, 0, 0);
            TimeSpan beforeThreePm = new TimeSpan(14, 59, 0);
            TimeSpan threePm = new TimeSpan(15, 0, 0);

            if (dateTime.TimeOfDay.CompareTo(threePm) >= 0 && dateTime.TimeOfDay.CompareTo(beforeElevenPm) <= 0)
                return dateTime.ToString("HHmm");

            return string.Empty;
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
            if ((value is long) == false)
                throw new InvalidOperationException("The target must be a int");

            return "MR" + (long)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class EditModeBooleanConverter : BaseConverter, IValueConverter
    {
        public EditModeBooleanConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is ActionMode) == false)
                throw new InvalidOperationException("The target must be an ActionMode");

            return (ActionMode)value == ActionMode.EditMode ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


#endregion Value Converters

} //End namespace SimPatient