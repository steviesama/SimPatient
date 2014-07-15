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

        private static PatientPoolControl _patientPoolControl;
        public static PatientPoolControl Instance
        {
            get
            {
                if (_patientPoolControl == null)
                    _patientPoolControl = new PatientPoolControl();

                return _patientPoolControl;
            }
        }

        private PatientPoolControl()
        {
            InitializeComponent();

            patients = new ObservableCollection<Patient>();
            patientPoolListView.DataContext = patients;

            patients.Add(new Patient { Name = "Twitter, Dora", DateOfBirth = new DateTime(1950, 1, 9), DrName = "Harko", Id = 789987 });
            patients.Add(new Patient { Name = "Watson, Josh", DateOfBirth = new DateTime(1986, 12, 30), DrName = "Ioda", Id = 123456 });
            patients.Add(new Patient { Name = "Taylor, Casi", DateOfBirth = new DateTime(1986, 6, 13), DrName = "Jeffreys", Id = 654321 });
        }
    }
} //End namespace SimPatient