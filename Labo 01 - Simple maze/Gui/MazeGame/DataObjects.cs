using System.Collections.Generic;

namespace MazeGame
{
    public struct MazeData
    {
        public string Title;
        public List<int> Size;
        public string CodeSingle;
        public double CameraHeight;
        public List<double> Spawn;
        public List<double> Goal;
    }

    public struct BlockType
    {
        public string TexturePath;
        public char BlockCode;
    }
}
