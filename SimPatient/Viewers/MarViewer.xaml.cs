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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for MarViewer.xaml
	/// </summary>
	public partial class MarViewer : UserControl
	{
		public static MedicationAdminstrationRecord SelectedMar { get; set; }
		public MarViewer()
		{
			this.InitializeComponent();
			this.DataContext = MedicationAdminstrationRecord.Records; 
		}

		private string _visualState;
		public string VisualState
		{
			get { return _visualState; }
			set
			{
				VisualStateManager.GoToState(this, value, false);
				_visualState = value;
			}
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.Instance.loadBottomGrid(PatientPoolControl.Instance);
		}

		private void selectButton_Click(object sender, RoutedEventArgs e)
		{
		}

		private void medicationPoolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((sender as ListView).SelectedItem != null)
			{
				SelectedMar = (sender as ListView).SelectedItem as MedicationAdminstrationRecord;
				selectButton.IsEnabled = true;
				reasonTextBox.Text = MedicationAdminstrationRecord.Reasons[SelectedMar.ReasonCode];
				reasonNotesTextBox.Text = SelectedMar.ReasonNotes;
				adminNotesTextBox.Text = SelectedMar.AdministrationNotes;

			}
			else
			{
				selectButton.IsEnabled = false;
				reasonTextBox.Text = reasonNotesTextBox.Text = adminNotesTextBox.Text = string.Empty;
			}
		}
	} //End class MarViewer
} //End namespace SimPatient