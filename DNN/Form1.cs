﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Neuron N = new Neuron(10, Neuron.ActivationFunction.TanH);
            N.Activate();
            MessageBox.Show(N.Neurons[3].ToString());
        }

  
    }
}
