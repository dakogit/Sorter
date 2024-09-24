using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSorter.Models;
using TextSorter.Services.Interfaces;

namespace TextSorter.Services
{
    public class MergeService : IMergeService
    {
        private int _chunkSize;

        public MergeService(int chunkSize = 10)
        {
            _chunkSize = chunkSize;
        }

        public async Task MergeFiles(List<string> files, string resultFileName, int fileCounter = 0)
        {
            if (files.Count <= _chunkSize)
            {
                await Sort(files, resultFileName);
                return;
            }

            List<string> mergedFiles = new List<string>();
            var chunks = files.Select((file, index) => new { file, index })
                              .GroupBy(x => x.index / _chunkSize)
                              .Select(g => g.Select(x => x.file).ToList());

            List<Task> sortTasks = new List<Task>();
            foreach (var chunk in chunks)
            {
                if (chunk.Count == 1)
                {
                    mergedFiles.Add(chunk[0]);
                }
                else
                {
                    string fileName = Path.Combine(FileConfig.ChunkFolder, $"merged_{fileCounter}.tmp");
                    sortTasks.Add(Sort(chunk, fileName).ContinueWith(t => mergedFiles.Add(fileName)));
                    fileCounter++;
                }
            }

            await Task.WhenAll(sortTasks);

            await MergeFiles(mergedFiles, resultFileName, fileCounter);
        }

        private async Task Sort(IEnumerable<string> fileNames, string outputPath)
        {
            var readers = fileNames.Select(file => new StreamReader(file)).ToList();
            List<ItemModel> rows = new List<ItemModel>();
            List<Task<string?>> readTasks = new List<Task<string?>>();

            foreach (var reader in readers)
            {
                if (!reader.EndOfStream)
                {
                    readTasks.Add(reader.ReadLineAsync());
                }
            }

            var results = await Task.WhenAll(readTasks);
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != null)
                {
                    rows.Add(ModelBuilder(results[i]!, i));
                }
            }

            using (var writer = new StreamWriter(outputPath))
            {
                while (rows.Count > 0)
                {
                    rows.Sort(new ItemModelComparer());
                    var min = rows[0];
                    await writer.WriteLineAsync(min.ToString());

                    if (readers[min.FileIndex].EndOfStream)
                    {
                        rows.RemoveAt(0);
                    }
                    else
                    {
                        var nextLine = await readers[min.FileIndex].ReadLineAsync();
                        if (nextLine != null)
                        {
                            rows[0] = ModelBuilder(nextLine, min.FileIndex);
                        }
                        else
                        {
                            rows.RemoveAt(0);
                        }
                    }
                }
            }

            foreach (var reader in readers)
            {
                reader.Dispose();
            }

            FileConfig.Delete(fileNames);
        }

        private ItemModel ModelBuilder(string line, int fileId)
        {
            var parts = line.Split('.');

            int id = int.Parse(parts[0]);
            string value = parts[1];

            return new ItemModel() { Id = id, Value = value, FileIndex = fileId };
        }

    }
}
