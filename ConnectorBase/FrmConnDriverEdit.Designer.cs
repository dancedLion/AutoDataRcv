namespace CHQ.RD.ConnectorBase
{
    partial class FrmConnDriverEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConnDriverEdit));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btntestdriver = new System.Windows.Forms.Button();
            this.btnselectdriverclass = new System.Windows.Forms.Button();
            this.btneditdriversetting = new System.Windows.Forms.Button();
            this.cbxdriverreadmode = new System.Windows.Forms.ComboBox();
            this.cbxdriversendmode = new System.Windows.Forms.ComboBox();
            this.cbxconndriversendmode = new System.Windows.Forms.ComboBox();
            this.cboconndriverreadmode = new System.Windows.Forms.ComboBox();
            this.tbxconndriverreadinterval = new System.Windows.Forms.TextBox();
            this.tbxdriverreadinterval = new System.Windows.Forms.TextBox();
            this.tbxdriverhost = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbxdriverclass = new System.Windows.Forms.TextBox();
            this.tbxname = new System.Windows.Forms.TextBox();
            this.tbxid = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.vwDataItem = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vwDataItem)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripSeparator2,
            this.toolStripButton4});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton1.Text = "保存";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton2.Text = "增行";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton3.Text = "删行";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton4.Text = "退出";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 425);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btntestdriver);
            this.tabPage1.Controls.Add(this.btnselectdriverclass);
            this.tabPage1.Controls.Add(this.btneditdriversetting);
            this.tabPage1.Controls.Add(this.cbxdriverreadmode);
            this.tabPage1.Controls.Add(this.cbxdriversendmode);
            this.tabPage1.Controls.Add(this.cbxconndriversendmode);
            this.tabPage1.Controls.Add(this.cboconndriverreadmode);
            this.tabPage1.Controls.Add(this.tbxconndriverreadinterval);
            this.tabPage1.Controls.Add(this.tbxdriverreadinterval);
            this.tabPage1.Controls.Add(this.tbxdriverhost);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.tbxdriverclass);
            this.tabPage1.Controls.Add(this.tbxname);
            this.tabPage1.Controls.Add(this.tbxid);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 399);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "主要";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btntestdriver
            // 
            this.btntestdriver.Location = new System.Drawing.Point(674, 174);
            this.btntestdriver.Name = "btntestdriver";
            this.btntestdriver.Size = new System.Drawing.Size(48, 23);
            this.btntestdriver.TabIndex = 34;
            this.btntestdriver.Text = "测试";
            this.btntestdriver.UseVisualStyleBackColor = true;
            // 
            // btnselectdriverclass
            // 
            this.btnselectdriverclass.Location = new System.Drawing.Point(615, 16);
            this.btnselectdriverclass.Name = "btnselectdriverclass";
            this.btnselectdriverclass.Size = new System.Drawing.Size(48, 23);
            this.btnselectdriverclass.TabIndex = 33;
            this.btnselectdriverclass.Text = "选择";
            this.btnselectdriverclass.UseVisualStyleBackColor = true;
            // 
            // btneditdriversetting
            // 
            this.btneditdriversetting.Location = new System.Drawing.Point(616, 174);
            this.btneditdriversetting.Name = "btneditdriversetting";
            this.btneditdriversetting.Size = new System.Drawing.Size(48, 23);
            this.btneditdriversetting.TabIndex = 32;
            this.btneditdriversetting.Text = "编辑";
            this.btneditdriversetting.UseVisualStyleBackColor = true;
            // 
            // cbxdriverreadmode
            // 
            this.cbxdriverreadmode.FormattingEnabled = true;
            this.cbxdriverreadmode.Location = new System.Drawing.Point(65, 138);
            this.cbxdriverreadmode.Name = "cbxdriverreadmode";
            this.cbxdriverreadmode.Size = new System.Drawing.Size(121, 20);
            this.cbxdriverreadmode.TabIndex = 31;
            // 
            // cbxdriversendmode
            // 
            this.cbxdriversendmode.FormattingEnabled = true;
            this.cbxdriversendmode.Location = new System.Drawing.Point(484, 138);
            this.cbxdriversendmode.Name = "cbxdriversendmode";
            this.cbxdriversendmode.Size = new System.Drawing.Size(121, 20);
            this.cbxdriversendmode.TabIndex = 30;
            // 
            // cbxconndriversendmode
            // 
            this.cbxconndriversendmode.FormattingEnabled = true;
            this.cbxconndriversendmode.Location = new System.Drawing.Point(484, 48);
            this.cbxconndriversendmode.Name = "cbxconndriversendmode";
            this.cbxconndriversendmode.Size = new System.Drawing.Size(121, 20);
            this.cbxconndriversendmode.TabIndex = 29;
            // 
            // cboconndriverreadmode
            // 
            this.cboconndriverreadmode.FormattingEnabled = true;
            this.cboconndriverreadmode.Location = new System.Drawing.Point(65, 48);
            this.cboconndriverreadmode.Name = "cboconndriverreadmode";
            this.cboconndriverreadmode.Size = new System.Drawing.Size(121, 20);
            this.cboconndriverreadmode.TabIndex = 28;
            // 
            // tbxconndriverreadinterval
            // 
            this.tbxconndriverreadinterval.Location = new System.Drawing.Point(284, 47);
            this.tbxconndriverreadinterval.Name = "tbxconndriverreadinterval";
            this.tbxconndriverreadinterval.Size = new System.Drawing.Size(123, 21);
            this.tbxconndriverreadinterval.TabIndex = 27;
            // 
            // tbxdriverreadinterval
            // 
            this.tbxdriverreadinterval.Location = new System.Drawing.Point(274, 137);
            this.tbxdriverreadinterval.Name = "tbxdriverreadinterval";
            this.tbxdriverreadinterval.Size = new System.Drawing.Size(123, 21);
            this.tbxdriverreadinterval.TabIndex = 26;
            // 
            // tbxdriverhost
            // 
            this.tbxdriverhost.Location = new System.Drawing.Point(65, 174);
            this.tbxdriverhost.Name = "tbxdriverhost";
            this.tbxdriverhost.Size = new System.Drawing.Size(540, 21);
            this.tbxdriverhost.TabIndex = 25;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 180);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 24;
            this.label14.Text = "设备主机：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(413, 145);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 23;
            this.label11.Text = "发送模式：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 145);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 22;
            this.label12.Text = "读取模式：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(216, 145);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 21;
            this.label13.Text = "读取间隔：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(413, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 20;
            this.label10.Text = "发送模式：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 19;
            this.label9.Text = "读取模式：";
            // 
            // tbxdriverclass
            // 
            this.tbxdriverclass.Location = new System.Drawing.Point(384, 17);
            this.tbxdriverclass.Name = "tbxdriverclass";
            this.tbxdriverclass.ReadOnly = true;
            this.tbxdriverclass.Size = new System.Drawing.Size(225, 21);
            this.tbxdriverclass.TabIndex = 12;
            // 
            // tbxname
            // 
            this.tbxname.Location = new System.Drawing.Point(160, 18);
            this.tbxname.Name = "tbxname";
            this.tbxname.Size = new System.Drawing.Size(175, 21);
            this.tbxname.TabIndex = 11;
            // 
            // tbxid
            // 
            this.tbxid.Location = new System.Drawing.Point(38, 18);
            this.tbxid.Name = "tbxid";
            this.tbxid.Size = new System.Drawing.Size(74, 21);
            this.tbxid.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "设备设置：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(216, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "读取间隔：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(341, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "驱动：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "名称：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "ID：";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.vwDataItem);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 399);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "变量";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // vwDataItem
            // 
            this.vwDataItem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.vwDataItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vwDataItem.Location = new System.Drawing.Point(3, 3);
            this.vwDataItem.Name = "vwDataItem";
            this.vwDataItem.RowTemplate.Height = 23;
            this.vwDataItem.Size = new System.Drawing.Size(786, 393);
            this.vwDataItem.TabIndex = 0;
            // 
            // FrmConnDriverEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FrmConnDriverEdit";
            this.Text = "驱动连接器编辑";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vwDataItem)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView vwDataItem;
        private System.Windows.Forms.TextBox tbxdriverclass;
        private System.Windows.Forms.TextBox tbxname;
        private System.Windows.Forms.TextBox tbxid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbxdriverhost;
        private System.Windows.Forms.ComboBox cboconndriverreadmode;
        private System.Windows.Forms.TextBox tbxconndriverreadinterval;
        private System.Windows.Forms.TextBox tbxdriverreadinterval;
        private System.Windows.Forms.ComboBox cbxdriverreadmode;
        private System.Windows.Forms.ComboBox cbxdriversendmode;
        private System.Windows.Forms.ComboBox cbxconndriversendmode;
        private System.Windows.Forms.Button btneditdriversetting;
        private System.Windows.Forms.Button btnselectdriverclass;
        private System.Windows.Forms.Button btntestdriver;
    }
}