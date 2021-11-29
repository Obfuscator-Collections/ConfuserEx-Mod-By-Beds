using Confuser.Core;
using Confuser.Core.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Rule = Confuser.Core.Project.Rule;

namespace Beds_Protector_GUI
{
    public partial class Form1 : Form, ILogger
    {
        CancellationTokenSource cancelSrc;
        DateTime begin;
        public Form1()
        {
            InitializeComponent();

        }

        private void thirteenButton1_Click(object sender, EventArgs e) => Environment.Exit(0);

        private void thirteenButton2_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        private void thirteenTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array array = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (array != null)
                {
                    string path = array.GetValue(0).ToString();
                    int num = path.LastIndexOf(".");
                    if (num != -1)
                    {
                        string extension = path.Substring(num).ToLower();
                        if (extension == ".exe" || extension == ".dll")
                        {
                            Activate();
                            thirteenTextBox1.Text = path;
                        }
                    }
                }
            } catch { }
        }

        private void thirteenButton5_Click(object sender, EventArgs e) => MessageBox.Show("Simply click browse and choose your file or drag/drop it into the test box , then choose protections and hit protect. Output will be in the \\ Confused folder of the input file");

        private void thirteenButton6_Click(object sender, EventArgs e)
        {

            if (thirteenTextBox1.Text == "")
                MessageBox.Show("Pick a file first goofball.");
            else
            {


                ConfuserProject proj = new ConfuserProject();
                proj.PluginPaths.Add(Directory.GetCurrentDirectory() + "\\KoiVM.Confuser.exe");
                proj.BaseDirectory = Path.GetDirectoryName(thirteenTextBox1.Text);
                proj.OutputDirectory = Path.Combine(Path.GetDirectoryName(thirteenTextBox1.Text) + @"\Confused"); //output directory

                //add a module to the project
                ProjectModule module = new ProjectModule(); //create a instance of ProjectModule
                module.Path = Path.GetFileName(thirteenTextBox1.Text); //sets the module name]
                proj.Add(module); //adds module to project
                Rule rule = new Rule("true", ProtectionPreset.None, false); //creates a Global Rule, with no preset and "true" patern, without inherit
                if (groupBox1.Enabled == true)
                {
                    if (antiTamper.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("anti tamper", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (antiDebug.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("anti debug", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (calli.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("Calli Protection", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (constants.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("constants", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (controlFlow.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("ctrl flow", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (invalidMetadat.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("invalid metadata", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (renamer.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("rename", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (refProxy.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("ref proxy", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (mildRefProxy.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("Clean ref proxy", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (moduleFlood.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("module flood", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (fakeNative.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("Fake Native", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (renameAssembly.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("Rename Module", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (renameAssembly.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("Mutations", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (local2field.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("local to field", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (hideMethods.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("Hide Methods", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (md5Checksum.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("MD5 Hash Check", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    if (eraseHeaders.Checked)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>("erase headers", SettingItemAction.Add);
                        rule.Add(protection);
                    }
                    proj.PluginPaths.Clear();
                }
                else if (fastVirt.Checked)
                {
                    string[] array = { "MD5 Hash Check", "erase headers", "virt", "Rename Module" };
                    foreach (string a in array)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>(a, SettingItemAction.Add);

                        rule.Add(protection);
                    }
                }
                else if (strongVirt.Checked)
                {

                    string[] array = {"MD5 Hash Check", "erase headers", "virt", "constants", "Clean ref proxy", "Rename Module","anti tamper" };
                    foreach (string a in array)
                    {
                        SettingItem<Protection> protection = new SettingItem<Protection>(a, SettingItemAction.Add);

                        rule.Add(protection);
                    }
                      
                    
                }
                
                proj.Rules.Add(rule); //add our Global rule to the project 

                XmlDocument doc = proj.Save(); //convert our project to xml document
                doc.Save("temp.crproj"); //save the xml document as a file


                Process.Start("Confuser.CLI.exe", "-n temp.crproj").WaitForExit();
                File.Delete("temp.crproj");
            
            }
        }

        private void thirteenButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog k = new OpenFileDialog();
            DialogResult result = k.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = k.FileName;
                thirteenTextBox1.Text = file;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (groupBox1.Enabled == true)
            if (checkBox4.Checked == true)
            {
                antiDebug.Checked = true;
                antiTamper.Checked = true;
                renameAssembly.Checked = true;
                renamer.Checked = true;
                constants.Checked = true;
                controlFlow.Checked = true;
                moduleFlood.Checked = true;
                refProxy.Checked = true;
                mildRefProxy.Checked = true;
                calli.Checked = true;
                mutations.Checked = true;
                local2field.Checked = true;
                eraseHeaders.Checked = true;
                hideMethods.Checked = true;
            }
            else
            {
                antiDebug.Checked = false;
                antiTamper.Checked = false;
                renameAssembly.Checked = false;
                renamer.Checked = false;
                constants.Checked = false;
                controlFlow.Checked = false;
                moduleFlood.Checked = false;
                refProxy.Checked = false;
                mildRefProxy.Checked = false;
                calli.Checked = false;
                mutations.Checked = false;
                local2field.Checked = false;
                hideMethods.Checked = false;
                eraseHeaders.Checked = false;
            }

        }

        private void thirteenButton4_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Simply click browse and choose your file \\ Or you may drag/drop your file , then choose protections and hit protect. Output will be in the \\ Confused folder of the input file");
        }

        private void thirteenTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void RunConfuser(ConfuserProject proj)
        {
            var parameters = new ConfuserParameters();
            parameters.Project = proj;
            if (File.Exists(Application.ExecutablePath))
                Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            parameters.Logger = this;

            richTextBox1.Text = "";

            cancelSrc = new CancellationTokenSource();
            begin = DateTime.Now;

            ConfuserEngine.Run(parameters, cancelSrc.Token)
                          .ContinueWith(_ => {
                              thirteenButton6.Text = "Protect";
                          });
        }

        public void Debug(string msg) => AppendToLog("[DEBUG] {0}", msg);

        public void DebugFormat(string format, params object[] args) => AppendToLog("[DEBUG] {0}", string.Format(format, args));

        public void EndProgress() { }

        public void Error(string msg) => AppendToLog("[ERROR] {0}", msg);

        public void ErrorException(string msg, Exception ex)
        {
            AppendToLog("[ERROR] {0}", msg);
            AppendToLog("Exception: {0}", ex);
        }

        public void ErrorFormat(string format, params object[] args) => AppendToLog("[ERROR] {0}", string.Format(format, args));
        public void clearProtections()
        {
            antiDebug.Checked = false;
            antiTamper.Checked = false;
            renameAssembly.Checked = false;
            renamer.Checked = false;
            constants.Checked = false;
            controlFlow.Checked = false;
            moduleFlood.Checked = false;
            refProxy.Checked = false;
            mildRefProxy.Checked = false;
            calli.Checked = false;
            mutations.Checked = false;
            local2field.Checked = false;
            hideMethods.Checked = false;
            eraseHeaders.Checked = false;
           
        }
        public void Finish(bool successful)
        {
            DateTime now = DateTime.Now;
            string timeString = string.Format(
                "at {0}, {1}:{2:d2} elapsed.",
                now.ToShortTimeString(),
                (int)now.Subtract(begin).TotalMinutes,
                now.Subtract(begin).Seconds);

            if (successful)
            {
                AppendToLog("Finished {0}", timeString);
                label2.ForeColor = Color.LimeGreen;
                label2.Text = "Protection complete, check log tab!";
            }
            else
            {
                AppendToLog("Failed {0}", timeString);
                label2.ForeColor = Color.Red;
                label2.Text = "Protection Failed, check log tab!";
            }
        }

        public void Info(string msg) => AppendToLog(" [INFO] {0}", msg);

        public void InfoFormat(string format, params object[] args) => AppendToLog(" [INFO] {0}", string.Format(format, args));

        public void Progress(int progress, int overall) { }

        public void Warn(string msg) => AppendToLog(" [WARN] {0}", msg);

        public void WarnException(string msg, Exception ex)
        {
            AppendToLog(" [WARN] {0}", msg);
            AppendToLog("Exception: {0}", ex);
        }

        public void WarnFormat(string format, params object[] args) => AppendToLog(" [WARN] {0}", string.Format(format, args));

        private void AppendToLog(string format, params object[] args) => richTextBox1.Text += (string.Format(format, args) + "\n");

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void thirteenForm1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Add me on Discord (Bed#9654) and we will discuss matters. Lowering price of it since there hasent been sales.");
        }

        private void koivmCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (koivmCheck.Checked)
            {
                groupBox1.Enabled = false;
                groupBox3.Enabled = true;
                clearProtections();
            }
            else
            {
                groupBox1.Enabled = true;
                groupBox3.Enabled = false;
                strongVirt.Checked = false;
                fastVirt.Checked = false;
            }

        }

        private void fastVirt_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
