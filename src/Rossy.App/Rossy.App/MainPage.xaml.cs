﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rossy.App
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            txtUtterance.Text = "what's up?";
            txtFilePath.Text = "";
        }

        private void btnPickFile_Clicked(object sender, EventArgs e)
        {

        }

        private void btnTakePicture_Clicked(object sender, EventArgs e)
        {

        }

        private void btnAnalyze_Clicked(object sender, EventArgs e)
        {

        }
    }
}
