using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace UIEssentials
{
    
    /// <summary>
    /// Questa classe rappresenta la viewport del gioco. Contiene uno SceneContainer che può essere manipolato direttamente.
    /// Inoltre, è possibile fermare la viewport e farla ripartire. Dispatcha gli eventi che riceve allo SceneContainer.
    /// </summary>
    public class GameViewport : Control
    {
        /* Variabili di istanza:
            * fpsRate = numero massimo di fotogrammi elaborati al secondo
            * scenes = SceneContainer utilizzato e manipolabile
            * isFrozen = variabile per far fermare il gameloop
            * timer = timer utilizzato per il "benchmark"
            * mspF = valore in funzione di fpsRate (= 1000 / fpsRate)
            */
        private byte fpsRate;
        private SceneContainer scenes;
        private bool isFrozen;
        private Stopwatch timer;
        private int mspF;
        private KeyboardMouseEventBundle queue;

        /// <summary>
        /// Handler sugli eventi Paint. Dispatchato allo SceneContainer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            this.scenes.Draw(g);
        }

        /*Dispatching degli eventi:
         * - mousemove
         * - mousedown
         * - mouseup
         * - mousewheel
         * - keydown
         * - keyup
         */
        /// <summary>
        /// Gestisce il movimento della rotella del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Mouse_Move);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce il rilascio di un tasto del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Mouse_Down);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce la pressione di un tasto del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Mouse_Up);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce il movimento della rotella del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Mouse_Wheel);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce il rilascio di un tasto della tastiera
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Key_Down);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce la pressione di un tasto della tastiera
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Key_Up);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }

        /// <summary>
        /// Effettua il setup del controllo grafico.
        /// </summary>
        /// <param name="FPS">Massima quantità fissata di frame disegnati al secondo</param>
        private void setup(byte FPS)
        {
            /* Impostazioni come controllo grafico:
                * Metto il double buffering, così da ridurre il flickering
                * Rendo il controllo tale da riempire tutto il clientsize del padre (client size = spazio dove mettere i controlli figli)
                * Aggiungo un colore solido al background
                */
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.ControlDarkDark;

            /* Impostazioni come GameViewport:
                * Setting del Frame Per Second Rate
                * Setting del Milliseconds Per Frame (in funzione del FPSRate)
                */
            if (FPS <= 10)
                FPS = 10;
            this.fpsRate = FPS;
            this.mspF = 1000 / this.fpsRate;

            //Creo il cronometro utile per la temporizzazione.
            this.timer = new Stopwatch();

            //Creo la coda di eventi "mouse e keyboard"
            this.queue = new KeyboardMouseEventBundle();
        }

        /// <summary>
        /// Costruisce la viewport precisando i FPS e lo SceneContainer da utilizzare.
        /// </summary>
        /// <param name="FPS">Numero massimo di fotogrammi processati al secondo</param>
        /// <param name="sList">SceneContainer da utilizzare, se è null se ne istanzia uno vuoto</param>
        public GameViewport(byte FPS, SceneContainer sList)
        {
            this.setup(FPS);
            if (sList == null)
                this.scenes = new SceneContainer(this);
            else
                this.scenes = sList;
            this.isFrozen = false;
        }

        /// <summary>
        /// Restituisce lo SceneContainer che può essere manipolato.
        /// </summary>
        public SceneContainer sceneList
        {
            get { return this.scenes; }
        }

        /// <summary>
        /// Imposta o restituisce il numero massimo di fotogrammi processati in un secondo.
        /// </summary>
        public byte FramesPerSecondCap
        {
            get { return fpsRate; }
            set
            {
                byte temp = value;
                if (value <= 10)
                    value = 10;
                this.fpsRate = value;
                this.mspF = 1000 / this.fpsRate;
            }
        }

        /// <summary>
        /// Ferma temporaneamente l'intera elaborazione del gioco.
        /// </summary>
        public void Freeze()
        {
            this.isFrozen = true;
            this.scenes.Pause();
            this.timer.Stop();
        }

        /// <summary>
        /// Fa avviare o riavviare l'elaborazione del gioco: è qui che è contenuto il game loop.
        /// </summary>
        public void Go()
        {
            
            long cur = 0;
            long delta = 0;
            long now = 0;
            
            
            double cycles = 0;
            double seconds = 0;
            long maxTime = 0;
            long minTime = long.MaxValue;
            
            this.timer.Start();
            this.isFrozen = false;
            this.scenes.Begin();
            
            while (!this.isFrozen)
            {
                cur = this.timer.ElapsedMilliseconds;
                Application.DoEvents();
                scenes.Update();
                this.Invalidate();
                now = this.timer.ElapsedMilliseconds;
                delta = now - cur;
                cur = now;
                if (delta < this.mspF)
                    Thread.Sleep(this.mspF - (int)delta);
                
                if (++cycles % 14000 == 0)
                    GC.Collect();
                if (delta > maxTime)
                    maxTime = delta;
                if (delta < minTime)
                    minTime = delta;
                
            }
            
            
            seconds = cur / 1000.0;
            cycles /= seconds;
            MessageBox.Show("Avg Cycles per second: " + cycles + "\nSlowest cycle: " + maxTime + "ms\nFastest cycle: " + minTime + "ms.", "Game Loop Diagnostics");
            
        }
    }
    

}
