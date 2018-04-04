using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Seps.Infomatic.Core;
using System.Data;
using Seps.Infomatic.MySql;
using System.ComponentModel;
using System.Windows;

namespace Seps.Infomatic.Gui
{
    public class SearchSignalsVM : VMBase,  INotifyPropertyChanged
    //DependencyObject,
    {
        private ModelSearchSignals modelSearch;
        public ICommand SearchCommand { get; set; }

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

        private string _signalsCount;
        public string SignalsCount
        {
            get { return _signalsCount; }
            set { _signalsCount = value; OnPropertyChanged("SignalsCount"); }
        }

        // если >= пред. кол-ва сигналов в поиске, то выводим в сообщении 
        private string _signalsOver;
        public string SignalsOver
        {
            get { return _signalsOver; }
            set { _signalsOver = value; OnPropertyChanged("SignalsOver"); }
        }

        public string Namesignal
        {
            get { return (string)GetValue(NamesignalProperty); }
            set { SetValue(NamesignalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Namesignal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NamesignalProperty =
            DependencyProperty.Register("Namesignal", typeof(string), typeof(SearchSignalsVM), new UIPropertyMetadata(""));

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(string), typeof(SearchSignalsVM), new UIPropertyMetadata(""));

        public string Contact
        {
            get { return (string)GetValue(ContactProperty); }
            set { SetValue(ContactProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Contact.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContactProperty =
            DependencyProperty.Register("Contact", typeof(string), typeof(SearchSignalsVM), new UIPropertyMetadata(""));


        public string Place
        {
            get { return (string)GetValue(PlaceProperty); }
            set { SetValue(PlaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Place.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceProperty =
            DependencyProperty.Register("Place", typeof(string), typeof(SearchSignalsVM), new UIPropertyMetadata(""));


        public DataView TableDV
        {
            get { return (DataView)GetValue(TableDVProperty); }
            set { SetValue(TableDVProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TableDV.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TableDVProperty =
            DependencyProperty.Register("TableDV", typeof(DataView), typeof(SearchSignalsVM), new UIPropertyMetadata(null));

        public string Identifier
        {
            get { return (string)GetValue(IdentifierProperty); }
            set { SetValue(IdentifierProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Identifier.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdentifierProperty =
            DependencyProperty.Register("Identifier", typeof(string), typeof(SearchSignalsVM), new UIPropertyMetadata(""));

        public string LogName
        {
            get { return (string)GetValue(LogNameProperty); }
            set { SetValue(LogNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LogName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogNameProperty =
            DependencyProperty.Register("LogName", typeof(string), typeof(SearchSignalsVM), new UIPropertyMetadata(""));

        public SearchSignalsVM() 
        {
            SearchCommand = new CommandBuilder(args => { modelSearch.Search(Namesignal,Identifier, LogName,Type,Contact,Place); 
                SignalsCount = modelSearch.Data.Rows.Count.ToString(); SignalsOverFlag(modelSearch.Data.Rows.Count); }, args2 =>
            {
                if (modelSearch != null) return !modelSearch.IsBusy;
                else return false;
            }
            );
        }

        protected override void initializeVM()
        {

            modelSearch = (ModelSearchSignals)((ModelList)Application.Current.Resources["ModelList"]).ModelDictionary["SearchInSignals"];
            Dispatcher.Invoke(new Action(() => { TableDV = new DataView(modelSearch.Data); }), null);
            SignalsCount = modelSearch.Data.Rows.Count.ToString();
            SignalsOverFlag(modelSearch.Data.Rows.Count);
        }


        private void SignalsOverFlag(int count)
        {
            if (count >= 900) SignalsOver = ">= ";
            else SignalsOver = "";
        }
        //protected override void initializeVM()
        //{
        //    base.initializeVM();
        //    //modelSearch = (ModelSearchSignals)((ModelList)Application.Current.Resources["ModelList"]).ModelDictionary["SearchInSignals"];
        //    //Dispatcher.Invoke(new Action(() => { TableDV = new DataView(modelSearch.Data); }), null);
        //}


    }
}
