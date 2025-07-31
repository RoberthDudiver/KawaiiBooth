using Android.Content;
using Android.Provider;
using Android.Net;
using Microsoft.Maui.ApplicationModel;
using System.IO;
using System.Threading.Tasks;
using System;

namespace KawaiiBooth.Platforms
{
    public static class ImageSaver
    {
        public static async Task SaveToGalleryAsync(Stream imageStream, string filename)
        {
            var context = Android.App.Application.Context;
            var resolver = context.ContentResolver;

            var values = new ContentValues();
            values.Put(MediaStore.IMediaColumns.DisplayName, filename);
            values.Put(MediaStore.IMediaColumns.MimeType, "image/jpeg");
            values.Put(MediaStore.IMediaColumns.RelativePath, "DCIM/Photobot");

            var uri = resolver.Insert(MediaStore.Images.Media.ExternalContentUri, values);

            if (uri != null)
            {
                using var outputStream = resolver.OpenOutputStream(uri);
                imageStream.Position = 0;
                await imageStream.CopyToAsync(outputStream);
            }
        }
    }
}
