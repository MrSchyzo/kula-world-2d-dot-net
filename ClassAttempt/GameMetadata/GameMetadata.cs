﻿using GameUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameMetadata
{
    #region Record: classe privata che incorpora le informazioni per i punteggi alti, immutabile e ordinabile secondo punteggio -> data -> nome
    [Serializable()]
	class Record : IComparable
    {
        private string name;
        private long score;
        private DateTime timestamp;

        #region Costruttori
        public Record(long punti, string player)
        {
            if (player == null)
                throw new ArgumentNullException("The player name mustn't be a null reference!");
            name = player;
            score = punti;
            timestamp = DateTime.UtcNow;
        }
        #endregion

        #region Proprietà only-get: Score, Player, Date
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
        #endregion

        #region Metodi pubblici per l'ordinamento
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
        #endregion
    } 
	#endregion

    #region Highscores: classe che incorpora i punteggi più alti
    /// <summary>
    /// Highscores: classe che incorpora i punteggi più alti, è modificabile
    /// </summary>
    [Serializable()]
    public class Highscores
    {
        private SortedSet<Record> records;
        private int length;

        #region Costruttori
        /// <summary>
        /// Restituisce una tabella di punteggi alti in maniera standard: massimo 10 record.
        /// </summary>
        public Highscores()
        {
            length = 10;
            records = new SortedSet<Record>();
        }
        /// <summary>
        /// Restituisce una tabella di punteggi con massimo tanti record quanti indicati dalla variabile intera in input
        /// </summary>
        /// <param name="len">Numero massimo di record: non può essere meno di 1</param>
        public Highscores(int len)
        {
            if (len < 1)
                throw new ArgumentException("The list length must be at least 1");

            length = len;
            records = new SortedSet<Record>();
        }
        #endregion

        #region [PRIVATE] Metodi che implementano quelli pubblici
        private bool addRecord(Record r)
        {
            long x = r.Score;
            if (records.Count >= length)
            {
                if (!(r.CompareTo(records.Max) < 0))
                    return false;
                records.Remove(records.Max);
                return records.Add(r);
            }
            else
                return records.Add(r);
        }
        #endregion

        #region [PUBLIC] Manipolazione dei record
        /// <summary>
        /// Prova ad aggiungere un record ai punteggi più alti: restituisce true se l'aggiunta è stata effettuata
        /// </summary>
        /// <param name="player">Nome del giocatore</param>
        /// <param name="score">Punteggio della partita</param>
        /// <returns></returns>
        public bool AddRecord(string player, long score)
        {
            Record nuovo = new Record(score, player);
            return addRecord(nuovo);
        }
        #endregion

        #region [PUBLIC] Salvataggio e caricamento dei punteggi alti
		/// <summary>
		/// Salva lo stato del tabellone dei punteggi alti nella memoria secondaria: se ciò ha successo viene restituito true
		/// </summary>
		/// <returns></returns>
        public bool SaveHighscores()
        {
            //Prima, sistemo il tabellone
            while (records.Count > length)
                records.Remove(records.Max);
            try
            {
                Stream saver = File.Create(GameApp.CurDir() + "\\" + FileNames.HighscoresFileName); //Preparo lo stream dove scrivere
                BinaryFormatter serz = new BinaryFormatter(); //Preparo il serializzatore
                serz.Serialize(saver, this); //Scrivo l'oggetto serializzato nel file
                saver.Close();
                saver.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("Sembrano esserci problemi nel salvataggio, infatti l'eccezione scaturita dice: \n" + e.ToString());
                return false;
            }
            return true;
        }
        /// <summary>
        /// Restituisce un oggetto tabellone dalla memoria secondaria: se non ci fosse il file, viene restiuito un nuovo oggetto
        /// </summary>
        /// <returns></returns>
        public static Highscores LoadHighscores()
        {
            Highscores ret = null;
            string scoretable = GameApp.CurDir() + "\\" + FileNames.HighscoresFileName;
            if (File.Exists(scoretable))
            {
                try
                {
                    Stream loader = File.OpenRead(scoretable);
                    BinaryFormatter deserz = new BinaryFormatter();
                    ret = (Highscores)deserz.Deserialize(loader);
                    loader.Close();
                    loader.Dispose();
                    return ret;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Sembrano esserci problemi nel caricamento, infatti l'eccezione scaturita dice: \n" + e.ToString());
                }
            }
            return new Highscores();
        }
	    #endregion

        #region [PUBLIC] Mostra i punteggi alti
        /// <summary>
        /// Mostra il tabellone
        /// </summary>
        public void ShowHighscores()
        {
            //TOREDO: mostra il punteggio
            string messaggio = "Best " + records.Count + " records:\n\n";
            int i = 1;
            foreach (Record r in records)
                messaggio += ("\n" + i++ + ". " + r.Player + " with " + r.Score + "pts. [UTC Date: " + r.Date + "]\n");

            if (records.Count == 0)
                messaggio = "The score list is empty: nobody saved any score.";

            MessageBox.Show(
                messaggio,
                "Best Scores",
                MessageBoxButtons.OK,
                MessageBoxIcon.None,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification
                );
        }
        /// <summary>
        /// Restituisce la capienza massima del tabellone
        /// </summary>
        public int MaxLength
        {
            get { return length; }
        }
        /// <summary>
        /// Restituisce il numero di record salvati
        /// </summary>
        public int RecordsNumber
        {
            get { return records.Count; } 
        }
        #endregion
    }
    #endregion
}
