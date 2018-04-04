using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Data;
using System.Windows.Input;

namespace Seps.Infomatic.MySql
{
    public class ModelTree : INotifyPropertyChanged
    {
        public bool IsBusy { get; set; }
        public event EventHandler TreeUpdating;
        public event EventHandler TreeUpdated;

        public void TreeSave()
        {
            //IsBusy = true;
            //BackgroundWorker bw = new BackgroundWorker();
            //bw.DoWork += (o, arg) =>
            //{
                substationModel.Save();
                voltModel.Save();
                grequipModel.Save();
                equipModel.Save();
                siteModel.Save();
                roomModel.Save();
                cubModel.Save();
                termModel.Save();
                unionJoinFunctionModel.Save();
            //};
            //bw.RunWorkerCompleted += (o, arg) => { IsBusy = false; CommandManager.InvalidateRequerySuggested(); };
            //bw.RunWorkerAsync();
            
        }
        public void TreeUpdate()
        {
            //IsBusy = true;
            //BackgroundWorker bw = new BackgroundWorker();
            //bw.DoWork += (o, arg) =>
            //{
            //что за строчка дальше?
            if (TreeUpdating != null) TreeUpdating.Invoke(this, new EventArgs());
            substationModel.Update();
            siteModel.Update();
            roomModel.Update();
            cubModel.Update();
            termModel.Update();
            voltModel.Update();
            grequipModel.Update();
            equipModel.Update();
            unionJoinFunctionModel.Update();
            //что за строчка дальше?
            if (TreeUpdated != null) this.TreeUpdated.Invoke(this, new EventArgs());
            //};
            //bw.RunWorkerCompleted += (o, arg) => { IsBusy = false; CommandManager.InvalidateRequerySuggested(); };
            //bw.RunWorkerAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ModelList modelList;

        public DataSet TreeData { get; set; }

        private ModelReadWrite substationModel;
        private ModelReadWrite siteModel;
        private ModelReadWrite roomModel;
        private ModelReadWrite cubModel;
        private ModelReadWrite termModel;
        private ModelReadWrite voltModel;
        private ModelReadWrite grequipModel;
        private ModelReadWrite equipModel;
        private ModelUnion unionModel;
        private ModelUnionFunction unionJoinFunctionModel;

        public ModelTree()
        {
            //IsBusy = false;
            modelList = ((ModelList)Application.Current.Resources["ModelList"]);
            try
            {
                modelList = ((ModelList)Application.Current.Resources["ModelList"]);

                substationModel = (ModelReadWrite)modelList.ModelDictionary["substation"];
                DataTable substationTable = substationModel.Data;
                substationTable.TableName = "substation";

                siteModel = (ModelReadWrite)modelList.ModelDictionary["site"];
                DataTable siteTable = siteModel.Data;
                siteTable.TableName = "site";

                roomModel = (ModelReadWrite)modelList.ModelDictionary["room"];
                DataTable roomTable = roomModel.Data;
                roomTable.TableName = "room";

                cubModel = (ModelReadWrite)modelList.ModelDictionary["cubicle"];
                DataTable cubTable = cubModel.Data;
                cubTable.TableName = "cub";

                termModel = (ModelReadWrite)modelList.ModelDictionary["terminal"];
                DataTable termTable = termModel.Data;
                termTable.TableName = "term";

                voltModel = (ModelReadWrite)modelList.ModelDictionary["voltage"];
                DataTable voltTable = voltModel.Data;
                voltTable.TableName = "voltage";

                grequipModel = (ModelReadWrite)modelList.ModelDictionary["grequipment"];
                DataTable grequipTable = grequipModel.Data;
                grequipTable.TableName = "grequipment";

                equipModel = (ModelReadWrite)modelList.ModelDictionary["equipment"];
                DataTable equipTable = equipModel.Data;
                equipTable.TableName = "equipment";

                unionModel = (ModelUnion)modelList.ModelDictionary["union_1"]; ;
                DataTable unionTable = unionModel.Data;
                unionTable.TableName = "union_1";

                //unionJoinFunctionModel = new ModelUnionFunction(modelList);
                unionJoinFunctionModel = new ModelUnionFunction();
                DataTable unionJoinFunctionTable = unionJoinFunctionModel.Data;
                unionJoinFunctionTable.TableName = "unionJoinFunction";

            
                TreeData = new DataSet();
                TreeData.Tables.Add(substationTable);

                TreeData.Tables.Add(siteTable);

                TreeData.Tables.Add(roomTable);

                TreeData.Tables.Add(cubTable);

                TreeData.Tables.Add(termTable);

                TreeData.Tables.Add(voltTable);

                TreeData.Tables.Add(grequipTable);

                TreeData.Tables.Add(equipTable);

                TreeData.Tables.Add(unionTable);

                TreeData.Tables.Add(unionJoinFunctionTable);

                try
                {
                    TreeData.Relations.Add("subst2site",
                            TreeData.Tables["substation"].Columns["idSubstation"],
                            TreeData.Tables["site"].Columns["FK_idSubstation"]
                            );
                    TreeData.Relations.Add("site2room",
                            TreeData.Tables["site"].Columns["idSite"],
                            TreeData.Tables["room"].Columns["FK_idSite"]
                            );
                    TreeData.Relations.Add("room2cub",
                            TreeData.Tables["room"].Columns["idRoom"],
                            TreeData.Tables["cub"].Columns["FK_idRoom"]
                            );
                    TreeData.Relations.Add("cub2term",
                            TreeData.Tables["cub"].Columns["idCubicle"],
                            TreeData.Tables["term"].Columns["FK_idCubicle"]
                            );
                    TreeData.Relations.Add("term2union_1",
                            TreeData.Tables["term"].Columns["idTerminal"],
                            TreeData.Tables["unionJoinFunction"].Columns["FK_idTerminal"]
                            );

                    TreeData.Relations.Add("subst2volt",
                    TreeData.Tables["substation"].Columns["idSubstation"],
                    TreeData.Tables["voltage"].Columns["FK_idSubstation"]
                            );
                    TreeData.Relations.Add("volt2grequip",
                            TreeData.Tables["voltage"].Columns["idVoltage"],
                            TreeData.Tables["grequipment"].Columns["FK_idVoltage"]
                            );
                    TreeData.Relations.Add("grequip2equip",
                            TreeData.Tables["grequipment"].Columns["idGrequipment"],
                            TreeData.Tables["equipment"].Columns["FK_idGrequipment"]
                            );

                    TreeData.Relations.Add("equip2union_1",
                            TreeData.Tables["equipment"].Columns["idEquipment"],
                            TreeData.Tables["unionJoinFunction"].Columns["FK_idEquipment"]
                            );
                    TreeData.DefaultViewManager.DataViewSettings["cub"].Sort = "number ASC";

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("{0}Error linking tables in DataSet: {1}", this.GetType().ToString(), ex.Message));
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}Error linking tables in DataSet: {1}", this.GetType().ToString(), ex.Message));
            }
        }

    }
}
