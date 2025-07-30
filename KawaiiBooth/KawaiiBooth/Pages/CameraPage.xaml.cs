using Microsoft.Maui.Controls;
using System;
using static Microsoft.Maui.ApplicationModel.Permissions;

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

        //private async void OnTakePhotoClicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        await AnimateCountdownAsync(3);

        //        // Pedir permiso si hace falta
        //        var status = await Permissions.RequestAsync<Permissions.Camera>();
        //        if (status != PermissionStatus.Granted)
        //        {
        //            await DisplayAlert("Permiso denegado", "No se puede acceder a la cámara", "OK");
        //            return;
        //        }

        //        // Abrir la cámara
        //        var photo = await MediaPicker.CapturePhotoAsync();
        //        if (photo == null)
        //            return;

        //        // Mostrar la foto
        //        var stream = await photo.OpenReadAsync();
        //        PreviewImage.Source = ImageSource.FromStream(() => stream);

        //        // Actualizar semicírculo
        //        await AnimatePhotoIndexAsync(2, 4); // cambia según la foto actual
        //    }
        //    catch (FeatureNotSupportedException)
        //    {
        //        await DisplayAlert("Error", "La cámara no está soportada en este dispositivo.", "OK");
        //    }
        //    catch (PermissionException)
        //    {
        //        await DisplayAlert("Error", "Permisos de cámara no concedidos.", "OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "OK");
        //    }
        //}

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var stream = await CameraView.CaptureImage(cts.Token);
            if (stream != null)
            {
                PreviewImage.Source = ImageSource.FromStream(() => stream);
                PreviewImage.IsVisible = true;
                await AnimatePhotoIndexAsync(2, 4);
            }
        }



        private void OnPreviewClicked(object sender, EventArgs e)
        {
            // Mostrar vista previa o galería
        }
    }
}
