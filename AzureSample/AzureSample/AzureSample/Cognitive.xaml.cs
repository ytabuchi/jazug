using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AzureSample
{
    public partial class Cognitive : ContentPage
    {
        public Cognitive()
        {
            InitializeComponent();
        }

        async void OcrUriClicked(object sender, EventArgs e)
        {
			image.Source = ImageSource.FromUri(new Uri(uriText.Text));
			
            var resJa = await Ocr.DoOcrUriAsync(uriText.Text);
            resJaLabel.Text = resJa;

            var resEn = await Translate.TranslateTextAsync(resJa);
            resEnLabel.Text = resEn;
        }
    }
}