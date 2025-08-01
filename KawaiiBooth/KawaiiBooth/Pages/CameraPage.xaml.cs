using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using SkiaSharp;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace KawaiiBooth.Pages
{
    public partial class CameraPage : ContentPage, IQueryAttributable
    {
        public CameraPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
        private byte[] _finalImageBytes;

        public ICommand SaveTapped => new Command(async () =>
        {
            PopupOverlay.IsVisible = false;


            await SaveImageToGalleryAsync();


        });
        public ICommand CloseTapped => new Command(() =>
        {
            PopupOverlay.IsVisible = false;


        });

        private async Task SaveImageToGalleryAsync()
        {

            var imageStream = new MemoryStream(_finalImageBytes);
            try
            {
                var fileName = $"photobot_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";

#if ANDROID
                await KawaiiBooth.Platforms.ImageSaver.SaveToGalleryAsync(imageStream, fileName);
#elif IOS
                var imageData = Foundation.NSData.FromStream(imageStream);
                var uiImage = UIKit.UIImage.LoadFromData(imageData);
                uiImage?.SaveToPhotosAlbum((img, err) =>
                {
                    // Podés mostrar mensaje si querés
                });
#elif WINDOWS || MACCATALYST
                var picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                var destPath = Path.Combine(picturesPath, fileName);
                using (var fileStream = File.Create(destPath))
                {
                    imageStream.Position = 0;
                    await imageStream.CopyToAsync(fileStream);
                }
#endif

                await Application.Current.MainPage.DisplayAlert("Éxito", "¡Imagen guardada en galería!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
            }

            PhotoIndexLabel.Text = $"0-{_template.PhotoCount}";
        }


        public int camera = 0;
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("templateName", out var nameObj) && nameObj is string templateName)
            {
                LoadTemplate(templateName);
            }
        }

        // Modificamos el método para que sea asíncrono y más robusto
        private async Task PlayShutterSoundAsync()
        {
            try
            {
                // Aseguramos que el MediaElement esté listo para reproducir
                ShutterSound.Stop();
                ShutterSound.SeekTo(TimeSpan.Zero);
                // Damos un pequeño retraso para que el sistema se prepare
                await Task.Delay(50);
                ShutterSound.Play();
            }
            catch (Exception ex)
            {
                // En caso de error, lo imprimimos en la consola
                System.Diagnostics.Debug.WriteLine($"Error al reproducir el sonido: {ex.Message}");
            }
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
                // Aumentar la escala y esperar un breve momento para que la UI se actualice
                await CountdownSemiCircle.ScaleTo(1.2, 150, Easing.CubicOut);
                await Task.Delay(500);
                // Volver a la escala normal y esperar otro momento
                await CountdownSemiCircle.ScaleTo(1.0, 150, Easing.CubicIn);
                await Task.Delay(500);
            }

            // Simular flash
            await CountdownSemiCircle.FadeTo(0, 100);
            await Task.Delay(50);
            await CountdownSemiCircle.FadeTo(1, 100);
            await Task.Delay(50);
            CountdownLabel.Text = "3";
        }

        private async Task AnimatePhotoIndexAsync(int current, int total)
        {
            PhotoIndexLabel.Text = $"{current}-{total}";
            // Animar el semicírculo y esperar
            await PhotoIndexSemiCircle.ScaleTo(1.1, 100);
            await Task.Delay(50);
            await PhotoIndexSemiCircle.ScaleTo(1.0, 100);
            await Task.Delay(50);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            camera = 0;
            var cts = new CancellationToken();
            var cameras = await CameraView.GetAvailableCameras(cts);
            var front = cameras.FirstOrDefault(c => c.Position == CommunityToolkit.Maui.Core.CameraPosition.Front);
            if (front != null)
                CameraView.SelectedCamera = front;
        }
        private async void OnSwitchCameraClicked(object sender, EventArgs e)
        {
            var cts = new CancellationToken();
            var cameras = await CameraView.GetAvailableCameras(cts);

            if (cameras is null || cameras.Count == 0)
                return;

            // Si está en frontal → cambiar a trasera
            if (CameraView.SelectedCamera?.Position == CommunityToolkit.Maui.Core.CameraPosition.Front)
            {
                camera = 1;
                var rear = cameras.FirstOrDefault(c => c.Position == CommunityToolkit.Maui.Core.CameraPosition.Rear);
                if (rear != null)
                    CameraView.SelectedCamera = rear;
            }
            else
            {
                // Si está en trasera → cambiar a frontal
                camera = 0;

                var front = cameras.FirstOrDefault(c => c.Position == CommunityToolkit.Maui.Core.CameraPosition.Front);
                if (front != null)
                    CameraView.SelectedCamera = front;
            }
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
            return transform.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

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
            _finalImageBytes = finalStream.ToArray();
            finalStream.Position = 0;

            await preview();
        }

        private async Task preview()
        {
            await Dispatcher.DispatchAsync(() =>
            {
                var img = ImageSource.FromStream(() => new MemoryStream(_finalImageBytes));
                PopupFinalImage.Source = img;
                PopupOverlay.IsVisible = true;

            });
        }

        public async Task<Stream?> GetStreamFromImageSourceAsync(ImageSource source)
        {
            if (source is StreamImageSource streamImageSource)
            {
                var cancellationToken = CancellationToken.None;
                var stream = await streamImageSource.Stream(cancellationToken);
                return stream;
            }

            await Application.Current.MainPage.DisplayAlert("Error", "La imagen no puede ser guardada (no es válida)", "OK");
            return null;
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                _photoStreams.Clear();

                for (int i = 0; i < _template.PhotoCount; i++)
                {
                    await AnimateCountdownAsync(3);
                    await AnimatePhotoIndexAsync(i + 1, _template.PhotoCount);

                    await Task.Delay(50);

                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                    var stream = await CameraView.CaptureImage(cts.Token);

                    if (stream != null)
                    {
                        var memStream = new MemoryStream();
                        await stream.CopyToAsync(memStream);
                        memStream.Position = 0;

                        memStream = camera == 1
                            ? FixOrientationToPortrait(memStream, false)
                            : FixOrientationToPortrait(memStream, true);

                        _photoStreams.Add(memStream);
                        await PlayShutterSoundAsync();

                        if (i == _template.PhotoCount - 1)
                            PreviewImage.Source = ImageSource.FromStream(() => new MemoryStream(memStream.ToArray()));
                    }
                }

                // 💖 Mostrar el overlay kawaii mientras procesa
                ProcessingOverlay.IsVisible = true;

                await Task.Delay(300); // pequeño delay para que se vea fluido
                await GenerateFinalImageAsync();

                // Ocultar el overlay kawaii cuando termine
                ProcessingOverlay.IsVisible = false;
            }
            catch (Exception ex)
            {
                ProcessingOverlay.IsVisible = false; // aseguramos ocultar en caso de error
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }


        private MemoryStream FixOrientationToPortrait(MemoryStream inputStream, bool isFrontCamera)
        {
            inputStream.Position = 0;
            using var bitmap = SKBitmap.Decode(inputStream);

            // Si ya es vertical, no rotamos
            if (bitmap.Height >= bitmap.Width)
            {
                inputStream.Position = 0;
                return new MemoryStream(inputStream.ToArray());
            }

            // Rotación para poner en vertical
            using var surfaceBitmap = new SKBitmap(bitmap.Height, bitmap.Width);
            using (var canvas = new SKCanvas(surfaceBitmap))
            {
                if (isFrontCamera)
                {
                    // Frontal: rotar al revés (-90°)
                    canvas.Translate(0, surfaceBitmap.Height);
                    canvas.RotateDegrees(-90);
                }
                else
                {
                    // Trasera: rotar normal (+90°)
                    canvas.Translate(surfaceBitmap.Width, 0);
                    canvas.RotateDegrees(90);
                }

                canvas.DrawBitmap(bitmap, 0, 0);
            }

            using var rotatedImage = SKImage.FromBitmap(surfaceBitmap);
            using var data = rotatedImage.Encode(SKEncodedImageFormat.Jpeg, 90);

            var output = new MemoryStream();
            data.SaveTo(output);
            output.Position = 0;
            return output;
        }


        private MemoryStream FlipImageHorizontally(MemoryStream originalStream, int grades = 90)
        {
            originalStream.Position = 0;
            using var bitmap = SKBitmap.Decode(originalStream);

            using var flippedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
            using (var canvas = new SKCanvas(flippedBitmap))
            {
                canvas.Scale(-1, 1); // Invertir horizontal
                canvas.Translate(-bitmap.Width, 0);
                canvas.DrawBitmap(bitmap, 0, 0);
            }

            using var image = SKImage.FromBitmap(flippedBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, grades);

            var output = new MemoryStream();
            data.SaveTo(output);
            output.Position = 0;
            return output;
        }


        private async void OnPreviewClicked(object sender, EventArgs e)
        {
            await preview();
            // Mostrar vista previa o galería
        }
    }
}
