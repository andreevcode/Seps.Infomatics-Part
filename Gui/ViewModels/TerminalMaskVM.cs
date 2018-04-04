using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data;
using Seps.Infomatic.Core;
using Seps.Infomatic.MySql;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Threading;

namespace Seps.Infomatic.Gui
{
    public class TerminalMaskVM:VMBase
    {
        public PrimaryTreeVM PrimaryTree { get; set; }
        public SecondaryTreeVM SecondaryTree { get; set; }
        TableConnector functions;
        private List<object> dbConnectors = new List<object>();

        /// <summary>
        /// Структура для создания коллекции таблиц с именами
        /// </summary>
        public struct MaskTableInstance
        {
            //Имя для отображения в header'e expander'a
            public string MaskTableName { get; set; }
            //Собственно таблица с данными
            public DataView MaskTable { get; set; }
        }
        public ICommand SaveCommand { get; set; }
        public ICommand UpdateCommand { get; set; }


        #region Notify-переменные
        /// <summary>
        /// Коллекция вкладок с масками различных терминалов
        /// </summary>
        /// 
        public int LoadProgress
        {
            get { return (int)GetValue(LoadProgressProperty); }
            set { SetValue(LoadProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadProgressProperty =
            DependencyProperty.Register("LoadProgress", typeof(int), typeof(TerminalMaskVM), new UIPropertyMetadata(0));

        public string LoadStatus
        {
            get { return (string)GetValue(LoadStatusProperty); }
            set { SetValue(LoadStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadStatusProperty =
            DependencyProperty.Register("LoadStatus", typeof(string), typeof(TerminalMaskVM), new UIPropertyMetadata(""));

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); OnPropertyChanged("IsBusy"); }
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(TerminalMaskVM), new UIPropertyMetadata(false));

        public static readonly DependencyProperty TerminalMaskTabsProperty =
            DependencyProperty.Register("TerminalMaskTabs",
                                        typeof(ObservableCollection<DataRowView>),
                                        typeof(TerminalMaskVM),
                                        new PropertyMetadata(null)
                                        );
        public ObservableCollection<DataRowView> TerminalMaskTabs
        {
            get
            { return (ObservableCollection<DataRowView>)GetValue(TerminalMaskTabsProperty); }
            set
            { SetValue(TerminalMaskTabsProperty, value); }
        }

        /// <summary>
        /// Коллекция таблиц масок (маски функций, диагностических сигналов, уникальных сигналов)
        /// </summary>
        public ObservableCollection<MaskTableInstance> TerminalMasks
        {
            get { return (ObservableCollection<MaskTableInstance>)GetValue(TerminalMasksProperty); }
            set { SetValue(TerminalMasksProperty, value); }
        }

        public static readonly DependencyProperty TerminalMasksProperty =
            DependencyProperty.Register("TerminalMasks",
                                        typeof(ObservableCollection<MaskTableInstance>), 
                                        typeof(TerminalMaskVM), 
                                        new UIPropertyMetadata(null)
                                        );

        public DataRowView SelectedTerminal
        {
            get
            { return selectedTerminal; }
            set
            { selectedTerminal = (DataRowView)value; base.OnPropertyChanged("SelectedTerminal"); }
        }
        #endregion

        private DataRowView selectedTerminal=null;


        public TerminalMaskVM()
            : base()
        {
            SaveCommand = new CommandBuilder(args => this.Save());
            UpdateCommand = new CommandBuilder(args => this.Update());
        }
        protected override void initializeVM()
        {
            base.initializeVM();
            base.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TerminalMaskVM_PropertyChanged);
            PrimaryTree = new PrimaryTreeVM();
            SecondaryTree = new SecondaryTreeVM();
            functions = new TableConnector("SELECT * FROM functiontech;");
        }
        private void Save()
        {
            foreach (object item in dbConnectors)
            {
                if (item as ISave != null) (item as ISave).Save();
            }
        }
        private void Update()
        {
            functions.UpdateCommand.Execute(null);
            foreach (object item in dbConnectors)
            {
                if (item as IUpdate != null) (item as IUpdate).Update();
            }
        }
        void TerminalMaskVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedTerminal" && this.selectedTerminal!=null)
            {
                if (this.selectedTerminal.DataView.Table.Columns.Contains("idTerminal"))
                {
                    BackgroundWorker bwu = new BackgroundWorker();
                    bwu.WorkerReportsProgress = true;
                    bwu.DoWork += new DoWorkEventHandler(bwu_DoWork);
                    bwu.ProgressChanged += (s, arg) => 
                        {
                            this.LoadProgress = arg.ProgressPercentage; 
                            this.LoadStatus = arg.UserState.ToString(); 
                        };
                    bwu.RunWorkerCompleted += (s, arg) => { this.IsBusy = false; };
                    this.IsBusy = true;
                    bwu.RunWorkerAsync(selectedTerminal);
                }
            }
        }

        void bwu_DoWork(object sender, DoWorkEventArgs e)
        {
            DataRowView terminal;
            if (e.Argument != null)
            {
                terminal = (DataRowView)e.Argument;
            }
            else
            {
                throw new Exception("No terminal selected");
            }
            BackgroundWorker self = (BackgroundWorker)sender;
            self.ReportProgress(0, "Инициализация");
            //Прерываем выполнение, если выбранный элемент дерева не является терминалом
            if (!terminal.DataView.Table.Columns.Contains("idTerminal")) return;

            //Все изменения коллекции таблиц терминала производятся через Dispatcher, т.к. принадлежат другому потоку
            Dispatcher.Invoke(new Action(() =>
            {
                TerminalMasks = TerminalMasks ?? new ObservableCollection<MaskTableInstance>();
                TerminalMasks.Clear();
            }
            ), null);

            //Сбор таблиц функций
            List<functionInstance> functions = GetFunctions((int)terminal["idTerminal"]);
            int functionsCount = functions.Count;
            int i = 0;
            foreach (functionInstance item in functions)
            {
                string functionName = GetFunctionName(item);
                self.ReportProgress((int)Math.Round(((double)i++ / functionsCount) * 100), functionName);
                DataTable functionMask = GetFunctionMask(item);
                Dispatcher.Invoke(new Action(() =>
                {
                    TerminalMasks.Add(new MaskTableInstance()
                    {
                        MaskTableName = functionName,
                        MaskTable = new DataView(functionMask)
                    });
                }), DispatcherPriority.Background, null);
            }

            //Таблица уникальных сигналов
            DataTable uniqMask = GetUniqMask((int)terminal["idTerminal"]);
            self.ReportProgress((int)Math.Round((double)(i++ / (functionsCount+2) * 100)), "Уникальные сигналы");
            Dispatcher.Invoke(new Action(() =>
            {
                TerminalMasks.Add(new MaskTableInstance()
                {
                    MaskTableName = "Уникальные сигналы терминала",
                    MaskTable = new DataView(uniqMask)
                });
            }), DispatcherPriority.Background, null);

            //Таблица диагностических сигналов
            self.ReportProgress((int)Math.Round((double)(i++ / (functionsCount+2) * 100)), "Диагностические сигналы");
            DataTable diagMask = GetDiagMask((int)terminal["idTerminal"]);
            Dispatcher.Invoke(new Action(() =>
            {
                TerminalMasks.Add(new MaskTableInstance()
                {
                    MaskTableName = "Диагностические сигналы терминала",
                    MaskTable = new DataView(diagMask)
                });
            }), DispatcherPriority.Background, null);

    
        }

        private DataTable GetDiagMask(int terminalId)
        {
            int modelId = (from terminal in SecondaryTree.TreeData.Tables["terminal"].AsEnumerable()
                           where (int)Converters.Try(terminal["idTerminal"]) == terminalId
                           select (int)Converters.Try(terminal["FK_idModel"])).First<int>();
            ConSignDiag csd = new ConSignDiag();
            csd.ChangeModel(modelId);

            //Для доступа к ICommand модели
            dbConnectors.Add(csd);
            return csd.DataTableSignal;
        }

        private DataTable GetUniqMask(int terminalId)
        {
            ConSignUniq2 csu2 = new ConSignUniq2();
            csu2.ChangeTerminal(terminalId);

            //Для доступа к ICommand модели
            dbConnectors.Add(csu2);
            return csu2.DataTableSignal;
        }

        private DataTable GetFunctionMask(functionInstance function)
        {
            ConMasks cm = new ConMasks();
            cm.ChangeSignalFunction(function.EquipmentId,function.FunctionId);

            //Для доступа к ICommand модели
            dbConnectors.Add(cm);
            return cm.DataTableSignal;
        }

        private string GetFunctionName(functionInstance function)
        {
            string functionName=(from item in functions.MyDataTable.AsEnumerable()
                   where (int)Converters.Try(item["idFunctionTech"]) == function.FunctionId
                   select (string)item["name"]).First<string>();
            string equipmentName=(from item in PrimaryTree.TreeData.Tables["equipment"].AsEnumerable()
                   where (int)Converters.Try(item["idEquipment"]) == function.EquipmentId
                   select (string)item["long_name"]).First<string>();
            string projName = (from item in PrimaryTree.TreeData.Tables["equipment"].AsEnumerable()
                                    where (int)Converters.Try(item["idEquipment"]) == function.EquipmentId
                                    select (string)item["proj_name"]).First<string>();
            return string.Format("Функция: {0} Оборудование: {1} ({2})", functionName, equipmentName, projName);
        }

        private struct functionInstance
        {
            public int FunctionId;
            public int EquipmentId;
        }
        private List<functionInstance> GetFunctions(int terminalId)
        {
            return (from item in PrimaryTree.TreeData.Tables["union_1"].AsEnumerable()
                    where (int)Converters.Try(item["FK_idTerminal"]) == terminalId
                    select new functionInstance()
                    {
                        FunctionId = (int)Converters.Try(item["FK_idfunctiontech"]),
                        EquipmentId = (int)Converters.Try(item["FK_idEquipment"])
                    }
                    ).ToList<functionInstance>();
        }
    }
}
