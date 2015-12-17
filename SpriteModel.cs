using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteGenerator
{
    public class SpriteModel
    {
        public string FilePath { get; set; }
        public Dictionary<int, SpritePlace> Places { get; set; }
    }
}
