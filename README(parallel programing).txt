Parallel Image Processing Project


This is a small C# console project made for parallel programming practice. The main idea is to take an image, apply a filter to it, and compare how long it takes when the work is done normally and when it is split between multiple threads.


The project uses ImageSharp to read and save images. The parallel part is done with Parallel.For. The image is divided into horizontal sections, and each thread works on its own part of the image.


What the project does


The program can apply three filters:


- grayscale
- blur
- edge detection


For each run, the program prints information like:


- image size
- selected filter
- number of threads used
- single-threaded time
- parallel time
- speedup
- where the output image was saved


This makes it easier to compare sequential processing with parallel processing.


How the parallel version works


Images are made of pixels arranged in rows and columns. Instead of processing the whole image using only one thread, the program splits the image by rows.


For example, if the image has 1000 rows and the computer has 8 logical processors, the work is divided between the available workers. Each worker processes only its own range of rows. The last worker also handles any remaining rows, so no part of the image is skipped.


Each filter receives:


- the original pixels
- the output pixel array
- the image width and height
- the start row
- the end row


Since each thread writes only inside its own row range, the threads do not overwrite each other. Because of this, the program does not need extra locks while applying the filters.


Project files


Program.cs


This file reads the command line arguments, checks which filter the user selected, and starts the image processor.


Processor.cs


This file loads the image, copies the pixels into arrays, runs the single-threaded version, runs the parallel version, prints the results, and saves the final image.


Filters/IFilter.cs


This file defines the common structure that every filter must follow.


Filters/GrayscaleFilter.cs


This file converts each pixel to grayscale using a luminance formula.


Filters/BlurFilter.cs


This file applies a simple blur by averaging nearby pixels.


Filters/EdgeDetectionFilter.cs


This file uses Sobel kernels to detect edges in the image.


Requirements


- .NET SDK
- ImageSharp package


The project file currently targets net10.0, so the matching .NET SDK should be installed. If the installed SDK is different, the target framework can be changed in the project file.


How to run


Open a terminal inside the project folder and run:


dotnet run -- <input image> <output image> <filter>


Examples:


dotnet run -- photo.jpg grayscale_result.jpg grayscale
dotnet run -- photo.jpg blur_result.jpg blur
dotnet run -- photo.jpg edge_result.jpg edge


The filter name must be one of these:


grayscale
blur
edge


Example output


Image: 1920x1080
Filter: Blur
Threads: 8
Single-threaded: 620 ms
Parallel: 140 ms
Speedup: 4.43x
Saved to: blur_result.jpg


The exact numbers will be different depending on the computer, image size, and selected filter.


Why this project is useful for parallel programming


Image processing is a good example for parallel programming because many pixel operations are independent. For example, when applying grayscale, one pixel does not need to wait for another pixel to be processed.


This makes the image easy to divide into smaller parts. Each thread can work on a different part of the image at the same time.


The project also shows that parallel programming is not only about using more threads. The work needs to be divided properly. In this project, splitting the image by rows keeps the logic simple and avoids unnecessary communication between threads.


Notes


The speedup is usually better for heavier filters, such as blur and edge detection, because there is more work for the threads to share.


For very small images, the parallel version may not always be faster because creating and managing parallel work also takes time.