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

using System.Collections.ObjectModel;

namespace SimPatient
{
    /// <summary>
    /// Interaction logic for PatientPoolControl.xaml
    /// </summary>
    public partial class PatientPoolControl : UserControl
    {
        ObservableCollection<Patient> patients;

        public PatientPoolControl()
        {
            InitializeComponent();

            patients = new ObservableCollection<Patient>();
            patientPoolListView.DataContext = patients;

            patients.Add(new Patient { Name = "Twitter, Dora", DOBString = "01/09/1950", Physician = "Harko", Id = "MR789987" });
            patients.Add(new Patient { Name = "Watson, Josh", DOBString = "12/30/1986", Physician = "Ioda", Id = "MR123456" });
            patients.Add(new Patient { Name = "Taylor, Casi", DOBString = "06/13/1986", Physician = "Jeffreys", Id = "MR654321" });
        }
    }

    public class Patient
    {
        public string Name { get; set; }
        public string DOBString { get; set; }
        public string Physician { get; set; }
        public string Id { get; set; }
    }
} //End namespace SimPatient