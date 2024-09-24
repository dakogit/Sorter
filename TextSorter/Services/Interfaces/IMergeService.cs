using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSorter.Services.Interfaces
{
    internal interface IMergeService
    {
        Task MergeFiles(List<string> files, string resultFileName, int fileCounter = 0);
    }
}
