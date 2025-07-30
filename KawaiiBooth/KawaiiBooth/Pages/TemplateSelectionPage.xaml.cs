using System.Diagnostics;
using System.Windows.Input;

namespace KawaiiBooth.Pages;

public partial class TemplateSelectionPage : ContentPage
{
    public TemplateSelectionPage()
    {
        InitializeComponent();
        BindingContext = this; // 👈 Esto hace que el XAML vea tus propiedades y comandos

    }
    public ICommand TemplateTappedCommand => new Command(() => {
        // Acción que deseas hacer
        Debug.WriteLine("¡Botón presionado!");
    });

}