using System;
using System.Collections.Concurrent;
using System.Text;
using TextSorter.Models;
using TextSorter.Services.Interfaces;

namespace TextSorter.Services
{
    public class SplitService : ISplitService
    {
        private readonly List<string> _chunkFiles;
        private readonly long _maxMemoryUsage;
        private readonly int _bufferSize;

        public SplitService()
        {
            _chunkFiles = new List<string>();
            _maxMemoryUsage = FileConfig.ChunkSize;
            _bufferSize = FileConfig.BufferSize;
        }

        public List<string> SplitFile(string filePath)
        {
            ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();

            using (var reader = new StreamReader(filePath, System.Text.Encoding.UTF8, true, _bufferSize))
            {
                int currentFileIndex = 0;
                while (!reader.EndOfStream)
                {
                    List<ItemModel> items = new List<ItemModel>();
                    long currentChunkSize = 0;
                    while (currentChunkSize < _maxMemoryUsage && !reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (String.IsNullOrEmpty(line))
                            continue;

                        items.Add(SplitLine(line));
                        currentChunkSize += line.Length * sizeof(char);
                    }
                    currentFileIndex++;

                    tasks.Add(Task.Run(() => ProcessFile(items, currentFileIndex)));
                }
            }
            Task.WhenAll(tasks).Wait();

            return _chunkFiles;
        }

        private async Task ProcessFile(List<ItemModel> models, int fileIndex)
        {
            models.Sort(new ItemModelComparer());
            await SaveToTxt(models, fileIndex);
        }

        private async Task SaveToTxt(List<ItemModel> list, int fileIndex)
        {
            string filePath = Path.Combine(FileConfig.ChunkFolder, $"chunk_{fileIndex}.tmp");
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendLine($"{item.Id}.{item.Value}");
            }
            await File.WriteAllTextAsync(filePath, sb.ToString());

            //using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8, BufferSize))
            //{
            //    foreach (var item in list)
            //    {
            //        writer.WriteLine($"{item.Id}.{item.Value}");
            //    }
            //}

            lock (_chunkFiles)
            {
                _chunkFiles.Add(filePath);
            }
        }

        private ItemModel SplitLine(string line)
        {
            var parts = line.Split('.');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Input string must contain exactly one period.");
            }

            int id = int.Parse(parts[0].Trim());
            string value = parts[1].Trim();

            return new ItemModel() { Id = id, Value = value };
        }
    }
}
