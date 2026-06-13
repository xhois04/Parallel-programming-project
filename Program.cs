using ImageProcessor;
using ImageProcessor.Filters;

if (args.Length < 3)
{
    Console.WriteLine("Usage: ImageProcessor <input> <output> <filter>");
    Console.WriteLine("Filters: grayscale, blur, edge");
    return;
}

string inputPath = args[0];
string outputPath = args[1];
string filterName = args[2].ToLower();

IFilter filter = filterName switch
{
    "grayscale" => new GrayscaleFilter(),
    "blur"      => new BlurFilter(),
    "edge"      => new EdgeDetectionFilter(),
    _           => throw new ArgumentException($"Unknown filter: {filterName}")
};

var processor = new Processor();
processor.Run(inputPath, outputPath, filter);