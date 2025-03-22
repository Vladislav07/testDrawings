using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormSW_Tree
{
    public partial class InfoForm : Form
    {

      
        //public event EventHandler<string> OperationCompleted;

        public InfoForm()
        {
            InitializeComponent();

            Controler.NumberModel += Controler_NumberModel;
            Controler.Init();
        }

        private void Controler_NumberModel(string obj)
        {
            this.Text = obj;
        }

        void  Grid()
        {
            dataGridView.Cursor = Cursors.WaitCursor;
            dataGridView.ColumnCount = 9;
            dataGridView.Columns[0].Name = "Cuby Number";
            dataGridView.Columns[1].Name = "Current Version";
            dataGridView.Columns[2].Name = "List of Ref Child Errors";
            dataGridView.Columns[3].Name = "Child";
            dataGridView.Columns[4].Name = "Child info";
            dataGridView.Columns[5].Name = "State";
            dataGridView.Columns[6].Name = "VersRef/VersParent";
            dataGridView.Columns[7].Name = "IsRebuildDraw";
            dataGridView.Columns[8].Name = "StateDraw";
            dataGridView.Cursor = Cursors.Default;
        }

        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            Controler.GetInfoFromPDM();
            FillDataGridView1();
        }

        private void chboxIncludeDraw_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; 
            if (checkBox.Checked == true)
            {
               
            }
            else
            {
                
            }
        }

        public void FillDataGridView1()
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

            foreach (Component comp in Tree.listComp)
            {
             
                if (comp.Section == "Стандартные изделия" || comp.Section == "") continue;

                if (comp.isDraw && Tree.listDraw.Count > 0)
                {
                    Drawing draw = Tree.listDraw.FirstOrDefault(d => d.CubyNumber == comp.CubyNumber);
                    dataGridView.Rows.Add(comp.CubyNumber, comp.CurVersion.ToString(), comp.IsRebuild.ToString(), "", "", comp.State.Name.ToString(),
                    draw.VersCompareToModel, draw.NeedsRegeneration.ToString(), draw.State.Name, comp.Section);
                }
                else
                {
                    if (!comp.IsRebuild) continue;
                    dataGridView.Rows.Add(comp.CubyNumber, comp.CurVersion.ToString(), comp.IsRebuild.ToString(), "", "", comp.State.Name.ToString(), "", "", "", comp.Section);
                }

                if (comp.listRefChildError != null)
                {
                    foreach (KeyValuePair<string, string> i in comp.listRefChildError)
                    {
                        dataGridView.Rows.Add("", "", "", i.Key, i.Value, "", "", "", "",comp.Section);
                    }
                }
            }
            dataGridView.Cursor = Cursors.Default;
        }

       
    }
}
