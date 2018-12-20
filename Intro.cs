﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Media;

namespace Project_Jack
{
    public partial class Intro : Form
    {
        int tick = 0;
        public Intro()
        {
            InitializeComponent();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tick++;
            //if(tick == 5 || tick == 10 || tick == 15 || tick == 20 || tick == 25 || tick == 30 || tick == 35 || tick == 40 || tick == 45)
            //{
            //    label4.Text += ".";
            //}

            if (tick > 5)
            {
                Timer.Stop();
                Cursor = Cursors.Default;
                SoundPlayer sd = new SoundPlayer(@"C:\임태근 Programming Files\Digital Study\인트로-2(완벽).wav");
                sd.PlaySync();
                this.Hide();
                LoginForm loginForm = new LoginForm();
                Thread.Sleep(100);
                loginForm.Show();
            }
        }

        private void Intro_Load(object sender, EventArgs e)
        {
            Timer.Start();
            Cursor = Cursors.AppStarting;
            label3.Text += Application.ProductVersion + "(FOR Digital Study)";
        }

        private void Intro_Shown(object sender, EventArgs e)
        {
            
        }
    }
}
