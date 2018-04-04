using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace Seps.Infomatic.MySql
{
    public class ModelReadWrite : ModelBase, ISave
    {
        public ModelReadWrite(string query)
            : base(query)
        {
            MySqlCommandBuilder mcb = new MySqlCommandBuilder(adapter);
            adapter.UpdateCommand = mcb.GetUpdateCommand();
            adapter.DeleteCommand = mcb.GetDeleteCommand();
            adapter.InsertCommand = mcb.GetInsertCommand();

            // В команде Insert по идее должно работать автоматическое обновление первичных ключей при отправлении
            // данных в источник данных (базу данных). Для этих целей используется поле adapter.InsertCommand.UpdatedRowChange,
            // а также модифицируется текст команды, в него добавляется строка: "; Select last_insert_id() as  id ";
            // где id совпадает с наменованием столбца первичного ключа таблицы.
            // Однако MysqlCommandBuider, как выяснилось, некорректо отрабатывает эту операцию. 
            // Другими словами, сделать этого не получислось. (проверялись версии Mysql.data.dll  6.4.4, 6.5.4 и 6.8.3)
            // ------------
            // Поэтому обновление первичных ключей при отправке данных из любой таблиц с базовой моделью ModelReadWrite
            // сделано вручную.
            adapter.RowUpdated += new MySqlRowUpdatedEventHandler(adapter_RowUpdated);

            adapter.Fill(Data);

            Data.PrimaryKey = new DataColumn[] { Data.Columns[0] };
            Data.PrimaryKey[0].AutoIncrement = true;
            Data.PrimaryKey[0].AutoIncrementSeed = -1;
            Data.PrimaryKey[0].AutoIncrementStep = -1;
        }

        //Исправление косяка MysqlCommandBuilder, в котором не работает правильно обновление первичных ключей 
        void adapter_RowUpdated(object sender, MySqlRowUpdatedEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Added)
            {
                MySqlCommand idSearchCommand = new MySqlCommand(";select last_insert_id()", e.Command.Connection);
                object last_id = idSearchCommand.ExecuteScalar();

                object[] obj= new object[Data.Columns.Count];
                for (int i =0; i<=Data.Columns.Count - 1; i++)
                {
                    obj[i] = e.Row[i];
                }
                obj[0] = last_id;
                e.Row.Delete();
                Data.Rows.Add(obj);
            }
        }


        #region Члены ISave

        public void Save()
        {
            try
            {
                adapter.Update(Data);
                Data.AcceptChanges();

            }
            catch (Exception ex)
            {
                Data.RejectChanges();
                Debug.WriteLine(ex.Message);
                switch (Data.TableName)
                {
                    case "protocol":
                        MessageBox.Show("Удаление некоторых записей невозможно.\n\n" +
                            "Причина:\nКоммуникационный протокол не может быть удален из БД, если он назначен хотя бы к одному терминалу!"
                            ,"Предупреждение",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                        break;

                    case "model":
                        MessageBox.Show("При сохранении изменений в базе данных произошло исключение.\n" +
                            "Удаление некоторых записей невозможно.\n\n" +
                            "Причина:\nТип оборудования не может быть удален из БД, если он назначен хотя бы к одному терминалу!"
                            , "Предупреждение");
                        break;
                    default:
                        break;
                }

            }
            finally
            {
              
            }
            ////Обертывание в BackgroundWorker
            //BackgroundWorker bw = new BackgroundWorker();
            //bw.DoWork += (o, arg) =>
            //{
            //    adapter.Update(Data);
            //};
            //bw.RunWorkerCompleted += (o, arg) =>
            //{
            //    IsBusy = false;
            //};
            //IsBusy = true;
            //bw.RunWorkerAsync();
        }

        #endregion
    }
}
