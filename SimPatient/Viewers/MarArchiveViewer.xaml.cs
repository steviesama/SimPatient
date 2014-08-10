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

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for MarArchiveViewer.xaml
	/// </summary>
	public partial class MarArchiveViewer : Window
	{
		public MarArchiveViewer()
		{
			this.InitializeComponent();
			Owner = MainWindow.Instance;
			
			marViewer.VisualState = "NoInputVisualState";

			adminDatePicker.SelectedDate = DateTime.Today;
			simulationComboBox.DataContext = Simulation.Simulations;
			Simulation.refreshSimulations();
			patientsListBox.DataContext = Patient.PatientPool;
			
			//---clear relevant pools
			Patient.PatientPool.Clear();
			MedicationAdminstrationRecord.Records.Clear();

			//use async invocation to allow the ui thread to update the combo box from the previous actions
			Dispatcher.BeginInvoke(new Action(() => {
				if (simulationComboBox.Items.Count > 0)
					simulationComboBox.SelectedIndex = 0;
			}));
		}

		private void loadButton_Click(object sender, RoutedEventArgs e)
		{
			if (simulationComboBox.SelectedIndex < 0)
			{
				MessageBox.Show("No simulations exist.\nSimulations must be created in order to view the MAR Archive.",
								"Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (adminDatePicker.SelectedDate == null)
			{
				MessageBox.Show("No valid date selected.\nSelect a date in order to view the MAR Archive.",
								"Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			Patient.refreshPatientPoolFromMars((simulationComboBox.SelectedItem as Simulation).Id,
												(DateTime)adminDatePicker.SelectedDate);
		}

		private void patientsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (patientsListBox.SelectedItem == null)
			{
				MedicationAdminstrationRecord.Records.Clear();
				return;
			}

			MedicationAdminstrationRecord.refreshRecords((patientsListBox.SelectedItem as Patient).Id,
														  (DateTime)adminDatePicker.SelectedDate);
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	} //End class MarArchiveViewer
} //End namespace SimPatient