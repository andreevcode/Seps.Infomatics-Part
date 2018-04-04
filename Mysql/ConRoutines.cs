using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Input;
using Seps.Infomatic.Core;
using System.Reflection;
using System.Collections.Specialized;

namespace Seps.Infomatic.MySql
{
    public class ConRoutines
    {
        public MySqlDataAdapter adapter;
        private ICommand _updateCommand;

        public DataTable DataTableSignal { get; set; }
        public MySqlCommand command;
        public ConRoutines()
        
        {
           
         int terminal = 3;

         MySqlConnection con = new MySqlConnection(DbParameters.Current.GetMySqlConnectionString());
          command = new MySqlCommand();
          command.Connection = con;
          command.CommandText = "call templtableanalog(" + terminal.ToString() + ");";
          DataTableSignal = new DataTable();
          adapter = new MySqlDataAdapter(command);
          adapter.Fill(DataTableSignal);

        }

        public ConRoutines(int id)
        {
            MySqlConnection con = new MySqlConnection(DbParameters.Current.GetMySqlConnectionString());
            MySqlCommand command = new MySqlCommand();
            command.Connection = con;
            //command.CommandText = "call templtable(" + id.ToString() + ");";
            command.CommandText = "call templtableanalog(" + id.ToString() + ");";
            DataTableSignal = new DataTable();
            adapter = new MySqlDataAdapter(command);
            adapter.Fill(DataTableSignal);
        }

        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new CommandBuilder(param => this.Refresh());
                }
                return _updateCommand;

            }
        }
        
       
        public void Refresh()
        {
            DataTableSignal.Clear();
            adapter.Fill(DataTableSignal);
           
        }
       
       
    }



}