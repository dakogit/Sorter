using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSorter.Models
{
    public class ItemModel
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public int FileIndex { get; set; }

        public override string ToString()
        {
            return $"{Id}. {Value}";
        }

    }

    public class ItemModelComparer : IComparer<ItemModel>
    {
        public int Compare(ItemModel x, ItemModel y)
        {

            int valueComparison = string.Compare(x.Value, y.Value, StringComparison.Ordinal);
            if (valueComparison != 0)
                return valueComparison;

            return x.Id.CompareTo(y.Id);
        }
    }
}
