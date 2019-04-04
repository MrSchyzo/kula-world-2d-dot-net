using System;
using System.ComponentModel;
using WMPLib;

namespace MultimediaClasses
{

    
    

    
    /// <summary>
    /// Classe che incorpora un WindowsMediaPlayer con le principali caratteristiche di un SoundPlayer
    /// </summary>
    public class SoundMediaPlayer
    {
        private WindowsMediaPlayer audio;
        private BackgroundWorker player;
        string path;
        
        private bool isEmpty()
        {
            return (path == null);
        }

        private bool isReady()
        {
            bool done = false;
            bool rdy = true;
            while (!done)
            {
                try
                {
                    done = true;
                    rdy = audio.URL != null;
                }
                catch (Exception e)
                {
                    done = false;
                    Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                }
            }
            return (rdy);
        }
        

        
        private void ConstructorsMutualPart()
        {
            path = null;
            bool done = false;
            player = new BackgroundWorker();
            while (!done)
            {
                try
                {
                    audio = new WindowsMediaPlayer();
                    done = true;
                    audio.settings.volume = 50;
                }
                catch (Exception e)
                {
                    done = false;
                    Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                }
            }

            player.DoWork += player_DoWork;
        }
        

        
        void player_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e != null && e.Argument != null)
            {
                PlayerArgs arg = (PlayerArgs)e.Argument;
                int volume = arg.Volume;
                if (arg.Operation == SoundMediaPlayerOperation.ChangeVolume)
                    setVol(volume);
                else if (arg.Operation == SoundMediaPlayerOperation.Pause)
                    pause();
                else if (arg.Operation == SoundMediaPlayerOperation.Play)
                    play(volume);
                else if (arg.Operation == SoundMediaPlayerOperation.PlayLooping)
                    playLooping(volume);
                else if (arg.Operation == SoundMediaPlayerOperation.Stop)
                    stop();
            }
        }

        bool setVol(int v)
        {
            if ((audio != null) && (v >= 0) && (v <= 100))
            {
                bool done = false;
                while (!done)
                {
                    try
                    {
                        done = true;
                        audio.settings.volume = v;
                        return true;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("C'è qualche problema con l'URL \n" + e.ToString());
                        return false;
                    }
                    catch (Exception e)
                    {
                        done = false;
                        Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                    }
                }
                return true;
            }
            else
                return false;
        }

        void play(int v)
        {
            if (v != -1)
                setVol(v);
            if (isReady())
            {
                try
                {
                    audio.controls.stop();
                    audio.settings.setMode("loop", false);
                    audio.controls.play();
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("C'è qualche problema con l'URL \n" + ex.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("C'è qualche problema... \n" + ex.ToString());
                }
            }
        }

        void pause()
        {
            if (isReady())
            {
                bool done = false;
                while (!done)
                {
                    try
                    {
                        done = true;
                        audio.controls.pause();
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("C'è qualche problema con l'URL \n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        done = false;
                        Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                    }
                }
            }
        }

        void playLooping(int v)
        {
            if (v != -1)
                setVol(v);
            if (isReady())
            {
                bool done = false;
                while (!done)
                {
                    try
                    {
                        done = true;
                        audio.settings.setMode("loop", true);
                        audio.controls.play();
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("C'è qualche problema con l'URL \n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        done = false;
                        Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                    }
                }
            }
        }

        void stop()
        {
            if (isReady())
            {
                bool done = false;
                while (!done)
                {
                    try
                    {
                        done = true;
                        audio.controls.stop();
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("C'è qualche problema con l'URL \n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        done = false;
                        Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                    }
                }
            }
        }
        

        
        /// <summary>
        /// Inizializza un riproduttore audio senza riferimenti.
        /// </summary>
        public SoundMediaPlayer()
        {
            this.ConstructorsMutualPart();
        }

        /// <summary>
        /// Inizializza un riproduttore audio basandosi sul pathname in input.
        /// </summary>
        /// <param name="Path">pathname dove cercare il file audio</param>
        public SoundMediaPlayer(string Path)
        {
            this.ConstructorsMutualPart();
            this.path = Path;
            bool done = false;
            while (!done)
            {
                try
                {
                    done = true;
                    audio.settings.setMode("autoRewind", true);
                }
                catch (Exception e)
                {
                    done = false;
                    Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                }
            }
        }
        

        
        /// <summary>
        /// Carica il file presente nel pathname identificato dalla proprietà SoundLocation.
        /// </summary>
        public void Load()
        {
            if (!isEmpty())
            {
                bool done = false;
                while (!done)
                {
                    try
                    {
                        done = true;
                        audio.URL = path;
                        audio.controls.stop();
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("C'è qualche problema con l'URL \n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        done = false;
                        Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Imposta o restituisce il path dove cercare l'audio.
        /// Tale valore può essere cambiato durante la riproduzione, fermerà quest'ultima.
        /// </summary>
        public string SoundLocation
        {
            get { return path; }
            set
            {
                bool done = false;
                while (!done)
                    try
                    {
                        done = true;
                        path = value;
                        audio.controls.stop();
                        audio.URL = null;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("C'è qualche problema con l'URL \n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        done = false;
                        Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                    }
            }
        }

        /// <summary>
        /// Riproduce asincronamente il file audio. Se il suono fosse ancora in esecuzione, viene fermato e fatto ripartire.
        /// </summary>
        public void Play()
        {
            if (!player.IsBusy)
                player.RunWorkerAsync(new PlayerArgs(-1, SoundMediaPlayerOperation.Play));
        }

        /// <summary>
        /// Riproduce l'audio in maniera aciclica con il volume indicato. Se il suono fosse ancora in esecuzione, viene fermato e fatto ripartire.
        /// </summary>
        /// <param name="vol"></param>
        public void Play(int vol)
        {
            if (!player.IsBusy)
                player.RunWorkerAsync(new PlayerArgs(vol, SoundMediaPlayerOperation.Play));
        }

        /// <summary>
        /// Riproduce l'audio in maniera ripetuta. Se il suono fosse ancora in esecuzione, viene fermato e fatto ripartire.
        /// </summary>
        public void PlayLooping()
        {
            //playLooping(100);
            if (!player.IsBusy)
                player.RunWorkerAsync(new PlayerArgs(-1, SoundMediaPlayerOperation.PlayLooping));
        }

        /// <summary>
        /// Riproduce l'audio in maniera ripetuta con il volume indicato. Se il suono fosse ancora in esecuzione, viene fermato e fatto ripartire.
        /// </summary>
        /// <param name="vol"></param>
        public void PlayLooping(int vol)
        {
            //playLooping(vol);
            if (!player.IsBusy)
                player.RunWorkerAsync(new PlayerArgs(vol, SoundMediaPlayerOperation.PlayLooping));
        }

        /// <summary>
        /// Mette in pausa la riproduzione dell'audio.
        /// </summary>
        public void Pause()
        {
            if (!player.IsBusy)
                player.RunWorkerAsync(new PlayerArgs(-1, SoundMediaPlayerOperation.Pause));
        }

        /// <summary>
        /// Mette in pausa la riproduzione audio e ritorna all'inizio della traccia.
        /// </summary>
        public bool Stop()
        {
            if (!player.IsBusy)
                player.RunWorkerAsync(new PlayerArgs(-1, SoundMediaPlayerOperation.Stop));
            return true;
        }

        /// <summary>
        /// Elimina la traccia caricata in questo momento, settando SoundLocation a null.
        /// </summary>
        public void Dispose()
        {
            if (isReady())
            {
                bool done = false;
                while (!done)
                {
                    try
                    {
                        done = true;
                        audio.controls.stop();
                        path = null;
                        audio.URL = path;
                        GC.Collect();
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("C'è qualche problema con l'URL \n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        done = false;
                        Console.WriteLine("C'è qualche problema... \n" + e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Imposta il volume (da 0 a 100) della riproduzione audio.
        /// </summary>
        /// <param name="v">Valore desiderato del volume (da 0 a 100)</param>
        /// <returns>true se e solo se l'impostazione del volume ha avuto successo.</returns>
        public bool SetVolume(int v)
        {
            if (!player.IsBusy)
                player.RunWorkerAsync(new PlayerArgs(v, SoundMediaPlayerOperation.ChangeVolume));
            return true;
        }
        
    }
    
}
