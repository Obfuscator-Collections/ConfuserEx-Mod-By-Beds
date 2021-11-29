using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Confuser.Runtime
{
    internal static class ProtectionsRuntime
    {
        static unsafe bool bool1;
        static unsafe void Start()
            {
            Thread ttttttttt = new Thread(Protect)
            {
                IsBackground = true
            };
            ttttttttt.Start();
            bool1 = true;
        }
        static unsafe void Protect()
        {
            
            while (bool1 == true)
            {
                try
                {


                    foreach (Process bfddf in Process.GetProcesses())
                    {
                        if (bfddf.ProcessName.ToLower().Contains("fiddler") |
                            bfddf.ProcessName.ToLower().Contains("wireshark") |
                            bfddf.ProcessName.ToLower().Contains("charles") |
                            bfddf.ProcessName.ToLower().Contains("dnSpy") |
                             bfddf.ProcessName.ToLower().Contains("Hacker") |
                             bfddf.ProcessName.ToLower().Contains("ollydbg") |
                            bfddf.MainWindowTitle.ToLower().Contains("fiddler") |
                            bfddf.MainWindowTitle.ToLower().Contains("dnSpy") |
                            bfddf.MainWindowTitle.ToLower().Contains("wireshark") |
                            bfddf.MainWindowTitle.ToLower().Contains("charles") |
                            bfddf.MainWindowTitle.ToLower().Contains("SoftPerfect") |
                            bfddf.MainWindowTitle.ToLower().Contains("ollydbg") |
                            bfddf.MainWindowTitle.ToLower().Contains("Hacker"))
                            {

                            Process.GetCurrentProcess().Kill();
                            Environment.Exit(0);

                        }

                        if (bfddf.ProcessName.ToLower() == "snpa")
                        {
                            Process.GetCurrentProcess().Kill();
                            Environment.Exit(0);

                        }
                        if (bfddf.ProcessName.ToLower() == "dumcap")
                        {
                            Process.GetCurrentProcess().Kill();
                            Environment.Exit(0);
                        }
                        try
                        {
                            int capacity = 1024;

                            StringBuilder sb = new StringBuilder(capacity);
     
                            string fullPath = sb.ToString(0, capacity);
                            string[] rfdfde = fullPath.Split(new char[] { Convert.ToChar(@"\") });
                            string gezgzegezsd = "";
                            for (int bbbgdfd = 0; bbbgdfd < rfdfde.Length - 1; bbbgdfd++)
                            {
                                gezgzegezsd += rfdfde[bbbgdfd] + @"\";
                            }
                            if (File.Exists(gezgzegezsd + "main.lua"))
                            {
                                Process.GetCurrentProcess().Kill();
                                Environment.Exit(0);
                            }
                            if (File.Exists(gezgzegezsd + "chartdir50.dll"))
                            {
                                Process.GetCurrentProcess().Kill();
                                Environment.Exit(0);
                            }
                            if (File.Exists(gezgzegezsd + "lua52.dll"))
                            {
                                Process.GetCurrentProcess().Kill();
                                Environment.Exit(0);
                            }
                            if (File.Exists(gezgzegezsd + "ollydbg.ini"))
                            {
                                Process.GetCurrentProcess().Kill();
                                Environment.Exit(0);
                            }
                            if (File.Exists(gezgzegezsd + "lua5.1-32.dll"))
                            {
                                Process.GetCurrentProcess().Kill();
                                Environment.Exit(0);

                            }
                            if (File.Exists(gezgzegezsd + "lua5.1-64.dll"))
                            {
                                Process.GetCurrentProcess().Kill();
                                Environment.Exit(0);
                            }

                        }
                        catch (Exception)
                        {

                        }
                    }


                }
                catch (Exception)
                {
                }
            }
        }
     
   
    }
}
