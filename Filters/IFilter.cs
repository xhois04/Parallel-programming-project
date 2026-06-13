namespace ImageProcessor.Filters;

public interface IFilter
{
    string Name { get; }

    // Each thread calls this with its own row range [startRow, endRow)
    void Apply(
        ReadOnlySpan<SixLabors.ImageSharp.PixelFormats.Rgba32> input,
        Span<SixLabors.ImageSharp.PixelFormats.Rgba32> output,
        int width,
        int height,
        int startRow,
        int endRow
    );
}