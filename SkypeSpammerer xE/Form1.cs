﻿using Microsoft.Win32;
using SKYPE4COMLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SkypeSpammerer_xE
{
    public partial class Form1 : Form
    {
        RegistryKey rk;
        RegistryKey rkS;
        Size sSize;
        Skype Skype1;
        Timer spammerthread = new Timer();
        string oldCity, oldMood;
        TUserStatus oldStatus;
        bool asMainProc = true;

        private string getActualHandle(string rawNameAsListed)
        {
            string se = string.Empty;
            string[] sx = rawNameAsListed.Split(' ');
            if (sx.Length != 0)
            {
                se = sx[sx.Length - 1];
                se.Replace("(", "");
                se.Replace(")", "");
            }
            return rawNameAsListed;
        }
                           
        

        public Form1(Size kk, bool isMainWindow)
        {
            asMainProc = isMainWindow;

            rk = Registry.CurrentUser.CreateSubKey(@"Software\Lx24\Spammerer");
            rkS = rk.CreateSubKey("Setting");

            if ((kk.Height == 0) && (kk.Width == 0))
            {
                sSize = new Size(((int)rkS.GetValue("SizeX", 300)), (int)rkS.GetValue("SizeY", 300));
            }
            else
            {
                sSize = kk;
            }

            InitializeComponent();

            try
            {
                Skype1 = new Skype();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("An error occured, this one to be exact:\n" + ex.ToString() + "..maybe you wanna recheck that\n1. Skype is running\n2. This application is allowed to access Skype\n3. You're not too fat to use this application.", "FAIL");
                Environment.Exit(-1);
            }
            spammerthread.Tick += spammerthread_Tick;
        }

        void spammerthread_Tick(object sender, EventArgs e)
        {
            if ((comboBox1.Text.Length >= 3) && (textBox2.Text.Length >= 1))
            {
                string[] sx = comboBox1.Text.Split(' ');
                if (sx.Length != 0)
                {
                    string se = sx[sx.Length - 1];
                    se.Replace("(", "");
                    se.Replace(")", "");
                    
                    try
                    {
                        MsgU(se, textBox2.Text);
                    }
                    catch { };
                }
                MsgU(comboBox1.Text, textBox2.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Skype1.Attach(Wait: true);
                if (asMainProc)
                {
                    oldStatus = Skype1.CurrentUserStatus;
                    oldCity = Skype1.CurrentUserProfile.City;
                    oldMood = Skype1.CurrentUserProfile.MoodText;
                    Skype1.ChangeUserStatus(TUserStatus.cusDoNotDisturb);
                    if (!Skype1.CurrentUserProfile.MoodText.StartsWith("Spamming", true, null)) Skype1.CurrentUserProfile.MoodText = "Spamming powered by Lasch24 Shitworks! - " + Skype1.CurrentUserProfile.MoodText;
                    Skype1.CurrentUserProfile.City = "Spammertown";
                    this.Text = "Spammer for " + Skype1.CurrentUserProfile.FullName;
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("An error occured, this one to be exact:\n" + ex.ToString() + "..maybe you wanna recheck that\n1. Skype is running\n2. This application is allowed to access Skype\n3. You're not too fat to use this application.", "FAIL");
                Environment.Exit(-1);
            }

            this.Size = sSize;

            foreach (User ux in Skype1.Friends)
            {
                if (ux.DisplayName != string.Empty)
                {
                    comboBox1.Items.Add(ux.DisplayName + " (" + ux.Handle + ")");
                }
                else if (ux.FullName != string.Empty)
                {
                    comboBox1.Items.Add(ux.FullName + " (" + ux.Handle + ")");
                }
                else
                {
                    comboBox1.Items.Add(ux.Handle);
                }
            }

            foreach (string s in rk.GetValueNames())
            {
                bool goodBreak = false;
                foreach (string str in comboBox1.Items)
                {
                    
                    if ((str.ToLower()).Contains(s.ToLower()))
                    {
                        goodBreak = false;
                        break;
                    }

                    //!!!!
                    else
                    {
                        break;
                    }
                    // !!!!!
                }
                if (goodBreak) comboBox1.Items.Add(s);
            }
        }

        private void MsgU(string skypeid = "echo123", string messagetext = "test")
        {
            try
            {
                Skype1.SendMessage(skypeid, messagetext);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("An error occured, this one to be exact:\n" + ex.ToString() + "..maybe you wanna recheck that\n1. Skype is running\n2. This application is allowed to access Skype\n3. You're not too fat to use this application.", "FAIL");
                Environment.Exit(-1);
            }
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            Process px = new Process();
            px.StartInfo.Arguments = this.Size.Width + " " + this.Size.Height + " 0";
            px.StartInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            px.Start();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            int newItv;
            if ((int.TryParse(textBox3.Text, out newItv)) && (comboBox1.Text.Length >= 3) && (textBox2.Text.Length >= 1))
            {
                Button sx = sender as Button;
                spammerthread.Interval = newItv;
                spammerthread.Enabled = !spammerthread.Enabled;
                if (spammerthread.Enabled)
                {
                    if (!(comboBox1.Items.Contains(comboBox1.Text)))
                    {
                        comboBox1.Items.Add(comboBox1.Text);
                    }
                    rk.SetValue(getActualHandle(comboBox1.Text), "!");
                    sx.Text = "Stop!";
                }
                else
                {
                    sx.Text = "Start!";
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            rkS.SetValue("SizeX", this.Size.Width);
            rkS.SetValue("SizeY", this.Size.Height);
            rkS.Close();
            rk.Close();
            if (asMainProc)
            {
                if ((Skype1.CurrentUserProfile.MoodText.Contains(oldMood)) && (Skype1.CurrentUserProfile.MoodText.StartsWith("Spamming")) && (Skype1.CurrentUserProfile.MoodText.Contains("Shitworks"))) Skype1.CurrentUserProfile.MoodText = oldMood;
                if ((Skype1.CurrentUserStatus == TUserStatus.cusDoNotDisturb) && (Skype1.CurrentUserStatus != oldStatus)) Skype1.ChangeUserStatus(oldStatus);
                if (Skype1.CurrentUserProfile.City == "Spammertown") Skype1.CurrentUserProfile.City = oldCity;
            }
        }
    }
}
