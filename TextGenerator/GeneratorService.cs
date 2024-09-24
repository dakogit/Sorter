using System.Text;

namespace TextGenerator
{
    public static class GeneratorService
    {
        private static string adjectivesPath = "Content/Adjectives.txt";

        private static string nounsPath = "Content/Nouns.txt";


        public static void GenerateTextFile(string filePath, long fileSizeInBytes)
        {
            Dictionary<int, string> adjectiveDictionary = ReadFileToDictionary(adjectivesPath);
            Dictionary<int, string> nounDictionary = ReadFileToDictionary(nounsPath);
            Random random = new Random();
            Random random2 = new Random();

            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8, bufferSize: 81920))
            {
                long currentFileSize = 0;
                long currentLine = 0;
                while (currentFileSize < fileSizeInBytes)
                {
                    currentLine++;
                    string noun = nounDictionary[random.Next(1, nounDictionary.Count)];
                    string adjective = adjectiveDictionary[random.Next(1, adjectiveDictionary.Count)];
                    if (!string.IsNullOrEmpty(adjective))
                    {
                        adjective = "is " + adjective;
                    }
                    string lineWithNewline = $"{currentLine}. {noun} {adjective}";
                    writer.WriteLine(lineWithNewline);

                    currentFileSize += Encoding.UTF8.GetByteCount(lineWithNewline);
                }
            }
        }

        private static Dictionary<int, string> ReadFileToDictionary(string filePath)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            try
            {
                string[] array = File.ReadAllLines(filePath);
                for (int i = 0; i < array.Length; i++)
                {
                    dictionary[i + 1] = array[i];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return dictionary;
        }
    }
}
