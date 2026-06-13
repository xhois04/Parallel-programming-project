using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.Filters;

public class GrayscaleFilter : IFilter
{
    public string Name => "Grayscale";

    public void Apply(
        ReadOnlySpan<Rgba32> input,
        Span<Rgba32> output,
        int width, int height,
        int startRow, int endRow)
    {
        for (int row = startRow; row < endRow; row++)
        {
            for (int col = 0; col < width; col++)
            {
                int index = row * width + col;
                Rgba32 pixel = input[index];

                // Standard luminance formula
                byte gray = (byte)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);

                output[index] = new Rgba32(gray, gray, gray, pixel.A);
            }
        }
    }
}
