using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace KawaiiBooth.Pages;

public partial class TemplateSelectionPage : ContentPage
{
    public TemplateSelectionPage()
    {
        InitializeComponent();
        BindingContext = this; // 👈 Esto hace que el XAML vea tus propiedades y comandos

    }
    public ICommand TemplateTappedCommand => new Command(async () =>
    {
        // Acción que deseas hacer
        await Open("template1");

    });
    public ICommand TemplateTappedcumpleCommand => new Command(async () =>
    {
        // Acción que deseas hacer
        await Open("cumple");

    });
    private async  Task Open(string template)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();
        foreach (var res in resources)
        {
            Console.WriteLine(res); // o usa Debug.WriteLine(res) en MAUI
        }
        await Shell.Current.GoToAsync($"{nameof(CameraPage)}?templateName={template}");
    }
}