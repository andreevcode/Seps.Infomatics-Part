using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using NLog;
using Seps.Infomatic.Core;
using Seps.Infomatic.MySql;

namespace Seps.Infomatic.Gui
{
    public class VMBase : DependencyObject, INotifyPropertyChanged
    {
    	protected static Logger log = LogManager.GetCurrentClassLogger();

        #region Privates
        #endregion
        #region Notify переменные
        private Boolean _isInitialized=false;
        public Boolean IsInitialized
        {
            get { return _isInitialized; }
            set { _isInitialized = value; OnPropertyChanged("IsInitialized"); }
        }
        #endregion
        public VMBase()
        {
            BackgroundWorker initializationBW = new BackgroundWorker();
            initializationBW.DoWork += (o, e) => { initializeVM(); };
            initializationBW.RunWorkerCompleted += (o, e) => {
                if (e.Error != null) Debug.WriteLine("Ошибка инициализации VMBase: " + e.Error.Message);
                else IsInitialized = true;
            };
            try
            { initializationBW.RunWorkerAsync(); }
            catch (Exception ex)
            { 
            	Debug.Print(ex.Message);
            	log.Debug("Error in VMBase init: {0}", ex.Message);
            }
        }
        /// <summary>
        /// Вся инициализация проходит здесь. Конструктор обеспечивает выполнение этого метода в асинхронном режиме.
        /// </summary>
        protected virtual void initializeVM()
        {

        }

        // метод Save для переназначения во VM
        public virtual void Save()
        {
            
        }

        #region Члены INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
