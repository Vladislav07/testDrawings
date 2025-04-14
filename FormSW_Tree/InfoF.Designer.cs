namespace FormSW_Tree
{
    partial class InfoF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lbMsg = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.chB_Clean = new System.Windows.Forms.CheckBox();
            this.chB_Impossible = new System.Windows.Forms.CheckBox();
            this.chB_ToRebuild = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.DataSource = this.bindingSource1;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView.Location = new System.Drawing.Point(0, 93);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(933, 357);
            this.dataGridView.TabIndex = 0;
            // 
            // lbMsg
            // 
            this.lbMsg.AutoSize = true;
            this.lbMsg.Location = new System.Drawing.Point(513, 22);
            this.lbMsg.Name = "lbMsg";
            this.lbMsg.Size = new System.Drawing.Size(41, 13);
            this.lbMsg.TabIndex = 1;
            this.lbMsg.Text = "label1";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(14, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "&RebuildTree";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.chB_Clean);
            this.groupBox1.Controls.Add(this.chB_Impossible);
            this.groupBox1.Controls.Add(this.chB_ToRebuild);
            this.groupBox1.Location = new System.Drawing.Point(167, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 64);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display components in state";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(136, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(72, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "&Blocked";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chB_Clean
            // 
            this.chB_Clean.AutoSize = true;
            this.chB_Clean.Location = new System.Drawing.Point(0, 42);
            this.chB_Clean.Name = "chB_Clean";
            this.chB_Clean.Size = new System.Drawing.Size(117, 17);
            this.chB_Clean.TabIndex = 2;
            this.chB_Clean.Text = "&CleanOrBlocked";
            this.chB_Clean.UseVisualStyleBackColor = true;
            this.chB_Clean.CheckedChanged += new System.EventHandler(this.chB_Clean_CheckedChanged);
            // 
            // chB_Impossible
            // 
            this.chB_Impossible.AutoSize = true;
            this.chB_Impossible.Location = new System.Drawing.Point(136, 42);
            this.chB_Impossible.Name = "chB_Impossible";
            this.chB_Impossible.Size = new System.Drawing.Size(143, 17);
            this.chB_Impossible.TabIndex = 1;
            this.chB_Impossible.Text = "&ImpossibleToRebuild";
            this.chB_Impossible.UseVisualStyleBackColor = true;
            this.chB_Impossible.CheckedChanged += new System.EventHandler(this.chB_Impossible_CheckedChanged);
            // 
            // chB_ToRebuild
            // 
            this.chB_ToRebuild.AutoSize = true;
            this.chB_ToRebuild.Location = new System.Drawing.Point(0, 19);
            this.chB_ToRebuild.Name = "chB_ToRebuild";
            this.chB_ToRebuild.Size = new System.Drawing.Size(84, 17);
            this.chB_ToRebuild.TabIndex = 0;
            this.chB_ToRebuild.Text = "&ToRebuild";
            this.chB_ToRebuild.UseVisualStyleBackColor = true;
            this.chB_ToRebuild.CheckedChanged += new System.EventHandler(this.chB_ToRebuild_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(501, 66);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(400, 10);
            this.progressBar1.TabIndex = 4;
            // 
            // InfoF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 450);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbMsg);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "InfoF";
            this.Text = "InfoF";
            this.Load += new System.EventHandler(this.InfoF_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Label lbMsg;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chB_Clean;
        private System.Windows.Forms.CheckBox chB_Impossible;
        private System.Windows.Forms.CheckBox chB_ToRebuild;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}