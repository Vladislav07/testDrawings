﻿namespace FormSW_Tree
{
    partial class InfoForm
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
            this.btnGetInfo = new System.Windows.Forms.Button();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.chboxIncludeDraw = new System.Windows.Forms.CheckBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView.Location = new System.Drawing.Point(0, 46);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(916, 383);
            this.dataGridView.TabIndex = 0;
            // 
            // btnGetInfo
            // 
            this.btnGetInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGetInfo.Location = new System.Drawing.Point(22, 17);
            this.btnGetInfo.Name = "btnGetInfo";
            this.btnGetInfo.Size = new System.Drawing.Size(100, 23);
            this.btnGetInfo.TabIndex = 1;
            this.btnGetInfo.Text = "&GetInfo";
            this.btnGetInfo.UseVisualStyleBackColor = true;
            this.btnGetInfo.Click += new System.EventHandler(this.btnGetInfo_Click);
            // 
            // btnRebuild
            // 
            this.btnRebuild.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRebuild.Location = new System.Drawing.Point(128, 17);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(126, 23);
            this.btnRebuild.TabIndex = 2;
            this.btnRebuild.Text = "&RebuildTree";
            this.btnRebuild.UseVisualStyleBackColor = true;
            // 
            // chboxIncludeDraw
            // 
            this.chboxIncludeDraw.AutoSize = true;
            this.chboxIncludeDraw.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chboxIncludeDraw.Location = new System.Drawing.Point(761, 17);
            this.chboxIncludeDraw.Name = "chboxIncludeDraw";
            this.chboxIncludeDraw.Size = new System.Drawing.Size(103, 20);
            this.chboxIncludeDraw.TabIndex = 3;
            this.chboxIncludeDraw.Text = "&IncludeBox";
            this.chboxIncludeDraw.UseVisualStyleBackColor = true;
            this.chboxIncludeDraw.CheckedChanged += new System.EventHandler(this.chboxIncludeDraw_CheckedChanged);
            // 
            // InfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 429);
            this.Controls.Add(this.chboxIncludeDraw);
            this.Controls.Add(this.btnRebuild);
            this.Controls.Add(this.btnGetInfo);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "InfoForm";
            this.Text = "InfoForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnGetInfo;
        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.CheckBox chboxIncludeDraw;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}