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
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;
using NLog;


namespace Seps.Infomatic.Gui
{
    //public partial class SepsTreeViewItem : TreeViewItem
    //{
    //    public Boolean IsExpandedAll
    //    {
    //        get { return (Boolean)GetValue(IsExpandedAllProperty); }
    //        set { SetValue(IsExpandedAllProperty, value); }
    //    }

    //    // Using a DependencyProperty as the backing store for IsExpandedAll.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty IsExpandedAllProperty =
    //        DependencyProperty.Register("IsExpandedAll", typeof(Boolean), typeof(SepsTreeView), new UIPropertyMetadata(false));
    //}

    public partial class SepsTreeView : TreeView, INotifyPropertyChanged
    {    
        private Point startPointGlobal;
        private Point startPointLocal;
        private Boolean expandedReturnFlag;
        private Logger log = LogManager.GetCurrentClassLogger();

        //Словарь для хранения открытых узлов дерева; 
        //ключем является кортеж (связанная пара данных ID и наименования таблицы)
        //значением  - булевое состояние isExpandedAll
        //в словарь записываются только узлы, isExpanded которых=true.
        private Dictionary<Tuple<int, string>, bool> OpenedItems = new Dictionary<Tuple<int, string>, bool>();


        //Свойство, определяющее возможность начала Drag-n-Drop из уровня 1 дерева
        public bool IsAllowDragOne
        {
            get { return (bool)GetValue(IsAllowDragOneProperty); }
            set { SetValue(IsAllowDragOneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowDragOne.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowDragOneProperty =
            DependencyProperty.Register("IsAllowDragOne", typeof(bool), typeof(SepsTreeView), new UIPropertyMetadata(false));

        //Свойство, определяющее возможность начала Drag-n-Drop из уровня 2 дерева
        public bool IsAllowDragTwo
        {
            get { return (bool)GetValue(IsAllowDragTwoProperty); }
            set { SetValue(IsAllowDragTwoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowDragTwo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowDragTwoProperty =
            DependencyProperty.Register("IsAllowDragTwo", typeof(bool), typeof(SepsTreeView), new UIPropertyMetadata(false));

        //Свойство, определяющее возможность начала Drag-n-Drop из уровня 3 дерева
        public bool IsAllowDragThree
        {
            get { return (bool)GetValue(IsAllowDragThreeProperty); }
            set { SetValue(IsAllowDragThreeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowDragThree.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowDragThreeProperty =
            DependencyProperty.Register("IsAllowDragThree", typeof(bool), typeof(SepsTreeView), new UIPropertyMetadata(false));

        //Свойство, определяющее возможность начала Drag-n-Drop из уровня 4 дерева
        public bool IsAllowDragFour
        {
            get { return (bool)GetValue(IsAllowDragFourProperty); }
            set { SetValue(IsAllowDragFourProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowDragFour.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowDragFourProperty =
            DependencyProperty.Register("IsAllowDragFour", typeof(bool), typeof(SepsTreeView), new UIPropertyMetadata(false));

        //Свойство, определяющее возможность начала Drag-n-Drop из уровня 5 или 6 дерева (функция)
        public bool IsAllowDragFive
        {
            get { return (bool)GetValue(IsAllowDragFiveProperty); }
            set { SetValue(IsAllowDragFiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowDragFive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowDragFiveProperty =
            DependencyProperty.Register("IsAllowDragFive", typeof(bool), typeof(SepsTreeView), new UIPropertyMetadata(false));

        //Свойство, определяющее возможность начала Drag-n-Drop из уровня 5 дерева (терминал)
        public bool IsAllowDragSix
        {
            get { return (bool)GetValue(IsAllowDragSixProperty); }
            set { SetValue(IsAllowDragSixProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowDragSix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowDragSixProperty =
            DependencyProperty.Register("IsAllowDragSix", typeof(bool), typeof(SepsTreeView), new UIPropertyMetadata(false));

        public SepsTreeView()
        {
            InitializeComponent();
        }

        private bool allowDrag = false;

        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPointGlobal = e.GetPosition(null);
            startPointLocal = e.GetPosition(this);

            // след. долго происходит делается.
            Visual element = VisualTreeMethod.GetItemUnderCursor(this, startPointLocal, typeof(TreeViewItem));
            allowDrag = element == null ? false : true;

            bool test = VisualTreeMethod.IsMouseOverScrollbar(sender, e.GetPosition(sender as IInputElement));
            allowDrag = test == true ? false : true;
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            //if (VisualTreeMethod.IsMouseOverScrollbar(sender, e.GetPosition(sender as IInputElement)))
            //{
            //    //startPoint = null;
            //    return;
            //}

            if (!allowDrag) return;

            Point mousePos = e.GetPosition(this);
            Visual element = VisualTreeMethod.GetItemUnderCursor(this, mousePos, typeof(TreeViewItem));
            allowDrag = element == null ? false : true;
            if (!allowDrag) return;
            // Get the current mouse position
            try
            {
                Vector diff = startPointLocal - mousePos;
                //if (e.LeftButton == MouseButtonState.Pressed &&
                //    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                //    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > 32 || Math.Abs(diff.Y) > 10))
                {
                    string st = (String)((DataTable)((DataView)((DataRowView)((TreeView)sender).
                                SelectedItem).DataView).Table).TableName;
                    switch (st)
                    {
                        case "substation":
                        case "project":
                            if (!IsAllowDragOne) return;
                            break;

                        case "voltage":
                        case "site":
                        case "tom":
                            if (!IsAllowDragTwo) return;
                            break;

                        case "equipment":
                        case "room":
                        case "terminal":
                            if (!IsAllowDragThree) return;
                            break;

                        case "grequipment":
                        case "cub":
                            if (!IsAllowDragFour) return;
                            break;

                        case "unionJoinFunction":
                            if (!IsAllowDragFive) return;
                            break;

                        case "term":
                            if (!IsAllowDragSix) return;
                            break;

                    }

                    TreeView tView = sender as TreeView;
                    DataRowView dRowView = ((DataRowView)tView.SelectedItem);

                    DataRow dRow = dRowView.Row;
                    // получаем индекс выбранного терминала 
                    int IDs = (int)dRow.ItemArray.GetValue(0);
                    int[] dataToTransfer = { IDs };

                    int IDs2 = -1;
                    if (((DataTable)((DataView)((DataRowView)((TreeView)sender).
                                SelectedItem).DataView).Table).PrimaryKey.Count() > 1)
                    {
                        IDs2 = (int)dRow.ItemArray.GetValue(1);
                    }
                    int[] dataToTransfer2 = { IDs2 };
                    DragObject dObject = new DragObject() { TableName = st, IDs = dataToTransfer, IDs2 = dataToTransfer2 };
                    DataObject dataObjIDs = new DataObject("IDs_Tree", dObject);
                    DragDrop.DoDragDrop(sender as DependencyObject, dataObjIDs, DragDropEffects.Move);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #region RoutedSelectionChangedTreeEvent - событие для подписки в классах-наследниках с EventArgs = Data
        //Регистрируем RoutedEvent для возможности подписки в классах-наследниках.
        //Передаются огранич.данные - e.NewValue, приведенное к типу DataRowView.
        //Реализация на основе CustomEventArgs.
        public static readonly RoutedEvent RoutedSelectionChangedTreeEvent =
            EventManager.RegisterRoutedEvent("RoutedSelectionChangedTree",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SepsTreeView));

        public event RoutedEventHandler RoutedSelectionChangedTree
        {
            add { AddHandler(RoutedSelectionChangedTreeEvent, value); }
            remove { RemoveHandler(RoutedSelectionChangedTreeEvent, value); }
        }
        #endregion

        #region FullRoutedSelectionChangedTree
        //Регистрируем RoutedEvent для возможности подписки в классах-наследниках.
        //Передается полный набор данных (e.NewValue, e.OldValue)
        //Используется для подписки в масках и отчетах масок функции;
        //там необходимоконтролировать старое значение на случай его удаления из VM.
        public static readonly RoutedEvent FullRoutedSelectionChangedTreeEvent =
            EventManager.RegisterRoutedEvent("FullRoutedSelectionChangedTree",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SepsTreeView));
        public event RoutedEventHandler FullRoutedSelectionChangedTree
        {
            add { AddHandler(FullRoutedSelectionChangedTreeEvent, value); }
            remove { RemoveHandler(FullRoutedSelectionChangedTreeEvent, value); }
        }
        #endregion
       
        // при выборе/удалении нового узла в дереве
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tView = ((TreeView)sender);
            DataRowView dRowView = tView.SelectedItem as DataRowView;            
            try 
            {
                //формирование кастомного типа передаваемых данных
                CustomEventArgs newEventArgs = new CustomEventArgs((DataRowView)dRowView,RoutedSelectionChangedTreeEvent);
                //запуск событий для подписчиков
                RaiseEvent(new RoutedEventArgs(FullRoutedSelectionChangedTreeEvent,e));
                RaiseEvent(newEventArgs);
            }
            catch (Exception ex)  // например выборе пустой строки
            {
               //Debug.WriteLine(string.Format("Ошибка в RoutedEvent: {0} , {1}",
               //this.GetType().ToString(), ex.Message));
               log.Error(string.Format("Ошибка в RoutedEvent: {0} , {1}",
               this.GetType().ToString(), ex.Message));
            }
        }

        //Событие раскрытие узла дерева
        private void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            if (expandedReturnFlag) 
                return;
            DataRowView drv = (DataRowView)((TreeViewItem)e.OriginalSource).Header;
            //обнуляем флаг наличия открываемого узла в коллекции открытых узлов
            bool currentItemExistsFlag = false;
            //создаем кортеж для записи ID и наименования таблицы
            Tuple<int,string> trp = Tuple.Create((int)drv[0],(string)drv.Row.Table.TableName);
            // поиск узла в словаре
            if (OpenedItems.Keys.Count > 0)
            {
                foreach (Tuple<int,string> arg in OpenedItems.Keys)
                {
                    if (arg.Equals(trp))
                    {
                        currentItemExistsFlag = true;
                        break;
                    }
                }
            }
            //Поиск состояния кнопки "Раскрыть все уровни" для узла по кнопке "Раскрыть"
            Boolean fl = false;
            List<Visual> ls = VisualTreeMethod.FindVisualChild((TreeViewItem)e.OriginalSource, typeof(ToggleButton));
            if (ls.Count>0 && ls[1] != null) fl=((ToggleButton)ls[1]).IsChecked??false;
            //добавляем данные об узле в словарь, если его там нет
            if (!currentItemExistsFlag) OpenedItems.Add(trp, fl);          
        }

        //Событие закрытия узла дерева
        private void TreeView_Collapsed(object sender, RoutedEventArgs e)
        {
            DataRowView drv = (DataRowView)((TreeViewItem)e.OriginalSource).Header;
            Tuple<int, string> target = null;
            //обнуляем флаг наличия закрываемого узла в коллекции открытых узлов
            bool currentItemExistsFlag = false;
            //создаем кортеж для записи ID и наименования таблицы
            Tuple<int, string> trp = Tuple.Create((int)drv[0], (string)drv.Row.Table.TableName);
            // поиск узла в словаре
            if (OpenedItems.Keys.Count > 0)
            {
                foreach (Tuple<int, string> arg in OpenedItems.Keys)
                {
                    if (arg.Equals(trp))
                    {
                        currentItemExistsFlag = true;
                        target = arg;
                        break;
                    }
                }
            }
            //удаляем данные об узле из словаря, если он там есть
            if (currentItemExistsFlag) OpenedItems.Remove(target);
        }

        #region Члены INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        //Событие перегрузки узла дерева
        public void TreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            if ((sender as TreeViewItem) == null) return;
            DataRowView drv = (DataRowView)((TreeViewItem)sender).Header;
            //создаем кортеж для записи ID и наименования таблицы
            Tuple<int, string> trp = Tuple.Create((int)drv[0], (string)drv.Row.Table.TableName);
            // поиск узла в словаре
            foreach (Tuple<int,string> arg in OpenedItems.Keys)
            {
                expandedReturnFlag = false;
                if (arg.Equals(trp))
                {
                    expandedReturnFlag = true;
                    ((TreeViewItem)sender).IsExpanded = true;

                    //Boolean fl = false;
                    List<Visual> ls = VisualTreeMethod.FindVisualChild((TreeViewItem)e.OriginalSource,
                                      typeof(ToggleButton));
                    if (ls[1] != null) ((ToggleButton)ls[1]).IsChecked = OpenedItems[arg];
                    break;
                }
            }
            expandedReturnFlag = false;
        }

        // Открытие/закрытие всех узлов
        private void Expander2_Click(object sender, RoutedEventArgs e)
        {
            if (((ToggleButton)sender).IsChecked == true)
            {
                Visual Parent = VisualTreeMethod.GetParentOfType((Visual)sender, typeof(TreeViewItem));
                ItemsControl ParentTVI = Parent as ItemsControl;
                if (Parent != null)
                {
                    VisualTreeMethod.ExpandAll(ParentTVI, true);
                }
            }
            else
            {
                Visual Parent = VisualTreeMethod.GetParentOfType((Visual)sender, typeof(TreeViewItem));
                ItemsControl ParentTVI = Parent as ItemsControl;
                if (Parent != null)
                {
                    VisualTreeMethod.ExpandAll(ParentTVI, false);
                }
            }
        }

        //фокус выделенного эелемента перед отрисовкой меню
        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        // поиск вверх по виз. дереву до ближ. TreeViewItem
        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
    }



}
