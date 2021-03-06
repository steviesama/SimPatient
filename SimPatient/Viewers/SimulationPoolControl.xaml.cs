﻿using System;
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

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for SimulationPoolControl.xaml
	/// </summary>
	public partial class SimulationPoolControl : UserControl
	{
		public static Simulation SelectedSimulation { get; set; }

		private static SimulationPoolControl _instance;
		public static SimulationPoolControl Instance
		{
			get
			{
				if (_instance == null)
					_instance = new SimulationPoolControl();
				
				Simulation.refreshSimulations();

				return _instance;
			}
		}
		
		private SimulationPoolControl()
		{
			this.InitializeComponent();

			simulationListView.DataContext = Simulation.Simulations;
			Simulation.refreshSimulations();

			ActionMode = ActionMode.EditMode;
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
						actionButton.Content = "Select";
						break;
					case ActionMode.EditMode:
						actionButton.Content = "Edit";
						break;
				}
			}
		}

		private void newButton_Click(object sender, RoutedEventArgs e)
		{
			//indicates that new button was clicked from other processes
			SelectedSimulation = null;
			UserAccount.UserAccounts.Clear();
			Patient.PatientPool.Clear();
			MainWindow.Instance.loadBottomGrid(SimulationEditorControl.getEmptyInstance(this));
		}


		private void actionButton_Click(object sender, RoutedEventArgs e)
		{
			UserControl userControl = SimulationEditorControl.getInstance(this);
			MainWindow.Instance.loadBottomGrid(userControl);
			UserAccount.refreshUserAccountPool(SelectedSimulation.Id);
			Patient.refreshPatientPool(SimulationPoolControl.SelectedSimulation.Id);
		}

		private void simulationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((sender as ListView).SelectedItem != null)
			{
				SelectedSimulation = (sender as ListView).SelectedItem as Simulation;
				actionButton.IsEnabled = deleteButton.IsEnabled = true;
			}
			else actionButton.IsEnabled = deleteButton.IsEnabled = false;
		}

		private void deleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBoxResult.No == MessageBox.Show("Are you sure you want to delete this simulation?",
													   "Confirmation", MessageBoxButton.YesNo,
													   MessageBoxImage.Warning)) return;
			if (MySqlHelper.connect() == false) return;

			if (MySqlHelper.dbCon.deleteQuery("DELETE FROM tblSimulation WHERE id=" +
											  (simulationListView.SelectedItem as Simulation).Id) == false)
				MessageBox.Show("Delete is not possible.\nSimulation is referenced by other records in the database.",
								"Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else Simulation.refreshSimulations();

			MySqlHelper.disconnect();
		}
	} // End class SimulationPoolControl
} //End namespace SimPatient