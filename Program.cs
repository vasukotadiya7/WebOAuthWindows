using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace WebOAuth
{
    internal static class Program
    {
        private static Form1 form;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*if (!string.IsNullOrEmpty(Properties.Settings.Default.UserUID))
            {
                MessageBox.Show($"Welcome back! ", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Load user data or proceed to main form
                Validate validate = new Validate("none", Properties.Settings.Default.UserUID);
                validate.Show();
                
            }
            else
            {*/


                bool isCreatedNew;
                using (var mutex = new System.Threading.Mutex(true, "WebAuthDesktopApp", out isCreatedNew))
                {
                    if (isCreatedNew)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        string token = null;

                        // Check if any arguments were passed
                        if (args.Length > 0)
                        {
                            // Extract the token from the URL
                            var uri = new Uri(args[0]);
                            var query = HttpUtility.ParseQueryString(uri.Query);
                            token = query.Get("usertoken");
                        }

                        form = new Form1(token);
                        form.Visible = true;
                        Task.Run(() => StartIPCServer(form));
                        Application.Run(form);
                    }
                    else
                    {
                        SendArgsToRunningInstance(args);
                    }
                }
            /*}*/
        }

        private static void StartIPCServer(Form1 form)
        {
            while (true)
            {
                using (var server = new NamedPipeServerStream("WebAuthDesktopAppPipe", PipeDirection.In))
                {
                    server.WaitForConnection();

                    using (var reader = new StreamReader(server))
                    {
                        var args = reader.ReadLine();
                        if (!string.IsNullOrEmpty(args))
                        {
                            // Extract the token from the URL
                            var uri = new Uri(args);
                            var query = HttpUtility.ParseQueryString(uri.Query);
                            string token = query.Get("usertoken");

                            form?.UpdateToken(token);
                        }
                    }
                }
            }
        }

        private static void SendArgsToRunningInstance(string[] args)
        {
            using (var client = new NamedPipeClientStream(".", "WebAuthDesktopAppPipe", PipeDirection.Out))
            {
                client.Connect();

                using (var writer = new StreamWriter(client))
                {
                    writer.WriteLine(args[0]);
                }
            }
        }
    }
}
