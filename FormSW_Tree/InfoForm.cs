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

        private void Controler_ActionRebuild(string msg, List<IDisplay> list)
        {
            FillDataGridView1(list);
            label1.Text = msg;
        }

        private void Controler_NumberModel(string obj)
        {
            this.Text = obj;
        }


        public void FillDataGridView1(List<IDisplay> listComp)
        {
            dataGridView.Rows.Clear();
            this.Refresh();
            dataGridView.Cursor = Cursors.WaitCursor;
            dataGridView.ColumnCount = 4;
            dataGridView.Columns[0].Name = "Cuby Number";
            dataGridView.Columns[1].Name = "Type";
            dataGridView.Columns[2].Name = "State";
            dataGridView.Columns[3].Name = "Level";


            foreach (IDisplay comp in listComp)
            {
                dataGridView.Rows.Add(comp.Print());
            }
            dataGridView.Cursor = Cursors.Default;
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            Controler.RebuildTree();
            Controler.GetInfoFromPDM();
          
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            Controler.NumberModel += Controler_NumberModel;
            Controler.ActionRebuild += Controler_ActionRebuild;
            Controler.MsgState += Controler_MsgState;
            Controler.Init();
            Controler.GetInfoFromPDM();
 
        }
    }
}
