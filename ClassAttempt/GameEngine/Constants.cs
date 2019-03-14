using System;

namespace GameEngine
{
    public class Constants
    {
        public readonly static float BlockWidth = 64f;
        public readonly static float SecsToDieBurnt = 3;
        public readonly static float SecsToChill = 4;
        public readonly static float SecsToChangeFace = 0.333f;
        public readonly static float DestructionTime = 0.75f;
        public readonly static float ViewportXTiles = 8;
        public readonly static float ViewportYTiles = 6;
        public readonly static float ViewportBallXRatio = 0.5f;
        public readonly static float ViewportBallYRatio = 2f / 6f;
        public readonly static float JumpableBlockRatio = 0.125f;
        public readonly static float NormalJumpYSpeed = -0.384f;
        public readonly static float RampJumpYSpeed = -0.48f;
        public readonly static float NormalJumpXSpeed = 0.256f;
        public readonly static float RampJumpXSpeed = 0.3072f;
        public readonly static float MaxVerticalSpeed = 0.48f;
        public readonly static float MinVerticalSpeed = -0.48f;
        public readonly static float GravityY = 0.001536f;
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
}
