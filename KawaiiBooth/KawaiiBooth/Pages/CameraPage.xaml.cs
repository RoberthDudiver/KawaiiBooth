using Microsoft.Maui.Controls;
using SkiaSharp;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace KawaiiBooth.Pages
{
    public partial class CameraPage : ContentPage, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("templateName", out var nameObj) && nameObj is string templateName)
            {
                LoadTemplate(templateName);
            }
        }
        public CameraPage()
        {
            InitializeComponent();
        }

        private async void LoadTemplate(string templateName)
        {
            _template = await LoadTemplateFromResource(templateName);
            await AnimatePhotoIndexAsync(1, _template.PhotoCount);
        }
        private List<Stream> _photoStreams = new();
        private TemplateModel _template;
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
        public async Task<TemplateModel> LoadTemplateFromResource(string name)
        {
            var file = $"KawaiiBooth.Resources.Templates.{name}.json";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<TemplateModel>(json);
        }

        public static string SanitizeBase64(string base64)
        {
            // Solo mantener caracteres válidos en base64: A-Z, a-z, 0-9, +, / y hasta 2 =
            return new string(base64
                .Where(c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=')
                .ToArray());
        }
        public static byte[] FromBase64String(string base64)
        {
            base64 = base64.Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");
            using var transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
            byte[] inputBytes = Encoding.ASCII.GetBytes(base64);
            return transform.TransformFinalBlock(inputBytes, 0, inputBytes.Length); throw new FormatException("Base64 inválido o corrupto.");
            
        }
        private async Task GenerateFinalImageAsync()
        {
            var canvas = new SKBitmap(_template.CanvasWidth, _template.CanvasHeight);
            using var surface = new SKCanvas(canvas);
            surface.Clear(SKColors.White);

            // Fondo
            if (!string.IsNullOrEmpty(_template.BackgroundBase64))
            {
              

                var bgBytes = FromBase64String(_template.BackgroundBase64);
                using var bgStream = new SKManagedStream(new MemoryStream(bgBytes));
                using var bgBitmap = SKBitmap.Decode(bgStream);
                surface.DrawBitmap(bgBitmap, new SKRect(0, 0, canvas.Width, canvas.Height));
            }

            // Capas
         
            // Fotos del usuario
            for (int i = 0; i < _photoStreams.Count; i++)
            {
                var slot = _template.PhotoSlots[i];
                _photoStreams[i].Position = 0;
                using var sBitmap = SKBitmap.Decode(_photoStreams[i]);
                surface.DrawBitmap(sBitmap, new SKRect((float)slot.X, (float)slot.Y, (float)(slot.X + slot.Width), (float)(slot.Y + slot.Height)));
            }

            foreach (var layer in _template.Layers)
            {
                if (string.IsNullOrEmpty(layer.Base64Image)) continue;
                var bytes = FromBase64String(layer.Base64Image);
                using var lStream = new SKManagedStream(new MemoryStream(bytes));
                using var lBitmap = SKBitmap.Decode(lStream);
                surface.DrawBitmap(
                    lBitmap,
                    new SKRect(
                        (float)layer.X,
                        (float)layer.Y,
                        (float)(layer.X + layer.Width),
                        (float)(layer.Y + layer.Height)
                    )
                );
            }

            // Mostrar el resultado
            using var image = SKImage.FromBitmap(canvas);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);
            var finalStream = new MemoryStream();
            data.SaveTo(finalStream);
            finalStream.Position = 0;

            await Dispatcher.DispatchAsync(() =>
            {
                PreviewImage.Source = ImageSource.FromStream(() => finalStream);
            });
        }



        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                _photoStreams.Clear(); // Limpiar fotos anteriores si las hay

                for (int i = 0; i < _template.PhotoCount; i++)
                {
                    await AnimateCountdownAsync(3);
                    await AnimatePhotoIndexAsync(i + 1, _template.PhotoCount);

                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                    var stream = await CameraView.CaptureImage(cts.Token);
                    if (stream != null)
                    {
                        // Guardar copia del stream para uso posterior
                        var memStream = new MemoryStream();
                        await stream.CopyToAsync(memStream);
                        memStream.Position = 0;
                        _photoStreams.Add(memStream);

                        // Previsualizar
                        if (i == _template.PhotoCount - 1)
                        {
                            PreviewImage.Source = ImageSource.FromStream(() => new MemoryStream(memStream.ToArray()));
                            PreviewImage.IsVisible = true;
                        }
                    }
                }

                // Generar imagen final
                await GenerateFinalImageAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }



        private void OnPreviewClicked(object sender, EventArgs e)
        {
            // Mostrar vista previa o galería
        }
    }
}
