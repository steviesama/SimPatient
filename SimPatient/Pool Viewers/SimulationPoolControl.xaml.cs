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

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for SimulationPoolControl.xaml
	/// </summary>
	public partial class SimulationPoolControl : UserControl
	{
        public static Simulation selectedSimulation { get; set; }

        private static SimulationPoolControl _simulationPoolControl;
        public static SimulationPoolControl Instance
        {
            get
            {
                if (_simulationPoolControl == null)
                    _simulationPoolControl = new SimulationPoolControl();
                
                Simulation.refreshSimulations();

                return _simulationPoolControl;
            }
        }
        
		private SimulationPoolControl()
		{
			this.InitializeComponent();

            simulationListView.DataContext = Simulation.Simulations;
            Simulation.refreshSimulations();

            ActionMode = ActionMode.EditMode;
		}

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.loadBottomGrid(SimulationEditorControl.EmptyInstance);
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

        private void actionButton_Click(object sender, RoutedEventArgs e)
        {
            selectedSimulation = simulationListView.SelectedItem as Simulation;
            MainWindow.Instance.loadBottomGrid(SimulationEditorControl.Instance);
        }
	/*End class SimulationPoolControl*/}
} //End namespace SimPatient