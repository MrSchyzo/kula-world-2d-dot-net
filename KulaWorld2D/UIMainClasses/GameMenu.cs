using GameEngine;
using GameUtils;
using MultimediaClasses;
using ResourcesBasics;
using ResourceItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using UIEssentials;
using MenuItem = UIEssentials.MenuItem;
using System.Drawing.Text;

namespace UIMainClasses
{
    
    /// <summary>
    /// Questa classe rappresenta un blueprint per i menù di gioco, come quello di pausa e quello principale.
    /// </summary>
    public class GameMenu : IIndipendentlyAnimable
    {
        
        /// <summary>
        /// Menu di livello più alto
        /// </summary>
        protected List<MenuItem> rtLevelItems;
        /// <summary>
        /// Menu corrente
        /// </summary>
        protected List<MenuItem> curLevelItems;
        /// <summary>
        /// Puntatore al menu della gerarchia superiore
        /// </summary>
        protected MenuItem upperItem;
        /// <summary>
        /// Puntatore all'elemento del menu corrente
        /// </summary>
        protected int pointer = 0;
        /// <summary>
        /// Immagine dell'header
        /// </summary>
        protected Bitmap header;
        /// <summary>
        /// Immagine per lo sfondo
        /// </summary>
        protected Bitmap background;
        /// <summary>
        /// Immagine dove scrivere il testo
        /// </summary>
        protected Bitmap text;
        /// <summary>
        /// Immagine dove tenere la bitmap del cursore
        /// </summary>
        protected Bitmap cursor;
        /// <summary>
        /// Immagine dove disegnare la bitmap del cursore
        /// </summary>
        protected Bitmap curLayer;
        /// <summary>
        /// Lista di rettangoli che contengono i menuitem: in coordinate Testo
        /// </summary>
        protected List<Rectangle> textBoxes;
        /// <summary>
        /// Periodo di oscillazione del cursore
        /// </summary>
        protected long period = 1500;
        /// <summary>
        /// Posizione di oscillazione del cursore
        /// </summary>
        protected double cursorPosition = 0.0;
        /// <summary>
        /// Musica di sottofondo
        /// </summary>
        protected SoundMediaPlayer bgMusic;
        /// <summary>
        /// Suono del menu
        /// </summary>
        protected SoundMediaPlayer selectSound;
        /// <summary>
        /// Variabile booleana per indicare se la scena riporterà messaggi di debug nella console
        /// </summary>
        protected bool inDebug;
        /// <summary>
        /// Nome della cartella logica in cui piazzare le risorse.
        /// </summary>
        protected string curName;
        /// <summary>
        /// Path da cui caricare le risorse.
        /// </summary>
        protected string curPath;
        //TODO: ricorda di aggiungere un riferimento al gamescreen: pausa e principale lo manipolano
        /// <summary>
        /// Riferimento alla schermata di gioco, così da poterci interagire.
        /// </summary>
        protected GameScreen game;
        

        
        /// <summary>
        /// Stampa alla console il messaggio indicato in input. Funziona solo se la variabile interna "inDebug" è true.
        /// </summary>
        /// <param name="s">Stringa da stampare alla console</param>
        protected void PrintToConsole(string s)
        {
            if (inDebug)
                Console.WriteLine(s);
        }

        /// <summary>
        /// Restituisce un numero tradotto in un numero compreso tra 0 e 2*PI
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected double Periodize(long x)
        {
            return GameApp.Periodize(x, (double)period);
        }

        /// <summary>
        /// Restituisce l'item indicato dall'intero p.
        /// </summary>
        /// <param name="p">Intero che indica l'elemento da restituire</param>
        protected MenuItem getItem(int p)
        {
            if (p >= 0 && p < curLevelItems.Count)
                return curLevelItems.ElementAt<MenuItem>(p);
            else
                return null;
        }
        

        
        /// <summary>
        /// Aggiorna il layer del testo.
        /// </summary>
        protected void updateText()
        {
            //Setup del canvas dove scrivere il testo e delle regioni che toccano il testo
            if (text == null)
                text = new Bitmap(480, 360);
            if (textBoxes == null)
                textBoxes = new List<Rectangle>();
            if (curLevelItems == null)
                return;
            int txtW = text.Width;
            int txtH = text.Height;
            float f_txtW = (float)txtW;
            float f_txtH = (float)txtH;

            //Inizializzazione delle variabili che delimiteranno il testo
            int len = curLevelItems.Count;
            float cellHeight = Math.Min(f_txtH / ((float)len), f_txtH / 5.0f);
            int cellH, cellW, cellX, cellY;
            float desiredFont = cellHeight / 1.8f;


            //Inizializzazione degli strumenti per disegnare
            Graphics txtModder = Graphics.FromImage(text);
            GraphicsPath p = new GraphicsPath();
            Pen penna = Pens.Black;
            SolidBrush pennello = new SolidBrush(Color.FromArgb(80, 150, 150, 150));

            //Inizializzazione dello stile della stringa
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            //Svuoto l'array relativo al disegno del testo
            txtModder.Clear(Color.FromArgb(0, 0, 0, 0));
            textBoxes.Clear();

            /* Forall i in T:
             * Exists k. (Forall i in T => i.Height == k);
             * max(i in T){i.Width} <= txtLayer.W;
             * Product(i in T)[i.Height] <= txtLayer.H;
             */
            for (int j = 0; j < curLevelItems.Count; j++)
            {
                MenuItem i = curLevelItems.ElementAt<MenuItem>(j);

                Font ft = new Font("Calibri", desiredFont, FontStyle.Bold);
                Size curCell = TextRenderer.MeasureText(i.ToString(), ft);

                if (curCell.Width > txtW || curCell.Height > cellHeight)
                {
                    float wRatio = ((float)(curCell.Width)) / (f_txtW);
                    float hRatio = ((float)(curCell.Height)) / ((float)cellHeight);
                    float cur_desiredRatio = Math.Max(wRatio, hRatio);
                    desiredFont = desiredFont / (cur_desiredRatio + 0.2f);
                }
            }
            /* Forall i in T:
             * i.X = (txtLayer.Width - i.Width)/2
             * i.Y = Sum(j < i)[j.Height]
             * Rectangle(i) = new Rectangle(i.X, i.Y, i.Width, i.Height)
             */
            cellY = 0;
            for (int j = 0; j < curLevelItems.Count; j++)
            {
                MenuItem i = curLevelItems.ElementAt<MenuItem>(j);

                Font ft = new Font("Calibri", desiredFont, FontStyle.Bold);
                Size curCell = TextRenderer.MeasureText(i.ToString(), ft);

                cellH = curCell.Height;
                cellW = Math.Min(curCell.Width, txtW);
                cellX = (txtW - cellW) / 2;

                Rectangle txtBox = new Rectangle(cellX, cellY, cellW, cellH);
                textBoxes.Add(txtBox);

                p.AddString(i.ToString(), new FontFamily("Calibri"), (int)FontStyle.Bold, desiredFont, txtBox, sf);
                p.FillMode = FillMode.Winding;
                p.CloseAllFigures();

                cellY += cellH;
            }

            //Disegno e poi ripongo tutte le risorse.
            txtModder.FillPath(pennello, p);
            txtModder.DrawPath(penna, p);
            p.Reset();
            p.Dispose();
            txtModder.Dispose();
            //penna.Dispose();
            //pennello.Dispose();
        }

        /// <summary>
        /// Disegna il layer del background.
        /// </summary>
        /// <param name="e">Contesto grafico da passare.</param>
        protected void drawBackground(Graphics e)
        {
            if (background != null)
                e.DrawImage(background, clipRegion);
        }

        /// <summary>
        /// Disegna il layer dello header.
        /// </summary>
        /// <param name="e">Contesto grafico da passare</param>
        protected void drawHeader(Graphics e)
        {
            if (header != null)
            {
                float viewportX = 0;
                float viewportY = 0;
                float viewportW = (float)(clipRegion.Width);
                float viewportH = ((float)clipRegion.Height) / ((float)3.0);
                RectangleF hdRect = new RectangleF(viewportX, viewportY, viewportW, viewportH);
                e.DrawImage(header, hdRect);
            }
        }

        /// <summary>
        /// Disegna il layer del testo.
        /// </summary>
        /// <param name="e">Contesto grafico da passare</param>
        protected void drawText(Graphics e)
        {
            if (text != null)
            {
                float viewportX = (float)(((double)clipRegion.Width) / 4.0);
                float viewportY = (float)(((double)clipRegion.Height) / 3.0);
                float viewportW = ((float)clipRegion.Width) / ((float)2.0);
                float viewportH = ((float)clipRegion.Height) / ((float)1.5);
                //Console.WriteLine("Drawing: (" + viewportX + ", " + viewportY + ", " + viewportW + ", " + viewportH + "\n In: " + clipRegion.ToString());
                RectangleF txtRect = new RectangleF(viewportX, viewportY, viewportW, viewportH);
                e.DrawImage(text, txtRect);
            }
        }

        /// <summary>
        /// Disegna il cursore per il testo. (WARNING: Crea a runtime un layer di supporto per disegnare il cursore sopra il testo.)
        /// </summary>
        /// <param name="e">Contesto grafico da passare</param>
        protected void drawCursor(Graphics e)
        {
            if (cursor != null)
            {
                //Resetto il layer del cursore
                if (curLayer != null)
                    curLayer.Dispose();
                curLayer = null;
                curLayer = new Bitmap(480, 360);

                //Ripulisco il layer del cursore
                Graphics g = Graphics.FromImage(curLayer);
                g.Clear(Color.FromArgb(0, 0, 0, 0));

                //Estraggo la scatola del menuitem scelto
                Rectangle curBox = textBoxes.ElementAt<Rectangle>(pointer);
                int len = Math.Max(curBox.Width - curBox.Height, 0);

                //Definisco il rettangolo in cui disegnare il cursore nel layer
                int curX = ((int)(((double)len) * cursorPosition)) + curBox.X;
                int curY = curBox.Y;
                int curW = curBox.Height;
                int curH = curBox.Height;

                g.DrawImage(cursor, new Rectangle(curX, curY, curW, curH));

                //Preparo il layer per la proiezione nel layer della viewport di gioco
                float viewportX = (float)(((double)clipRegion.Width) / 4.0);
                float viewportY = (float)(((double)clipRegion.Height) / 3.0);
                float viewportW = ((float)clipRegion.Width) / ((float)2.0);
                float viewportH = ((float)clipRegion.Height) / ((float)1.5);
                e.DrawImage(curLayer, new RectangleF(viewportX, viewportY, viewportW, viewportH));

                g.Dispose();
                curLayer.Dispose();
                curLayer = null;
            }
        }

        /// <summary>
        /// Verisone potenziata del metodo drawCursor. Evita di creare a runtime una bitmap.
        /// </summary>
        protected void drawCursorEnhanced(Graphics e)
        {
            if (cursor != null && textBoxes != null && clipRegion != null && text != null)
            {
                //Scatola del menuitem puntato.
                Rectangle rPointed = textBoxes.ElementAt<Rectangle>(pointer);
                float rX = rPointed.X;
                float rY = rPointed.Y;
                float rW = rPointed.Width;
                float rH = rPointed.Height;

                //Dimensioni della regione della viewport
                float vpW = clipRegion.Width;
                float vpH = clipRegion.Height;

                //Dimensioni del layer di testo
                float txtW = text.Width;
                float txtH = text.Height;

                //Preparazione della trasformazione del rettangolo da coordinate Testo a coordinate Viewport
                PointF[] luCorner = new PointF[] { new PointF(rX, rY) };
                PointF[] size = new PointF[] { new PointF(rW, rH) };

                //Creazione trasformazione text to viewport:
                //Prima lo scaling inverso, poi la traslazione alle coordinate Viewport
                Matrix _txt2viewport = new Matrix();
                _txt2viewport.Scale(vpW / (2.0f * txtW), (vpH * 2.0f) / (3.0f * txtH), MatrixOrder.Append);
                _txt2viewport.TransformPoints(size);
                _txt2viewport.Translate(vpW / 4.0f, vpH / 3.0f, MatrixOrder.Append);
                _txt2viewport.TransformPoints(luCorner);

                //Estraggo i nuovi parametri del rettangolo.
                rX = luCorner[0].X;
                rY = luCorner[0].Y;
                rW = size[0].X;
                rH = size[0].Y;

                //Preparo la lunghezza del percorso che il cursore dovrà fare.
                float len = Math.Max(rW - rH, 0);

                //Preparo il rettangolo del cursore in coordinate Viewport e lo disegno
                int curX = ((int)(((double)len) * cursorPosition)) + ((int)rX);
                int curY = (int)rY;
                int curW = (int)rH;
                int curH = (int)rH;
                e.DrawImage(cursor, new RectangleF(curX, curY, curW, curH));
            }
        }
        

        
        /// <summary>
        /// Metodo per inizializzare in maniera standard un GameMenu. (Inizializza lo stato del menù)
        /// </summary>
        /// <param name="sc">SceneContainer che conterrà il GameMenu generato</param>
        protected void GameMenuMutualPart(SceneContainer sc)
        {
            if (sc != null)
            {
                inDebug = false;
                container = sc;
                timer = new Stopwatch();
                rtLevelItems = null;
                curLevelItems = null;
                upperItem = null;
                pointer = 0;
                this.clipRegion = new Rectangle(0, 0, 800, 600);
                header = null;
                background = null;
                cursor = null;
                updateText();
            }
        }

        /// <summary>
        /// Procura le risorse necessarie a disegnare le scene.
        /// </summary>
        protected void LoadAndGatherResources()
        {
            if (curName == null || curPath == null || container == null)
                return;

            ResourceDirectory root = container.getResourceDirectory;

            root.NewDirectory(curName);
            this.loadMediaFiles(curName, curPath + @"\");
            Bitmap a;
            
            try
            {
                a = ((ImageResourceItem)(root.GetFile(curName, "Background.bmp"))).Content;
                this.background = GameApp.ResizeImg(a, 500, 375);
                a.Dispose();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Errore nel caricamento dello sfondo di un menù! \n " + e.ToString() + "\n");
            }
            
            
            try
            {
                a = ((ImageResourceItem)(root.GetFile(curName, "Cursor.png"))).Content;
                this.cursor = GameApp.ResizeImg(a, 32, 32);
                a.Dispose();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Errore nel caricamento di uno del cursore di un menù! \n " + e.ToString() + "\n");
            }
            
            
            try
            {
                a = ((ImageResourceItem)(root.GetFile(curName, "Header.png"))).Content;
                this.header = GameApp.ResizeImg(a, 500, 125);
                a.Dispose();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Errore nel caricamento dell'intestazione di un menù! \n " + e.ToString() + "\n");
            }
            
            
            try
            {
                this.BackgroundMusic = ((SoundResourceItem)(root.GetFile(curName, "BackgroundMusic.mp3"))).Content;

            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Errore nel caricamento della musica di sottofondo di un menù! \n " + e.ToString() + "\n");
            }
            
            
            try
            {
                this.selectSound = ((SoundResourceItem)(root.GetFile(curName, "Sound.wav"))).Content;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Errore nel caricamento del suono di selezione di un menù! \n " + e.ToString() + "\n");
            }
            
            GC.Collect();
        }
        

        
        /// <summary>
        /// Inizializza il menù partendo dalla lista di menuitem data in input (viene passato SOLO il riferimento di tale lista!).
        /// </summary>
        /// <param name="l">Lista di MenuItem da mettere all'inizio</param>
        /// <param name="cur">Immagine che rappresenta il cursore </param>
        /// <param name="hd">Immagine che rappresenta lo header </param>
        /// <param name="bg">Immagine che rappresenta lo sfondo </param>
        /// <param name="sc">Contenitore della scena a cui collegarsi </param>
        public GameMenu(SceneContainer sc, List<MenuItem> l, Bitmap hd, Bitmap bg, Bitmap cur)
        {
            if (l != null)
                if (l.Count != 0)
                {
                    GameMenuMutualPart(sc);
                    rtLevelItems = l;
                    curLevelItems = l;
                    header = hd;
                    background = bg;
                    cursor = cur;
                    updateText();
                }
        }

        /// <summary>
        /// Crea un menù a partire dalla sola lista di MenuItem
        /// </summary>
        /// <param name="l">Menù Antenato nella gerarchia</param>
        /// <param name="sc">Contenitore della scena a cui collegare il menù</param>
        public GameMenu(SceneContainer sc, List<MenuItem> l)
        {
            if (l != null && l.Count != 0)
            {
                GameMenuMutualPart(sc);
                rtLevelItems = l;
                curLevelItems = l;
                updateText();
            }
        }

        /// <summary>
        /// Inizializza un GameMenu a partire dal contenitore di scene a cui farà riferimento
        /// </summary>
        /// <param name="sc">SceneContainer che conterrà il GameMenù generato</param>
        public GameMenu(SceneContainer sc)
        {
            GameMenuMutualPart(sc);
        }

        /// <summary>
        /// Restituisce un menù di gioco standard.
        /// </summary>
        public GameMenu()
        {
            //Niente.
        }
        

        
        /// <summary>
        /// Sposta il puntatore alla voce precedente
        /// </summary>
        protected void previousVoice()
        {
            int len = curLevelItems.Count;
            pointer--;
            if (pointer < 0)
                pointer += len;
        }

        /// <summary>
        /// Sposta il puntatore alla voce successiva
        /// </summary>
        protected void nextVoice()
        {
            int len = curLevelItems.Count;
            pointer = (pointer + 1) % len;
        }

        /// <summary>
        /// Aumenta tutti i valori associati alla voce
        /// </summary>
        protected void increaseVoice()
        {
            MenuItem l = getItem(pointer);
            if (l.isEditable)
            {
                l.increaseDoubleValue();
                l.increaseIntValue();
                l.switchBoolValue();
                this.HandleChoice(l);
                updateText();
            }
        }

        /// <summary>
        /// Decrementa tutti i valori associati alla voce
        /// </summary>
        protected void decreaseVoice()
        {
            MenuItem l = getItem(pointer);
            if (l.isEditable)
            {
                l.decreaseDoubleValue();
                l.decreaseIntValue();
                l.switchBoolValue();
                this.HandleChoice(l);
                updateText();
            }
        }

        /// <summary>
        /// Apre l'item puntato, modificando eventualmente il menù.
        /// </summary>
        protected void OpenItem()
        {
            MenuItem chosen = getItem(pointer);
            List<MenuItem> l = chosen.getChildren();
            if (l.Count != 0)
            {
                curLevelItems = l;
                pointer = 0;
                upperItem = chosen.fatherItem;
                updateText();
            }
            else
                HandleChoice(chosen);
        }

        /// <summary>
        /// Risale nella gerarchia del menù.
        /// </summary>
        protected void AscendMenu()
        {
            if (upperItem == null)
                curLevelItems = rtLevelItems;
            else
            {
                List<MenuItem> l = upperItem.getChildren();
                curLevelItems = l;
                upperItem = upperItem.fatherItem;
            }
            pointer = 0;
            updateText();
        }
        
        /// <summary>
        /// (Overridabile): Compie un'azione in funzione al menuitem aperto.
        /// </summary>
        /// <param name="chosen">MenuItem che è stato selezionato e provoca l'azione</param>
        protected virtual void HandleChoice(MenuItem chosen)
        {
            //TODO, questa voce provoca un'azione
        }

        /// <summary>
        /// Riproduce il suono dle menù
        /// </summary>
        protected void doSound()
        {
            if (selectSound != null)
                selectSound.Play(this.container.VolumeFX);
        }
        
        
        
        /// <summary>
        /// Gestisce la pressione di un tasto della tastiera
        /// </summary>
        /// <param name="e"> </param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (curLevelItems == null)
                return;
            doSound();
            int len = curLevelItems.Count;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        previousVoice();
                        break;
                    }
                case Keys.W:
                    {
                        previousVoice();
                        break;
                    }
                case Keys.Down:
                    {
                        nextVoice();
                        break;
                    }
                case Keys.S:
                    {
                        nextVoice();
                        break;
                    }
                case Keys.Enter:
                    {
                        OpenItem();
                        break;
                    }
                case Keys.Space:
                    {
                        OpenItem();
                        break;
                    }
                case Keys.Left:
                    {
                        decreaseVoice();
                        break;
                    }
                case Keys.A:
                    {
                        decreaseVoice();
                        break;
                    }
                case Keys.Right:
                    {
                        increaseVoice();
                        break;
                    }
                case Keys.D:
                    {
                        increaseVoice();
                        break;
                    }
                case Keys.Escape:
                    {
                        AscendMenu();
                        break;
                    }
                case Keys.Back:
                    {
                        AscendMenu();
                        break;
                    }
                case Keys.F1:
                    {
                        inDebug = !inDebug;
                        break;
                    }
            }
        }
        /// <summary>
        /// Gestisce il rilascio di un tasto della tastiera
        /// </summary>
        /// <param name="e"> </param>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            //Non faccio nulla
        }
        /// <summary>
        /// Gestisce la pressione di un tasto del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && textBoxes != null && text != null)
            {
                float w = (float)clipRegion.Width;
                float h = (float)clipRegion.Height;
                float txtW = text.Width;
                float txtH = text.Height;
                Point[] originalHit = new Point[] { e.Location };
                Matrix m = new Matrix();
                m.Translate(-w / 4.0f, -h / 3.0f, MatrixOrder.Append);
                m.Scale((txtW * 2.0f) / w, (txtH * 3.0f) / (2.0f * h), MatrixOrder.Append);
                m.TransformPoints(originalHit);
                Point hit = originalHit[0];

                if (textBoxes.Exists(x => x.Contains(hit)))
                {
                    doSound();
                    OpenItem();
                }
            }
        }
        /// <summary>
        /// Gestisce il rilascio di un tasto del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            //Non faccio nulla
        }
        /// <summary>
        /// Gestisce il movimento del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            if (textBoxes != null && text != null)
            {
                float w = (float)clipRegion.Width;
                float h = (float)clipRegion.Height;
                float txtW = text.Width;
                float txtH = text.Height;
                Point[] originalHit = new Point[] { e.Location };
                Matrix m = new Matrix();
                m.Translate(-w / 4.0f, -h / 3.0f, MatrixOrder.Append);
                m.Scale((txtW * 2.0f) / w, (txtH * 3.0f) / (2.0f * h), MatrixOrder.Append);
                m.TransformPoints(originalHit);
                Point hit = originalHit[0];

                if (textBoxes.Exists(x => x.Contains(hit)))
                {
                    int oldpointer = pointer;
                    pointer = textBoxes.FindIndex(x => x.Contains(hit)); //x => f(x) ricorda molto una lambda expression!!
                    if (oldpointer != pointer)
                        doSound();
                }
            }

        }
        /// <summary>
        /// Gestisce il movimento della rotella del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            //Non faccio nulla
        }
        

        
        /// <summary>
        /// Avvia l'elaborazione dello stato della scena.
        /// </summary>
        public override void Play()
        {
            timer.Start();
            updateText();
            bgMusic?.PlayLooping(this.container.VolumeMusic);
        }

        /// <summary>
        /// Disegna lo stato della scena, viene aggiunto il feedback del disegno.
        /// </summary>
        /// <param name="e">Contesto grafico su cui disegnare la scena</param>
        public override void Draw(Graphics e)
        {
            try
            {
                if (inDebug)
                    Console.Clear();
            }
            catch (Exception e1)
            {
                e1.ToString();
            }
            PrintToConsole("\n==================================");

            long now = timer.ElapsedTicks;
            base.Draw(e);
            long after = timer.ElapsedTicks;
            long total = 0;
            total += (after - now);
            PrintToConsole("Superclass draw: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");

            now = timer.ElapsedTicks;
            drawBackground(e);
            after = timer.ElapsedTicks;
            total += (after - now);
            PrintToConsole("Drawing Background: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");

            now = timer.ElapsedTicks;
            drawHeader(e);
            after = timer.ElapsedTicks;
            total += (after - now);
            PrintToConsole("Drawing Header: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");

            now = timer.ElapsedTicks;
            drawText(e);
            after = timer.ElapsedTicks;
            total += (after - now);
            PrintToConsole("Drawing Text: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");

            now = timer.ElapsedTicks;
            drawCursorEnhanced(e);
            after = timer.ElapsedTicks;
            total += (after - now);
            PrintToConsole("DrawingCursor: " + GameApp.TicksToMillis(after - now) + " milliseconds.\n");

            PrintToConsole("All the drawing took: " + GameApp.TicksToMillis(total) + " milliseconds.");
            PrintToConsole("==================================");
        }

        /// <summary>
        /// Ferma l'elaborazione dello stato della scena
        /// </summary>
        public override void Pause()
        {
            if (bgMusic != null)
                while (!bgMusic.Stop()) ;
            timer.Stop();
            //Altro
            curLevelItems = rtLevelItems;
            pointer = 0;
            upperItem = null;
        }

        /// <summary>
        /// Gestisce gli eventi ricevuti.
        /// </summary>
        /// <param name="bundle"> </param>
        public override void HandleEvent(KeyboardMouseEventBundle bundle)
        {
            KeyboardMouseEventArgs evt = bundle.extractEvent();
            if (evt != null)
            {
                if (evt.isKeyEvent && (!evt.isNullKeyEvent()))
                {
                    KeyEventArgs e = evt.keyEvent;
                    if (evt.getEventType == KeyboardMouseEventID.Key_Up)
                        this.OnKeyUp(e);
                    else
                        this.OnKeyDown(e);
                }
                else if (!evt.isNullMouseEvent())
                {
                    MouseEventArgs e = evt.mouseEvent;
                    switch (evt.getEventType)
                    {
                        case KeyboardMouseEventID.Mouse_Down:
                            {
                                this.OnMouseDown(e);
                                break;
                            }
                        case KeyboardMouseEventID.Mouse_Up:
                            {
                                this.OnMouseUp(e);
                                break;
                            }
                        case KeyboardMouseEventID.Mouse_Wheel:
                            {
                                this.OnMouseWheel(e);
                                break;
                            }
                        case KeyboardMouseEventID.Mouse_Move:
                            {
                                this.OnMouseMove(e);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Reimposta lo stato della scena
        /// </summary>
        public override void Reset()
        {
            timer.Reset();
            //Altro
        }

        /// <summary>
        /// Aggiorna lo stato della scena
        /// </summary>
        public override void Update()
        {
            long now = timer.ElapsedMilliseconds;
            cursorPosition = (1 + (Math.Sin(Periodize(now)))) / 2.0;
        }
        

        
        /// <summary>
        /// Restituisce o imposta il numero
        /// </summary>
        public long Period
        {
            get { return period; }
            set
            {
                if (value > 0)
                    period = value;
            }
        }

        /// <summary>
        /// Imposta la musica di sottofondo.
        /// </summary>
        public SoundMediaPlayer BackgroundMusic
        {
            set
            {
                bgMusic = value;
                bgMusic.Load();
            }
        }

        /// <summary>
        /// Imposta il suono di selezione del menù.
        /// </summary>
        public SoundMediaPlayer SelectSound
        {
            set
            {
                selectSound = value;
                selectSound.Load();
            }
        }
        
    }
    
}
