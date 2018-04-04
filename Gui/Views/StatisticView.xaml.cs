using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Controls.DataVisualization.Charting;
using System.Data;

namespace Seps.Infomatic.Gui
{
    /// <summary>
    /// Логика взаимодействия для UC08_Statistic.xaml
    /// </summary>
    public partial class StatisticView : UserControl
    {
        private StatisticVM stVM;
        public StatisticView()
        {
            InitializeComponent();
            stVM = (StatisticVM)this.TryFindResource("StatisticVM");
            // подключение 
            if (stVM != null)
            {
                // подключение команд ниже возможно только после завершения инициализации VM
                stVM.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((o2, e2) => 
                {
                    if (e2.PropertyName == "IsInitialized" && ((StatisticVM)o2).IsInitialized) 
                    {
                        stVM.ConStatSignal.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((o, e)
                            => { StatPSeries.ItemsSource = new DataView(stVM.ConStatSignal.DataTableSignal); });
                        stVM.ConStatTerminal.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((o, e)
                            => { TerminalPSeries.ItemsSource = new DataView(stVM.ConStatTerminal.DataTableSignal); });
                        stVM.ConStatTom.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((o, e)
                            => { TomPSeries.ItemsSource = new DataView(stVM.ConStatTom.DataTableSignal); });
                        stVM.ConStatProtocol.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((o, e)
                            => { ProtocolPSeris.ItemsSource = new DataView(stVM.ConStatProtocol.DataTableSignal); });
                        stVM.ConStatAllSystems.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((o, e)
                            => { AllSystemsPSeris.ItemsSource = new DataView(stVM.ConStatAllSystems.DataTableSignal); });
                    }
                });
            }
        }
    }
}
