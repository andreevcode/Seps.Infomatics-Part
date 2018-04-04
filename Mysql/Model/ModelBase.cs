using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using Seps.Infomatic.Core;
using MySql.Data.MySqlClient;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Seps.Licence;

namespace Seps.Infomatic.MySql      
{

    public class ModelBase : LicencedModelBase, INotifyPropertyChanged, IUpdate        
    {
        public ModelBase(string query)
        {
            Query = query;
            Data = new DataTable();
            DataBaseParameters = DbParameters.Current;
            con = new MySqlConnection(DataBaseParameters.GetMySqlConnectionString());
            command = new MySqlCommand();
            command.Connection = con;
            command.CommandText = Query;
            adapter = new MySqlDataAdapter(command);
        }

        private MySqlConnection con;
        private MySqlCommand command;
        public MySqlDataAdapter adapter;
        public string Query;
        private Boolean _IsBusy=false;
        public Boolean IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged("IsBusy"); }
        }
        public DataTable Data;
        public DbParameters DataBaseParameters;

        #region Члены IUpdate
        /// <summary>
        /// Реализация IUpdate
        /// </summary>
        public void Update()
        {
            if (Data.TableName == "union_1")
            {
                updateComplicatedPrimaryKeys();
                return;
            }
            try
            {
            Data.RejectChanges();
            MySqlCommand selectCommand = adapter.SelectCommand;
            if (adapter.SelectCommand.Connection.State==ConnectionState.Closed) adapter.SelectCommand.Connection.Open();
            
                MySqlDataReader reader = selectCommand.ExecuteReader();
                Collection<int> primaryKeys = new Collection<int>();
                while (reader.Read())
                {
                    primaryKeys.Add(reader.GetInt16(0));

                    object[] readerValues = new object[Data.Columns.Count];
                    reader.GetValues(readerValues);
                    if (Data.Rows.Contains(reader.GetInt16(0)))
                    {
                        UpdateSource(Data.Rows.Find(reader.GetInt16(0)), readerValues);
                    }
                    else
                    {
                        Data.Rows.Add(readerValues);
                    }
                }
                var idsToDelete = from item in Data.AsEnumerable()
                                  where !primaryKeys.Contains((int)item[0])
                                  select (int)item[0];
                foreach (int item in idsToDelete)
                {
                    Data.Rows.Find(item).Delete();
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Model Update ERROR: "+ex.Message.ToString());
            }
            finally
            {
                try
                {
                    adapter.SelectCommand.Connection.Close();
                    Data.AcceptChanges();
                    OnPropertyChanged("Data");
                }
                catch (Exception ex)
                {
                    Debug.Print("Model Update ERROR: " + ex.Message.ToString());
                }
            }
         
          
            ////Обертывание в BackgroundWorker
            //BackgroundWorker bw = new BackgroundWorker();
            //bw.DoWork += (o, arg) =>
            //{
            //    Data.RejectChanges();
            //    adapter.Fill(Data);
            //};
            //bw.RunWorkerCompleted += (o, arg) =>
            //{
            //    IsBusy = false;
            //};
            //IsBusy = true;
            //bw.RunWorkerAsync();
        }

        private void updateComplicatedPrimaryKeys()
        {
            try
            {
                Data.RejectChanges();
                MySqlCommand selectCommand = adapter.SelectCommand;
                if (adapter.SelectCommand.Connection.State == ConnectionState.Closed) adapter.SelectCommand.Connection.Open();

                MySqlDataReader reader = selectCommand.ExecuteReader();
                Collection<object[]> primaryKeys = new Collection<object[]>();
               
                while (reader.Read())
                {
                    object[] readerValues = new object[Data.Columns.Count];
                    object[] complicated_keys = new object[2];
                    reader.GetValues(readerValues);
                    complicated_keys[0] = readerValues[0];
                    complicated_keys[1] = readerValues[1];
                    primaryKeys.Add(complicated_keys);
                    if (Data.Rows.Contains(complicated_keys))
                    {
                        UpdateSource(Data.Rows.Find(complicated_keys), readerValues);
                    }
                    else
                    {
                        Data.Rows.Add(readerValues);
                    }
                }
                //var idsToDelete = from item in Data.AsEnumerable()
                //                  where !primaryKeys.Contains(complicated_keys)
                //                  select item;
                Collection<object[]> idsToDelete = new Collection<object[]>();
                foreach (DataRow dr in Data.Rows)
                {
                    object[] complicated_keys = new object[2];
                    complicated_keys[0] = dr[0];
                    complicated_keys[1] = dr[1];
                    if (!(primaryKeys.Any<object[]>(item => item[0].Equals(complicated_keys[0]) && item[1].Equals(complicated_keys[1]))))
                    {
                        idsToDelete.Add(complicated_keys);
                    }
                }

                foreach (object[] item in idsToDelete)
                {
                    Data.Rows.Find(item).Delete();
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Model Update ERROR: " + ex.Message.ToString());
            }
            finally
            {
                try
                {
                    adapter.SelectCommand.Connection.Close();
                    Data.AcceptChanges();
                    OnPropertyChanged("Data");
                }
                catch (Exception ex)
                {
                    Debug.Print("Model Update ERROR: " + ex.Message.ToString());
                }
            }
        }

        private bool UpdateSource(DataRow dataRow, object[] readerValues)
        {
            bool identical = true;
            for (int i = 0; i < dataRow.ItemArray.Length; i++)
            {
                if (!dataRow[i].Equals(readerValues[i]))
                {
                    dataRow[i] = readerValues[i]==null?DBNull.Value:readerValues[i];
                }
            }
            return identical;
        }

        #endregion

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
    }
}
        