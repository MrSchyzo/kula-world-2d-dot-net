using MultimediaClasses;
using ResourcesBasics;
using System;
using System.Windows.Forms;

namespace ResourceItems
{
    /// <summary>
    /// ResourceItem specializzato nel trattare Bitmaps: è possibile caricare e scaricare un suono, oltre che a avviarlo sia ciclicamente che non.
    /// Il caricamento della risorsa è sincrono!
    /// </summary>
    public class SoundResourceItem : ResourceItem
    {
        private SoundMediaPlayer sound;

        /// <summary>
        /// SoundMediaPlayer contenuto nella risorsa.
        /// </summary>
        public SoundMediaPlayer Content
        {
            get { return sound; }
        }

        private bool isValid()
        {
            return (sound != null);
        }

        private void load(string Path)
        {
            sound = new SoundMediaPlayer();
            try
            {
                sound.SoundLocation = Path;
                sound.Load();
            }
            catch (Exception e)
            {
                Console.WriteLine("Minchia! Non è stato possibile caricare l'audio " + Path + " !\n" + e.ToString());
                Console.WriteLine();
                MessageBox.Show("Minchia! Non è stato possibile caricare l'audio " + Path + " !\n" + e.ToString());
                sound.Dispose();
                sound = null;
            }
        }

        private void unload()
        {
            if (sound != null)
            {
                sound.Stop();
                sound.Dispose();
            }
            sound = null;
            GC.Collect();
        }

        /// <summary>
        /// Inizializza una risorsa di suono a partire dal file path.
        /// </summary>
        /// <param name="Path">Path del file</param>
        public SoundResourceItem(string Path)
        {
            sound = null;
            Load(Path);
        }

        /// <summary>
        /// Carica una risorsa di suono a partire dal file path, restituisce true se e solo se il caricamento è andato a buon fine
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public override bool Load(string Path)
        {
            if (sound != null)
                unload();
            if (Path != null)
                load(Path);
            return isValid();
        }

        /// <summary>
        /// Scarica il contenuto della risorsa.
        /// </summary>
        public override void Unload()
        {
            unload();
        }

        /// <summary>
        /// Suona asincronamente la risorsa.
        /// </summary>
        public void Play()
        {
            if (sound != null)
                sound.Play();
        }

        /// <summary>
        /// Suona asincronamente la risorsa al volume desiderato.
        /// </summary>
        /// <param name="vol">Volume desiderato</param>
        public void Play(int vol)
        {
            if (sound != null)
                sound.Play(vol);
        }

        /// <summary>
        /// Ferma la riproduzione del suono e lo riavvolge
        /// </summary>
        public void Stop()
        {
            if (sound != null)
                while (!sound.Stop()) ;
        }

        /// <summary>
        /// Ferma la riproduzione del suono
        /// </summary>
        public void Pause()
        {
            if (sound != null)
                sound.Pause();
        }

        /// <summary>
        /// Suona asincronamente e ciclicamente il suono
        /// </summary>
        public void PlayLooping()
        {
            if (sound != null)
                sound.PlayLooping();
        }

        /// <summary>
        /// Suona asincronamente e ciclicamente il suono al volume desiderato
        /// </summary>
        /// <param name="vol">Volume desiderato</param>
        public void PlayLooping(int vol)
        {
            if (sound != null)
                sound.PlayLooping(vol);
        }

        /// <summary>
        /// Modifica il volume della risorsa sonora.
        /// </summary>
        /// <param name="v">Volume desiderato</param>
        /// <returns></returns>
        public bool ChangeVolume(int v)
        {
            return sound.SetVolume(v);
        }
    }
    
}
