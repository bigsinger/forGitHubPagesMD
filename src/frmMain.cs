using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace forGitHubPagesMD
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            loadCats();
            loadTags();
        }

        private bool loadCats()
        {
            this.cmbCat.Items.Clear();
            string sTags = ConfigHelper.GetAppConfig("cat");
            string[] catList = sTags.Split(' ');
            foreach (string cat in catList)
            {
                if (string.IsNullOrEmpty(cat) == false)
                {
                    this.cmbCat.Items.Add(cat);
                }
            }

            return true;
        }

        private bool loadTags()
        {
            this.ctrlTags.Items.Clear();
            string sTags = ConfigHelper.GetAppConfig("tag");
            string[] tagList = sTags.Split(' ');
            foreach (string tag in tagList)
            {
                if (string.IsNullOrEmpty(tag)==false)
                {
                    this.ctrlTags.Items.Add(tag);
                }
            }

            return true;
        }

    
        private void rtLog_TextChanged_1(object sender, EventArgs e)
        {
            this.rtLog.SelectionStart = this.rtLog.TextLength;
            // Scrolls the contents of the control to the current caret position.
            this.rtLog.ScrollToCaret();
        }


        #region 日志记录、支持其他线程访问 参考：C#在RichTextBox中显示不同颜色文字的方法 http://www.jb51.net/article/69791.htm
        public delegate void LogAppendDelegate(string text, Color color);
        private void _log(string text, Color color)
        {
            this.rtLog.SelectionColor = color;
            this.rtLog.AppendText(text + "\n");
        }

        private void log(string text, Color color)
        {
            LogAppendDelegate la = new LogAppendDelegate(_log); 
            this.rtLog.Invoke(la, text, color);
        }

        private void LOGD(string text)
        {
            log(text, Color.Black);
        }

        private void LOGW(string text)
        {
            log(text, Color.Orange);
        }

        private void LOGE(string text)
        {
            log(text, Color.Red);
        }
        #endregion

        private void btnStart_Click(object sender, EventArgs e)
        {
            string des = "";
            string title = "";
            string time = DateTime.Now.ToString("yyyy-MM-dd");  // 2008-09-04

            title = time + "-" + this.txtTitle.Text;
            LOGD(title + "\r\n");

            string tags = "";
            
            int nTagsCount = this.ctrlTags.CheckedItems.Count;
            if ( nTagsCount > 0 )
	        {
                tags = "[";
                for (int i = 0; i < nTagsCount; i++)
			    {
                    tags += this.ctrlTags.CheckedItems[i].ToString();
    			    if (i!=nTagsCount - 1)
	                {
                		tags += ","; 
	                }
	            }
                tags += "]";
            }
            else
            {
                tags = "[]";
            }


            des = string.Format(
                "---\r\nlayout:\t\tpost\r\ncategory:\t\"{0}\"\r\ntitle:\t\t\"{1}\"\r\ntags:\t\t{2}\r\n---\r\n", 
                this.cmbCat.Text, this.txtTitle.Text, tags);
            LOGD(des);
            Clipboard.SetDataObject(des);

            string sDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            CreateMDFile(Path.Combine(sDesktopPath, title + ".md"), des);
        }

        private void CreateMDFile(string sFileName, string sMDDes)
        {
            if (File.Exists(sFileName))
            {
                DialogResult ret = MessageBox.Show(sFileName + "\n\n文件已经存在，是否替换？", "", MessageBoxButtons.YesNo);
                if (ret==DialogResult.No)
                {
                    return;
                }
            }

            FileStream fs = new FileStream(sFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.Write(sMDDes);
            sw.Flush();
            fs.Close();
            LOGD("已创建文件：" + sFileName);
        }

      
    }
}
