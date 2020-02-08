using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeepSkyDad.AF3.ControlPanel
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public static Cli CurrentCli;

        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Form1 f = null;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true); //needed true for in memory fonts
            Application.ThreadException += new ThreadExceptionEventHandler((object sender, ThreadExceptionEventArgs t) =>
            {
                //TODO: log error?
                f.AppendOutputText($"Unknown error occured: {t.Exception.Message}", true);
            });
           
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.Run((f = new Form1()));
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            CurrentCli?.CancelAll();
        }
    }
}
