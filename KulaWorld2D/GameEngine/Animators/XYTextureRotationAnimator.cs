using GameUtils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngine.Animators
{
    /// <summary>
    /// Questa classe è utile ad animare il movimento della palla e la sua rotazione
    /// </summary>
    public class XYTextureRotationAnimator
    {
        private Animator animX;
        private Animator animY;
        private Animator animTexRot;
        private double perspectiveRot;
        private bool bindTexRotToMovement;

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
        public XYTextureRotationAnimator(float startingX, float startingY, float startingTextureRotation, float perspectiveRotation, long timeStamp)
        {
            bindTexRotToMovement = true;
            animX = new SteadyAnimator(startingX, timeStamp);
            animY = new SteadyAnimator(startingY, timeStamp);
            animTexRot = new SteadyAnimator(startingTextureRotation, timeStamp);
            perspectiveRot = perspectiveRotation;
        }
        public bool ChangeAnimator(byte valueToAnimate, Animator animation)
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
                return startTexRot + ((CalculateX(thisTime, false) - startX)/(2.0*Math.PI*Constants.BlockWidth/4.0)) * 360.0;
            else
                return animTexRot.CalculateValue(thisTime);
        }
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
            if (animX is ParabolicToLinearAnimator)
                animTexRot = new ParabolicToLinearAnimator(animTexRot.CalculateValue(animX.GetStartingTime()), animX.GetStartingTime(), -0.02, 0.09, 1.08);
        }
        public Animator GetAnimation(byte whichOne)
        {   
            if (whichOne == 0) return animX;
            if (whichOne == 1) return animY;
            if (whichOne == 2) return animTexRot;
            else return null;
        }
    }
}
