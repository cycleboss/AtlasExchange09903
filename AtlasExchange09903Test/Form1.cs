﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AtlasExchangePlusTest
{
    public partial class Form1 : Form
    {
        Test test;
        public Form1()
        {
            InitializeComponent();
            test = new Test();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            test.Start(new string[] { });
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            test.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, new EventArgs());
            }
        }
    }
}
