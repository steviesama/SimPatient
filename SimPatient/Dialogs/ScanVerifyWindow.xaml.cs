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

using System.Windows.Threading;
using System.Threading;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for ScanVerifyWindow.xaml
	/// </summary>
	public partial class ScanVerifyWindow : Window
	{
		private Patient patient;
		private Medication medication;
		private DispatcherTimer timer;
		private bool scanMatches;

		private ScanVerifyWindow()
		{
			this.InitializeComponent();
			Owner = MainWindow.Instance;
			patient = null;
			medication = null;
			timer = new DispatcherTimer();
			timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			timer.Tick += readScanInput;

			scanTextBox.TextChanged += scanTextBox_TextChanged;
		}

		private void readScanInput(object sender, EventArgs e)
		{
			timer.Stop();
			timer.Tick += closeScanVerifyWindow;
			scanTextBox.IsEnabled = false;
			
			long id = 0;
			int index = 0;

			if (patient != null)
			{
				id = patient.Id;
				index = 2;
			}
			else if (medication != null)
			{
				id = medication.Id;
				index = 3;
			}

			if (scanTextBox.Text.Trim().Length >= 3 &&
				id.ToString() == scanTextBox.Text.Trim().Substring(index))
			{
				scanMatches = true;
				statusTextBlock.Text = "Verified!";
				statusTextBlock.Style = FindResource("GreenTextBlockStyle") as Style;
			}
			else
			{
				scanMatches = false;
				statusTextBlock.Text = "Not Verified!";
				statusTextBlock.Style = FindResource("RedTextBlockStyle") as Style;
			}

			timer.Start();
		}

		private void closeScanVerifyWindow(object sender, EventArgs e)
		{
			timer.Stop();
			Close();
		}

		private void scanTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if(timer.IsEnabled == false)
				timer.Start();
		}        

		public static bool verifyPatient(Patient patient)
		{           
			ScanVerifyWindow scan = new ScanVerifyWindow();
			scan.patient = patient;
			scan.ShowDialog();
			return scan.scanMatches;
		}

		public static bool verifyMedication(Medication medication)
		{
			ScanVerifyWindow scan = new ScanVerifyWindow();
			scan.medication = medication;
			scan.ShowDialog();
			return scan.scanMatches;
		}
	} //End class ScanVerifyWindow
}// End namespace SimPatient