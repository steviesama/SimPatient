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

			//use async invocation to allow the ui thread to update the combo box from the previous actions
			Dispatcher.BeginInvoke(new Action(() => {
				if (simulationComboBox.Items.Count > 0)
					simulationComboBox.SelectedIndex = 0;
			}));
		}
	} //End class MarArchiveViewer
} //End namespace SimPatient