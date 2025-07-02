using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormSW_Tree
{
    public partial class InfoF : Form
    {
        private DataGridView dataGridView;
        private TabControl tabControl;
        private TabPage tab1;
        private TabPage tab2;
        private TabPage tab3;
        private ListBox lbLogger;
        private Button btnRecordToFile;
        private CheckBox chB_ToRebuild;
        private CheckBox chB_Clean;
        private CheckBox checkBox1;
        private CheckBox chB_Impossible;
        private CheckBox chB_IsVisible;
        private CheckBox chB_IsAll;
        private Label lbMsg;
        private ProgressBar progressBar1;
        private Label lbStart;
        private Label lbCount;
        private Label lbNumber;
        private ListBox lv;
        private Button button1;
        public InfoF()
        {
            InitializeComponent();
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            GenerateTabOne();          
            GenerateTabTwo();         
            GenerateTabThree();
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            tabControl.Selecting += TabControl_Selecting;
            numberLogger = 0;

        }

        private void TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 1 && stateFopm==StateFopm.Init)
            {
                e.Cancel = true; 
            }
            if (e.TabPageIndex == 0 && stateFopm == StateFopm.Display)
            {
                e.Cancel = true;
            }
            if (e.TabPageIndex == 2 && stateFopm == StateFopm.Init)
            {
                e.Cancel = true;
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            TabPage selectedTab = tabControl.SelectedTab;
            switch (selectedTab.Text)
            {
                case "Process":
                    this.Height = 300;
                    this.Width = 500;
                    break;
                case "Data":
                    SetDispleyTabTwo();
                    break;
                case "Loger":
                    this.Width = 500;
                    SetDispleyListBox();
                    break;
                default:
                    break;
            }
            selectedTab.Invalidate();
        }

        private void GenerateTabOne()
        {

            tab1 = new TabPage("Process");
            tab1.Dock = DockStyle.Fill;
            GenerateLblMsg();
            progressBar1 = new ProgressBar();
            progressBar1.Location = new Point(100, 100);
            progressBar1.Width = 250;
            progressBar1.Height = 15;
            tab1.Controls.Add(progressBar1);

            lbStart = new Label();
            lbStart.Text = "0";
            lbStart.Location = new Point(50, 100);
            tab1.Controls.Add(lbStart);

            lbCount = new Label();         
            lbCount.Text = "0";
            lbCount.Location = new Point(370, 100);
            tab1.Controls.Add(lbCount);

            lbNumber = new Label();
            lbNumber.Text = "...";
            lbNumber.Width = 200;
            lbNumber.Location = new Point(100, 80);
            tab1.Controls.Add(lbNumber);  
            tabControl.TabPages.Add(tab1);
        }
        private void GenerateTabThree()
        {
            tab3 = new TabPage("Loger");
            lbLogger = new ListBox();
            lbLogger.Dock = DockStyle.Fill;
            lbLogger.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            lbLogger.DrawMode = DrawMode.OwnerDrawFixed;
            lbLogger.MultiColumn = false;
            lbLogger.DrawItem += LbLogger_DrawItem;
            tab3.Controls.Add(lbLogger);

            btnRecordToFile = new Button();
            btnRecordToFile.Name = "btnRecordToFile";
            btnRecordToFile.Text = "btnRecordToFile";
            btnRecordToFile.Location = new Point(0, 0);
            btnRecordToFile.Click += BtnRecordToFile_Click;
            tab3.Controls.Add(btnRecordToFile);
            tabControl.TabPages.Add(tab3);
            this.Controls.Add(tabControl);
        }
        private void GenerateTabTwo()
        {
            tab2 = new TabPage("Data");
            tab2.Dock = DockStyle.Fill;
            GenerateNamedCheckBoxes();
            GenerateDataGridView();
            tabControl.TabPages.Add(tab2);
        }   
        private void GenerateDataGridView()
        {
            dataGridView = new DataGridView();
            dataGridView.Location = new Point(0, 75);
            dataGridView.BackgroundColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            
            dataGridView.AutoGenerateColumns = true;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.BackgroundColor = Color.White; 
            dataGridView.BorderStyle = BorderStyle.None; 
            dataGridView.GridColor = Color.White; 

            dt = new DataTable();
            dt.Columns.Add("Cuby_Number", typeof(string));
            dt.Columns.Add("Type", typeof(byte[]));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("PDM", typeof(string));
            dt.Columns.Add("IsState", typeof(byte[]));
            dt.Columns.Add("IsLocked", typeof(string));
            dt.Columns.Add("DrawType", typeof(byte[]));
            dt.Columns.Add("DrawState", typeof(string));
            dt.Columns.Add("DrawVersRev", typeof(string));
            dt.Columns.Add("NeedsRebuild", typeof(string));
            dt.Columns.Add("IsStateDraw", typeof(byte[]));
            dt.Columns.Add("DrawIsLocked", typeof(string));

            dataGridView.DataSource = dt;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;      
            dataGridView.DataBindingComplete += (sender, e) =>
            {
                SetDispleyTabTwo();
            };
            tab2.Controls.Add(dataGridView);
        }

        private void SetDispleyTabTwo()
        {
            int rowHeight1 = dataGridView.RowTemplate.Height;
            int headerHeight1 = dataGridView.ColumnHeadersHeight;
            int rowsHeight1 = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
            int totalHeight1 = rowsHeight1 + headerHeight1;
            int maxAllowedHeight1 = 500;
            int desiredHeight1 = Math.Min(totalHeight1, maxAllowedHeight1);
            int minHeight1 = rowHeight1 + headerHeight1;

            dataGridView.Height = Math.Max(desiredHeight1, minHeight1);
            this.Height = 200 + dataGridView.Height;
            int totalColumnsWidth = 0;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                totalColumnsWidth += column.Width;
            }
            dataGridView.Width = totalColumnsWidth + 50;
            this.Width = totalColumnsWidth + 75;
        }

        private void GenerateNamedCheckBoxes()
        {
            lv = new ListBox();
            lv.Top = 0;
            lv.Left = 0;
            lv.Width = 500;
            lv.Height = 50;
            lv.ControlAdded += Lv_ControlAdded;
            tab2.Controls.Add(lv);

            chB_IsVisible = new CheckBox();
            chB_IsVisible.Checked = false;
            chB_IsVisible.Text = "IsVisible";
            chB_IsVisible.Name = "chB_IsVisible";
            chB_IsVisible.Location = new Point(525, 15);
            chB_IsVisible.Size = new Size(100, 20);
            chB_IsVisible.CheckedChanged += ChB_IsVisible_CheckedChanged;
            tab2.Controls.Add(chB_IsVisible);

            chB_IsAll = new CheckBox();
            chB_IsAll.Checked = false;
            chB_IsAll.Text = "IsAll";
            chB_IsAll.Name = "chB_IsAll";
            chB_IsAll.Location = new Point(700, 15);
            chB_IsAll.Size = new Size(100, 20);
            chB_IsAll.CheckedChanged += ChB_IsAll_CheckedChanged; ;
            tab2.Controls.Add(chB_IsAll);

            chB_ToRebuild = new CheckBox();
            chB_ToRebuild.Checked = true;
            chB_ToRebuild.Text = "To Rebuild";
            chB_ToRebuild.Name = "chB_ToRebuild";
            chB_ToRebuild.Location = new Point(50, lv.Bottom + 5);
            chB_ToRebuild.Size = new Size(100, 20);
            chB_ToRebuild.CheckedChanged += ChB_ToRebuild_CheckedChanged;
            tab2.Controls.Add(chB_ToRebuild);

            chB_Clean = new CheckBox();
            chB_Clean.Text = "Clean";
            chB_Clean.Name = "chB_Clean";
            chB_Clean.Location = new Point(170, lv.Bottom + 5);
            chB_Clean.Size = new Size(100, 20);
            chB_Clean.CheckedChanged += ChB_Clean_CheckedChanged;
            tab2.Controls.Add(chB_Clean);

            checkBox1 = new CheckBox();
            checkBox1.Text = "Blocked";
            checkBox1.Name = "checkBox1";
            checkBox1.Location = new Point(290, lv.Bottom + 5);
            checkBox1.Size = new Size(100, 20);
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            tab2.Controls.Add(checkBox1);

            chB_Impossible = new CheckBox();
            chB_Impossible.Text = "Manufacturing";
            chB_Impossible.Name = "chB_Impossible";
            chB_Impossible.Location = new Point(410, lv.Bottom + 5);
            chB_Impossible.Size = new Size(100, 20);
            chB_Impossible.CheckedChanged += ChB_Impossible_CheckedChanged;
            tab2.Controls.Add(chB_Impossible);

            button1 = new Button();
            button1.Text = "&Rebuild";
            button1.Location = new Point(800, 15);
            button1.Click += Button1_Click;
            tab2.Controls.Add(button1);

        }

        private void ChB_IsAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isAll = true;
            }
            else
            {
                isAll = false;
            }
            RefreshForm();
        }

        private void GenerateLblMsg()
        {
            lbMsg = new Label();
            lbMsg.Text = "...";
            lbMsg.Location = new Point(20, 10);
            lbMsg.Width = 300;
            lbMsg.ForeColor = Color.Green;
            lbMsg.AutoSize = true;
            tab1.Controls.Add(lbMsg);
        }
        private void Lv_ControlAdded(object sender, ControlEventArgs e)
        {
            int totalItemHeight = 0;
            foreach (object item in lv.Items)
            {
                totalItemHeight += TextRenderer.MeasureText(item.ToString(), lv.Font).Height;
            }
            int requiredHeight = totalItemHeight + lv.Items.Count * 3;
            lv.Height = requiredHeight;
        }
        private void ChB_ToRebuild_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isDispleyRebuild = true;
            }
            else
            {
                isDispleyRebuild = false;

            }
            RefreshForm();
        }
        private void ChB_Clean_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isClean = true;
            }
            else
            {
                isClean = false;
            }
            RefreshForm();
        }
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isBlocked = true;
            }
            else
            {
                isBlocked = false;
            }
            RefreshForm();
        }
        private void ChB_Impossible_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isImpossible = true;
            }
            else
            {
                isImpossible = false;
            }
            RefreshForm();
        }
        private void ChB_IsVisible_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isVisible = true;
            }
            else
            {
                isVisible = false;
            }
            RefreshForm();
        }
        private byte[] GetImageData(int i)
        {

            Image image;
            switch (i)
            {
                case 0:
                    image = Properties.Resources.part_bmp;
                    break;
                case 1:
                    image = Properties.Resources.assembly_bmp;
                    break;
                case 2:
                    image = Properties.Resources.Drawings;
                    break;
                case 3:
                    image = Properties.Resources.free_icon_ok;
                    break;
                case 4:
                    image = Properties.Resources.free_icon_repairing;
                    break;
                case 5:
                    image = Properties.Resources.closed;
                    break;
                case 6:
                    image = Properties.Resources.Warning;
                    break;
                case 7:
                    image = Properties.Resources.x1;
                    break;
                case 8:
                    image = Properties.Resources.bolts;
                    break;
                default:
                    image = Properties.Resources.empty;
                    break;
            }
        
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        private void LbLogger_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();

            // Получаем текст элемента и цвет из объекта
            var item = (dynamic)lbLogger.Items[e.Index];
            string itemText = item.Text;
            Color itemColor = item.Color;

            // Создаем кисть для текста нужного цвета
            Brush brush = new SolidBrush(itemColor);

            // Рисуем текст элемента
            e.Graphics.DrawString(itemText, e.Font, brush, e.Bounds);

            // Освобождаем ресурсы
            brush.Dispose();
        }
        private void AddItemWithColor(string text, Color color)
        {
            var item = new { Text = text, Color = color };
            lbLogger.Items.Add(item);
        }
        private void SetDispleyListBox()
        {
            int itemHeight = lbLogger.ItemHeight; 
            int totalItemsHeight = lbLogger.Items.Count * itemHeight; 
            int maxHeight = Math.Min(totalItemsHeight, 800);
            lbLogger.Height = maxHeight;
          
            this.Height = maxHeight + 100;
        }
        private void BtnRecordToFile_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    DateTime now = DateTime.Now;
                    string formattedDateTime = now.ToString("yyyy-MM-dd HH-mm");
                    string nameFile = this.Text.Substring(0,13)  + "-" + formattedDateTime + "-Log.txt";
                    string filePath = Path.Combine(folderDialog.SelectedPath, nameFile);
                    int startIndex;
                    int endIndex;
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (var item in lbLogger.Items)
                        {
                            string line = item.ToString();
                            startIndex = line.IndexOf("= ") + 2;
                            endIndex = line.LastIndexOf(",");
                            string text = line.Substring(startIndex, endIndex - startIndex).Trim();
                            writer.WriteLine(text);
                        }
                    }

                    MessageBox.Show("ListBox items saved to file: " + filePath, "File Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

    }
}
