﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BizHawk.MultiClient
{
    public partial class PathConfig : Form
    {
        public PathConfig()
        {
            InitializeComponent();
        }

        private void PathConfig_Load(object sender, EventArgs e)
        {

        }

        private void SaveSettings()
        {

        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }
    }
}
