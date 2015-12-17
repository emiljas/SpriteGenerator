using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriteGenerator
{
    public class LayoutProperties
    {
        public List<InputFile> InputFilePaths { get; set; }
        public string OutputSpriteFilePath { get; set; }
        public LayoutMode LayoutMode { get; set; }
        public int DistanceBetweenImages { get; set; }
        public int MarginWidth { get; set; }
        public int ImagesInRow { get; set; }
        public int ImagesInColumn { get; set; }
    }
}
