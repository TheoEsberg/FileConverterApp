using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace FileConverterApp.Converters
{
    public static class ImageConverter
    {
        public static async Task ConvertImageAsync(List<string> inputFilePath, string outputFilePath, string format)
        {
            await Task.Run(() =>
            {
                foreach (var file in inputFilePath)
                {
                    if (!File.Exists(file))
                    {
                        throw new FileNotFoundException($"Input file {file} does not exist.");
                    } else
                    {
                        using var image = new MagickImage(file);

                        image.Format = format.ToUpper() switch
                        {
                            ".JPEG" or ".JPG" => MagickFormat.Jpeg,
                            ".PNG" => MagickFormat.Png,
                            ".GIF" => MagickFormat.Gif,
                            ".BMP" => MagickFormat.Bmp,
                            ".WEBP" => MagickFormat.WebP,
                            ".HEIC" => MagickFormat.Heic,
                            _ => throw new NotSupportedException($"Format {format} is not supported.")
                        };

                        var finalPath = Path.Combine(outputFilePath, "converted_" + Path.GetFileNameWithoutExtension(file) + format);
                        image.Write(finalPath);
                    }
                }
            });
        }
    }
}
