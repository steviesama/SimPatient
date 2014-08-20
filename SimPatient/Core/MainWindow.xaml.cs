using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Markup;
using System.Globalization;

using SimPatient.DataModel;

namespace SimPatient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Represents the active instance of the MainWindow.
        /// </summary>
        public static MainWindow Instance { get; set; }

        /// <summary>
        /// When a user logs in, an instance of UserAccount will be
        /// filled with the information of the current user for
        /// static access globally.
        /// </summary>
        public static UserAccount CurrentUser { get; set; }

        /// <summary>The current control that is docked in the Dock.Bottom
        /// position of the dock panel on the MainWindow instance.
        /// </summary>
        private UserControl currentControl = null;

        /// <summary>
        /// Only constructor, which sets the Instance property as well as loads
        /// the bottom dock space with the LoginControl instance.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Instance = this;
            //---add control            
            loadBottomGrid(LoginControl.Instance);

            this.Closing += MainWindow_Closing;
        }

        /// <summary>
        /// MainWindow Closing event handler.  Is triggered when the user attempts to close
        /// the main window with the red x or the system menu. Is also triggered when a window's
        /// Close() function is called.
        /// </summary>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Are you sure you wish to exit?", "Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question))
                Application.Current.Shutdown();
            else e.Cancel = true;
        }

        /// <summary>
        /// mnuPreferences Click event handler.
        /// </summary>
        private void mnuPreferences_Click(object sender, RoutedEventArgs e)
        {
            PreferencesWindow.Instance.ShowDialog();
        }

        /// <summary>
        /// MainWindow Loaded event handler.
        /// </summary>
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

        /// <summary>
        /// This function is used to load a control into the bottom dock space in the
        /// MainWindow dock control.
        /// </summary>
        /// <param name="userControl">The UserControl that should be added to the bottom dock space.</param>
        public void loadBottomGrid(UserControl userControl)
        {
            if (userControl == null) return;
            //whatever the current control is, remove it from the bottom grid.
            bottomGrid.Children.Remove(currentControl);
            //assign a new current control
            currentControl = userControl;
            //add this control to the bottom grid
            bottomGrid.Children.Add(currentControl);
        }

        /// <summary>
        /// mnuLogout Click event handler.
        /// </summary>
        private void mnuLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.mnuMarArchiverViewer.IsEnabled = false;
            loadBottomGrid(LoginControl.Instance);
        }

        private void mnuMarArchiverViewer_Click(object sender, RoutedEventArgs e)
        {
            MarArchiveViewer marArchiveViewer = new MarArchiveViewer();
            marArchiveViewer.ShowDialog();
        }

    } //End class MainWindow

    /// <summary>
    /// Used in conjunction with Editors and Pool controls to determine
    /// what mode they are in in order to dictate execution behavior of
    /// the editor/pool control/window.
    /// </summary>
    public enum ActionMode
    {
        NewMode, SelectMode, EditMode
    }

    /// <summary>
    /// These converters are used by the XAML design side of the application
    /// to convert between non-primitive values for display on a control.
    /// </summary>
    #region Value Converters

    /// <summary>
    /// Base class for value converters so they can be used in XAML as a Markup Extension
    /// which required less code to use.
    /// </summary>
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    /// <summary>
    /// Takes a boolean value and returns it inverse.
    /// </summary>
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

    /// <summary>
    /// Takes a DateTime value as an object and returns a string representation of the date passed.
    /// </summary>
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

    /// <summary>
    /// Takes a MedicationDose as an object and determines what time period value to return, or
    /// nothing if the schedule code is PRN or does not fit within the current time period.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class FirstTimePeriodConverter : BaseConverter, IValueConverter
    {
        public FirstTimePeriodConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is MedicationDose) == false)
                throw new InvalidOperationException("The target must be a MedicationDose");

            return Util.get1stTimePeriod(value as MedicationDose);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Takes a MedicationDose as an object and determines what time period value to return, or
    /// nothing if the schedule code is PRN or does not fit within the current time period.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class SecondTimePeriodConverter : BaseConverter, IValueConverter
    {
        public SecondTimePeriodConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is MedicationDose) == false)
                throw new InvalidOperationException("The target must be a MedicationDose");

            return Util.get2ndTimePeriod(value as MedicationDose);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Takes a MedicationDose as an object and determines what time period value to return, or
    /// nothing if the schedule code is PRN or does not fit within the current time period.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class ThirdTimePeriodConverter : BaseConverter, IValueConverter
    {
        public ThirdTimePeriodConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is MedicationDose) == false)
                throw new InvalidOperationException("The target must be a MedicationDose");

            return Util.get3rdTimePeriod(value as MedicationDose);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Takes a long value and returns the proper patient medical record number.
    /// </summary>
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

    /// <summary>
    /// Takes an ActionMode as an object then returns a boolean value indicating whether
    /// or not the passed ActionMode was set to ActionMode.EditMode or not.
    /// </summary>
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

    /// <summary>
    /// Takes a DateTime as an object and returns a HHmm time format parsed from the hours and minutes
    /// components of the passed DateTime object. The check for dateTime == null should no long be
    /// applicable as I believe all possible nullable DateTimes were never used.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class AdministrationTimeConverter : BaseConverter, IValueConverter
    {
        public AdministrationTimeConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is DateTime) == false)
                throw new InvalidOperationException("The target must be a DateTime");

            DateTime dateTime = (DateTime)value;

            return dateTime == null ? "Refused" : dateTime.ToString("HHmm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Takes an int value as an object which represents a ReasonCode.  If the reason code is 0,
    /// then "Yes" is returned indicating that the medication was administered on time; if the reason
    /// code is non-zero, "No" is returned indicating that the medication was not administered on time.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class ReasonCodeConverter : BaseConverter, IValueConverter
    {
        public ReasonCodeConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is int) == false)
                throw new InvalidOperationException("The target must be an int");

            int reason = (int)value;
            
            return  reason == 0 ? "Yes" : "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    #endregion Value Converters
} //End namespace SimPatient