using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMG
{
    public static class Pizza_selecter_right
    {
        static public Dictionary<int, List<Slice>> pizza_dictionary = new Dictionary<int, List<Slice>>()
        {
            { 1, new List<Slice> { new Slice(SliceType.Вверх, 0), new Slice(SliceType.ПоворотВправо, 1), new Slice(SliceType.Вниз, 2), new Slice(SliceType.ПоворотВлево, 3) } },
            { 2, new List<Slice> { new Slice(SliceType.Вверх, 0), new Slice(SliceType.Вперед, 1), new Slice(SliceType.Вниз, 2), new Slice(SliceType.Назад, 3) } },
        };
    }
}
