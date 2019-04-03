using GameMetadata;
using ResourcesBasics;

namespace ResourceItems
{

    

    
    
    

    
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
    
}
