﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plate_Visualization
{
    public partial class StiffnessForm : Form
    {
        public StiffnessForm()
        {
            InitializeComponent();
        }

        public StiffnessForm(Stiffness stiffness)
        {
            InitializeComponent();
            eInput.Text = stiffness.E.ToString();
            hInput.Text = stiffness.H.ToString();
            vInput.Text = stiffness.V.ToString();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            MainForm parent = (MainForm)this.Owner;
            float E, V, H;
            if (!float.TryParse(eInput.Text, out E) ||
                !float.TryParse(hInput.Text, out H) ||
                !float.TryParse(vInput.Text, out V))
            {
                // TODO: Add dialog to show error message
                return;
            }
            if (E < 0 || V < 0 || H < 0)
            {
                // TODO: Add dialog to show error message
                return;
            }
            Stiffness s = new Stiffness(E, V, H);
            parent.SetStiffness(s);
            Close();
        }
    }
}