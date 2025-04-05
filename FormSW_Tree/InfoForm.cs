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

        public void FillDataGridView1(List<IDisplay> listComp)
        {
            dataGridView.Rows.Clear();
            this.Refresh();
            dataGridView.Cursor = Cursors.WaitCursor;
            dataGridView.ColumnCount = 5;
            dataGridView.Columns[0].Name = "Cuby Number";
            dataGridView.Columns[0].Width = 125;
            dataGridView.Columns[1].Name = "Type";
            dataGridView.Columns[1].Width = 75;
            dataGridView.Columns[2].Name = "State";
            dataGridView.Columns[2].Width = 90;
            dataGridView.Columns[3].Name = "Level";
            dataGridView.Columns[3].Width = 50;
            dataGridView.Columns[4].Name = "IsLocked";
            dataGridView.Columns[4].Width = 60;


            foreach (IDisplay comp in listComp)
            {
                dataGridView.Rows.Add(comp.Print());
            }
            dataGridView.Cursor = Cursors.Default;
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
       
            //Controler.RebuildTree();
      
          
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
           // Controler.NumberModel += Controler_NumberModel;
           // Controler.ActionRebuild += Controler_ActionRebuild;
           // Controler.MsgState += Controler_MsgState;
           // Controler.Init();
         
 
        }
    }
}
