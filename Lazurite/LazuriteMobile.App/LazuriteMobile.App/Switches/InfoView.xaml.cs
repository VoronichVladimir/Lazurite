﻿using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class InfoView : ContentView
    {
        public InfoView()
        {
            InitializeComponent();
        }

        public InfoView(ScenarioInfo scenario, UserVisualSettings visualSettings) : this()
        {
            this.BindingContext = new ScenarioModel(scenario, visualSettings);
        }
    }
}
