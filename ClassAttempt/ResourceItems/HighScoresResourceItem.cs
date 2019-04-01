using GameMetadata;
using ResourcesBasics;

namespace ResourceItems
{

    #region LoadResourceItem: Metarisorsa utile a caricare e scaricare i livelli del gioco (è incluso il caricamento automatico dei temi)

    #endregion
    #region LevelResourceItem: classe che incorpora un KulaLevel nei file "logici"
    #endregion

    #region HighScoresResourceItem: classe che incorpora la tabella dei punteggi alti
    public class HighScoresResourceItem : ResourceItem
    {
        private Highscores scores = null;

        public Highscores Content
        {
            get { return scores; }
        }

        public HighScoresResourceItem()
        {
            scores = Highscores.LoadHighscores();
        }
        
        public override bool Load(string Path)
        {
            scores = Highscores.LoadHighscores();
            return true;
        }

        public override void Unload()
        {
            //Non fa nulla
        }
    }
    #endregion
}
