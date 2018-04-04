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

namespace Seps.Infomatic.Gui
{
    /// <summary>
    /// Логика взаимодействия для PrimaryTreeView.xaml
    /// </summary>
    public partial class PrimaryTreeView : UserControl
    {
        #region RoutedEvent для обновления линий связи
        public static readonly RoutedEvent NeedRedrawLinesEvent =
            EventManager.RegisterRoutedEvent("NeedRedrawLines", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PrimaryTreeView));
        public event RoutedEventHandler NeedRedrawLines
        {
            add { AddHandler(NeedRedrawLinesEvent, value); }
            remove { RemoveHandler(NeedRedrawLinesEvent, value); }
        }
        private void RaiseNeedRedrawLinesEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(NeedRedrawLinesEvent);
            RaiseEvent(args);
        }
        #endregion

        public DataRowView SelectedItem
        {
            get { return (DataRowView)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(DataRowView), typeof(PrimaryTreeView), new UIPropertyMetadata(null));
        private int _idEquipmentFromTree;
        public int IdEquipmentFromTree
        {
            get { return _idEquipmentFromTree; }
            set { _idEquipmentFromTree = value; OnPropertyChanged("IdEquipmentFromTree"); }
        }
        Point startPoint;

        public bool IsAllowDragOne { get; set; }
        public bool IsAllowDragTwo { get; set; }
        public bool IsAllowDragThree { get; set; }
        public bool IsAllowDragFour { get; set; }
        public bool IsAllowDragFive{ get; set; }

        public PrimaryTreeView()
        {
            InitializeComponent();
            IsAllowDragOne = false;
            IsAllowDragTwo = false;
            IsAllowDragThree = false;
            IsAllowDragFour = false;
            IsAllowDragFive = false;
            //NameScope.SetNameScope(ContextMenu, NameScope.GetNameScope(this));
            
        }

        public static readonly DependencyProperty SubExpStatusDP = DependencyProperty.Register("SubExpStatus", typeof(Boolean), typeof(PrimaryTreeView));
        public Boolean SubExpStatus
        {
            get { return (Boolean)GetValue(SubExpStatusDP); }
            set { SetValue(SubExpStatusDP, value); }
        }

        public static readonly DependencyProperty VoltExpStatusDP = DependencyProperty.Register("VoltExpStatus", typeof(Boolean), typeof(PrimaryTreeView));
        public Boolean VoltExpStatus
        {
            get { return (Boolean)GetValue(VoltExpStatusDP); }
            set { SetValue(VoltExpStatusDP, value); }
        }

        public static readonly DependencyProperty GrEquipStatusDP = DependencyProperty.Register("GrEquipExpStatus", typeof(Boolean), typeof(PrimaryTreeView));
        public Boolean GrEquipExpStatus
        {
            get { return (Boolean)GetValue(GrEquipStatusDP); }
            set { SetValue(GrEquipStatusDP, value); }
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((TextBox)sender).IsReadOnly = false;
        }

        //private void TextBox_KeyDown(object sender, KeyEventArgs e)
        //{      
        //   if (e.Key.ToString() == "Return")
        //   {         
        //       ((TextBox)sender).IsReadOnly = true;
               
        //       Keyboard.Focus(grid);
        //       ((HierarchicalDataSetConnector)this.DataContext).SaveTree();
        //   }
        //}

        //проверка формата данных при DragOver
        public void OnDragOver(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                if (!e.Data.GetDataPresent("IDs_Tree"))
                    e.Effects = DragDropEffects.Move;
                else
                    e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        //проверка формата данных при DragEnter
        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                if (!e.Data.GetDataPresent("IDs_Tree"))
                    e.Effects = DragDropEffects.Move;
                else
                    e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        // drop
        private void OnDrop(object sender, DragEventArgs e)
        {
            // передача во VM данных об узле, в которую проихсодит Drop? 
            // чтобы его раскрыть
            try
            {
                Visual st = VisualTreeMethod.GetParentOfType(sender as Visual, typeof(TreeViewItem));
                if (st != null && st.GetType() == typeof(TreeViewItem))
                {
                    ((PrimaryTreeVM)dock.DataContext).TreeVItoExpand = st as TreeViewItem;
                    ((PrimaryTreeVM)dock.DataContext).notSendToDBNodesExist = true;

                }
            }
            catch { }
            PrimaryTreeVM ptvm = (PrimaryTreeVM)this.Resources["PrimaryTreeVM"];
            DragObject dObject = (DragObject)e.Data.GetData("IDs");
            ptvm.IdEquiment = (int)((DataRowView)((FrameworkElement)sender).DataContext)["idEquipment"];
            ptvm.EquipmentName = (string)((DataRowView)((FrameworkElement)sender).DataContext)["proj_name"];
            ptvm.Drop(dObject);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }

        public static readonly DependencyProperty DpSelectedFunction =
            DependencyProperty.Register("SelectedFunction", typeof(DataRowView), typeof(PrimaryTreeView));

        public DataRowView SelectedFunction
        {
            get { return (DataRowView)GetValue(DpSelectedFunction); }
            set 
            { 
                SetValue(DpSelectedFunction, (DataRowView)value);
                OnPropertyChanged("SelectedFunction");
            }
        }

        private void control_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.SelectedFunction = (DataRowView)((PrimaryTreeView)sender).treeView1.SelectedItem;
        }

        //private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    // Store the mouse position
        // startPoint = e.GetPosition(null);
        //}

        //private void TreeView_MouseMove(object sender, MouseEventArgs e)
        //{
        //    // Get the current mouse position

        //    try
        //    {
        //        Point mousePos = e.GetPosition(null);
        //        Vector diff = startPoint - mousePos;

        //        if (e.LeftButton == MouseButtonState.Pressed &&
        //            (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
        //            Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
        //        {
        //            TreeView tView = sender as TreeView;
        //            DataRowView dRowView = (DataRowView)tView.SelectedItem;

        //            string st = (String)((DataTable)((DataView)(dRowView).DataView).Table).TableName;
        //            //TreeViewItem tVitem = tView.SelectedItem as TreeViewItem;
        //            //ItemsControl parent = GetSelectedTreeViewItemParent(tVitem);

        //            if ((dRowView != null) && (st == "unionJoinFunction"))
        //            {
        //                //DataRow dRow = dRowView.Row;
        //                //int idFunctionTech = (int)dRow.ItemArray.GetValue(0);
        //                //int idEquipment = (int)dRow.ItemArray.GetValue(1);
        //                //int[] dataToTransfer = { idFunctionTech, idEquipment };
        //                //DataObject mydata = new DataObject("dataDrop_FunctionEquip", dataToTransfer);
        //                //DragDrop.DoDragDrop(sender as DependencyObject, mydata, DragDropEffects.All);

        //                DataRow dRow = dRowView.Row;
        //                DataObject mydata = new DataObject("dataDrop_FunctionEquip", dRow);
        //                DragDrop.DoDragDrop(sender as DependencyObject, mydata, DragDropEffects.All);

        //            }
        //        }
        //    }
        //    catch { }
        //}

        private static TreeViewItem GetParentTreeViewItem(DependencyObject item)
        {
            if (item != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(item);
                TreeViewItem parentTreeViewItem = parent as TreeViewItem;
                return parentTreeViewItem ?? GetParentTreeViewItem(parent);
            }

            return null;
        }

        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            { parent = VisualTreeHelper.GetParent(parent);}
            return parent as ItemsControl;
        }

        private int FindTreeLevel(DependencyObject control)
        {
            var level = -1;
            if (control != null)
            {
                var parent = VisualTreeHelper.GetParent(control);
                while (!(parent is TreeView) && (parent != null))
                {
                    if (parent is TreeViewItem)
                        level++;
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }
            return level;
        }

        //private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        //{
        //    HTreeVM vm = (HTreeVM)this.Resources["HTreeVM"];
        //    vm.SelectedItem = ((TreeView)sender).SelectedItem as DataRowView;
        //    TreeViewItem item = e.OriginalSource as TreeViewItem;
        //    if (item != null)
        //    {
        //        ItemsControl parent = GetSelectedTreeViewItemParent(item);
        //    }
        //}

        //private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        //{
        //    HierarchicalDataSetConnector topTreeDs = (HierarchicalDataSetConnector)TryFindResource("DataSet1");
        //    if (topTreeDs != null)
        //    {
        //        DataRowView drv = (DataRowView)((TreeViewItem)sender).Header;
        //        string tName = drv.DataView.Table.TableName;
        //        switch (tName)
        //        {
        //            case "substation":
        //                DataRow[] dr1 = topTreeDs.IsExpandedSubstation.Select("idSubstation=" + drv["idSubstation"]);
        //                if (dr1.Length == 0)
        //                {
        //                    topTreeDs.IsExpandedSubstation.Rows.Add(drv["idSubstation"], DBNull.Value,
        //                      ((TreeViewItem)sender).IsExpanded);
        //                }
        //                else { dr1[0]["isExpanded"] = ((TreeViewItem)sender).IsExpanded; }
        //                break;

        //            case "voltage":

        //                DataRow[] dr2 = topTreeDs.IsExpandedVoltage.Select("idVoltage=" + drv["idVoltage"]);
        //                if (dr2.Length == 0)
        //                { topTreeDs.IsExpandedVoltage.Rows.Add(drv["idVoltage"], DBNull.Value, ((TreeViewItem)sender).IsExpanded); }
        //                else { dr2[0]["isExpanded"] = ((TreeViewItem)sender).IsExpanded; }
        //                break;

        //            //DataRow dr2 = topTreeDs.IsExpandedSite.Select("idSite=" + drv["idSite"])[0];
        //            //dr2["isExpanded"] = ((TreeViewItem)sender).IsExpanded;
        //            //break;
        //            case "grequipment":
        //                DataRow[] dr3 = topTreeDs.IsExpandedGrequipment.Select("idGrequipment=" + drv["idGrequipment"]);
        //                if (dr3.Length == 0)
        //                {
        //                    topTreeDs.IsExpandedGrequipment.Rows.Add(drv["idGrequipment"], DBNull.Value, ((TreeViewItem)sender).IsExpanded);
        //                }
        //                else { dr3[0]["isExpanded"] = ((TreeViewItem)sender).IsExpanded; }
        //                break;
        //            case "equipment":
        //                DataRow[] dr4 = topTreeDs.IsExpandedEquipment.Select("idEquipment=" + drv["idEquipment"]);
        //                if (dr4.Length == 0)
        //                {
        //                    topTreeDs.IsExpandedEquipment.Rows.Add(drv["idEquipment"], DBNull.Value, DBNull.Value, DBNull.Value,
        //                      ((TreeViewItem)sender).IsExpanded);
        //                }
        //                else { dr4[0]["isExpanded"] = ((TreeViewItem)sender).IsExpanded; }
        //                break;
        //        }
        //    }
        //}

        //private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        //{
        //    HierarchicalDataSetConnector topTreeDs = (HierarchicalDataSetConnector)TryFindResource("DataSet1");
        //    if (topTreeDs != null)
        //    {
        //        DataRowView drv = (DataRowView)((TreeViewItem)sender).Header;
        //        string tName = drv.DataView.Table.TableName;
        //        switch (tName)
        //        {
        //            case "substation":
        //                DataRow[] dr1 = topTreeDs.IsExpandedSubstation.Select("idSubstation=" + drv["idSubstation"]);
        //                if (dr1.Length == 0)
        //                {
        //                    topTreeDs.IsExpandedSubstation.Rows.Add(drv["idSubstation"], DBNull.Value,
        //                      ((TreeViewItem)sender).IsExpanded);
        //                }
        //                else { dr1[0]["isExpanded"] = ((TreeViewItem)sender).IsExpanded; }
        //                break;

        //            case "voltage":

        //                DataRow[] dr2 = topTreeDs.IsExpandedVoltage.Select("idVoltage=" + drv["idVoltage"]);
        //                if (dr2.Length == 0)
        //                { topTreeDs.IsExpandedVoltage.Rows.Add(drv["idVoltage"], DBNull.Value, ((TreeViewItem)sender).IsExpanded); }
        //                else { dr2[0]["isExpanded"] = ((TreeViewItem)sender).IsExpanded; }
        //                break;

        //            //DataRow dr2 = topTreeDs.IsExpandedSite.Select("idSite=" + drv["idSite"])[0];
        //            //dr2["isExpanded"] = ((TreeViewItem)sender).IsExpanded;
        //            //break;
        //            case "grequipment":
        //                DataRow[] dr3 = topTreeDs.IsExpandedGrequipment.Select("idGrequipment=" + drv["idGrequipment"]);
        //                if (dr3.Length == 0)
        //                {
        //                    topTreeDs.IsExpandedGrequipment.Rows.Add(drv["idGrequipment"], DBNull.Value, ((TreeViewItem)sender).IsExpanded);
        //                }
        //                else { dr3[0]["isExpanded"] = ((TreeViewItem)sender).IsExpanded; }
        //                break;
        //            case "equipment":
        //                DataRow[] dr4 = topTreeDs.IsExpandedEquipment.Select("idEquipment=" + drv["idEquipment"]);
        //                if (dr4.Length == 0)
        //                {
        //                    topTreeDs.IsExpandedEquipment.Rows.Add(drv["idEquipment"], DBNull.Value, DBNull.Value, DBNull.Value,
        //                      ((TreeViewItem)sender).IsExpanded);
        //                }
        //                else { dr4[0]["isExpanded"] = ((TreeViewItem)sender).IsExpanded; }
        //                break;
        //        }
        //    }
        //}

        //private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = true;
        //}


        private void RefreshExpandedState()
        {

        }

        //старый метод сохранения состояния дерева при обновлении данных
        private void TreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            HierarchicalDataSetConnector topTreeDs = (HierarchicalDataSetConnector)TryFindResource("DataSet1");
            if (topTreeDs != null)
            {
                DataRowView drv = (DataRowView)((TreeViewItem)sender).Header;
                string tName = drv.DataView.Table.TableName;
                switch (tName)
                {
                    case "substation":
                        DataRow[] dr1 = topTreeDs.IsExpandedSubstation.Select("idSubstation=" + drv["idSubstation"]);
                        if (dr1.Length == 0)
                        {
                            topTreeDs.IsExpandedSubstation.Rows.Add(drv["idSubstation"], DBNull.Value, ((TreeViewItem)sender).IsExpanded);
                            ((TreeViewItem)sender).IsExpanded = true;
                        }
                        else { ((TreeViewItem)sender).IsExpanded = StrToBool(dr1[0]["isExpanded"].ToString()); }
                        break;
                    case "voltage":
                        DataRow[] dr2 = topTreeDs.IsExpandedVoltage.Select("idVoltage=" + drv["idVoltage"]);
                        if (dr2.Length == 0)
                        {
                            topTreeDs.IsExpandedVoltage.Rows.Add(drv["idVoltage"], DBNull.Value, ((TreeViewItem)sender).IsExpanded);
                            ((TreeViewItem)sender).IsExpanded = true;
                        }
                        else { ((TreeViewItem)sender).IsExpanded = StrToBool(dr2[0]["isExpanded"].ToString()); }
                        break;
                    case "grequipment":
                        DataRow[] dr3 = topTreeDs.IsExpandedGrequipment.Select("idGrequipment=" + drv["idGrequipment"]);
                        if (dr3.Length == 0)
                        {
                            topTreeDs.IsExpandedGrequipment.Rows.Add(drv["idGrequipment"], DBNull.Value, ((TreeViewItem)sender).IsExpanded);
                            ((TreeViewItem)sender).IsExpanded = true;
                        }
                        else { ((TreeViewItem)sender).IsExpanded = StrToBool(dr3[0]["isExpanded"].ToString()); }
                        break;
                    case "equipment":
                        DataRow[] dr4 = topTreeDs.IsExpandedEquipment.Select("idEquipment=" + drv["idEquipment"]);
                        if (dr4.Length == 0)
                        {
                            topTreeDs.IsExpandedEquipment.Rows.Add(drv["idEquipment"], DBNull.Value, DBNull.Value, 0,
                              ((TreeViewItem)sender).IsExpanded);
                            ((TreeViewItem)sender).IsExpanded = true;
                        }
                        else { ((TreeViewItem)sender).IsExpanded = StrToBool(dr4[0]["isExpanded"].ToString()); }
                        break;
                }
            }
        }


        private static bool StrToBool(string input)
        { return (input == "true" || input == "True") ? true : false; }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {


        }

        private void MenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void toCopy_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dr = (DataRowView)treeView1.SelectedItem;
            int id = (int)dr[0];
           
        }
        private void RouteUpdateLines(object sender, EventArgs e)
        {
            RaiseNeedRedrawLinesEvent();
        }

        //Передача данных о выбранном TreeViewItem во ViewModel,
        // Чтобы при добавлении вложенных узлов, происходило автоматическое открытие узла
        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            ((PrimaryTreeVM)dock.DataContext).TreeVItoExpand = e.OriginalSource as TreeViewItem;
        }

        private void RouteUpdateLines(object sender, ScrollChangedEventArgs e)
        {

        }

        // передача команды обработчика ctrl+s в VMBase
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender != null)
                try
                {
                    // указывает  System.Windows.Controls.DockPanel полностью, поскольку 
                    // namespace System.Windows.Controls не определен
                    ((VMBase)(((System.Windows.Controls.DockPanel)sender).DataContext)).Save();
                }
                catch { }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (e.Source as DockPanel != null)
            { e.CanExecute = true; }
        }




        #region NewButtonClick

        //private void ToggleButton_Click(object sender, RoutedEventArgs e)
        //{
        //    switch (((ToggleButton)e.Source).Content.ToString())
        //    {
        //        case ("1"):
        //            VisualTreeMethod.ExpandTreeLevel(this.Tree1.treeView1, 0, SubButton.IsChecked ?? false);
        //            break;

        //        case ("2"):
        //            VisualTreeMethod.ExpandTreeLevel(this.Tree1.treeView1, 1, VoltButton.IsChecked ?? false);
        //            break;

        //        case ("3"):
        //            VisualTreeMethod.ExpandTreeLevel(this.Tree1.treeView1, 2, GreqButton.IsChecked ?? false);
        //            break;
        //        case ("4"):
        //            VisualTreeMethod.ExpandTreeLevel(this.Tree1.treeView1, 3, EqButton.IsChecked ?? false);
        //            break;
        //    }
        #endregion

        #region OldButtonClick
        //if (sender.Equals(SubButton))
        //{
        //    DataTable Dt = ((HierarchicalDataSetConnector)SubButton.DataContext).Data.Tables["substation"];
        //    try { foreach (DataRow Dr in Dt.Rows) { Dr["isExpanded"] = SubButton.IsChecked; } }
        //    //foreach (DataRow Dr in Dt.Rows) { Dr["isExpanded"] = (SubButton.IsChecked == true) ? "True" : "False"; }
        //    catch { }
        //}
        //else
        //{
        //    if (sender.Equals(VoltButton))
        //    {
        //        DataTable Dt = ((HierarchicalDataSetConnector)VoltButton.DataContext).Data.Tables["voltage"];
        //        try { foreach (DataRow Dr in Dt.Rows) { Dr["isExpanded"] = VoltButton.IsChecked; } }
        //        catch { }
        //    }
        //    else
        //    {
        //        if (sender.Equals(GreqButton))
        //        {
        //            DataTable Dt = ((HierarchicalDataSetConnector)GreqButton.DataContext).Data.Tables["grequipment"];
        //            try { foreach (DataRow Dr in Dt.Rows) { Dr["isExpanded"] = GreqButton.IsChecked; } }
        //            catch { }
        //        }
        //        else
        //        {
        //            if (sender.Equals(EqButton))
        //            {
        //                DataTable Dt = ((HierarchicalDataSetConnector)EqButton.DataContext).Data.Tables["equipment"];
        //                try { foreach (DataRow Dr in Dt.Rows) { Dr["isExpanded"] = EqButton.IsChecked; } }
        //                catch { }
        //            }
        //        }
        //    }
        //} 
        #endregion

        //}


    }
}
