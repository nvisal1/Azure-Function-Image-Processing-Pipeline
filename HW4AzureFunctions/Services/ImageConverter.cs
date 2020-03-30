using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace HW4AzureFunctions
{
    public class ImageConverter
    {
        public static MemoryStream ConvertImageToSepia(Stream originalImage)
        {
            originalImage.Seek(0, SeekOrigin.Begin);

            MemoryStream convertedMemoryStream = new MemoryStream();
            Image<Rgba32> image = (Image<Rgba32>)Image.Load(originalImage);
            
            image.Mutate(x => x.Sepia());
            image.SaveAsJpeg(convertedMemoryStream);

            convertedMemoryStream.Seek(0, SeekOrigin.Begin);

            return convertedMemoryStream;
        }

        public static MemoryStream ConvertImageToGreyScale(Stream originalImage)
        {
            originalImage.Seek(0, SeekOrigin.Begin);

            MemoryStream convertedMemoryStream = new MemoryStream();
            Image<Rgba32> image = (Image<Rgba32>)Image.Load(originalImage);
            
            image.Mutate(x => x.Grayscale());
            image.SaveAsJpeg(convertedMemoryStream);

            convertedMemoryStream.Seek(0, SeekOrigin.Begin);

            return convertedMemoryStream;
        }
    }
}
