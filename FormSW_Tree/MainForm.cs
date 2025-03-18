using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace FormSW_Tree
{
    public partial class MainForm :Form
    {

        SW sw;
        bool isClearTree = false;
        bool isIncludeDraw = false;
        public MainForm()
        {
            InitializeComponent();
            checkBox1.Checked = isIncludeDraw;
            PDM.UnLock += PDM_UnLock;
        }

        private void PDM_UnLock(string obj)
        {
            GetTree();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.dataGridView.AutoGenerateColumns = true;
            
        }

        private void btnConnectSW_Click(object sender, EventArgs e)
        {
            sw = new SW();
            sw.action += Sw_action;
            sw.numberModel += Sw_numberModel;
            sw.btnConnectSW();
            sw.BuildTree();
            Tree.SearchParentFromChild();
        }

        private void Sw_numberModel(string numberCuby)
        {
          txtActiveDoc.Text = numberCuby;
        }

        private void Sw_action(string message)
        {
           txtStatus.Text = message;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GetTree();
        }

        private void GetTree()
        {
            Tree.FillCollection();
            Tree.CompareVersions();
            if (isIncludeDraw) Tree.IsDrawings();
            FillDataGridView1();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            sw.Cancel();
            // change button config
            this.btnRefresh.Enabled = true;
            //this.cmdCancel.Enabled = false;
        }


        public void FillDataGridView(DataTable dt)
        {

            dataGridView.Cursor = Cursors.WaitCursor;
            this.bindingSource1.DataSource = dt;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            { dataGridView.Columns[i].ReadOnly = true; }

            dataGridView.Cursor = Cursors.Default;
        }

       
        public void FillDataGridView1()
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

            foreach (Component comp in Tree.listComp)
            {
                if (comp.isDraw && Tree.listDraw.Count>0)
                {
                    Drawing draw = Tree.listDraw.FirstOrDefault(d => d.CubyNumber == comp.CubyNumber);
                   
                    dataGridView.Rows.Add(comp.CubyNumber, comp.CurVersion.ToString(), comp.IsRebuild.ToString(), "", "", comp.State.Name.ToString(),
                    draw.VersCompareToModel, draw.NeedsRegeneration.ToString(),draw.State.ToString());
                }
                else
                {
                   dataGridView.Rows.Add(comp.CubyNumber, comp.CurVersion.ToString(), comp.IsRebuild.ToString(), "", "", comp.State.Name.ToString(), "", "", "");
                }
                
                if (comp.listRefChildError != null)
                {
                    foreach (KeyValuePair<string, string> i in comp.listRefChildError)
                    {
                        dataGridView.Rows.Add("", "", "", i.Key, i.Value, "", "", "", "");
                    }
                }
            }
            dataGridView.Cursor = Cursors.Default;
        }

 

        private void btn_RefreshTree_Click(object sender, EventArgs e)
        {

            List<Component> l = Tree.listComp.Where(c => c.IsRebuild == true && c.State.Name== "In work").ToList();
            if (l.Count == 0)
            {
                MessageBox.Show("Updating is not possible");
                return;
            }
            PDM.AddSelItemToList(l);
            PDM.BatchGet(l);
            l.Reverse();
            sw.OpenAndRefresh(l);        
            PDM.DocBatchUnLock();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; // приводим отправителя к элементу типа CheckBox
            if (checkBox.Checked == true)
            {
                isIncludeDraw = true;
            }
            else
            {
                isIncludeDraw = false;
            }
        }
    }

}
