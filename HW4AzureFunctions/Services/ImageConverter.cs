using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace HW4AzureFunctions
{
    /// <summary>
    /// Contains all logic for applying image filters
    /// </summary>
    public class ImageConverter
    {
        /// <summary>
        /// Applies a sepia filter on the given image
        /// and returns a new memory stream
        /// </summary>
        /// <param name="originalImage"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Applies a grey scale filter on the given images
        /// and returns a new memory stream
        /// </summary>
        /// <param name="originalImage"></param>
        /// <returns></returns>
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
