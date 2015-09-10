namespace WindowsFormsApplication1
{
    partial class Form1
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
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.txtDataset = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdProcessSingle = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdWrite = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdCompare = new System.Windows.Forms.Button();
            this.txtChanges = new System.Windows.Forms.TextBox();
            this.cmdBrowseDC = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBase = new System.Windows.Forms.TextBox();
            this.cmdBrowseCO = new System.Windows.Forms.Button();
            this.lblWait = new System.Windows.Forms.Label();
            this.ckbReload = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(444, 33);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(75, 23);
            this.cmdBrowse.TabIndex = 0;
            this.cmdBrowse.Text = "Browse";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // txtDataset
            // 
            this.txtDataset.Location = new System.Drawing.Point(80, 36);
            this.txtDataset.Name = "txtDataset";
            this.txtDataset.Size = new System.Drawing.Size(358, 20);
            this.txtDataset.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "CSV Dataset";
            // 
            // cmdProcessSingle
            // 
            this.cmdProcessSingle.Location = new System.Drawing.Point(525, 33);
            this.cmdProcessSingle.Name = "cmdProcessSingle";
            this.cmdProcessSingle.Size = new System.Drawing.Size(75, 23);
            this.cmdProcessSingle.TabIndex = 3;
            this.cmdProcessSingle.Text = "Process";
            this.cmdProcessSingle.UseVisualStyleBackColor = true;
            this.cmdProcessSingle.Click += new System.EventHandler(this.cmdProcessSingle_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmdProcessSingle);
            this.groupBox1.Controls.Add(this.txtDataset);
            this.groupBox1.Controls.Add(this.cmdBrowse);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(758, 81);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Single I/O";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ckbReload);
            this.groupBox2.Controls.Add(this.lblWait);
            this.groupBox2.Controls.Add(this.cmdWrite);
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cmdCompare);
            this.groupBox2.Controls.Add(this.txtChanges);
            this.groupBox2.Controls.Add(this.cmdBrowseDC);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtBase);
            this.groupBox2.Controls.Add(this.cmdBrowseCO);
            this.groupBox2.Location = new System.Drawing.Point(13, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(757, 248);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Compare DataSets";
            // 
            // cmdWrite
            // 
            this.cmdWrite.Location = new System.Drawing.Point(540, 52);
            this.cmdWrite.Name = "cmdWrite";
            this.cmdWrite.Size = new System.Drawing.Size(165, 23);
            this.cmdWrite.TabIndex = 13;
            this.cmdWrite.Text = "Write Compare Difference";
            this.cmdWrite.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(9, 81);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(742, 161);
            this.dataGridView1.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "CSV Dataset(Changes)";
            // 
            // cmdCompare
            // 
            this.cmdCompare.Location = new System.Drawing.Point(459, 52);
            this.cmdCompare.Name = "cmdCompare";
            this.cmdCompare.Size = new System.Drawing.Size(75, 23);
            this.cmdCompare.TabIndex = 11;
            this.cmdCompare.Text = "Compare";
            this.cmdCompare.UseVisualStyleBackColor = true;
            this.cmdCompare.Click += new System.EventHandler(this.cmdCompare_Click);
            // 
            // txtChanges
            // 
            this.txtChanges.Location = new System.Drawing.Point(123, 53);
            this.txtChanges.Name = "txtChanges";
            this.txtChanges.Size = new System.Drawing.Size(249, 20);
            this.txtChanges.TabIndex = 9;
            // 
            // cmdBrowseDC
            // 
            this.cmdBrowseDC.Location = new System.Drawing.Point(378, 52);
            this.cmdBrowseDC.Name = "cmdBrowseDC";
            this.cmdBrowseDC.Size = new System.Drawing.Size(75, 23);
            this.cmdBrowseDC.TabIndex = 8;
            this.cmdBrowseDC.Text = "Browse";
            this.cmdBrowseDC.UseVisualStyleBackColor = true;
            this.cmdBrowseDC.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Base CSV (Older)";
            // 
            // txtBase
            // 
            this.txtBase.Location = new System.Drawing.Point(123, 28);
            this.txtBase.Name = "txtBase";
            this.txtBase.Size = new System.Drawing.Size(249, 20);
            this.txtBase.TabIndex = 5;
            // 
            // cmdBrowseCO
            // 
            this.cmdBrowseCO.Location = new System.Drawing.Point(378, 25);
            this.cmdBrowseCO.Name = "cmdBrowseCO";
            this.cmdBrowseCO.Size = new System.Drawing.Size(75, 23);
            this.cmdBrowseCO.TabIndex = 4;
            this.cmdBrowseCO.Text = "Browse";
            this.cmdBrowseCO.UseVisualStyleBackColor = true;
            this.cmdBrowseCO.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // lblWait
            // 
            this.lblWait.AutoSize = true;
            this.lblWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWait.Location = new System.Drawing.Point(233, 76);
            this.lblWait.Name = "lblWait";
            this.lblWait.Size = new System.Drawing.Size(266, 46);
            this.lblWait.TabIndex = 14;
            this.lblWait.Text = "Please Wait...";
            this.lblWait.Visible = false;
            // 
            // ckbReload
            // 
            this.ckbReload.AutoSize = true;
            this.ckbReload.Location = new System.Drawing.Point(459, 28);
            this.ckbReload.Name = "ckbReload";
            this.ckbReload.Size = new System.Drawing.Size(133, 17);
            this.ckbReload.TabIndex = 15;
            this.ckbReload.Text = "Reload Data from CSV";
            this.ckbReload.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 370);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Gatineau2OSM";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.TextBox txtDataset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdProcessSingle;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdWrite;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdCompare;
        private System.Windows.Forms.TextBox txtChanges;
        private System.Windows.Forms.Button cmdBrowseDC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBase;
        private System.Windows.Forms.Button cmdBrowseCO;
        private System.Windows.Forms.Label lblWait;
        private System.Windows.Forms.CheckBox ckbReload;
    }
}

