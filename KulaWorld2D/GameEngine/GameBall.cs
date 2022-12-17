using GameEngine.Animators;
using GameEngine.Enumerations;
using GameUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngine
{
    /**
     * Marco, 4y after says: AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
     */
    public class Ball
    {
        
        private bool movingRight = true;
        private bool isGoingToMove = false;
        private bool needToFall = false;
        
        private int phase = 0;
        private Stopwatch cf_timer = new Stopwatch();
        private long lastMoment = 0;
        float rotaz = 90f;
        private bool toTheLeft = false;
        
        private double startingX = Constants.BlockWidth / 2f;
        private double startingY = Constants.BlockWidth / 2f;
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
        
        private XYTextureRotationAnimator xyrotatorAnim;
        private Animator lifeAnim;
        private Animator rotAnim;
        private Animator scaleXAnim;
        private Animator scaleYAnim;
        
        private SortedDictionary<SurfType, bool> foundSurfaces;
        
        public float Radium
        {
            get
            {
                return Constants.BlockWidth / 4f;
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
        
        private bool canIJump(long thisTime)
        {
            bool goodState =
                !isThisStateAlso(state, BallState.Flying) &&
                !isThisStateAlso(state, BallState.GroundForced) &&
                !isThisStateAlso(state, BallState.ChangingFace);
            int relativeX = getRelativeX(thisTime) - ((int)(Constants.BlockWidth / 2f));
            float maxRegion = ((int)(Constants.BlockWidth / 2f)) - Constants.BlockWidth * Constants.JumpableBlockRatio;
            bool goodPosition = Math.Abs(maxRegion) >= Math.Abs(relativeX);
            return goodState && goodPosition;
        }
        private bool canIRumpJump(long thisTime)
        {
            bool goodState =
                !isThisStateAlso(state, BallState.Flying) &&
                !isThisStateAlso(state, BallState.GroundForced) &&
                !isThisStateAlso(state, BallState.ChangingFace);
            int relativeX = getRelativeX(thisTime) - ((int)(Constants.BlockWidth / 2f));
            float maxRegion = ((int)(Constants.BlockWidth / 2f)) - Constants.BlockWidth * Constants.JumpableBlockRatio * 2;
            bool goodPosition = Math.Abs(maxRegion) >= Math.Abs(relativeX);
            return goodState && goodPosition;
        }
        private bool canIMove(long thisTime)
        {
            bool goodState =
                !isThisStateAlso(state, BallState.ChangingFace) &&
                !isThisStateAlso(state, BallState.Flying) &&
                !IsStateAlso(BallState.Sliding);
            bool isSteady = xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime) == 0;
            return goodState && isSteady;
        }
        
        
        //Questo metodo è fatto proprio per il bug della rotazione
        private void bugRecovery(int relX)
        {
            Matrix m = new Matrix();
            m.Rotate(-rot);
            PointF offset = m.TransformPointF(new PointF(1, 0));

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
                        rot, 
                        thisTime,
                        -(rotaz / (Constants.SecsToChangeFace * 1000)),
                        (long)(Constants.SecsToChangeFace * 1000)
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
            
            if (needToFall && !IsStateAlso(BallState.ChangingFace))
            {
                blockAll(thisTime);
                xyrotatorAnim.ChangeAnimator(
                    1,
                    new ParabolicToLinearAnimator(
                        Math.Round(centerY),
                        thisTime,
                        Constants.GravityY,
                        xyrotatorAnim.GetAnimation(1).GetCurrentSpeed(thisTime),
                        Constants.MaxVerticalSpeed
                        )
                    );
                state |= BallState.Flying;
                needToFall = false;
            }
            

            
            if (IsStateAlso(BallState.Sliding))
                xyrotatorAnim.ChangeAnimator(2, new SteadyAnimator(texRot, thisTime));
            else
                xyrotatorAnim.BindTextureRotationToMovement(true);
            

            
            life = (float)lifeAnim.CalculateValue(thisTime);
            texRot = (float)xyrotatorAnim.CalcolateTexRot(thisTime);
            centerX = (float)xyrotatorAnim.CalculateX(thisTime, true);
            centerY = (float)xyrotatorAnim.CalculateY(thisTime, true);
            rot = ((float)Math.Round(rotAnim.CalculateValue(thisTime))) % 360;
            while (rot < 0) rot += 360f; //Non mi piace se l'angolo è negativo!
            scaleX = Math.Min(Math.Max((float)scaleXAnim.CalculateValue(thisTime), 0.85f), 1f);
            scaleY = Math.Min(Math.Max((float)scaleYAnim.CalculateValue(thisTime), 0.85f), 1f);
            
        }
        private bool lifeControl(long thisTime)
        {
            if (life <= 0)
            {
                game.Die(DeathType.Fire);
                return false;
            }
            if (!isThisStateAlso(state, BallState.Burning))
                lifeAnim = new LinearBoundedAnimator(life, thisTime, 0.1 / Constants.SecsToChill, 100, 0);
            return true;
        }
        private void centeringControl(long thisTime)
        {
            if (IsStateAlso(BallState.Sliding) && (Math.Abs(xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime)) < 0.256))
            {
                Console.WriteLine("Devo scivolare!");
                xyrotatorAnim.ChangeAnimator(
                    0,
                    new LinearUnboundedAnimator(
                        Math.Round(centerX),
                        thisTime,
                        movingRight ? 0.256 : -0.256
                        )
                   );
            }
            
            int relativeX = getRelativeX(thisTime) - ((int)(Constants.BlockWidth/2f));
            float maxRegion = Constants.BlockWidth * Constants.JumpableBlockRatio;
            if (Math.Abs(maxRegion) > Math.Abs(relativeX) && !isThisStateAlso(state, BallState.Sliding) && !isGoingToMove)
                blockAll(thisTime);
            if (!game.CheckBlock(0, 1))
                StartFalling(thisTime);
        }
        private void changingFaceControl(long thisTime)
        {
            float bw = Constants.BlockWidth / 2;
            int maxRange = 23; //Variabile per vedere quando iniziare il cambio di faccia

            int relX = getRelativeX(thisTime) - (int)(bw);
            float maxR = bw * (Constants.JumpableBlockRatio * 4); //Questo serve per allineare a 16 e 48 pt
            double d = xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime);
            if (d > 0 && relX > maxRange && game.CheckBlocks(false) != 0)
            {
                //TODO: Sono a destra e vado a destra
                float offset = -relX + maxR;
                Matrix m = new Matrix();
                m.Rotate(-rot);

                PointF off = m.TransformPointF(new PointF(offset, 0));
                
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

                PointF off = m.TransformAndThenRound(new PointF(offset, 0));

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
        
        
        private bool isThisStateAlso(BallState thisState, BallState also)
        {
            return (int)(thisState | also) == (int)thisState;
        }
        private void prepareTexture()
        {
            if (finalTex != null)
                finalTex.Dispose();
            finalTex = new Bitmap(srcTex, new Size((int)(Constants.BlockWidth / 2f),(int)(Constants.BlockWidth / 2f)));

            if (finalLight != null)
                finalLight.Dispose();
            finalLight = new Bitmap(srcLight, new Size((int)(Constants.BlockWidth / 2f), (int)(Constants.BlockWidth / 2f)));
            
            Graphics g = Graphics.FromImage(finalTex);
            GraphicsPath gp = new GraphicsPath();
            Rectangle textureBounds = new Rectangle(0, 0, finalTex.Width, finalTex.Height);
            gp.AddEllipse(textureBounds);
            g.Clip = new Region(gp);
            g.FillRectangle(new SolidBrush(Color.Blue), textureBounds);
            
            Matrix m = new Matrix();
            m.RotateAt(texRot, new PointF(finalTex.Width / 2f, finalTex.Height / 2f));
            g.Transform = m;
            
            g.DrawImage(srcTex, textureBounds);
            
            float multiplier = (1f - life / 100f);
            if (multiplier < 0)
                multiplier = 0f;
            else if (multiplier > 1)
                multiplier = 1f;
            int alpha = (int)(255f * multiplier);
            
            g.FillEllipse(new SolidBrush(Color.FromArgb(alpha, Color.Red)), textureBounds);
            
            m.Reset();
            m.RotateAt(rot, new PointF(finalLight.Width / 2f, finalLight.Height / 2f));
            g.Transform = m;
            
            g.DrawImage(finalLight, textureBounds);
            g.Dispose();
        }
        private int getRelativeX(long thisTime)
        {
            Matrix m = new Matrix();
            m.Rotate(-Rotation);
            PointF[] dir = new PointF[1] { new PointF(1, 0) };
            m.TransformPoints(dir);

            PointF tileBall = new PointF(Center.X % Constants.BlockWidth, Center.Y % Constants.BlockWidth);

            int ret = (int)Math.Round(MatrixUtils.ScalarProduct(tileBall, dir[0]));
            while (ret < 0) ret += (int)Constants.BlockWidth;

            return ret;
        }
        
        
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
                case Command.JumpLeft:
                    {
                        startMove(thisTime, false);
                        isGoingToMove = true;
                        tryJump(thisTime);
                        break;
                    }
                case Command.JumpRight:
                    {
                        startMove(thisTime, true);
                        isGoingToMove = true;
                        tryJump(thisTime);
                        break;
                    }
                default:
                     break;
            }
        }

        private void tryJump(long thisTime)
        {
            if (!canIJump(thisTime)) return;

            double d = Math.Sign(xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime));
            state |= BallState.Flying;

            Matrix m = new Matrix();
            PointF off = new PointF(0, -2);
            m.Rotate((float)-rotAnim.CalculateValue(thisTime));
            off = m.TransformAndThenRound(off);

            xyrotatorAnim.ChangeAnimator(
               1,
               new ParabolicToLinearAnimator(
                   Math.Round(centerY + off.Y),
                   thisTime,
                   Constants.GravityY,
                   Constants.NormalJumpYSpeed,
                   Constants.MaxVerticalSpeed
                   )
            );

            xyrotatorAnim.ChangeAnimator(
                0,
                new LinearBoundedAnimator(
                    Math.Round(centerX + off.X),
                    thisTime,
                    d * Constants.NormalJumpXSpeed,
                    475
                    )
                );
            updateProperties(thisTime);

            return;
        }
        private void startMove(long thisTime, bool goRight)
        {
            if (canIMove(thisTime))
            {
                double d = goRight ? 1 : -1;
                
                xyrotatorAnim.ChangeAnimator(
                    0,
                    new ParabolicToLinearAnimator(
                        Math.Round(centerX),
                        thisTime,
                        d * 0.0128,
                        d * Double.Epsilon,
                        0.256
                        )
                    );
                xyrotatorAnim.BindTextureRotationToMovement(true);
                
                movingRight = goRight;
            }
            if (goRight != xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime) < 0)
                isGoingToMove = true;
        }
        
        
        
        
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

            if (!IsStateAlso(BallState.ChangingFace))
            {
                commandDispatcher(thisTime, c);
                if (!isThisStateAlso(state, BallState.Flying))
                {
                    centeringControl(thisTime);
                    changingFaceControl(thisTime);
                }
                return;
            }

            if (phase == 0)
            {
                long now = cf_timer.ElapsedMilliseconds;

                if (now - lastMoment > 125)
                {
                    //Ho finito di spostarmi, rotazione
                    Matrix m = new Matrix();
                    m.Rotate(-rot);

                    PointF off = m.TransformAndThenRound(new PointF(0, Constants.BlockWidth / 4f));


                    xyrotatorAnim.ChangeAnimator(
                        0,
                        new QuarterRotationAnimator(
                            Math.Round(centerX + off.X),
                            thisTime,
                            (long)(Constants.SecsToChangeFace * 1000),
                            rotaz > 0,
                            true,
                            Constants.BlockWidth / 4f
                            )
                            );

                    xyrotatorAnim.ChangeAnimator(
                        1,
                        new QuarterRotationAnimator(
                            Math.Round(centerY + off.Y),
                            thisTime,
                            (long)(Constants.SecsToChangeFace * 1000),
                            rotaz > 0,
                            false,
                            Constants.BlockWidth / 4f
                            )
                            );
                    xyrotatorAnim.ChangeAnimator(
                        2,
                        new SteadyAnimator(
                            Math.Round(texRot),
                            thisTime)
                            );


                    rotAnim = new LinearBoundedAnimator(
                    Math.Round(rot),
                    thisTime,
                    (double)-(rotaz / (Constants.SecsToChangeFace * 1000)),
                    (long)(Constants.SecsToChangeFace * 1000)
                    );
                    phase = 1;
                    lastMoment = cf_timer.ElapsedMilliseconds;
                }
            }
            else if (phase == 1)
            {
                long now = cf_timer.ElapsedMilliseconds;
                if (now - lastMoment > Constants.SecsToChangeFace * 1000)
                {
                    float speed = 0.256f;
                    this.ChangeGravity((float)Math.Round(rot), thisTime);
                    if (toTheLeft)
                        speed *= -1;


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
        }
        /// <summary>
        /// Metodo per disegnare la palla: il contesto grafico deve già essere impostato in maniera tale da disegnare la palla
        /// sullo schermo finale, infatti rimanendo sempre al centro dello schermo, il suo disegno NON dipende dalla sua posizione.
        /// Quindi solo lo scaling è richiesto.
        /// </summary>
        /// <param name="e">Contesto grafico in input</param>
        public void Draw(Graphics e)
        {
            
            float x = Constants.BlockWidth * Constants.ViewportXTiles * Constants.ViewportBallXRatio;
            float y = Constants.BlockWidth * Constants.ViewportYTiles * Constants.ViewportBallYRatio;
            RectangleF b = new RectangleF(x - Radium + (2 - 2 * scaleX) * Radium, y - Radium + (2 - 2 * scaleY) * Radium, 2 * scaleX * Radium, 2 * scaleY * Radium);
            
            prepareTexture();
            
            e.DrawEllipse(Pens.Black, b);
            e.DrawImage(finalTex, b);
            
        }
        

        
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
        

        
        
        public void ResetState(long thisTime)
        {
            setVariables();
            setAnimators(0);
            setFoundSurfaces();
        }
        
        
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
        
        public void BounceLateral(double offX, long thisTime)
        {
            float h = Math.Sign(offX);

            Matrix persp = new Matrix();
            persp.Rotate(-rot);
            PointF offset = persp.TransformAndThenRound(new PointF((float)offX, 0));

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
                    Constants.GravityY, 
                    xyrotatorAnim.GetAnimation(1).GetCurrentSpeed(thisTime), 
                    Constants.MaxVerticalSpeed
                    )
                );
            xyrotatorAnim.BindTextureRotationToMovement(true);

            scaleXAnim = new ParabolicUnboundedAnimator(1, thisTime, 0.00003f, -0.003f);
            movingRight = (h >= 0);
            
            state |= BallState.Flying;
            game.PlaySound("Bounce");
        }
        public void BounceDown(double offY, long thisTime)
        {
            Matrix persp = new Matrix();
            persp.Rotate(-this.rot);
            PointF offset = persp.TransformAndThenRound(new PointF(0, (float)offY));

            double speed = Math.Abs(xyrotatorAnim.GetAnimation(1).GetCurrentSpeed(thisTime));
            xyrotatorAnim.ChangeAnimator(
                1,
                new ParabolicToLinearAnimator(
                    Math.Round(centerY + offset.Y),
                    thisTime,
                    Constants.GravityY,
                    Math.Max(speed, 0.064),
                    Constants.MaxVerticalSpeed
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

            scaleYAnim = new ParabolicUnboundedAnimator(1, thisTime, 0.00003f, -0.003f);
            state |= BallState.Flying;

            game.PlaySound("Bounce");
        }
        public void Land(double offY, long thisTime)
        {
            Matrix persp = new Matrix();
            persp.Rotate(-this.rot);

            PointF offset = persp.TransformAndThenRound(new PointF(0, (float)offY));
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
        
        
        public void RampJump(long thisTime)
        {
            if (!canIRumpJump(thisTime)) return;

            double d = Math.Sign(xyrotatorAnim.GetAnimation(0).GetCurrentSpeed(thisTime));

            if (d == 0 && movingRight)
                d = 1;
            else if (d == 0 && !movingRight)
                d = -1;

            state |= BallState.Flying;

            Matrix m = new Matrix();
            PointF off = new PointF(0, -2);
            m.Rotate((float)-rotAnim.CalculateValue(thisTime));
            off = m.TransformAndThenRound(off);

            xyrotatorAnim.ChangeAnimator(
               1,
               new ParabolicToLinearAnimator(
                   Math.Round(centerY + off.Y),
                   thisTime,
                   Constants.GravityY,
                   Constants.RampJumpYSpeed,
                   Constants.MaxVerticalSpeed
                   )
            );

            xyrotatorAnim.ChangeAnimator(
                0,
                new LinearBoundedAnimator(
                    Math.Round(centerX + off.X),
                    thisTime,
                    d * Constants.RampJumpXSpeed,
                    670
                    )
                );

            updateProperties(thisTime);
            game.PlaySound("Ramp");
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
                            SetState(BallState.Burning, true);
                            double l = Math.Max(lifeAnim.CalculateValue(thisTime) - 27, 0);
                            lifeAnim =
                                new LinearBoundedAnimator(
                                    l,
                                    thisTime,
                                    -83 * 0.001 / Constants.SecsToDieBurnt,
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
        
    }
}
