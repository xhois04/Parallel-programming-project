using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.Filters;

public class BlurFilter : IFilter
{
    public string Name => "Blur";

    private const int Radius = 3;

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
                int rSum = 0, gSum = 0, bSum = 0, count = 0;

                for (int dy = -Radius; dy <= Radius; dy++)
                {
                    for (int dx = -Radius; dx <= Radius; dx++)
                    {
                        int neighborRow = row + dy;
                        int neighborCol = col + dx;

                        if (neighborRow < 0 || neighborRow >= height) continue;
                        if (neighborCol < 0 || neighborCol >= width) continue;

                        Rgba32 neighbor = input[neighborRow * width + neighborCol];
                        rSum += neighbor.R;
                        gSum += neighbor.G;
                        bSum += neighbor.B;
                        count++;
                    }
                }

                output[row * width + col] = new Rgba32(
                    (byte)(rSum / count),
                    (byte)(gSum / count),
                    (byte)(bSum / count),
                    input[row * width + col].A
                );
            }
        }
    }
}