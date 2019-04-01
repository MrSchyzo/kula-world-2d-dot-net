namespace MultimediaClasses
{
    #region Oggetto per il worker del soundmediaplayer

    class PlayerArgs
    {
        /// <summary>
        /// Volume da impostare nel soundmediaplayer
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        /// Tipo di operazione da effettuare
        /// </summary>
        public SoundMediaPlayerOperation Operation { get; set; }
        /// <summary>
        /// Inizializza una classe utile per il backgroundworker presente nel soundmediaplayer
        /// </summary>
        /// <param name="vol"></param>
        /// <param name="onlyadjust"></param>
        public PlayerArgs(int vol, SoundMediaPlayerOperation op)
        {
            Volume = vol;
            Operation = op;
        }
    }
    #endregion
}
