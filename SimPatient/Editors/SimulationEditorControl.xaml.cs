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

using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections;

using MySqlFunctions = MySql.Data.MySqlClient.MySqlHelper;
using PatientGender = SimPatient.Patient.PatientGender;
using System.IO;
using System.Diagnostics;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for SimulationEditorControl.xaml
	/// </summary>
	public partial class SimulationEditorControl : UserControl
	{
		/// <summary>
		/// The UserControl that navigated to this control.
		/// </summary>
		public static UserControl ParentControl { get; set; }

		public static SimulationEditorControl getInstance(UserControl parentControl)
		{
			SimulationEditorControl.ParentControl = parentControl;
			return Instance;
		}

		public static SimulationEditorControl getEmptyInstance(UserControl parentControl)
		{
			SimulationEditorControl.ParentControl = parentControl;
			return EmptyInstance;
		}

		private static SimulationEditorControl _instance;
		public static SimulationEditorControl Instance
		{
			get
			{
				if (_instance == null)
					_instance = new SimulationEditorControl();

				_instance.simulationNameTextBox.Background = Brushes.White;

				_instance.Dispatcher.BeginInvoke(new Action<UIElement>(x =>
				{
					if (SimulationPoolControl.Instance.ActionMode == ActionMode.EditMode)
					{
						Simulation sim = SimulationPoolControl.SelectedSimulation;
						if (sim != null)
						{
							_instance.simulationNameTextBox.Text = sim.Name;
							_instance.simulationDescriptionTextBox.Text = sim.Description;
						}
					}

					_instance.simulationNameTextBox.Focus();
				}), DispatcherPriority.ApplicationIdle, _instance);

				if (SimulationPoolControl.SelectedSimulation == null)
				{
					_instance.addAccountButton.IsEnabled = false;
					_instance.addPatientButton.IsEnabled = false;
				}
				else
				{
					_instance.addAccountButton.IsEnabled = true;
					_instance.addPatientButton.IsEnabled = true;
				}

				return _instance;
			}
		}

		public static SimulationEditorControl EmptyInstance
		{
			get
			{
				emptyControls();   
				return Instance;
			}
		}

		private SimulationEditorControl()
		{
			this.InitializeComponent();

			accountPoolListView.DataContext = UserAccount.UserAccounts;
			UserAccount.refreshUserAccountPool(SimulationPoolControl.SelectedSimulation == null ? 0 : SimulationPoolControl.SelectedSimulation.Id);
						
			patientPoolListView.DataContext = Patient.PatientPool;
			if(SimulationPoolControl.SelectedSimulation != null)
				Patient.refreshPatientPool(SimulationPoolControl.SelectedSimulation.Id);
		}

		//don't use this in the Instance property
		internal static void emptyControls()
		{
			Instance.simulationNameTextBox.Text = "";
			Instance.simulationDescriptionTextBox.Text = "";

		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.Instance.loadBottomGrid(ParentControl);
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if(Util.validateStringTextBox(simulationNameTextBox) == false)
			{
				MessageBox.Show("No valid simulation name entered.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
				simulationNameTextBox.Focus();
				return;
			}

			if (MySqlHelper.connect() == false) return;

			if (SimulationPoolControl.SelectedSimulation == null)
				mySqlAddNewSimulation();
			else mySqlUpdateSimulation();

			MainWindow.Instance.loadBottomGrid(ParentControl);
		}

		private void mySqlAddNewSimulation()
		{
			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(string.Format("SELECT add_simulation(NULL, '{0}', '{1}', NULL, 0, '{2}')",
																 MySqlFunctions.EscapeString(simulationNameTextBox.Text),
																 MySqlFunctions.EscapeString(simulationDescriptionTextBox.Text),
																 MySqlFunctions.EscapeString(MainWindow.CurrentUser.Username)));

			MySqlHelper.disconnect();

			if ((long)(response[0] as ArrayList)[0] == 0)
				MessageBox.Show("Simulation not saved.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else Simulation.refreshSimulations();
		}

		private void mySqlUpdateSimulation()
		{
			DBConnection dbCon = MySqlHelper.dbCon;
			
			Simulation sim = SimulationPoolControl.SelectedSimulation;
			if (sim == null) return;

			ArrayList response = dbCon.selectQuery(string.Format("SELECT update_simulation({0}, '{1}', '{2}', '{3}', {4}, '{5}')",
																 sim.Id,
																 MySqlFunctions.EscapeString(simulationNameTextBox.Text),
																 MySqlFunctions.EscapeString(simulationDescriptionTextBox.Text),
																 sim.CreationDate.ToString("yyyy-MM-dd HH:mm:ss"),
																 sim.IsDeleted ? 1 : 0,
																 MySqlFunctions.EscapeString(sim.Creator)));

			MySqlHelper.disconnect();

			if ((sbyte)(response[0] as ArrayList)[0] == 0)
				MessageBox.Show("Simulation not Update.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else Simulation.refreshSimulations();
		}

		private void addPatientButton_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.Instance.loadBottomGrid(PatientPoolControl.getInstance(this, "AdminVisualState"));
			Patient.refreshPatients();
		}

		private void addAccountButton_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.Instance.loadBottomGrid(AccountPoolControl.getInstance(this));
		}

		private void removeAccountButton_Click(object sender, RoutedEventArgs e)
		{
			MySqlHelper.removeUserAccountFromSimulation((accountPoolListView.SelectedItem as UserAccount).Id);
			UserAccount.refreshUserAccountPool(SimulationPoolControl.SelectedSimulation.Id);
		}

		private void accountPoolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			removeAccountButton.IsEnabled = accountPoolListView.SelectedItem == null ? false : true;
		}

		private void removePatientButton_Click(object sender, RoutedEventArgs e)
		{
			MySqlHelper.removePatientFromSimulation((patientPoolListView.SelectedItem as Patient).Id);
			Patient.refreshPatientPool(SimulationPoolControl.SelectedSimulation.Id);
		}

		private void patientPoolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			removePatientButton.IsEnabled = generateMarButton.IsEnabled = patientPoolListView.SelectedItem == null ? false : true;
		}
		
		private void generateMarButton_Click(object sender, RoutedEventArgs e)
		{
			StreamWriter sWriter = new StreamWriter("mar.html");
			if (sWriter == null)
			{
				MessageBox.Show("mar.html could not be created.", "Write Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			Patient pat = patientPoolListView.SelectedItem as Patient;
			MedicationDose.refreshMedicationDosePool(pat.Id);
			ObservableCollection<MedicationDose> doses = MedicationDose.MedicationDosePool;

			string style = @"<style>body {margin: auto;font-size: 10pt;font-family: Arial, Verdana, sans-serif;}" +
							".border {border: 1px solid black; border-collapse: collapse; border-spacing: 0;} .border td {border: 1px solid black;}" +
							"table {width: 100%;table-layout: fixed;border-collapse: collapse; border-spacing: 0;} bottom-border {border: 0;} .bottom-border td {border-bottom: 1px solid black;}</style>";
			sWriter.WriteLine(@"<!doctype html><html lang='en'><head><title>MAR Sheet</title><meta charset='utf-8'>" + style + "</head><body>");
			sWriter.WriteLine(@"<table>");
			sWriter.WriteLine(string.Format("<tr><td colspan='2'>&nbsp;&nbsp;{0}</td><td colspan='7'>MEDICATION ADMINISTRATION RECORD</td></tr>",
							  DateTime.Today.ToString("MM/dd/yyyy")));
			sWriter.WriteLine("<tr><td colspan='2'>Time Verified 2300</td><td colspan='7'>Gulf Coast State College Nursing Hospital</td></tr><tr><td colspan='9'>&nbsp;<br>&nbsp;</td></tr>");
			sWriter.WriteLine("<tr><td colspan='9'>Checked by:__________________________________</td></tr>");
			sWriter.WriteLine(string.Format("<tr><td>Diagnosis:</td><td colspan='2'>{0}</td></tr>", pat.Diagnosis));
			sWriter.WriteLine(string.Format("<tr><td>Allergies:</td><td colspan='2'>{0}</td><td>Diet:</td><td colspan='2'>{1}</td>" +
							  "<td colspan='3'><b>{2}</b></td></tr>", pat.Allergies, pat.Diet, pat.Name));
			sWriter.WriteLine(string.Format("<tr><td colspan='6'>&nbsp;</td><td colspan='3'>Rm {0}</td></tr>", pat.RoomNumber));
			sWriter.WriteLine(string.Format("<tr><td colspan='3'>Notes:</td><td colspan='3'>Admission Date: {0}</td>" +
							  "<td colspan='3'>{1}</td></tr>", pat.AdmissionDate.ToString("MM/dd/yyyy"), pat.DrName));
			sWriter.WriteLine(string.Format("<tr class='bottom-border'><td colspan='3'>&nbsp;</td><td colspan='2'>Wt: {0} lbs.</td>" +
							  "<td colspan='1'>Age: 34</td><td colspan='1'>Gender: {1}</td><td colspan='2'>DOB: {2}</td></tr>",
							  pat.Weight, pat.Gender == PatientGender.Male ? 'M' : 'F', pat.DateOfBirth.ToString("MM/dd/yyyy")));
			sWriter.WriteLine(string.Format("<tr><td colspan='4' style='text-align: right;'>Administration Period</td><td>&nbsp;</td>" +
							  "<td colspan='4'>2300 {0} to 2259 {1}</td></tr></table>", DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("MM/dd/yyyy"),
							  DateTime.Today.ToString("MM/dd/yyyy")));
			//---
			sWriter.WriteLine("<table class='border'>");
			sWriter.WriteLine("<tr class='border'><td style='text-align: left;' colspan='4'>Drug Name, Strength, Route, Schedule</td>" +
							  "<td style='text-align: center;'>Start</td><td style='text-align: center;'>Stop</td><td style='text-align: center;'>2300-0659</td>" +
							  "<td style='text-align: center;'>0700-1459</td><td style='text-align: center;'>1500-2259</td></tr>");

			//preformatted text for medication doses
			string preformatted = "<tr class='border'><td colspan='4'>{0}, {1}, {2}, {3}<br>&nbsp;</td>" +
							  "<td>{4}<br>&nbsp;</td><td>&nbsp;</td><td>{5}<br>&nbsp;</td><td>{6}<br>&nbsp;</td><td>{7}<br>&nbsp;</td></tr>";
			//write the doses out
			foreach (MedicationDose d in doses)
				sWriter.WriteLine(string.Format(preformatted, d.ForMedication.Name, d.ForMedication.Strength,
								  Medication.Routes[d.ForMedication.Route], d.Schedule, d.StartTime.ToString("MM/dd/yyyy"),
								  Util.get1stTimePeriod(d), Util.get2ndTimePeriod(d), Util.get3rdTimePeriod(d)));

			sWriter.WriteLine("</table>");
			//---
			sWriter.WriteLine("<table>");
			sWriter.WriteLine("<tr><td colspan='3'>INJECTION CODE:</td><td colspan='4'>DOCUMENT EACH INJ. SITE</td>" +
							  "<td colspan='2'>WITH A CODE LETTER</td></tr>");
			sWriter.WriteLine("<tr><td colspan='2'>A.= L.U.Q.</td><td colspan='2'>B.= R.U.Q.</td><td colspan='2'>C.= L. Thigh</td>" +
							  "<td colspan='2'>D.= R. Thigh</td><td>E.= L. Arm</td></tr>");
			sWriter.WriteLine("<tr><td colspan='2'>F.= R. Arm</td><td colspan='2'>G.= L. ABD.</td><td colspan='2'>H.= R. ABD.</td>" +
							  "<td colspan='2'>I.= RVG</td><td>J.= LVG</td></tr>");
			sWriter.WriteLine(@"</table>");
			//---
			sWriter.WriteLine("<table class='border' style='border-style: double;'>");
			sWriter.WriteLine("<tr><td>Initials</td><td colspan='2'>Signature</td><td>Initials</td>" +
							  "<td colspan='2'>Signature</td><td>Initials</td><td colspan='2'>Signature</td></tr>");
			sWriter.WriteLine("<tr><td>&nbsp;</td><td colspan='2'>&nbsp;</td><td>&nbsp;</td>" +
							  "<td colspan='2'>&nbsp;</td><td>&nbsp;</td><td colspan='2'>&nbsp;</td></tr>");
			sWriter.WriteLine("<tr><td>Initials</td><td colspan='2'>Signature</td><td>Initials</td>" +
							  "<td colspan='2'>Signature</td><td>Initials</td><td colspan='2'>Signature</td></tr>");
			sWriter.WriteLine("<tr><td>&nbsp;</td><td colspan='2'>&nbsp;</td><td>&nbsp;</td>" +
							  "<td colspan='2'>&nbsp;</td><td>&nbsp;</td><td colspan='2'>&nbsp;</td></tr>");
			sWriter.WriteLine("<tr><td>Initials</td><td colspan='2'>Signature</td><td>Initials</td>" +
							  "<td colspan='2'>Signature</td><td>Initials</td><td colspan='2'>Signature</td></tr>");
			sWriter.WriteLine(@"</table>");
			//---
			sWriter.WriteLine(@"<table>");
			sWriter.WriteLine("<tr><td>&nbsp;</td><td colspan='2'>&nbsp;</td><td>&nbsp;</td>" +
							  "<td colspan='2'>&nbsp;</td><td>&nbsp;</td><td colspan='2'>&nbsp;</td></tr>");
			sWriter.WriteLine(string.Format("<table><tr><td colspan='6'>&nbsp;<br>&nbsp;<br>Room: {0}</td>" +
							  "<td colspan='3'>&nbsp;<br>&nbsp;<br>Physician: {1}</td></tr>", pat.RoomNumber, pat.DrName));
			sWriter.WriteLine(@"</table>");
			sWriter.WriteLine(@"</body></html>");

			sWriter.Close();

			//launch in default browser
			Process.Start("mar.html");
		} //End generateMarButton_Click()
	} //End class SimulationEditorControl
} //End namespace SimPatient