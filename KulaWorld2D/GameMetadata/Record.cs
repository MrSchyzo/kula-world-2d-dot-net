using System;

namespace GameMetadata
{
    
    [Serializable()]
	class Record : IComparable
    {
        private string name;
        private long score;
        private DateTime timestamp;

        
        public Record(long punti, string player)
        {
            if (player == null)
                throw new ArgumentNullException("The player name mustn't be a null reference!");
            name = player;
            score = punti;
            timestamp = DateTime.UtcNow;
        }
        

        
        public long Score
        {
            get { return score; }
        }

        public string Player
        {
            get { return name; }
        }

        public DateTime Date
        {
            get { return timestamp.Add(new TimeSpan(0, 0, 0)); }
        }
        

        
        public int CompareTo(object comp)
        {
            if (comp == null)
                throw new ArgumentNullException("In Record.CompareTo(obj), obj is a null reference!");
            Record other = (Record)comp;
            int compare = -score.CompareTo(other.score); //Questo perché il punteggio più alto precede il punteggio più basso
            if (compare == 0)
            {
                compare = DateTime.Compare(timestamp, other.timestamp); //La data precedente precede la data successiva (De Lapalisse would be proud...)
                if (compare == 0)
                    compare = name.CompareTo(other.name); //Brutto, ma il punteggio con il nome alfabeticamente precedente vince (PROTIP: tanto vale digitare sempre "AAA")
            }
            return compare;
        }

        public override bool Equals(object comp)
        {
            if (comp == null)
                throw new ArgumentNullException("In Record.CompareTo(obj), obj is a null reference!");
            Record other = (Record)comp;

            return score.Equals(other.score) && name.Equals(other.name) && timestamp.Equals(other.timestamp);
        }

        public override int GetHashCode()
        {
            return score.GetHashCode() + name.GetHashCode() + timestamp.GetHashCode();
        }
        
    } 
    
}
