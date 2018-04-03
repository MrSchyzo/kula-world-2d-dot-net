
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class EngineConst
    {
        public static float BlockWidth = 64f;
        public static float SecsToDieBurnt = 3;
        public static float SecsToChill = 4;
        public static float SecsToChangeFace = 0.333f;
        public static float DestructionTime = 0.75f;
        public static float ViewportXTiles = 8;
        public static float ViewportYTiles = 6;
        public static float ViewportBallXRatio = 0.5f;
        public static float ViewportBallYRatio = 2f / 6f;
        public static float JumpableBlockRatio = 0.125f;
        public static float NormalJumpYSpeed = -0.384f;
        public static float RampJumpYSpeed = -0.48f;
        public static float NormalJumpXSpeed = 0.256f;
        public static float RampJumpXSpeed = 0.3072f;
        public static float MaxVerticalSpeed = 0.48f;
        public static float MinVerticalSpeed = -0.48f;
        public static float GravityY = 0.001536f;
        public static float JumpBorder
        {
            get { return BlockWidth * JumpableBlockRatio; }
        }
        public static float ImageXTiles
        {
            get { return (float)Math.Sqrt(ViewportXTiles * ViewportXTiles + ViewportYTiles * ViewportYTiles); }
        }
        public static float ImageYTiles
        {
            get { return (float)Math.Sqrt(ViewportXTiles * ViewportXTiles + ViewportYTiles * ViewportYTiles); }
        }
    }

    public enum GameState
    {
        InGame,
        Exit,
        Death
    }

    #region EngUtils: utilità varie del motore
    public class EngUtils
    {
        public static bool IncludesAllTheOnesOf(int thisInt, int also)
        {
            return (thisInt | also) == thisInt;
        }

        public static double Distance(PointF a, PointF b)
        {
            float bX = b.X;
            float bY = b.Y;
            float aX = a.X;
            float aY = a.Y;
            return Math.Sqrt((aX - bX) * (aX - bX) + (aY - bY) * (aY - bY));
        }
    }
    #endregion
}
