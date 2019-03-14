namespace GameEngine.Enumerations
{
    public enum BallState
    {
        Rolling = 1,
        Flying = 2,
        ChangingFace = 4,
        Burning = 8,
        Sliding = 16,
        NeedToCenter = 32,
        GroundForced = 64,
        Bonused = 128,
        GoingToVertex = 256,
        Exiting = 512,
        ViewMore = 1024
    }
}
