﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AzureSample.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StartPage : ContentPage
	{
		public StartPage ()
		{
			InitializeComponent ();
		}

        private void PushClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TodoList());
        }

        private void CognitiveClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Cognitive());
        }
    }
}