using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSorter
{
    public static class FileConfig
    {
        public static string Root { get; set; }
        public static string ChunkFolder { get; set; }
        public static long ChunkSize { get; set; }
        public static int BufferSize { get; set; }
        public static bool RemoveOriginFileAfterSpliting { get; set; }
        public static string RandomFile { get; set; }
        public static string ResultFolder { get; set; }
        public static string ResultFile { get; set; }
        public static string UploadFolder { get; set; }

        static FileConfig()
        {
            Root = "Sorter";
            ChunkSize = 100 * 1024 * 1024;
            BufferSize = 81920;
            RemoveOriginFileAfterSpliting = false;
            ChunkFolder = Path.Combine(Root, "Temp"); ;
            ResultFolder = Path.Combine(Root, "Result");
            ResultFile = Path.Combine(ResultFolder, "result.txt");
            UploadFolder = Path.Combine(Root, "Upload");
            RandomFile = Path.Combine(UploadFolder, "random.txt");

            Directory.CreateDirectory(FileConfig.Root);
            Directory.CreateDirectory(FileConfig.ChunkFolder);
            Directory.CreateDirectory(FileConfig.UploadFolder);
            Directory.CreateDirectory(FileConfig.ResultFolder);

        }

        public static void Delete(IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                Delete(filename);
            }
        }
        public static void Delete(string filename)
        {
            File.Delete(filename);
        }
    }
}
