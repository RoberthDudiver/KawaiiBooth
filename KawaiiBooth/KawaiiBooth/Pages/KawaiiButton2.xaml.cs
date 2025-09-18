using System.Windows.Input;

namespace KawaiiBooth.Pages;

using System.Windows.Input;
using Microsoft.Maui.Controls;


public partial class KawaiiButton2 : ContentView
{
    public KawaiiButton2()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty Text1Property =
        BindableProperty.Create(nameof(Text1), typeof(string), typeof(KawaiiButton), string.Empty);

    public static readonly BindableProperty Text2Property =
        BindableProperty.Create(nameof(Text2), typeof(string), typeof(KawaiiButton), string.Empty);

    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(KawaiiButton), Colors.Gray);

    public static readonly BindableProperty TopColorProperty =
        BindableProperty.Create(nameof(TopColor), typeof(Color), typeof(KawaiiButton), Colors.LightGray);

    public static readonly BindableProperty BottomColorProperty =
        BindableProperty.Create(nameof(BottomColor), typeof(Color), typeof(KawaiiButton), Colors.Gray);

    public static readonly BindableProperty GlossColorProperty =
        BindableProperty.Create(nameof(GlossColor), typeof(Color), typeof(KawaiiButton), Colors.White);

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(KawaiiButton), default(ImageSource));

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(KawaiiButton), null);

    public string Text1
    {
        get => (string)GetValue(Text1Property);
        set => SetValue(Text1Property, value);
    }

    public string Text2
    {
        get => (string)GetValue(Text2Property);
        set => SetValue(Text2Property, value);
    }

    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public Color TopColor
    {
        get => (Color)GetValue(TopColorProperty);
        set => SetValue(TopColorProperty, value);
    }

    public Color BottomColor
    {
        get => (Color)GetValue(BottomColorProperty);
        set => SetValue(BottomColorProperty, value);
    }

    public Color GlossColor
    {
        get => (Color)GetValue(GlossColorProperty);
        set => SetValue(GlossColorProperty, value);
    }

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    private async void OnTapped(object sender, EventArgs e)
    {
        try
        {


            // Animación de escala
            await AnimationContainer.ScaleTo(0.93, 100, Easing.CubicInOut);
            await Task.Delay(100); // Pausa para permitir que la animación se complete visualmente
            await AnimationContainer.ScaleTo(1, 100, Easing.CubicInOut);

            // Reanudar el reconocimiento de gestos


            // Ejecutar comando si existe
            if (TapCommand?.CanExecute(null) == true)
                TapCommand.Execute(null);
        }
        catch { }
    }
}

