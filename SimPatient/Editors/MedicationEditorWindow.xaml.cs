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

using System.Collections;
using MySqlFunctions = MySql.Data.MySqlClient.MySqlHelper;

using SUT.PrintEngine;
using SUT.PrintEngine.Utils;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for MedicationEditorWindow.xaml
	/// </summary>
	public partial class MedicationEditorWindow : Window
	{
		public MedicationEditorWindow()
		{
			this.InitializeComponent();
			ActionMode = ActionMode.NewMode;
			Owner = MainWindow.Instance;

            //---events
			this.Closing += MedicationEditorWindow_Closing;
            this.IsVisibleChanged += MedicationEditorWindow_IsVisibleChanged;
		}

        private void MedicationEditorWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            medicationNameTextBox.Focus();
        }

		private void MedicationEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		/// <summary>
		/// The UserControl that navigated to this control.
		/// </summary>
		public static UserControl ParentControl { get; set; }

		public static MedicationEditorWindow getInstance(UserControl parentControl)
		{
			MedicationEditorWindow.ParentControl = parentControl;
			return Instance;
		}

		public static MedicationEditorWindow getEmptyInstance(UserControl parentControl)
		{
			MedicationEditorWindow.ParentControl = parentControl;
			emptyControls();
			return Instance;
		}

		private static MedicationEditorWindow _instance;
		public static MedicationEditorWindow Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MedicationEditorWindow();

				_instance.printBarcodeButton.IsEnabled = (_instance.ActionMode == ActionMode.EditMode) ? true : false;

				return _instance;
			}
		}

		public static MedicationEditorWindow EmptyInstance
		{
			get
			{
				Instance.ActionMode = ActionMode.NewMode;
				Instance.resetUserControls(true);
				return Instance;
			}
		}

		private ActionMode _actionMode;
		public ActionMode ActionMode
		{
			get { return _actionMode; }
			set
			{
				_actionMode = value;

				switch (value)
				{
					case ActionMode.SelectMode:
						//actionButton.Content = "Select";
						break;
					case ActionMode.EditMode:
						//actionButton.Content = "Edit";
						break;
				}
			}
		}

		private static void emptyControls()
		{
			Instance.medicationNameTextBox.Text = string.Empty;
			Instance.strengthTextBox.Text = string.Empty;
			Instance.routeComboBox.SelectedIndex = 0;
		}

		public static void fillMedicationInfo(Medication med)
		{
			if (med == null) return;
            Instance.resetUserControls(false);
			Instance.medicationNameTextBox.Text = med.Name;
			Instance.strengthTextBox.Text = med.Strength;
			Instance.routeComboBox.SelectedIndex = med.Route;
			Instance.ActionMode = ActionMode.EditMode;
		}        

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}

		public void resetUserControls(bool emptyText)
		{
			medicationNameTextBox.Background = Brushes.White;
			strengthTextBox.Background = Brushes.White;
			routeComboBox.Background = Brushes.White;
			closeOnSaveCheckBox.IsChecked = true;
			if (emptyText == true) emptyControls();
		}

		public bool isInputValid()
		{
			bool isValid = true;

			if (Util.validateStringTextBox(medicationNameTextBox) == false) isValid = false;
			if (Util.validateStringTextBox(strengthTextBox) == false) isValid = false;

			return isValid;
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if (isInputValid() == false)
			{
				MessageBox.Show("Invalid input entered.\nPlease correct errors marked by red fields.",
								"Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (MySqlHelper.connect() == false) return;

			switch(ActionMode)
			{
				case ActionMode.NewMode:
					mySqlAddNewMedication();
					ActionMode = ActionMode.EditMode;
					break;
				case ActionMode.EditMode:
					mySqlUpdateMedication();
					break;
			}

			if (closeOnSaveCheckBox.IsChecked == true) Hide();
			else printBarcodeButton.IsEnabled = true;
		}

		private long mySqlAddNewMedication()
		{
			DBConnection dbCon = MySqlHelper.dbCon;

			ArrayList response = dbCon.selectQuery(
			string.Format("SELECT add_medication('{0}', '{1}', '{2}', '{3}')",
						  "NULL",
						  MySqlFunctions.EscapeString(medicationNameTextBox.Text),
						  MySqlFunctions.EscapeString(strengthTextBox.Text),
						  routeComboBox.SelectedIndex.ToString()));

			MySqlHelper.disconnect();

			long newId = (long)(response[0] as ArrayList)[0];

			if (newId == 0)
				MessageBox.Show("Medication not saved.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
			//set selected medication and refresh med pool
			else
			{
				MedicationPoolWindow.SelectedMedication = Medication.fromMySqlMedication(newId);
				Medication.refreshMedicationPool();
			}

			return (long)(response[0] as ArrayList)[0];
		}

		private sbyte mySqlUpdateMedication()
		{
			DBConnection dbCon = MySqlHelper.dbCon;

			ArrayList response = dbCon.selectQuery(
			string.Format("SELECT update_medication('{0}', '{1}', '{2}', '{3}')",
						  MedicationPoolWindow.SelectedMedication.Id,
						  MySqlFunctions.EscapeString(medicationNameTextBox.Text),
						  MySqlFunctions.EscapeString(strengthTextBox.Text),
						  routeComboBox.SelectedIndex.ToString()));

			MySqlHelper.disconnect();

			if ((sbyte)(response[0] as ArrayList)[0] == 0)
				MessageBox.Show("Medication not updated.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
			//update selected medication and refresh med pool
			else
			{
				MedicationPoolWindow.SelectedMedication.Name = medicationNameTextBox.Text;
				MedicationPoolWindow.SelectedMedication.Strength = strengthTextBox.Text;
				MedicationPoolWindow.SelectedMedication.Route = routeComboBox.SelectedIndex;
				Medication.refreshMedicationPool();
			}

			return (sbyte)(response[0] as ArrayList)[0];
		}

		private void printBarcodeButton_Click(object sender, RoutedEventArgs e)
		{
			printBarcode();
		}

		private void printBarcode()
		{
			//if (getPatientInfo() == null)
			//{
			//    MessageBox.Show("Invalid input entered.\nPlease correct errors marked by red fields.",
			//                    "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
			//    return;
			//}

			DrawingVisual visual = Util.renderMedicationBarcode(MedicationPoolWindow.SelectedMedication);

			var visualSize = new Size(visual.ContentBounds.Width, visual.ContentBounds.Height);
			var printControl = PrintControlFactory.Create(visualSize, visual);

			printControl.ShowPrintPreview();
		}
	} //End class MedicationEditorWindow
} //End namespace SimPatient