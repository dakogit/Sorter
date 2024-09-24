using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSorter.Services.Interfaces
{
    internal interface ISplitService
    {
        List<string> SplitFile(string filePath);
    }
}
