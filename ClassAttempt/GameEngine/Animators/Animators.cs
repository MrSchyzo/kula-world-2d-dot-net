using GameUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngine.Animators
{
    #region Animatori singoli indipendenti
    #region S_Animator: superclasse per animazioni di un singolo valore
    /// <summary>
    /// Classe che rappresenta un animatore singolo, senza problemi di rotazione annessi
    /// </summary>
    public abstract class S_Animator
    {
        protected double beginningValue;
        protected long lastTimeStamp;

        protected void S_Animator_MutualPart(double val, long timeStamp)
        {
            beginningValue = val;
            lastTimeStamp = timeStamp;
        }

        public S_Animator()
        {
        }

        public S_Animator(double val, long timeStamp)
        {
            S_Animator_MutualPart(val, timeStamp);
        }

        public abstract double GetCurrentSpeed(long thisTime);

        public abstract double CalculateValue(long thisTime);

        public long GetStartingTime()
        {
            return lastTimeStamp;
        }

        public double GetBeginningValue()
        {
            return beginningValue;
        }
        
        public void ResynchronizeAnimation(long thisTime)
        {
            beginningValue = CalculateValue(thisTime);
            lastTimeStamp = thisTime;
        }

        public void ResynchronizeValue(long thisTime, double val)
        {
            beginningValue = val;
            lastTimeStamp = thisTime;
        }
    }
    #endregion

    //Fa comodo quando la palla è ferma
    #region SteadyAnimator: funzione costante
    public class SteadyAnimator : S_Animator
    {
        public SteadyAnimator(double val, long timeStamp)
        {
            S_Animator_MutualPart(val, timeStamp);
        }

        public override double CalculateValue(long thisTime)
        {
            return beginningValue;
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            return 0;
        }
        
    }
    #endregion

    //Fa comodo per tutti i movimenti periodici che partono dal centro
    #region SinusoidalAnimator: funzione sinusoidale
    public class SinusoidalAnimator : S_Animator
    {
        private double periodo;
        private double ampiezza;

        public SinusoidalAnimator(double val, long timeStamp, double period, double width)
        {
            S_Animator_MutualPart(val, timeStamp);
            periodo = period;
            ampiezza = width;
        }

        public override double CalculateValue(long thisTime)
        {
            double argToSin = (((double)thisTime - lastTimeStamp) / periodo) * 2.0 * Math.PI;
            return beginningValue + (Math.Sin(argToSin) * ampiezza);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double argToSin = (((double)thisTime - lastTimeStamp) / periodo) * 2.0 * Math.PI;
            return (Math.Cos(argToSin) * ampiezza);
        }
    }
    #endregion

    //Fa comodo per il movimento non limitato con velocità costante
    #region LinearUnboundedAnimator: funzione lineare "pura"
    public class LinearUnboundedAnimator : S_Animator
    {
        private double c_angolare;

        public LinearUnboundedAnimator(double val, long timeStamp, double slope)
        {
            S_Animator_MutualPart(val, timeStamp);
            c_angolare = slope;
        }

        public override double CalculateValue(long thisTime)
        {
            double arg = ((double)thisTime - lastTimeStamp);
            return beginningValue + arg * c_angolare;
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            return c_angolare;
        }
    }
    #endregion

    //Fa comodo per il movimento sottoposto a gravità, senza limiti di velocità
    #region ParabolicUnboundedAnimator: funzione parabolica "pura"
    public class ParabolicUnboundedAnimator : S_Animator
    {
        private double b_parabola;
        private double a_parabola;

        public ParabolicUnboundedAnimator(double val, long timeStamp, double gravity, double initialSpeed)
        {
            S_Animator_MutualPart(val, timeStamp);
            b_parabola = initialSpeed;
            a_parabola = gravity / 2.0;
        }

        public double getCurrentVelocity(long thisTime)
        {
            double arg = (double)thisTime - lastTimeStamp;
            return b_parabola + 2 * a_parabola * arg;
        }

        public override double CalculateValue(long thisTime)
        {
            double arg = (double)thisTime - lastTimeStamp;
            return beginningValue + (a_parabola * (arg * arg) + b_parabola * arg);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double arg = (double)thisTime - lastTimeStamp;
            return 2 * a_parabola * arg + b_parabola;
        }
    }
    #endregion

    //Fa comodo per l'accentramento automatico della palla
    #region LinearBoundedAnimator: funzione lineare con limitazione di massimo e minimo (o di tempo)
    public class LinearBoundedAnimator : S_Animator
    {
        private double c_angolare;
        private double maxValue;
        private double minValue;
        private long timeCap;
        private bool isTimeBounded;

        public LinearBoundedAnimator(double val, long timeStamp, double slope, double maxVal, double minVal)
        {
            if (minVal > maxVal)
                throw new ArgumentException("In LinearBoundedAnimator(...), minVal MUST BE less than or equal maxVal!!");
            S_Animator_MutualPart(val, timeStamp);
            isTimeBounded = false;
            c_angolare = slope;
            maxValue = maxVal;
            minValue = minVal;
        }

        public LinearBoundedAnimator(double val, long timeStamp, double slope, long timeBeforeCap)
        {
            if (timeBeforeCap < 0)
                throw new ArgumentException("In LinearBoundedAnimator(...), timeBeforeCap MUST BE more than or equal 0");
            S_Animator_MutualPart(val, timeStamp);
            isTimeBounded = true;
            timeCap = timeBeforeCap;
            c_angolare = slope;
        }

        public override double CalculateValue(long thisTime)
        {
            double arg = ((double)thisTime - lastTimeStamp);
            double ret = beginningValue + arg * c_angolare;
            if (isTimeBounded)
                return beginningValue + (Math.Min(arg, (double)timeCap) * c_angolare);
            else
                return Math.Min(Math.Max(minValue, ret), maxValue);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double arg = ((double)thisTime - lastTimeStamp);
            double ret = beginningValue + arg * c_angolare;
            if (isTimeBounded && (arg < 0 || arg > timeCap))
                return 0;
            if (maxValue > ret || ret < minValue)
                return 0;
            else
                return c_angolare;
                
        }
    }
    #endregion

    //Fa comodo per il movimento sottoposto a gravità, ma con limiti di velocità
    #region ParabolicToLinearAnimator: funzione a tratti: parabolica "nel mezzo" e lineare "ai lati"
    public class ParabolicToLinearAnimator : S_Animator
    {
        private double maxAbsoluteValue;
        private double a_parabola;
        private double b_parabola;

        private double linearUpper(double arg)
        {
            double chi = (maxAbsoluteValue - b_parabola);
            return
                -(chi * chi / (4 * a_parabola)) +
                maxAbsoluteValue * arg;
        }

        private double linearLower(double arg)
        {
            double chi = -(maxAbsoluteValue + b_parabola);
            return
               -(chi * chi / (4 * a_parabola)) -
               maxAbsoluteValue * arg;
        }

        public ParabolicToLinearAnimator(double val, long timeStamp, double gravity, double initialVelocity, double maxAbsValue)
        {
            S_Animator_MutualPart(val, timeStamp);
            maxAbsoluteValue = Math.Abs(maxAbsValue);
            a_parabola = gravity / 2.0;
            b_parabola = initialVelocity;
        }

        public override double CalculateValue(long thisTime)
        {
            double t = (double)thisTime - lastTimeStamp;
            double cSpeed = 2 * a_parabola * t + b_parabola;
            if (cSpeed > maxAbsoluteValue)
                return beginningValue + linearUpper(t);
            else if (cSpeed < -maxAbsoluteValue)
                return beginningValue + linearLower(t);
            else
                return beginningValue + a_parabola * t * t + b_parabola * t;
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double t = (double)thisTime - lastTimeStamp;
            return Math.Max(-maxAbsoluteValue, Math.Min(maxAbsoluteValue, 2 * a_parabola * t + b_parabola));
        }

    }
    #endregion

    //Fa comodo per il movimento della X e della Y durante un cambio di faccia
    #region QuarterRotationAnimator: funzione che ruota di un quarto di giro una coordinata
    public class QuarterRotationAnimator: S_Animator
    {
        private long quarterDuration;
        private bool clockWise;
        private bool calculateWithSinus;
        private double radium;

        public QuarterRotationAnimator(double startVal, long timeStamp, long quarterPeriod, bool isClockWise, bool isX, double radium)
        {
            if (quarterPeriod <= 0 || radium <= 0)
                throw new ArgumentException("In QuarterRotationAnimator(...), the period of the quarter or the radium are non-positive.");
            S_Animator_MutualPart(startVal, timeStamp);
            this.radium = radium;
            quarterDuration = quarterPeriod;
            clockWise = isClockWise;
            calculateWithSinus = isX;
        }

        public override double CalculateValue(long thisTime)
        {
            #region Calcolo il tempo passato (e lo rendo tale da rimanere compreso in [0, quarterDuration])
            double t =
                Math.Min
                (
                    Math.Max
                    (
                        0,
                        (double)thisTime - lastTimeStamp

                    ),
                    quarterDuration
                );
            #endregion
            double arg = (t / quarterDuration) * (Math.PI / 2.0);
            if (!clockWise)
                arg *= -1;
            if (calculateWithSinus)
                return beginningValue + radium * Math.Sin(arg);
            else
                return beginningValue - radium * Math.Cos(arg);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            #region Calcolo il tempo passato (e lo rendo tale da rimanere compreso in [0, quarterDuration])
            double t =
                Math.Min
                (
                    Math.Max
                    (
                        0,
                        (double)thisTime - lastTimeStamp

                    ),
                    quarterDuration
                );
            #endregion
            double arg = (t / quarterDuration) * (Math.PI / 2.0);
            if (arg > 90)
                arg = 90;
            if (arg < 0)
                arg = 0;
            if (!clockWise)
                arg *= -1;
            if (calculateWithSinus)
                return radium * Math.Cos(arg);
            else
                return radium * Math.Sin(arg);
        }
    }
    #endregion

    //Fa comodo per l'apparizione e la sparizione dei blocchi intermittenti
    #region AlternateAnimator: funzione ciclica che alterna due valori
    public class AlternateAnimator : S_Animator
    {
        private long timeOffset;
        private double firstValue;
        private double secondValue;
        private long transitionPeriod;
        private long steadyPeriod;

        public AlternateAnimator(double startVal, long timeStamp, long offset, double firstVal, double secondVal, long transPeriod, long standPeriod)
        {
            if (transPeriod < 0 && standPeriod < 0)
                throw new ArgumentException("In AlternateAnimator(...) at least one of the two last args is incorrect.");
            beginningValue = startVal;
            lastTimeStamp = timeStamp;
            timeOffset = offset;
            firstValue = firstVal;
            secondValue = secondVal;
            transitionPeriod = transPeriod;
            steadyPeriod = standPeriod;
        }

        private double derivative(long time)
        {
            if (time <= transitionPeriod)
            {
                double m = (firstValue - secondValue) / ((double)transitionPeriod);
                return m;
            }
            else if (time > transitionPeriod && time <= transitionPeriod + steadyPeriod)
            {
                return 0;
            }
            else if (time > transitionPeriod + steadyPeriod && time <= 2 * transitionPeriod + steadyPeriod)
            {
                double m = (secondValue - firstValue) / ((double)transitionPeriod);
                return m;
            }
            else if (time < 2 * transitionPeriod + 2 * steadyPeriod)
            {
                return 0;
            }
            else
                throw new Exception("Calculating the alternator's value went wrong.");
        }

        private double calculator(long time)
        {
            if (time <= transitionPeriod)
            {
                double m = (secondValue - firstValue) / ((double)transitionPeriod);
                double x = time;
                double y0 = firstValue;
                return y0 + m * x;
            }
            else if (time > transitionPeriod && time <= transitionPeriod + steadyPeriod)
            {
                return secondValue;
            }
            else if (time > transitionPeriod + steadyPeriod && time <= 2 * transitionPeriod + steadyPeriod)
            {
                double m = (firstValue - secondValue) / ((double)transitionPeriod);
                double x = time;
                double y0 = secondValue;
                double x0 = transitionPeriod + steadyPeriod;
                return m * x - m * x0 + y0;
            }
            else if (time < 2 * transitionPeriod + 2 * steadyPeriod)
            {
                return firstValue;
            }
            else
                throw new Exception("Calculating the alternator's value went wrong.");
        }

        public override double CalculateValue(long thisTime)
        {
            long totalPeriod = 2 * transitionPeriod + 2 * steadyPeriod;
            long newTime = (thisTime + timeOffset) % totalPeriod;
            return calculator(newTime);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            long totalPeriod = 2 * transitionPeriod + 2 * steadyPeriod;
            long newTime = (thisTime + timeOffset) % totalPeriod;
            return derivative(newTime);
        }
    }
    #endregion
    #endregion

    #region Animatori multipli ma indipendenti
    #region MultiAnimator: superclasse per contenitore di animazioni multiple ma indipendenti
    public class MultiAnimator
    {
        protected SortedDictionary<string, S_Animator> animators;

        #region Costruttori
        public MultiAnimator()
        {
            animators = new SortedDictionary<string, S_Animator>();
        }
        #endregion

        #region Metodi pubblici per il calcolo delle animazioni
        public double CalculateSpecificValue(string symbol, long thisTime)
        {
            S_Animator an;
            if (animators.TryGetValue(symbol, out an))
                return an.CalculateValue(thisTime);
            throw new KeyNotFoundException("In MultiAnimator.CalculateSpecificValue(...) the animator was not found.");
        }

        public SortedDictionary<string, double> CalculateAllValues(long thisTime)
        {
            SortedDictionary<string, double> ret = new SortedDictionary<string, double>();
            foreach (string s in animators.Keys)
            {
                S_Animator an;
                if (animators.TryGetValue(s, out an))
                    ret.Add(s, an.CalculateValue(thisTime));
            }
            return ret;
        }
        #endregion

        #region Metodi pubblici per la manipolazione delle animazioni
        public bool AddAnimation(string s, S_Animator an)
        {
            if (an == null)
                throw new ArgumentNullException("In MultiAnimator.AddAnimation(...) the animator was NULL.");
            if (animators.ContainsKey(s))
                return false;
            else
                animators.Add(s, an);
            return true;
        }

        public bool ReplaceAnimation(string s, S_Animator an)
        {
            if (an == null)
                throw new ArgumentNullException("In MultiAnimator.ReplaceAnimation(...) the animator was NULL.");
            if (!animators.ContainsKey(s))
                return false;
            animators.Remove(s);
            animators.Add(s, an);
            return true;
        }

        public S_Animator GetAnimation(string s)
        {
            S_Animator an = null;
            if (animators.TryGetValue(s, out an))
                return an;
            else
                throw new KeyNotFoundException("In MultiAnimator.GetAnimation(...) the animator was not found.");
        }

        public void RemoveAllAnimations()
        {
            animators.Clear();
        }
        #endregion

    }
    #endregion
    #endregion

    #region Animatori multipli legati tra loro
    #region XYTextureRotationAnimator: animatore per la posizione x, posizione y e la rotazione della texture
    /// <summary>
    /// Questa classe è utile ad animare il movimento della palla e la sua rotazione
    /// </summary>
    public class XYTextureRotationAnimator
    {
        private S_Animator animX;
        private S_Animator animY;
        private S_Animator animTexRot;
        private double perspectiveRot;
        private bool bindTexRotToMovement;

        #region Metodi privati per la restituzione del punto di partenza e il punto attuale
        private double startX { get { return animX.GetBeginningValue(); } }

        private double startY { get { return animY.GetBeginningValue(); } }

        private double startTexRot { get { return animTexRot.GetBeginningValue(); } }
        
        private PointF getOrigin()
        {
            return new PointF((float)startX, (float)startY);
        }

        private PointF calculateXY(long thisTime)
        {
            return new PointF((float)animX.CalculateValue(thisTime), (float)animY.CalculateValue(thisTime));
        }
        #endregion

        #region Costruttori
        public XYTextureRotationAnimator(float startingX, float startingY, float startingTextureRotation, float perspectiveRotation, long timeStamp)
        {
            bindTexRotToMovement = true;
            animX = new SteadyAnimator(startingX, timeStamp);
            animY = new SteadyAnimator(startingY, timeStamp);
            animTexRot = new SteadyAnimator(startingTextureRotation, timeStamp);
            perspectiveRot = perspectiveRotation;
        }
        #endregion

        #region Metodi per la manipolazione delle animazioni
        public bool ChangeAnimator(byte valueToAnimate, S_Animator animation)
        {
            if (animation == null)
                throw new ArgumentNullException("In ChangeAnimator(...) the animator was NULL");
            if (valueToAnimate > 2)
                return false;
            if (valueToAnimate == 0)
            {
                bindTexRotToMovement = true;
                animX = animation;
            }
            else if (valueToAnimate == 1)
            {
                bindTexRotToMovement = true;
                animY = animation;
            }
            else
            {
                animTexRot = animation;
                bindTexRotToMovement = false;
            }
            return true;
        }

        public void Reset(float startingX, float startingY, float startingTextureRotation, float perspectiveRotation, long timeStamp)
        {
            bindTexRotToMovement = true;
            animX = new SteadyAnimator(startingX, timeStamp);
            animY = new SteadyAnimator(startingY, timeStamp);
            animTexRot = new SteadyAnimator(startingTextureRotation, timeStamp);
            perspectiveRot = perspectiveRotation;
        }
        #endregion

        #region Metodi per il calcolo della X, della Y, e della rotazione della texture
        public double CalculateX(long thisTime, bool viewCoordinates)
        {
            Matrix m = new Matrix();
            if (viewCoordinates)
                m.RotateAt(-(float)perspectiveRot, getOrigin());
            double ret = MatrixUtils.TransformPointF(m, calculateXY(thisTime)).X;
            m.Dispose();
            return ret;
        }
        public double CalculateY(long thisTime, bool viewCoordinates)
        {
            Matrix m = new Matrix();
            if (viewCoordinates)
                m.RotateAt(-(float)perspectiveRot, getOrigin());
            double ret = MatrixUtils.TransformPointF(m, calculateXY(thisTime)).Y;
            m.Dispose();
            return ret;
        }
        public double CalcolateTexRot(long thisTime)
        {
            if (bindTexRotToMovement)
                return startTexRot + ((CalculateX(thisTime, false) - startX)/(2.0*Math.PI*EngineConst.BlockWidth/4.0)) * 360.0;
            else
                return animTexRot.CalculateValue(thisTime);
        }
        #endregion

        #region Metodi che alterano lo stato delle animazioni
        public void ChangePerspective(float rot, long thisTime, bool autoRefresh)
        {
            int rotation = ((int)Math.Round(rot, 0)) % 360;
            if (rotation % 90 != 0)
                throw new ArgumentException("In ChangePerspective(...) the rotation must be a multiple of 90.");
            perspectiveRot = rot; 
            if (autoRefresh)
            {
                /*animX.ResynchronizeValue(thisTime, calculateXY(thisTime).X);
                animY.ResynchronizeValue(thisTime, calculateXY(thisTime).Y);
                animTexRot.ResynchronizeValue(thisTime, CalcolateTexRot(thisTime));*/
                BlockAll(thisTime);
            }
        }

        public void BlockAll(long thisTime)
        {
            ChangeAnimator(
                0,
                new SteadyAnimator(
                    CalculateX(thisTime, true),
                    thisTime)
                    ); 
            ChangeAnimator(
                 1,
                 new SteadyAnimator(
                     CalculateY(thisTime, true),
                     thisTime)
                     ); 
            ChangeAnimator(
                 2,
                 new SteadyAnimator(
                     CalcolateTexRot(thisTime),
                     thisTime)
                     );
        }

        public void BindTextureRotationToMovement(bool val)
        {
            bindTexRotToMovement = val;
            animTexRot = new ParabolicToLinearAnimator(animTexRot.CalculateValue(animX.GetStartingTime()), animX.GetStartingTime(), -0.02, 0.09, 1.08);
        }
        #endregion

        #region Metodi pubblici per la restituzione delle animazioni
        public S_Animator GetAnimation(byte whichOne)
        {   
            if (whichOne == 0) return animX;
            if (whichOne == 1) return animY;
            if (whichOne == 2) return animTexRot;
            else return null;
        }
        #endregion

    }
    #endregion
    #endregion
}
