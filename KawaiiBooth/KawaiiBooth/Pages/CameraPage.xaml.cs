using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Media;

namespace KawaiiBooth.Pages;

public partial class CameraPage : ContentPage
{
    public CameraPage()
    {
        InitializeComponent();
    }

    private async void OnStartCameraClicked(object sender, EventArgs e)
    {
        CountdownLabel.IsVisible = true;

        for (int i = 3; i > 0; i--)
        {
            CountdownLabel.Text = i.ToString();
            await Task.Delay(1000);
        }

        CountdownLabel.Text = "¡Foto!";
        // Aquí podrías reproducir un sonido, ejemplo básico:
        try
        {
            var player = await FileSystem.OpenAppPackageFileAsync("shutter.mp3");
            var stream = player;
            // reproducir sonido según plataforma (requiere implementación por plataforma)
        }
        catch (Exception)
        {
            // manejar error
        }

        await Task.Delay(1000);
        CountdownLabel.IsVisible = false;

        await Shell.Current.GoToAsync("ResultPage");
    }
}