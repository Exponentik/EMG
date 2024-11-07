using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EMG
{
    public enum SliceType
    {
        Вперед, Назад, Влево, Вправо, Вверх, Вниз,  ПоворотВлево, ПоворотВправо,
    }
    
    public class Slice
    {
        public Slice(SliceType type,int slice_position)
        {
            this.type = type;  
            this.slice_position = slice_position;
        }
        public SliceType type;
        public int slice_position;
    }
}
