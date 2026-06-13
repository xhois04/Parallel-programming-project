# Parallel Image Processor

A C# project demonstrating parallel programming through image filter processing.  
Each filter is applied across image strips using `Parallel.For`, with benchmarking against single-threaded execution.

## Requirements

- .NET 8 SDK

## Run

```bash
dotnet run -- <input.jpg> <output.jpg> <filter>
```

**Filters:** `grayscale`, `blur`, `edge`

**Example:**
```bash
dotnet run -- photo.jpg result.jpg grayscale
dotnet run -- photo.jpg result.jpg blur
dotnet run -- photo.jpg result.jpg edge
```

## Sample Output

```
Image: 3840x2160
Filter: Blur
Threads: 8
Single-threaded: 842 ms
Parallel (8 threads): 134 ms
Speedup: 6.28x
Saved to: result.jpg
```

## How Parallelism Works

The image is split into horizontal strips — one per logical CPU core.  
Each thread processes its own strip independently (no shared mutable state),  
then results are combined into the output image.

```
Thread 0 → rows   0–269
Thread 1 → rows 270–539
Thread 2 → rows 540–809
...
Thread 7 → rows 1890–2159
```

## Lab Milestones

| Lab | Goal |
|-----|------|
| 1   | Load image, apply grayscale in parallel, show before/after |
| 2   | Add blur filter, show benchmark numbers |
| 3   | Add edge detection, plot speedup curve across thread counts |
| 4   | Polish CLI, final benchmarks, presentation |

## Project Structure

```
ImageProcessor/
├── Program.cs                  # Entry point + CLI
├── Processor.cs                # Parallel strip splitting + benchmarking
└── Filters/
    ├── IFilter.cs              # Filter interface
    ├── GrayscaleFilter.cs      # Lab 1
    ├── BlurFilter.cs           # Lab 2
    └── EdgeDetectionFilter.cs  # Lab 3
```
