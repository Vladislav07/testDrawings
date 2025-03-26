using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FormSW_Tree
{
    public partial class InfoForm : Form
    {

     

        public InfoForm()
        {
            InitializeComponent();

        }

        private void Controler_MsgState(string MsgState)
        {
            label1.Text = MsgState;
        }

        private void Controler_ActionRebuild(string msg, List<Model> list)
        {
            FillDataGridView1(list);
            label1.Text = msg;
        }

        private void Controler_NumberModel(string obj)
        {
            this.Text = obj;
        }

    

      
   
        public void FillDataGridView1(List<Model> listComp)
        {
            dataGridView.Rows.Clear();
            dataGridView.Cursor = Cursors.WaitCursor;
            dataGridView.ColumnCount = 10;
            dataGridView.Columns[0].Name = "Cuby Number";
            dataGridView.Columns[1].Name = "Current Version";
            dataGridView.Columns[2].Name = "List of Ref Child Errors";
            dataGridView.Columns[3].Name = "Child";
            dataGridView.Columns[4].Name = "Child info";
            dataGridView.Columns[5].Name = "State";
            dataGridView.Columns[6].Name = "VersRef/VersParent";
            dataGridView.Columns[7].Name = "IsRebuildDraw";
            dataGridView.Columns[8].Name = "StateDraw";
            dataGridView.Columns[9].Name = "Section";

            foreach (Model comp in listComp)
            {
             
               // if (comp.Section == "Стандартные изделия" || comp.Section == "") continue;

                if (comp.isDraw)
                {
                    Drawing draw = Tree.listDraw.FirstOrDefault(d => d.CubyNumber == comp.CubyNumber);
                    dataGridView.Rows.Add(comp.CubyNumber, comp.NeedsRegeneration.ToString(), comp.IsRebuild.ToString(), "", "", comp.State.Name.ToString(),
                    draw.VersCompareToModel, draw.NeedsRegeneration.ToString(), draw.State.Name, comp.Section);
                }
                else
                {
                    if (!comp.IsRebuild) continue;
                    dataGridView.Rows.Add(comp.CubyNumber, comp.NeedsRegeneration.ToString(), comp.IsRebuild.ToString(), "", "", comp.State.Name.ToString(), "", "", "", comp.Section);
                }

                if (comp.listRefChildError != null)
                {
                    foreach (KeyValuePair<string, string> i in comp.listRefChildError)
                    {
                        dataGridView.Rows.Add("", "", "", i.Key, i.Value, "", "", "", "","");
                    }
                }
            }
            dataGridView.Cursor = Cursors.Default;
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            Controler.RebuildTree();
            Tree.ClearCompTree();
            Controler.GetInfoFromPDM();
            FillDataGridView1(Tree.listComp);
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            Controler.NumberModel += Controler_NumberModel;
            Controler.ActionRebuild += Controler_ActionRebuild;
            Controler.MsgState += Controler_MsgState;
            Controler.Init();
            Controler.GetInfoFromPDM();
            FillDataGridView1(Tree.listComp);
        }
    }
}
