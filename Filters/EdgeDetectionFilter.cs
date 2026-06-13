using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.Filters;

public class EdgeDetectionFilter : IFilter
{
    public string Name => "Edge Detection";

    // Sobel kernels
    private static readonly int[,] KernelX =
    {
        { -1, 0, 1 },
        { -2, 0, 2 },
        { -1, 0, 1 }
    };

    private static readonly int[,] KernelY =
    {
        { -1, -2, -1 },
        {  0,  0,  0 },
        {  1,  2,  1 }
    };

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
                double gx = 0, gy = 0;

                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        int neighborRow = Math.Clamp(row + ky, 0, height - 1);
                        int neighborCol = Math.Clamp(col + kx, 0, width - 1);

                        Rgba32 pixel = input[neighborRow * width + neighborCol];
                        double brightness = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;

                        gx += brightness * KernelX[ky + 1, kx + 1];
                        gy += brightness * KernelY[ky + 1, kx + 1];
                    }
                }

                byte magnitude = (byte)Math.Clamp(Math.Sqrt(gx * gx + gy * gy), 0, 255);
                output[row * width + col] = new Rgba32(magnitude, magnitude, magnitude, 255);
            }
        }
    }
}