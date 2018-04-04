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
using System.Data;
using Seps.Infomatic.Core;

namespace Seps.Infomatic.Gui
{
    /// <summary>
    /// Логика взаимодействия для MainTemplateView.xaml
    /// </summary>
    public partial class MainTemplateView : UserControl
    {

        private GridLength sizeExpand;
        public MainTemplateView()
        {
            InitializeComponent();
        }

        private void Expander1_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sizeExpand != null) && (sizeExpand.IsAuto != true))
                Grid2.ColumnDefinitions[2].Width = sizeExpand;
            else
            {
                Grid2.ColumnDefinitions[0].Width = new GridLength(0.5, GridUnitType.Star);
                Grid2.ColumnDefinitions[2].Width = new GridLength(0.5, GridUnitType.Star);
            }
            Grid2.ColumnDefinitions[2].MinWidth = 300;
            GrSp1.IsEnabled = true;
        }

        private void Expander1_Collapsed(object sender, RoutedEventArgs e)
        {
            sizeExpand = Grid2.ColumnDefinitions[2].Width;
            Grid2.ColumnDefinitions[2].Width = GridLength.Auto;
            GrSp1.IsEnabled = false;
            Grid2.ColumnDefinitions[2].MinWidth = 24;
        }

        private void FunctionTechView_RoutedSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((CustomEventArgs)e).IdRow == null)
            {
                ((TemplateFunctionVM)this.Resources["TemplateFunctionVM"]).IdFunction = null;
            }
            else
            {
                if ((int)((CustomEventArgs)e).IdRow[0] < 0)
                {
                    ((TemplateFunctionVM)this.Resources["TemplateFunctionVM"]).IdFunction = null;
                    MessageBox.Show("Для открытия шаблона данной функции необходимо \n"+
                        "предварительно обновить таблицу технологических функций", "Справка");               
                }
                else ((TemplateFunctionVM)this.Resources["TemplateFunctionVM"]).IdFunction = (int)((CustomEventArgs)e).IdRow[0];
            }
        }


        // автоматический выбор сигнала в списке всех сигналов при выборе его в шаблоне
        private void TemplateFunctionView_RoutedSelectionChanged(object sender, RoutedEventArgs e)
        {
            int    i = 0;
            bool   flag = true;
            while (flag == true)
            {
                this.AllsignalVM.TableAllSignal.SelectedIndex = i;
                if (this.AllsignalVM.TableAllSignal.SelectedItem.ToString() == "{NewItemPlaceholder}")
                {
                    flag = false;
                    continue;
                }
                DataRowView dRowView2 = (DataRowView)this.AllsignalVM.TableAllSignal.SelectedItem;
                DataRow dRow2 = (DataRow)dRowView2.Row;

                if ((int)Converters.Try(dRow2["idallsignal"]) == (int)((CustomEventArgs)e).IdRow[2])
                {
                    int indx = this.AllsignalVM.TableAllSignal.Items.IndexOf(dRowView2);
                    this.AllsignalVM.TableAllSignal.SelectedIndex = this.AllsignalVM.TableAllSignal.Items.IndexOf(dRowView2);
                    flag = false;
                    this.AllsignalVM.TableAllSignal.ScrollIntoView(this.AllsignalVM.TableAllSignal.SelectedItem as object);
                }
                i++;
            }
        }
    }
}
