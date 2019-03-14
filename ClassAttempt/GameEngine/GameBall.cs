using GameEngine.Animators;
using GameUtils;
using LevelsStructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngine
{
    #region RotationUtilities: utilità per le rotazioni
    /// <summary>
    /// Classe utilità per le rotazioni
    /// </summary>
    public class RotationUtilities
    {
        /// <summary>
        /// Restituisce la matrice di rotazione rispetto all'orientamento "down".
        /// </summary>
        /// <param name="o">Orientamento dato</param>
        /// <returns></returns>
        public static Matrix getRotationFromDownOrientation(KulaLevel.Orientation o)
        {
            Matrix m = new Matrix();
            switch(o)
            {
                #region Caso UP
                case (KulaLevel.Orientation.Up):
                    {
                        m.Rotate(180.0f, MatrixOrder.Append);
                        break;
                    }
                #endregion
                #region Caso LEFT
                case (KulaLevel.Orientation.Left):
                    {
                        m.Rotate(90.0f, MatrixOrder.Append);
                        break;
                    }
                #endregion
                #region Caso RIGHT
                case (KulaLevel.Orientation.Right):
                    {
                        m.Rotate(-90.0f, MatrixOrder.Append);
                        break;
                    }
                #endregion
                #region Default case
                default:
                    {
                        break;
                    }
                #endregion
            }
            return m;
        }

        /// <summary>
        /// Restituisce un rettangolo contenente i 4 vertici trasformati di un dato rettangolo.
        /// </summary>
        /// <param name="input">Rettangolo in input</param>
        /// <param name="m">Matrice di trasformazione</param>
        /// <returns></returns>
        public static RectangleF TransformRectangle(RectangleF input, Matrix m)
        {
            RectangleF res = new RectangleF();
            if (input != null)
            {
                PointF[] coords = new PointF[4];
                coords[0] = new PointF(input.Location.X, input.Location.Y);
                coords[1] = new PointF(coords[0].X + input.Width, coords[0].Y);
                coords[2] = new PointF(coords[0].X + input.Width, coords[0].Y + input.Height);
                coords[3] = new PointF(coords[0].X, coords[0].Y + input.Height);
                m.TransformPoints(coords);
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                foreach(PointF p in coords)
                {
                    if (p.X < minX)
                        minX = p.X;
                    else if (p.X > maxX)
                        maxX = p.X;

                    if (p.Y < minY)
                        minY = p.Y;
                    else if (p.Y > maxY)
                        maxY = p.Y;
                }
                res = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }
            return res;
        }

        /// <summary>
        /// Restituisce l'angolo assoluto di rotazione rispetto all'orientamento "down".
        /// </summary>
        /// <param name="o">Orientamento dato</param>
        /// <returns></returns>
        public static float getAngleFromDownOrientation(KulaLevel.Orientation o)
        {
            if (o == KulaLevel.Orientation.Left)
                return -90f;
            else if (o == KulaLevel.Orientation.Right)
                return 90f;
            else if (o == KulaLevel.Orientation.Up)
                return 180f;
            else
                return 0f;
        }
    }
    #endregion

    #region CollisionUtil: utilità per gestire le collisioni
    /// <summary>
    /// Classe utilità per gestire le collisioni
    /// </summary>
    public class CollisionUtil
    {
        /// <summary>
        /// Cerca di approssimare in maniera primitiva la collisione tra un cerchio ed un rettangolo: richiede che il rettangolo ed
        /// il cerchio siano nello stesso spazio di coordinate.
        /// </summary>
        /// <param name="dots">Numero di approssimazioni</param>
        /// <param name="circleCenter">Centro del cerchio</param>
        /// <param name="radium">Raggio del cerchio</param>
        /// <param name="rect">Rettangolo con cui fare il test delle collisioni</param>
        /// <returns></returns>
        public static bool CircleIntersectsRectangle(int dots, PointF circleCenter, float radium, RectangleF rect)
        {
            if (circleCenter != null && rect != null && dots > 0)
            {
                float cX = circleCenter.X;
                float cY = circleCenter.Y;
                float delta = 360f / ((float)dots);
                float angle = 0f;
                bool isHit = false;
                while (!isHit && angle < 360f)
                {
                    float dx = (float)Math.Sin(angle) * radium;
                    float dy = (float)Math.Cos(angle) * radium;
                    isHit = rect.Contains(cX + dx, cY + dy);
                    angle += delta;
                    if (isHit)
                        Console.Write("");//Line("Collision at: (" + (cX + dx) + ", " + (cY + dy) + ")");
                }
                return isHit;
            }
            else if (dots <= 0)
                throw new ArgumentException();
            else
                throw new NullReferenceException();
        }
    }
    #endregion

    #region Enum utili per il gioco: Command, BallState, SurfType, DeathType
    public enum Command
    {
        Left,
        Right,
        Jump,
        Nothing
    }

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

    public enum SurfType
    {
        Fire,
        Ice,
        Forced
    }

    public enum DeathType
    {
        Spiked,
        Captured,
        Fire,
        Fell,
        TimeOut,
        Retry
    }
    #endregion

    public class Ball
    {
        #region Variabili di stato della palla: posizione, rotazione, vita, velocità, stato e texture
        private bool movingRight = true;
        private bool isGoingToMove = false;
        private bool needToFall = false;
        #region Variabili per il cambiamento di faccia
        private int phase = 0;
        private Stopwatch cf_timer = new Stopwatch();
        private long lastMoment = 0;
        float rotaz = 90f;
        private bool toTheLeft = false;
        #endregion
        private double startingX = EngineConst.BlockWidth / 2f;
        private double startingY = EngineConst.BlockWidth / 2f;
        private float startingRot = 0;
        private double centerX;
        private double centerY;
        private float texRot = 0;
        private float rot;
        private float life = 100;
        private float scaleX = 1;
        private float scaleY = 1;
        private BallState state;
        private Bitmap srcTex;
        private Bitmap finalTex;
        private Bitmap srcLight = new Bitmap(1, 1);
        private Bitmap finalLight;
        private GameScreen game;
        #endregion

        #region Animatori
        private XYTextureRotationAnimator xyrotatorAnim;
        private Animator lifeAnim;
        private Animator rotAnim;
        private Animator scaleXAnim;
        private Animator scaleYAnim;
        #endregion

        #region Hash di utilità per le collisioni
        private SortedDictionary<SurfType, bool> foundSurfaces;
        #endregion

        #region Proprietà only Get
        public float Radium
        {
            get
            {
                return EngineConst.BlockWidth / 4f;
            }
        }
        public float ScaleX
        {
            get
            {
                return scaleX;
            }
        }
        public float ScaleY
        {
            get
            {
                return scaleY;
            }
        }
        public float Rotation
        {
            get
            {
                return rot;
            }
        }
        public float Life
        {
            get
            {
                return life;
            }
        }
        public PointF Center
        {
            get
            {
                return new PointF((float)centerX, (float)centerY);
            }
        }
        public Matrix DirectionModifier
        {
            get
            {
                if (Math.Round(Rotation) % 360 == 0)
                    return new Matrix(1, 0, 0, 1, 0, 0);
                else if (Math.Round(Rotation) % 360 == 90)
                    return new Matrix(0, -1, 1, 0, 0, 0);
                else if (Math.Round(Rotation) % 360 == 180)
                    return new Matrix(-1, 0, 0, -1, 0, 0);
                else if (Math.Round(Rotation) % 360 == 270)
                    return new Matrix(0, 1, -1, 0, 0, 0);
                else
                {
                    Matrix m = new Matrix();
                    //La rotazione è in orario ma perché il sistema di riferimento ha la y verso il basso!!
                    m.Rotate(-Rotation);
                    return m;
                }
            }
        }
        public BallState CurrentState
        {
            get { return state; }
        }
        #endregion

        #region Metodi privati di utilità
        #region Metodi per verificare la possibilità di movimento
        private bool canIJump(long thisTime)
        {
            bool goodState =
                !isThisStateAlso(state, BallState.Flying) &&
                !isThisStateAlso(state, BallState.GroundForced) &&
                !isThisStateAlso(state, BallState.ChangingFace);
            bool goodPosition = false;
            int relativeX = getRelativeX(thisTime) - ((int)(EngineConst.BlockWidth / 2f));
            float maxRegion = ((int)(EngineConst.BlockWidth / 2f)) - EngineConst.BlockWidth * EngineConst.JumpableBlockRatio;
            goodPosition = Math.Abs(maxRegion) >= Math.Abs(relativeX);
            return goodState && goodPosition;
        }
        private bool canIRumpJump(long thisTime)
        {
            bool goodState =
                !isThisStateAlso(state, BallState.Flying) &&
                !isThisStateAlso(state, BallState.GroundForced) &&
                !isThisStateAlso(state, BallState.ChangingFace);
            bool goodPosition = false;
            int relativeX = getRelativeX(thisTime) - ((int)(EngineConst.BlockWidth / 2f));
            float maxRegion = ((int)(EngineConst.BlockWidth / 2f)) - EngineConst.BlockWidth * EngineConst.JumpableBlockRatio * 2;
            goodPosition = Math.Abs(maxRegion) >= Math.Abs(relativeX);
            return goodState && goodPosition;
        }
        private bool canIMove(long thisTime)
        {
            bool goodState =
                !isThisStateAlso(state, BallState.ChangingFace) &&
                !isThisStateAlso(state, BallState.Flying) &&
                ! IsStateAlso(BallState.Sliding);
            bool isSteady = (xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime) == 0);
            return goodState && isSteady;
        }
        #endregion
        #region Metodi di aggiornamento dello stato
        //Questo metodo è fatto proprio per il bug della rotazione
        private void bugRecovery(int relX)
        {
            Matrix m = new Matrix();
            m.Rotate(-rot);
            PointF offset = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, new PointF(1, 0)));

            centerX += offset.X;
            centerY += offset.Y;
        }
        private void changeFace(bool toLeft, int blockDirection, float offset, long thisTime)
        {
            
            if (blockDirection != 0)
            {
                toTheLeft = toLeft;
                updateProperties(thisTime);
                SetState(BallState.ChangingFace, true);
                if (blockDirection == 1)
                {
                    //Verso l'alto, modifico la rotazione
                    if (!toLeft)
                        rotaz = -90f;
                    else 
                        rotaz = 90f;

                    rotAnim = new LinearBoundedAnimator(
                        (double)rot, 
                        thisTime, 
                        (double)-(rotaz / (EngineConst.SecsToChangeFace*1000)), 
                        (long)(EngineConst.SecsToChangeFace * 1000)
                        );

                    phase = 1;
                    lastMoment = cf_timer.ElapsedMilliseconds;
                }
                else
                {
                    //Verso il basso, modifico la posizione
                    float speed = 0.128f;
                    phase = 0;

                    if (toLeft)
                    {
                        speed *= -1;
                        rotaz = -90f;
                    }
                    else
                        rotaz = 90f;

                    #region Cambio gli animatori
                    xyrotatorAnim.ChangeAnimator(
                        0,
                        new LinearBoundedAnimator(
                            Math.Round(centerX),
                            thisTime,
                            speed,
                            125)
                            );

                    xyrotatorAnim.ChangeAnimator(
                        1,
                        new SteadyAnimator(
                            Math.Round(centerY),
                            thisTime)
                            );
                    #endregion

                    lastMoment = cf_timer.ElapsedMilliseconds;
                }
            }
        }
        private void blockAll(long thisTime)
        {
            xyrotatorAnim.ChangeAnimator(0, new SteadyAnimator(Math.Round(centerX), thisTime));
            xyrotatorAnim.ChangeAnimator(1, new SteadyAnimator(Math.Round(centerY), thisTime));
            xyrotatorAnim.ChangeAnimator(2, new SteadyAnimator(Math.Round(texRot), thisTime));
        }
        private void updateProperties(long thisTime)
        {
            #region Devo cadere?
            if (needToFall && !IsStateAlso(BallState.ChangingFace))
            {
                blockAll(thisTime);
                xyrotatorAnim.ChangeAnimator(
                    1,
                    new ParabolicToLinearAnimator(
                        Math.Round(centerY),
                        thisTime,
                        EngineConst.GravityY,
                        xyrotatorAnim.GetAnimation(1).GetCurrentSpeed(thisTime),
                        EngineConst.MaxVerticalSpeed
                        )
                    );
                state |= BallState.Flying;
                needToFall = false;
            }
            #endregion

            #region Controllo se sto scivolando
            if (IsStateAlso(BallState.Sliding))
                xyrotatorAnim.ChangeAnimator(2, new SteadyAnimator(texRot, thisTime));
            else
                xyrotatorAnim.BindTextureRotationToMovement(true);
            #endregion

            #region Aggiorno tutte le proprietà
            life = (float)lifeAnim.CalculateValue(thisTime);
            texRot = (float)xyrotatorAnim.CalcolateTexRot(thisTime);
            centerX = (float)xyrotatorAnim.CalculateX(thisTime, true);
            centerY = (float)xyrotatorAnim.CalculateY(thisTime, true);
            rot = ((float)Math.Round(rotAnim.CalculateValue(thisTime))) % 360;
            while (rot < 0) rot += 360f; //Non mi piace se l'angolo è negativo!
            scaleX = Math.Min(Math.Max((float)scaleXAnim.CalculateValue(thisTime), 0.85f), 1f);
            scaleY = Math.Min(Math.Max((float)scaleYAnim.CalculateValue(thisTime), 0.85f), 1f);
            #endregion
        }
        private bool lifeControl(long thisTime)
        {
            if (life <= 0)
            {
                game.Die(DeathType.Fire);
                return false;
            }
            if (!isThisStateAlso(state, BallState.Burning))
                lifeAnim = new LinearBoundedAnimator(life, thisTime, 0.1 / EngineConst.SecsToChill, 100, 0);
            return true;
        }
        private void centeringControl(long thisTime)
        {
            #region Evito di stare fermo se sono in sliding
            if (IsStateAlso(BallState.Sliding) && (Math.Abs(xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime)) < 0.256))
            {
                Console.WriteLine("Devo scivolare!");
                double d = 1;
                if (!movingRight)
                    d *= -1;
                xyrotatorAnim.ChangeAnimator(
                    0,
                    new LinearUnboundedAnimator(
                        Math.Round(centerX),
                        thisTime,
                        d * 0.256
                        )
                   );
            }
            #endregion
            int relativeX = getRelativeX(thisTime) - ((int)(EngineConst.BlockWidth/2f));
            float maxRegion = EngineConst.BlockWidth * EngineConst.JumpableBlockRatio;
            if (Math.Abs(maxRegion) > Math.Abs(relativeX) && !isThisStateAlso(state, BallState.Sliding) && !isGoingToMove)
                blockAll(thisTime);
            if (!game.CheckBlock(0, 1))
                StartFalling(thisTime);
        }
        private void changingFaceControl(long thisTime)
        {
            float bw = EngineConst.BlockWidth / 2;
            int maxRange = 23; //Variabile per vedere quando iniziare il cambio di faccia

            int relX = getRelativeX(thisTime) - (int)(bw);
            float maxR = bw * (EngineConst.JumpableBlockRatio * 4); //Questo serve per allineare a 16 e 48 pt
            double d = xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime);
            if (d > 0 && relX > maxRange && game.CheckBlocks(false) != 0)
            {
                //TODO: Sono a destra e vado a destra
                float offset = -relX + maxR;
                Matrix m = new Matrix();
                m.Rotate(-this.rot);

                PointF off = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, new PointF(offset, 0)));

                #region Sposto la palla dove devo
                xyrotatorAnim.ChangeAnimator(
                    0,
                    new SteadyAnimator(
                        Math.Round(centerX + off.X),
                        thisTime)
                        );
                xyrotatorAnim.ChangeAnimator(
                    1,
                    new SteadyAnimator(
                        Math.Round(centerY + off.Y),
                        thisTime)
                        );
                #endregion

                int block = game.CheckBlocks(false);
                changeFace(false, block, offset, thisTime);
            }
            else if (d < 0 && relX < -maxRange && game.CheckBlocks(true) != 0)
            {
                //TODO: Sono a sinistra e vado a sinistra
                float offset = - relX - maxR;
                Console.WriteLine("E qui? Relative X =" + relX + "; MaxR = " + maxR + "=> offset = " + offset);
                Matrix m = new Matrix();
                m.Rotate(-this.rot);

                PointF off = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, new PointF(offset, 0)));

                #region Sposto la palla dove devo
                xyrotatorAnim.ChangeAnimator(
                    0,
                    new SteadyAnimator(
                        Math.Round(centerX + off.X),
                        thisTime)
                        );
                xyrotatorAnim.ChangeAnimator(
                    1,
                    new SteadyAnimator(
                        Math.Round(centerY + off.Y),
                        thisTime)
                        );
                #endregion

                if (relX == -32)
                {
                    Console.WriteLine("Caso anomalo: Relative X =" + relX + "; MaxR = " + maxR + "=> offset = " + offset);
                    Console.WriteLine("Per evitare di finire dentro un blocco (problemini numerici...), sposto la palla indietro");
                    bugRecovery(relX);
                }
                int block = game.CheckBlocks(true);
                changeFace(true, block, offset, thisTime);
            }
        }
        #endregion
        #region Altri metodi
        private bool isThisStateAlso(BallState thisState, BallState also)
        {
            return (int)(thisState | also) == (int)thisState;
        }
        private void prepareTexture()
        {
            #region Ricreo la texture della palla e la sua luce
            if (finalTex != null)
                finalTex.Dispose();
            finalTex = new Bitmap(srcTex, new Size((int)(EngineConst.BlockWidth / 2f),(int)(EngineConst.BlockWidth / 2f)));

            if (finalLight != null)
                finalLight.Dispose();
            finalLight = new Bitmap(srcLight, new Size((int)(EngineConst.BlockWidth / 2f), (int)(EngineConst.BlockWidth / 2f)));
            #endregion

            #region Preparo il contesto grafico per ricreare la texture, clippando pure (evito di disegnare rettangoli)
            Graphics g = Graphics.FromImage(finalTex);
            GraphicsPath gp = new GraphicsPath();
            Rectangle textureBounds = new Rectangle(0, 0, finalTex.Width, finalTex.Height);
            gp.AddEllipse(textureBounds);
            g.Clip = new Region(gp);
            g.FillRectangle(new SolidBrush(Color.Blue), textureBounds);
            #endregion
            #region Preparo la rotazione della texture
            Matrix m = new Matrix();
            m.RotateAt(texRot, new PointF(finalTex.Width / 2f, finalTex.Height / 2f));
            g.Transform = m;
            #endregion
            g.DrawImage(srcTex, textureBounds);
            #region Preparo l'eventuale ustione della palla
            float multiplier = (1f - life / 100f);
            if (multiplier < 0)
                multiplier = 0f;
            else if (multiplier > 1)
                multiplier = 1f;
            int alpha = (int)(255f * multiplier);
            #endregion
            g.FillEllipse(new SolidBrush(Color.FromArgb(alpha, Color.Red)), textureBounds);
            #region Ripreparo la rotazione per il riflesso
            m.Reset();
            m.RotateAt(rot, new PointF(finalLight.Width / 2f, finalLight.Height / 2f));
            g.Transform = m;
            #endregion
            g.DrawImage(finalLight, textureBounds);
            g.Dispose();
        }
        private int getRelativeX(long thisTime)
        {
            Matrix m = new Matrix();
            m.Rotate(-Rotation);
            PointF[] dir = new PointF[1] { new PointF(1, 0) };
            m.TransformPoints(dir);

            PointF tileBall = new PointF(Center.X % EngineConst.BlockWidth, Center.Y % EngineConst.BlockWidth);

            int ret = (int)Math.Round(MatrixUtils.ScalarProduct(tileBall, dir[0]));
            while (ret < 0) ret += (int)EngineConst.BlockWidth;

            return ret;
        }
        #endregion
        #region Metodi di inizializzazione
        private void setVariables()
        {
            centerX = startingX;
            centerY = startingY;
            rot = startingRot;
            cf_timer.Restart();
            state = BallState.Flying;
        }
        private void setAnimators(long thisTime)
        {
            lifeAnim = new SteadyAnimator(100, thisTime);
            xyrotatorAnim = new XYTextureRotationAnimator((float)startingX, (float)startingY, (float)texRot, (float)startingRot, thisTime);
            rotAnim = new SteadyAnimator(startingRot, thisTime);
            scaleXAnim = new SteadyAnimator(1, thisTime);
            scaleYAnim = new SteadyAnimator(1, thisTime);
        }
        private void setFoundSurfaces()
        {
            foundSurfaces = new SortedDictionary<SurfType, bool>();
            foundSurfaces.Add(SurfType.Fire, isThisStateAlso(state, BallState.Burning));
            foundSurfaces.Add(SurfType.Forced, isThisStateAlso(state, BallState.GroundForced));
            foundSurfaces.Add(SurfType.Ice, isThisStateAlso(state, BallState.Sliding));
        }
        #endregion
        #region Interpretazione comandi
        private void commandDispatcher(long thisTime, Command c)
        {
            isGoingToMove = false;
            switch (c)
            {
                case Command.Jump:
                    {
                        tryJump(thisTime);
                        break;
                    }
                case Command.Left:
                    {
                        startMove(thisTime, false);
                        isGoingToMove = true;
                        break;
                    }
                case Command.Right:
                    {
                        startMove(thisTime, true);
                        isGoingToMove = true;
                        break;
                    }
                default:
                     break;
            }
        }
        private void tryJump(long thisTime)
        {
            if (canIJump(thisTime))
            {
                double d = Math.Sign(xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime));
                state |= BallState.Flying;

                Matrix m = new Matrix();
                PointF offs = new PointF(0, -2);
                m.Rotate((float)-rotAnim.CalculateValue(thisTime));
                offs = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, offs));

                #region Preparo verticalmente il salto
                xyrotatorAnim.ChangeAnimator(
                   1,
                   new ParabolicToLinearAnimator(
                       Math.Round(centerY + offs.Y),
                       thisTime,
                       EngineConst.GravityY,
                       EngineConst.NormalJumpYSpeed,
                       EngineConst.MaxVerticalSpeed
                       )
                );
                #endregion
                #region Preparo orizzontalmente il salto
                xyrotatorAnim.ChangeAnimator(
                    0,
                    new LinearBoundedAnimator(
                        Math.Round(centerX + offs.X),
                        thisTime,
                        d * EngineConst.NormalJumpXSpeed,
                        475
                        )
                    );
                updateProperties(thisTime);
                #endregion
            }
        }
        private void startMove(long thisTime, bool goRight)
        {
            if (canIMove(thisTime))
            {
                double d = 0;
                if (goRight)
                    d = 1;
                else
                    d = -1;
                #region Preparo l'animazione per lo spostamento laterale
                xyrotatorAnim.ChangeAnimator(
                    0,
                    new ParabolicToLinearAnimator(
                        Math.Round(centerX),
                        thisTime,
                        d * 0.0128,
                        0,
                        0.256
                        )
                    );
                xyrotatorAnim.BindTextureRotationToMovement(true);
                #endregion
                movingRight = goRight;
            }
            if (goRight != xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime) < 0)
                isGoingToMove = true;
        }
        #endregion
        #endregion
        
        #region Costruttori
        public Ball()
        {
            //Costruttore vuoto
            setVariables();
            setAnimators(0);
            setFoundSurfaces();
        }

        public Ball(Bitmap texture, Bitmap lightMap, GameScreen gameReference)
        {
            if (texture == null || gameReference == null || lightMap == null)
                return;
            srcTex = texture;
            game = gameReference;
            srcLight = lightMap;
            setVariables();
            setAnimators(0);
            setFoundSurfaces();
        }
        #endregion

        #region Metodi pubblici per l'aggiornamento e il disegno della palla
        /// <summary>
        /// Aggiorna lo stato della palla durante il gioco
        /// </summary>
        /// <param name="thisTime">Tempo passato dall'inizio del livello</param>
        /// <param name="c">Comando da passare alla palla</param>
        public void Update(long thisTime, Command c)
        {
            updateProperties(thisTime);
            if (!lifeControl(thisTime))
                return;
            if (IsStateAlso(BallState.ChangingFace))
            {
                //TODO da continuare
                #region Caso spostamento verso il vertice
                if (phase == 0)
                {
                    long now = cf_timer.ElapsedMilliseconds;

                    if (now - lastMoment > 125)
                    {
                        //Ho finito di spostarmi, rotazione
                        Matrix m = new Matrix();
                        m.Rotate(-this.rot);

                        PointF off = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, new PointF(0, EngineConst.BlockWidth / 4f)));

                        #region Modifico il movimento della palla nella rotazione attorno al vertice
                        xyrotatorAnim.ChangeAnimator(
                            0,
                            new QuarterRotationAnimator(
                                Math.Round(centerX + off.X),
                                thisTime,
                                (long)(EngineConst.SecsToChangeFace * 1000),
                                rotaz > 0,
                                true,
                                EngineConst.BlockWidth / 4f
                                )
                                );

                        xyrotatorAnim.ChangeAnimator(
                            1,
                            new QuarterRotationAnimator(
                                Math.Round(centerY + off.Y),
                                thisTime,
                                (long)(EngineConst.SecsToChangeFace * 1000),
                                rotaz > 0,
                                false,
                                EngineConst.BlockWidth / 4f
                                )
                                );
                        xyrotatorAnim.ChangeAnimator(
                            2,
                            new SteadyAnimator(
                                Math.Round(texRot),
                                thisTime)
                                );
                        #endregion

                        rotAnim = new LinearBoundedAnimator(
                        Math.Round(rot),
                        thisTime,
                        (double)-(rotaz / (EngineConst.SecsToChangeFace * 1000)),
                        (long)(EngineConst.SecsToChangeFace * 1000)
                        );
                        phase = 1;
                        lastMoment = cf_timer.ElapsedMilliseconds;
                    }
                }
                #endregion
                #region Caso rotazione
                else if (phase == 1)
                {
                    long now = cf_timer.ElapsedMilliseconds;
                    if (now - lastMoment > EngineConst.SecsToChangeFace * 1000)
                    {
                        float speed = 0.256f;
                        this.ChangeGravity((float)Math.Round(rot), thisTime);
                        if (toTheLeft)
                            speed *= -1;

                        #region Sistemo le animazioni
                        xyrotatorAnim.ChangeAnimator(
                            0,
                            new LinearBoundedAnimator(
                                Math.Round(centerX),
                                thisTime,
                                speed,
                                125)
                                );

                        xyrotatorAnim.ChangeAnimator(
                            1,
                            new SteadyAnimator(
                                Math.Round(centerY),
                                thisTime
                                )
                                );
                        #endregion

                        phase = 2;
                        lastMoment = cf_timer.ElapsedMilliseconds;
                    }
                }
                else if (phase == 2)
                {
                    long now = cf_timer.ElapsedMilliseconds;
                    if (now - lastMoment > 125)
                    {
                        blockAll(thisTime);
                        SetState(BallState.ChangingFace, false);
                        if (IsStateAlso(BallState.Sliding))
                        {
                            double d = 1;
                            if (!movingRight)
                                d *= -1;

                            xyrotatorAnim.ChangeAnimator(
                            0,
                            new LinearUnboundedAnimator(
                                Math.Round(centerX),
                                thisTime,
                                d * 0.256
                                )
                           );
                        }
                    }
                }
                #endregion
            }
            else
            {
                commandDispatcher(thisTime, c);
                if (!isThisStateAlso(state, BallState.Flying))
                {
                    centeringControl(thisTime);
                    changingFaceControl(thisTime);
                    //TODO da continuare?
                }
            }
        }
        /// <summary>
        /// Metodo per disegnare la palla: il contesto grafico deve già essere impostato in maniera tale da disegnare la palla
        /// sullo schermo finale, infatti rimanendo sempre al centro dello schermo, il suo disegno NON dipende dalla sua posizione.
        /// Quindi solo lo scaling è richiesto.
        /// </summary>
        /// <param name="e">Contesto grafico in input</param>
        public void Draw(Graphics e)
        {
            #region Preparo le coordinate per la palla e la sua regione di disegno
            float x = EngineConst.BlockWidth * EngineConst.ViewportXTiles * EngineConst.ViewportBallXRatio;
            float y = EngineConst.BlockWidth * EngineConst.ViewportYTiles * EngineConst.ViewportBallYRatio;
            RectangleF b = new RectangleF(x - Radium + (2 - 2 * scaleX) * Radium, y - Radium + (2 - 2 * scaleY) * Radium, 2 * scaleX * Radium, 2 * scaleY * Radium);
            #endregion
            #region Preparo la texture della palla.
            prepareTexture();
            #endregion
            #region Disegno la palla con la texture modificata.
            e.DrawEllipse(Pens.Black, b);
            e.DrawImage(finalTex, b);
            #endregion
        }
        #endregion

        #region Metodi pubblici per la modifica delle proprietà statiche della palla
        public void SetTexture(Bitmap tex)
        {
            if (tex != null)
                srcTex = tex;
        }
        public void SetLightMap(Bitmap lm)
        {
            if (lm != null)
                srcLight = lm;
        }
        public void SetStartingPoint(float x, float y, float rotation, float texRot)
        {
            startingX = x;
            startingY = y;
            startingRot = rotation;
            this.texRot = texRot;
        }
        #endregion

        #region Metodi pubblici per la modifica dello stato dinamico della palla
        #region Metodi di reset
        public void ResetState(long thisTime)
        {
            setVariables();
            setAnimators(0);
            setFoundSurfaces();
        }
        #endregion
        #region Metodi di dispatching al gamescreen
        public void ModifyScore(long amount)
        {
            game.ModScore(amount);
        }

        public void Die(DeathType dt, long thisTime)
        {
            blockAll(thisTime);
            game.Die(dt);
        }

        public void IncreaseKey()
        {
            game.AddKey();
        }

        public void IncreaseFruit()
        {
            game.AddFruit();
        }

        public void InvertTime(long thisTime)
        {
            game.InvertTime(thisTime);
        }

        public void RemoveTime(int amount, long thisTime)
        {
            game.RemoveTime(amount, thisTime);
        }

        public void Exit()
        {
            game.EndLevel();
        }

        public void PlaySound(string smp)
        {
            game.PlaySound(smp);
        }
        #endregion
        #region Metodi di semplice modifica di stato
        public void SetState(BallState s, bool val)
        {
            if (val)
                state |= s;
            else
                state &= ~s;
        }
        public bool IsStateAlso(BallState s)
        {
            return isThisStateAlso(state, s);
        }
        public void ViewMore()
        {
            state |= BallState.ViewMore;
        }
        public void StartEscaping()
        {
            state |= BallState.Exiting;
        }
        #endregion
        #region Metodi di modifica ricevuti da un blocco
        public void BounceLateral(double offX, long thisTime)
        {
            float h = Math.Sign(offX);

            Matrix persp = new Matrix();
            persp.Rotate(-this.rot);
            PointF offset = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(persp, new PointF((float)offX, 0)));

            #region Preparo le animazioni nuove
            xyrotatorAnim.ChangeAnimator(
                0, 
                new LinearBoundedAnimator(
                    Math.Round(centerX + offset.X), 
                    thisTime, 
                    h * 0.064, 
                    250)
                    );
            xyrotatorAnim.ChangeAnimator(
                1, 
                new ParabolicToLinearAnimator(
                    Math.Round(centerY + offset.Y), 
                    thisTime, 
                    EngineConst.GravityY, 
                    xyrotatorAnim.GetAnimation(1).GetCurrentSpeed(thisTime), 
                    EngineConst.MaxVerticalSpeed
                    )
                    );
            xyrotatorAnim.BindTextureRotationToMovement(true);

            scaleXAnim = new ParabolicUnboundedAnimator(1, thisTime, 0.00003f, -0.003f);
            movingRight = (h >= 0);
            #endregion
            state |= BallState.Flying;

        }
        public void BounceDown(double offY, long thisTime)
        {
            Matrix persp = new Matrix();
            persp.Rotate(-this.rot);
            PointF offset = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(persp, new PointF(0, (float)offY)));

            #region Preparo le animazioni nuove
            double speed = Math.Abs(xyrotatorAnim.GetAnimation(1).GetCurrentSpeed(thisTime));
            xyrotatorAnim.ChangeAnimator(
                1,
                new ParabolicToLinearAnimator(
                    Math.Round(centerY + offset.Y),
                    thisTime,
                    EngineConst.GravityY,
                    Math.Max(speed, 0.064),
                    EngineConst.MaxVerticalSpeed
                    )
                );
            xyrotatorAnim.ChangeAnimator(
                0,
                new LinearBoundedAnimator(
                    Math.Round(centerX + offset.X),
                    thisTime,
                    Math.Sign(xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime)) * 0.256,
                    250
                    )
                );
            xyrotatorAnim.BindTextureRotationToMovement(true);
            #endregion
            
            scaleYAnim = new ParabolicUnboundedAnimator(1, thisTime, 0.00003f, -0.003f);
            state |= BallState.Flying;

            game.PlaySound("Bounce");
        }
        public void Land(double offY, long thisTime)
        {
            Matrix persp = new Matrix();
            persp.Rotate(-this.rot);

            PointF offset = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(persp, new PointF(0, (float)offY)));
            centerX += offset.X;
            centerY += offset.Y;

            blockAll(thisTime);
            scaleYAnim = new ParabolicUnboundedAnimator(1, thisTime, 0.00003f, -0.003f);

            state &= ~BallState.Flying;
            game.PlaySound("Bounce");
        }
        public void StartFalling(long thisTime)
        {
            //TODO: E se nel mentre cambia faccia?!
            needToFall = true;
        }
        #endregion
        #region Metodi ricevuti da una superficie o un piazzabile
        public void RampJump(long thisTime)
        {
            if (canIRumpJump(thisTime))
            {
                double d = Math.Sign(xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime));

                if (d == 0 && movingRight)
                    d = 1;
                else if (d == 0 && !movingRight)
                    d = -1;

                state |= BallState.Flying;

                Matrix m = new Matrix();
                PointF offs = new PointF(0, -2);
                m.Rotate((float)-rotAnim.CalculateValue(thisTime));
                offs = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, offs));

                #region Preparo verticalmente il salto
                xyrotatorAnim.ChangeAnimator(
                   1,
                   new ParabolicToLinearAnimator(
                       Math.Round(centerY + offs.Y),
                       thisTime,
                       EngineConst.GravityY,
                       EngineConst.RampJumpYSpeed,
                       EngineConst.MaxVerticalSpeed
                       )
                );
                #endregion
                #region Preparo orizzontalmente il salto
                xyrotatorAnim.ChangeAnimator(
                    0,
                    new LinearBoundedAnimator(
                        Math.Round(centerX + offs.X),
                        thisTime,
                        d * EngineConst.RampJumpXSpeed,
                        670
                        )
                    );
                #endregion

                updateProperties(thisTime);
                game.PlaySound("Ramp");
            }
            
        }
        public void StartBallModify(SurfType st, long thisTime)
        {
            switch (st)
            {
                case SurfType.Ice:
                    {
                        SetState(BallState.Sliding, true);
                        if (Math.Round(xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime)) == 0)
                        {
                            double d = 1;
                            if (!movingRight)
                                d *= -1;

                            xyrotatorAnim.ChangeAnimator(
                                0,
                                new LinearUnboundedAnimator(
                                    Math.Round(centerX),
                                    thisTime,
                                    d * 0.256
                                    )
                                );
                        }
                        break;
                    }
                case SurfType.Forced:
                    {
                        SetState(BallState.GroundForced, true);
                        break;
                    }
                case SurfType.Fire:
                    {
                        if (!IsStateAlso(BallState.Burning))
                        {
                            Console.WriteLine("BurnBabyBurn.");
                            SetState(BallState.Burning, true);
                            double l = Math.Max(lifeAnim.CalculateValue(thisTime) - 27, 0);
                            lifeAnim =
                                new LinearBoundedAnimator(
                                    l,
                                    thisTime,
                                    -83 * 0.001 / EngineConst.SecsToDieBurnt,
                                    l,
                                    0
                                    );
                        }
                        break;
                    }
            }
        }
        public void ChangeGravity(float rotation, long thisTime)
        {
            rot = rotation;
            xyrotatorAnim.ChangePerspective(rot, thisTime, true);
            rotAnim = new SteadyAnimator(rot, thisTime);
        }
        #endregion
        #endregion

        #region Metodi pubblici per modificare la gestione delle collisioni con le superfici
		public void SetFoundSurface(SurfType st, bool b)
        {
            if (foundSurfaces.ContainsKey(st))
                foundSurfaces.Remove(st);
            foundSurfaces.Add(st, b);
        }

        public bool IsFoundSurface(SurfType st)
        {
            bool b;
            if (!foundSurfaces.TryGetValue(st, out b))
                throw new KeyNotFoundException("In IsFoundSurface(...) the surface type is not treated");
            return b;
        }
	    #endregion

        #region Metodi pubblici di debugging
        public string printState(long thisTime)
        {
            float spX = (float)xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime);
            float spY = (float)xyrotatorAnim.GetAnimation(1).GetCurrentSpeed(thisTime);

            string res = "=====================\n====================\n";
            res += "Ball state:\n";
            res += "Position at: " + Center.ToString() + "\n";
            res += "Perspective: " + Rotation.ToString() + "\n";
            res += "Texture rotation: " + texRot + "\n";
            res += "Velocity: " + new PointF(spX, spY).ToString() + "\n";
            res += "Surfaces status: " + foundSurfaces.ToString() + "\n";
            foreach (BallState bs in Enum.GetValues(typeof(BallState)))
                res += bs.ToString() + ": " + IsStateAlso(bs) + "\n";
            res += "=====================\n====================\n";

            return res;
        }
        #endregion
    }
}
