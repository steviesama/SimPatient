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
	/// Interaction logic for MedicationOffScheduleWindow.xaml
	/// </summary>
	public partial class MedicationOffScheduleWindow : Window
	{
		private static MedicationAdminstrationRecord Mar { get; set; }

		public MedicationOffScheduleWindow()
		{
			this.InitializeComponent();
		}

		public static void getReason(MedicationAdminstrationRecord Mar)
		{
			MedicationOffScheduleWindow offSchedule = new MedicationOffScheduleWindow();
			MedicationOffScheduleWindow.Mar = Mar;
			offSchedule.ShowDialog();
		}

        private bool isInputValid()
        {
            bool isValid = true;

            if (optPatientNa.IsChecked == true)
                Mar.ReasonCode = 1;
            else if (optNurseNa.IsChecked == true)
                Mar.ReasonCode = 2;
            else if (optMedicationNa.IsChecked == true)
                Mar.ReasonCode = 3;
            else if (optPatientRefused.IsChecked == true)
                Mar.ReasonCode = 4;
            else if (optOther.IsChecked == true)
                Mar.ReasonCode = 5;
            else
            {
                MessageBox.Show("You must selected a Reason option.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isValid = false;
            }

            if (Mar.ReasonCode == 5)
            {
                if (Util.validateStringTextBox(notesTextBox) == false)
                {
                    MessageBox.Show("Notes are required for Reason \"Other\".", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    isValid = false;
                }
            }
            else notesTextBox.Background = Brushes.White;

            return isValid;
        }

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
            if (isInputValid() == false) return;

            Mar.ReasonNotes = notesTextBox.Text;
            Close();
		}
	} //End class MedicationOffScheduleWindow
}// End namespace SimPatient