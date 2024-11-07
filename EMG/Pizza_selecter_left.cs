using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMG
{
    public static class Pizza_selecter_left
    {
        static public Dictionary<int, List<Slice>> pizza_dictionary = new Dictionary<int, List<Slice>>()
        {
            { 1, new List<Slice> { new Slice(SliceType.Вперед, 0), new Slice(SliceType.Вправо, 1), new Slice(SliceType.Назад, 2), new Slice(SliceType.Влево, 3) } },
            { 2, new List<Slice> { new Slice(SliceType.Вправо, 0), new Slice(SliceType.ПоворотВправо, 1), new Slice(SliceType.Влево, 2), new Slice(SliceType.ПоворотВлево, 3) } },
        };
    }
}
