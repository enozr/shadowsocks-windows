﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using shadowsocks_csharp.Controller;
using shadowsocks_csharp.Model;

namespace shadowsocks_csharp.View
{
    public partial class ConfigForm : Form
    {
        ShadowsocksController controller;

        public ConfigForm(ShadowsocksController controller)
        {
            InitializeComponent();
            notifyIcon1.ContextMenu = contextMenu1;

            this.controller = controller;
            controller.EnableStatusChanged += controller_EnableStatusChanged;
            controller.ConfigChanged += controller_ConfigChanged;
            controller.PACFileReadyToOpen += controller_PACFileReadyToOpen;

            updateUI();
        }

        private void controller_ConfigChanged(object sender, EventArgs e)
        {
            updateUI();
        }

        private void controller_EnableStatusChanged(object sender, EventArgs e)
        {
            updateUI();
        }

        void controller_PACFileReadyToOpen(object sender, ShadowsocksController.PathEventArgs e)
        {
            string argument = @"/select, " + e.Path;

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        
        private void showWindow()
        {
            this.Opacity = 1;
            this.Show();
        }

        private void updateUI()
        {
            Server server = controller.GetCurrentServer();

            textBox1.Text = server.server;
            textBox2.Text = server.server_port.ToString();
            textBox3.Text = server.password;
            textBox4.Text = server.local_port.ToString();
            comboBox1.Text = server.method == null ? "aes-256-cfb" : server.method;

            enableItem.Checked = controller.GetConfiguration().enabled;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!controller.GetConfiguration().isDefault)
            {
                this.Opacity = 0;
                BeginInvoke(new MethodInvoker(delegate
                {
                    this.Hide();
                }));
            }
        }

        private void Config_Click(object sender, EventArgs e)
        {
            showWindow();
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                Server server = new Server
                {
                    server = textBox1.Text,
                    server_port = int.Parse(textBox2.Text),
                    password = textBox3.Text,
                    local_port = int.Parse(textBox4.Text),
                    method = comboBox1.Text
                };
                Configuration config = controller.GetConfiguration();
                config.configs.Clear();
                config.configs.Add(server);
                config.index = 0;
                controller.SaveConfig(config);
                this.Hide();
            }
            catch (FormatException)
            {
                MessageBox.Show("illegal port number format");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            updateUI();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.Stop();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/clowwindy/shadowsocks-csharp");
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            showWindow();
        }


        private void EnableItem_Click(object sender, EventArgs e)
        {
            enableItem.Checked = !enableItem.Checked;
            controller.ToggleEnable(enableItem.Checked);
        }

        private void editPACFileItem_Click(object sender, EventArgs e)
        {
            controller.TouchPACFile();
        }

    }
}
