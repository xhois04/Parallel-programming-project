using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using ImageProcessor.Filters;

namespace ImageProcessor;

public class Processor
{
    public void Run(string inputPath, string outputPath, IFilter filter)
    {
        // Load the image from disk and decode it into an in-memory representation.
        // The generic type Rgba32 means every pixel is stored as 4 bytes: Red, Green, Blue, Alpha.
        // The 'using' keyword ensures the image is disposed (memory freed) when we're done.
        using var image = Image.Load<Rgba32>(inputPath);

        int width = image.Width;    // number of pixels per row
        int height = image.Height;  // number of rows

        // Allocate two flat arrays to hold pixel data:
        //   inputPixels  — the original image, read-only during processing
        //   outputPixels — where each filter writes its results
        //
        // Using flat arrays (instead of a 2D array or the image object directly) gives us
        // safe, index-based access across threads without locks.
        // Pixel at row r, column c is at index: r * width + c
        Rgba32[] inputPixels = new Rgba32[width * height];
        Rgba32[] outputPixels = new Rgba32[width * height];

        // Copy every pixel from the loaded image into our flat inputPixels array.
        // After this, the image object is no longer needed for reading.
        image.CopyPixelDataTo(inputPixels);

        // Use one thread per logical CPU core for the parallel benchmark.
        // On a machine with 8 cores this will be 8; on Apple Silicon M-series it's typically 8–12.
        int threadCount = Environment.ProcessorCount;

        // Each thread gets a horizontal "strip" of rows to process independently.
        // stripHeight = how many rows each thread handles (last thread gets any remainder rows).
        int stripHeight = height / threadCount;

        Console.WriteLine($"Image: {width}x{height}");
        Console.WriteLine($"Filter: {filter.Name}");
        Console.WriteLine($"Threads: {threadCount}");

        // ── Single-threaded benchmark ─────────────────────────────────────────────
        // Run the filter on the entire image using one thread, and time it.
        // This gives us a baseline to compare against the parallel version.
        var singleTimer = Stopwatch.StartNew();
        filter.Apply(inputPixels, outputPixels, width, height, 0, height);
        singleTimer.Stop();
        Console.WriteLine($"Single-threaded: {singleTimer.ElapsedMilliseconds} ms");

        // Clear the output array so the parallel run starts from a clean slate.
        Array.Clear(outputPixels, 0, outputPixels.Length);

        // ── Parallel benchmark ────────────────────────────────────────────────────
        // Parallel.For splits the iteration range [0, threadCount) across the thread pool.
        // Each iteration index represents one thread's strip of the image.
        var parallelTimer = Stopwatch.StartNew();

        Parallel.For(0, threadCount, threadIndex =>
        {
            // Calculate the row range this thread is responsible for.
            int startRow = threadIndex * stripHeight;

            // The last thread takes any leftover rows so no rows are skipped
            // (e.g. if height=1000 and threadCount=8, stripHeight=125 but 8*125=1000 so it works out;
            //  if height=1001, the last thread handles rows 875–1000 instead of 875–999).
            int endRow = (threadIndex == threadCount - 1) ? height : startRow + stripHeight;

            // Each thread writes only to its own row range in outputPixels.
            // Because the ranges never overlap, no locking or synchronization is needed.
            filter.Apply(inputPixels, outputPixels, width, height, startRow, endRow);
        });

        parallelTimer.Stop();
        Console.WriteLine($"Parallel ({threadCount} threads): {parallelTimer.ElapsedMilliseconds} ms");

        // Speedup = how many times faster the parallel version was.
        // e.g. 800ms single / 130ms parallel ≈ 6.15x speedup
        double speedup = (double)singleTimer.ElapsedMilliseconds / parallelTimer.ElapsedMilliseconds;
        Console.WriteLine($"Speedup: {speedup:F2}x");

        // ── Save output ───────────────────────────────────────────────────────────
        // Build a new Image object from the processed pixel array, then save it to disk.
        // The file format (JPEG, PNG, etc.) is determined automatically from the output path extension.
        using var outputImage = Image.LoadPixelData<Rgba32>(outputPixels, width, height);
        outputImage.Save(outputPath);
        Console.WriteLine($"Saved to: {outputPath}");
    }
}