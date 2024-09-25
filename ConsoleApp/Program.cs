using System.Diagnostics;
using TextGenerator;
using TextSorter;
using TextSorter.Services;

string consoleText = "(Enter the number of the command)";
Console.WriteLine("Hello, World!");
Console.WriteLine($"Root folder: {Path.GetFullPath(FileConfig.Root)}");
while (true)
{
    Console.WriteLine();
    Console.WriteLine("-----------------------------------------------------------");
    string[] files = Directory.GetFiles(FileConfig.ChunkFolder);
    if (files != null)
    {
        FileConfig.Delete(files);
    }

    var existingFile = GetTextFilesInDirectory();

    Console.WriteLine($"What would you like to do? {consoleText}");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine("1. Generate a random txt file.");
    Console.WriteLine("2. Enter a file path");
    if (existingFile.Any())
        Console.WriteLine("3. Select from Upload folder");

    string userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            AskFileSizeAndGenerateRandomFile();
            break;
        case "2":
            await InputFilePath();
            break;
        case "3":
            await SelectExistingFile(existingFile);
            break;
        default:
            Console.WriteLine("Invalid input.");
            break;
    }
}

static void AskFileSizeAndGenerateRandomFile()
{
    Console.WriteLine("Enter the size of the file (in MB):");
    if (double.TryParse(Console.ReadLine(), out double fileSizeMB) && fileSizeMB > 0)
    {
        long fileSizeBytes = (long)(fileSizeMB * 1024 * 1024);
        long requiredSpaceBytes = fileSizeBytes * 2;

        string filePath = Path.GetFullPath(FileConfig.RandomFile);
        string driveName = Path.GetPathRoot(filePath);
        DriveInfo driveInfo = new DriveInfo(driveName);

        if (driveInfo.AvailableFreeSpace >= requiredSpaceBytes)
        {
            Stopwatch stopwatch = new Stopwatch();
            Console.WriteLine($"Start generating a text file with size {fileSizeMB}Mb");
            stopwatch.Start();
            GeneratorService.GenerateTextFile(FileConfig.RandomFile, fileSizeBytes);
            stopwatch.Stop();
            Console.WriteLine($"File is generated: {filePath}");
            Console.WriteLine(stopwatch);
        }
        else
        {
            Console.WriteLine("Not enough disk space. Please ensure there is at least double the file size available.");
        }
    }
    else
    {
        Console.WriteLine("Invalid file size. Please enter a positive number.");
    }
}
static async Task InputFilePath()
{
    Console.WriteLine("Please enter the file path:");
    string filePath = Console.ReadLine();

    if (File.Exists(filePath))
    {
        Console.WriteLine($"The file path you entered is: {filePath}");
        await SplittingAndSorting(filePath);
    }
    else
    {
        Console.WriteLine("The entered file path does not exist.");
    }
}


static async Task SplittingAndSorting(string inputFilePath)
{
    Stopwatch stopwatch = new Stopwatch();
    Console.WriteLine("Start split and sort");
    SplitService splitService = new SplitService();
    stopwatch.Start();
    var files = splitService.SplitFile(inputFilePath);
    stopwatch.Stop();
    Console.WriteLine($"Done: {stopwatch}");
    stopwatch.Reset();

    if(FileConfig.RemoveOriginFileAfterSpliting)
    {
       FileConfig.Delete(inputFilePath);
    }
    await Merge(files);
}


static async Task Merge(List<string> files)
{
    Stopwatch stopwatch = new Stopwatch();
    Console.WriteLine("Merge has been started");
    stopwatch.Start();
    MergeService mergeService = new MergeService();
    await mergeService.MergeFiles(files, FileConfig.ResultFile);
    stopwatch.Stop();
    Console.WriteLine($"Merge complete in {stopwatch}");
    Console.WriteLine($"Result: {Path.GetFullPath(FileConfig.ResultFile)}");
}

static async Task SelectExistingFile(Dictionary<int, string> files)
{
    if (files.Count > 0)
    {
        Console.WriteLine($"Select a .txt file from the list below.");
        foreach (var file in files)
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(FileConfig.UploadFolder,file.Value));
            string formattedSize = GetFileSizeFormatted(fileInfo.Length);
            Console.WriteLine($"{file.Key}: {file.Value} ({formattedSize})");
        }

        Console.WriteLine("Enter the number corresponding to the file you want to select:");
        if (int.TryParse(Console.ReadLine(), out int fileNumber) && files.ContainsKey(fileNumber))
        {
            string selectedFileName = files[fileNumber];
            Console.WriteLine($"You selected: {selectedFileName}");
            await SplittingAndSorting(Path.Combine(FileConfig.UploadFolder, selectedFileName));
        }
        else
        {
            Console.WriteLine("Invalid selection. Please enter a valid number.");
        }
    }
    else
    {
        Console.WriteLine("The folder is empty or does not contain any .txt files.");
    }
}

static Dictionary<int, string> GetTextFilesInDirectory()
{
    Dictionary<int, string> filesDict = new Dictionary<int, string>();
    string[] files = Directory.GetFiles(FileConfig.UploadFolder, "*.txt");

    for (int i = 0; i < files.Length; i++)
    {
        filesDict.Add(i + 1, Path.GetFileName(files[i]));
    }
    return filesDict;
}
static string GetFileSizeFormatted(long fileSizeInBytes)
{
    if (fileSizeInBytes >= 1048576) // 1 MB = 1024 * 1024 bytes
    {
        double fileSizeInMb = fileSizeInBytes / (1024.0 * 1024.0);
        return $"{fileSizeInMb:F2} MB";
    }
    else if (fileSizeInBytes >= 1024) // 1 KB = 1024 bytes
    {
        double fileSizeInKb = fileSizeInBytes / 1024.0;
        return $"{fileSizeInKb:F2} KB";
    }
    else
    {
        return $"{fileSizeInBytes} bytes";
    }
}