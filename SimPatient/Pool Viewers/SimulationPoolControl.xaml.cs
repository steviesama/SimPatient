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
        public ObservableCollection<Simulation> simulations;
        
		public SimulationPoolControl()
		{
			this.InitializeComponent();

            simulations = new ObservableCollection<Simulation>();
            simulationListView.DataContext = simulations;

            simulations.Add(new Simulation { Name = "Simulation #1", Creator = "Watson, Josh", Notes = "Josh made this!!!" });
            simulations.Add(new Simulation { Name = "Simulation #2", Creator = "Taylor, Casi", Notes = "Casi made this!!!" });
		}

        private ActionMode _actionMode;
        public ActionMode ActionMode 
        {
            get { return _actionMode; } 
            set 
            {
                _actionMode = value;

                switch(value)
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
	/*End class SimulationPoolControl*/}

    public class Simulation
    {
        public string Name { get; set; }
        public string Creator { get; set; }
        public string Notes { get; set; }
    }

} //End namespace SimPatient