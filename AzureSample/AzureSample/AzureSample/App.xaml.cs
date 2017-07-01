﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AzureSample
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            var navi = new NavigationPage(new Views.StartPage());
            navi.BarBackgroundColor = Color.FromHex("#3498DB");
            navi.BarTextColor = Color.White;
            MainPage = navi;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
