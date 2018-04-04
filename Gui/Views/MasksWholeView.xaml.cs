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
using Seps.Infomatic.MySql;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using Seps.Infomatic.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace Seps.Infomatic.Gui
{
    /// <summary>
    /// Логика взаимодействия для MasksWholeView.xaml
    /// </summary>
    public partial class MasksWholeView : UserControl
    {
        private GridLength sizeExpand1;
        private GridLength sizeExpand2;

        public MasksWholeView()
        {
            InitializeComponent();
        }

        private void Expander1_Collapsed(object sender, RoutedEventArgs e)
        {
            sizeExpand1 = Grid1.ColumnDefinitions[0].Width;
            //Grid1.ColumnDefinitions[0].Width = GridLength.Auto;
            //Grid1.ColumnDefinitions[0].Width = new GridLength(24, GridUnitType.Pixel);
            Grid1.ColumnDefinitions[0].MinWidth = 24;
            Grid1.ColumnDefinitions[0].MaxWidth = 24;
        }

        private void Expander1_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sizeExpand1 != null) && (sizeExpand1.IsAuto != true)) Grid1.ColumnDefinitions[0].Width = sizeExpand1;
            else Grid1.ColumnDefinitions[0].Width = new GridLength(300, GridUnitType.Pixel);
            Grid1.ColumnDefinitions[0].MinWidth = 200;
            Grid1.ColumnDefinitions[0].MaxWidth = 500;
        }

        private void Expander2_Collapsed(object sender, RoutedEventArgs e)
        {
            sizeExpand2 = Grid1.ColumnDefinitions[2].Width;
            //Grid1.ColumnDefinitions[2].Width = GridLength.Auto;
            //Grid1.ColumnDefinitions[2].Width = new GridLength(24, GridUnitType.Pixel);
            Grid1.ColumnDefinitions[2].MinWidth = 24;
            Grid1.ColumnDefinitions[2].MaxWidth = 24;

        }

        private void Expander2_Expanded(object sender, RoutedEventArgs e)
        {
            if (sizeExpand2 != null) Grid1.ColumnDefinitions[2].Width = sizeExpand2;
            else
            {
                Grid1.ColumnDefinitions[2].Width = new GridLength(0.4, GridUnitType.Star);
                Grid1.ColumnDefinitions[4].Width = new GridLength(0.6, GridUnitType.Star);
            }
            Grid1.ColumnDefinitions[2].MinWidth = 300;
            Grid1.ColumnDefinitions[2].MaxWidth = 500;
            Grid1.ColumnDefinitions[4].MinWidth = 300;
        }

        // изменение DataContext таблицы масок и таблицы копирования масок.
        private void htree_RoutedSelectionChangedTree(object sender, RoutedEventArgs e)
        {
            DataRowView dr = ((CustomEventArgs)e).IdRow;
            if (dr.Row.Table.TableName == "unionJoinFunction")
            {
                this.MaskTable.DataContext = dr;
                //DataContext здесь используется просто как DependencyProperty, внутри view
                // Datacontext другой и он не меняется.
                this.MaskTableCopy.DataContext = dr;
            }
        }

        // подписка на событие SelectionChanged в дереве
        private void htree_FullRoutedSelectionChangedTree(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView dr_new = ((DataRowView)((RoutedPropertyChangedEventArgs<object>)(e.OriginalSource)).NewValue);
                DataRowView dr_old = ((DataRowView)((RoutedPropertyChangedEventArgs<object>)(e.OriginalSource)).OldValue);
                if (dr_old != null && dr_old.Row.Table.TableName == "unionJoinFunction")
                {
                    if (dr_old.Row.RowState == DataRowState.Deleted || dr_old.Row.RowState == DataRowState.Detached)
                    {
                        this.MaskTable.DataContext = null;
                        //DataContext здесь используется просто как DependencyProperty, внутри view
                        // Datacontext другой и он не меняется.
                        this.MaskTableCopy.DataContext = null;
                    }
                    else
                        if (dr_new != null && dr_new.Row.Table.TableName == "unionJoinFunction")
                        {
                            this.MaskTable.DataContext = dr_new;
                            //DataContext здесь используется просто как DependencyProperty, внутри view
                            // Datacontext другой и он не меняется.
                            this.MaskTableCopy.DataContext = dr_new;
                        }
                }
                else
                {
                    if (dr_new != null && dr_new.Row.Table.TableName == "unionJoinFunction")
                    {
                        this.MaskTable.DataContext = dr_new;
                        //DataContext здесь используется просто как DependencyProperty, внутри view
                        // Datacontext другой и он не меняется.
                        this.MaskTableCopy.DataContext = dr_new;
                    }
                }

            }
            catch { }

        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    ((TableConnector)((ObjectDataProvider)FindResource("equipment")).ObjectInstance).Refresh();
        //    ((TableConnector)((ObjectDataProvider)FindResource("functiontech")).ObjectInstance).Refresh();
        //}


        // копирование масок функции.
        //private void CopyFunTech_Click(object sender, RoutedEventArgs e)
        //{
        //    MySqlConnection con = new MySqlConnection(DbParameters.Default.GetMySqlConnectionString());
        //    MySqlCommand command = new MySqlCommand();
        //    int var11 = (int)((DataRowView)(this.htree.Tree1.SelectedFunction))["FK_idFunctiontech"];
        //    int var12 = (int)((DataRowView)(this.htree.Tree1.SelectedFunction))["FK_idEquipment"];
        //    Collection<int> var21toTransfer = new Collection<int>(); // массив соотв. функций
        //    Collection<int> var22toTransfer = new Collection<int>(); // коллекция соотв. оборудования

        //    // заполнение коллекций данных технологиечских функций - принимающих шаблон.
        //    for (int i = 0; i < this.dg2.Items.Count; ++i)
        //    {
        //        // find row for the first selected item
        //        DataGridRow row = this.GetDataGridRowItem(i);
        //        if (row != null)
        //        {
        //            DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

        //            // find grid cell object for the cell with index 0
        //            DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridCell;

        //            //if (cell != null)  
        //            if (((CheckBox)cell.Content).IsChecked == true)
        //            {
        //                var21toTransfer.Add((int)((DataRowView)row.Item)["FK_idFunctionTech"]);
        //                var22toTransfer.Add((int)((DataRowView)row.Item)["FK_idEquipment"]);
        //            }
        //        }
        //    }

        //    for (int i = 0; i < var21toTransfer.Count; ++i)
        //    {
        //        command.Connection = con;
        //        command.CommandText = "call CopyTechFunctionMask(" + var11 + "," + var12 + "," +
        //        var21toTransfer[i] + "," + var22toTransfer[i] + ");";
        //        con.Open();
        //        command.ExecuteNonQuery();
        //        con.Close();
        //    }

            //((ConMasks)(((ObjectDataProvider)FindResource("mask")).Data)).
            //   ChangeSignalFunction(this.htree.Tree1.SelectedFunction);
            //ChoosenFunction.Content = ((DataRowView)(this.htree.Tree1.SelectedFunction))["name"];





        }

        //private DataGridRow GetDataGridRowItem(int index)
        //{
        //    if (this.dg2.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        //        return null;

        //    return this.dg2.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        //}

        ////public static DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        ////{
        ////    DataGridRow rowContainer = GetRow(dataGrid, row);
        ////    if (rowContainer != null)
        ////    {
        ////        DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

        ////        // try to get the cell but it may possibly be virtualized
        ////        DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        ////        if (cell == null)
        ////        {
        ////            // now try to bring into view and retreive the cell
        ////            dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);

        ////            cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        ////        }

        ////        return cell;
        ////    }

        ////    return null;
        ////}

        //static T GetVisualChild<T>(Visual parent) where T : Visual
        //{
        //    T child = default(T);
        //    int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
        //    for (int i = 0; i < numVisuals; i++)
        //    {
        //        Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
        //        child = v as T;
        //        if (child == null) child = GetVisualChild<T>(v);
        //        if (child != null) break;
        //    }
        //    return child;
        //}



     

    }

