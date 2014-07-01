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

using System.Collections.ObjectModel;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for PatientEditorWindow.xaml
	/// </summary>
	public partial class PatientEditorWindow : Window
	{
        private ObservableCollection<MedicationSchedule> medicationSchedules;

		public PatientEditorWindow()
		{
			this.InitializeComponent();

            medicationSchedules = new ObservableCollection<MedicationSchedule>();
            medicationPoolListView.DataContext = medicationSchedules;

            medicationSchedules.Add(new MedicationSchedule
            {
                Name = "Practi-Hydro",
                Strength = "50mg",
                Route = "PO",
                Schedule = "QHS",
                Start = "08/16/2011",
                Stop = "",
                FirstPeriod = "",
                SecondPeriod = "", 
                ThirdPeriod = "2100"
            });

            medicationSchedules.Add(new MedicationSchedule
            {
                Name = "Practi-Empco",
                Strength = "30mg",
                Route = "PO",
                Schedule = "QD",
                Start = "08/16/2011",
                Stop = "",
                FirstPeriod = "",
                SecondPeriod = "0900",
                ThirdPeriod = ""
            });

		}

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //patientNameTextBox.Text = string.Format("Width: {0} Height: {1}", LayoutRoot.ActualWidth, LayoutRoot.ActualHeight);
        }
	} //End class PatientEditorWindow

    public class MedicationSchedule
    {
        public string Name { get; set; }
        public string Strength { get; set; }
        public string Route { get; set; }
        public string Schedule { get; set; }
        public string Start { get; set; }
        public string Stop { get; set; }
        public string FirstPeriod { get; set; }
        public string SecondPeriod { get; set; }
        public string ThirdPeriod { get; set; }
    }
} //End namespace SimPatient