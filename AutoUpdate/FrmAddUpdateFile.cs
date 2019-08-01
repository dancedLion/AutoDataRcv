using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace AutoUpdate
{
    public partial class FrmAddUpdateFile : Form
    {
        public FrmAddUpdateFile()
        {
            InitializeComponent();
        }
        private int m_result=-1;
        private  string m_file = "";
        private  AUFileInfo m_fileinfo;
        public string SelectedFile
        {
            get { return m_file; }
            set { m_file = value; }
        }
        public AUFileInfo SelectedFileInfo
        {
            get { return m_fileinfo; }
        }

        public int AddnewFile()
        {
            m_fileinfo = new AUFileInfo();
            toLoadFile();
            this.ShowDialog();
            return m_result;
        }
        public int EditFile(string filename,AUFileInfo fileinfo)
        {
            m_fileinfo = fileinfo;
            toLoadFileInfo(filename, fileinfo);
            this.ShowDialog();
            return m_result;
        }
        #region local methods
        void toLoadFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                toLoadFileInfo(openFileDialog1.FileName);
            }
        }
        void toLoadFileInfo(string filename)
        {
            tbxFile.Text = filename;
            try
            {
                FileInfo finfo = new FileInfo(tbxFile.Text);
                tbxFileId.Text = finfo.Name;
                tbxFileName.Text = finfo.Name;
                tbxFileSize.Text = finfo.Length.ToString();
                System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(tbxFile.Text);
                tbxFileVersion.Text = info.FileVersion;
                //tbxFileId.Text = info.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void toLoadFileInfo(string filename,AUFileInfo fileinfo)
        {
            try
            {
                toLoadFileInfo(filename);
                tbxFileVersion.Text = fileinfo.FileVersion;
                tbxFilePath.Text = fileinfo.FilePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void toSave()
        {
            try
            {
                m_file = tbxFile.Text;
                m_fileinfo.FileName = tbxFileName.Text;
                m_fileinfo.FileSize = int.Parse(tbxFileSize.Text);
                m_fileinfo.FilePath = tbxFilePath.Text;
                m_fileinfo.FileVersion = tbxFileVersion.Text;
                m_fileinfo.FileId = tbxFileId.Text;
                m_result = 0;
                this.DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void toExit()
        {
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toSave();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toExit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toLoadFile();
        }
    }
}
