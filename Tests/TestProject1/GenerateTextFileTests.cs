using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextGenerator;
using Moq;

namespace UnitTests
{
    public class GenerateTextFileTests
    {
        [Fact]
        public void GenerateTextFile_CreatesFileWithExpectedSize()
        {
            // Arrange
            long expectedFileSize = 1 * 1024 * 1024;
            string path = "tests/test.txt";
            Directory.CreateDirectory(path);

            // Act
            GeneratorService.GenerateTextFile(path, expectedFileSize);
            FileInfo fileInfo = new FileInfo(path);

            // Assert
            Assert.True(fileInfo.Length >= expectedFileSize, $"Expected file size to be at least {expectedFileSize}, but was {fileInfo.Length}");
        }
    }
}
