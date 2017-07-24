using System;
using System.Collections.Generic;
using Plugin.Media;
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

        async void PhotoClicked(object sender, EventArgs e)
        {
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("No Camera", ":( No camera available.", "OK");
				return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{
				Directory = "Sample",
				Name = "test.jpg"
			});

			if (file == null)
				return;

			//await DisplayAlert("File Location", file.Path, "OK");

			image.Source = ImageSource.FromStream(() =>
			{
				var stream = file.GetStream();
				return stream;
			});

			var resJa = await Ocr.DoOcrStreamAsync(file.GetStream());
			//var resJa = await Ocr.MakeOCRRequest(file.Path);
			resJaLabel.Text = resJa;

			var resEn = await Translate.TranslateTextAsync(resJa);
			resEnLabel.Text = resEn;


			//file.Dispose();

			//or:
			//image.Source = ImageSource.FromFile(file.Path);
			//image.Dispose();
		}
    }
}