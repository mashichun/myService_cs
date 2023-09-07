using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myService
{
    public partial class Service1 : ServiceBase
    {

        PowerBroadcastStatus status ;
        public Service1()
        {
            InitializeComponent();

        }

        private void GetPowerStatus(StreamWriter writer) {
          //  StreamWriter writer = new StreamWriter(filePath, true);
            Type t = typeof(PowerStatus);
            PropertyInfo[] p3 = t.GetProperties();
            for (int i = 0; i < 5; i++)
            {
                writer.WriteLine("name：" + p3[i].Name);
                writer.WriteLine("value：" + p3[i].GetValue(SystemInformation.PowerStatus, null));
                //   writer.WriteLine("contens：" + pi[i].GetValue(pi[i].));
            }
        }
        string filePath = "C:\\shutdownlog.txt";
        protected override void OnStart(string[] args)
        {
            StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine(DateTime.Now.ToString("yy-MM-dd HH:mm:ss")+"：start service");
            GetPowerStatus(writer);
            writer.Flush();
            writer.Close();
            
        }
        public new virtual bool CanHandlePowerEvent { get; set; }
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            status = powerStatus;
            Type type = powerStatus.GetType();
            PropertyInfo[] pi = type.GetProperties();

            StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine("check1："+powerStatus);
            writer.WriteLine("check2：" + pi);
            writer.Flush();
            writer.Close();
            bool bo = base.OnPowerEvent(powerStatus);
            Console.Beep();
            return bo;
        }
        protected override void OnStop()
        {
            
            StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine(DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "：stop service");
            GetPowerStatus(writer);
            writer.Flush();
            writer.Close();
        }
        
        protected override void OnShutdown()///
		{
            string msg = System.Environment.MachineName + "-" + System.Environment.UserName + "at " + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "shutdown event";
            StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine(msg);
            writer.Flush();
            writer.Close();
            base.OnShutdown();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine(DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "：timer");
            GetPowerStatus(writer);
            writer.Flush();
            writer.Close();
            //     GetPowerStatus();
        }
        /* for get power status
* refer MSDN: http://msdn.microsoft.com/en-us/library/system.windows.forms.powerstatus.aspx
*/

    }
}
