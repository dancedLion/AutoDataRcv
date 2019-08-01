using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CHQ.RD.ConnectorBase;
using CHQ.RD.ConnectorRunTime;
namespace Console
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            AutoUpdate.AutoUpdater upd = new AutoUpdate.AutoUpdater();
            upd.LoadUpdate();
            
            Application.EnableVisualStyles();
           
            Application.Run(new Form1());
        }
    }
}
