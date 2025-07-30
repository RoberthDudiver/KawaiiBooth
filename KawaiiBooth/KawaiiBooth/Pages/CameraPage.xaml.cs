using System;
using Microsoft.Maui.Controls;

namespace KawaiiBooth.Pages
{
    public partial class CameraPage : ContentPage
    {
        public CameraPage()
        {
            InitializeComponent();
        }
        private async Task AnimateCountdownAsync(int count)
        {
            for (int i = count; i > 0; i--)
            {
                CountdownLabel.Text = i.ToString();
                await CountdownSemiCircle.ScaleTo(1.2, 150, Easing.CubicOut);
                await CountdownSemiCircle.ScaleTo(1.0, 150, Easing.CubicIn);
            }

            // Simular flash
            await CountdownSemiCircle.FadeTo(0, 100);
            await CountdownSemiCircle.FadeTo(1, 100);
            CountdownLabel.Text = "3";
        }

        private async Task AnimatePhotoIndexAsync(int current, int total)
        {
            PhotoIndexLabel.Text = $"{current}-{total}";
            await PhotoIndexSemiCircle.ScaleTo(1.1, 100);
            await PhotoIndexSemiCircle.ScaleTo(1.0, 100);
        }

        private void OnSwitchCameraClicked(object sender, EventArgs e)
        {
            // Cambiar entre cámara frontal y trasera
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            await AnimateCountdownAsync(3);

         
            await AnimatePhotoIndexAsync(2, 4);
        }


        private void OnPreviewClicked(object sender, EventArgs e)
        {
            // Mostrar vista previa o galería
        }
    }
}
