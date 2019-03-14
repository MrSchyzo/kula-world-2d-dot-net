using System;
using System.Collections.Generic;

namespace GameEngine.Animators
{
    public class MultiAnimator
    {
        protected SortedDictionary<string, Animator> animators;

        public MultiAnimator()
        {
            animators = new SortedDictionary<string, Animator>();
        }
        public double CalculateSpecificValue(string symbol, long thisTime)
        {
            Animator an;
            if (animators.TryGetValue(symbol, out an))
                return an.CalculateValue(thisTime);
            throw new KeyNotFoundException("In MultiAnimator.CalculateSpecificValue(...) the animator was not found.");
        }

        public SortedDictionary<string, double> CalculateAllValues(long thisTime)
        {
            SortedDictionary<string, double> ret = new SortedDictionary<string, double>();
            foreach (string s in animators.Keys)
            {
                Animator an;
                if (animators.TryGetValue(s, out an))
                    ret.Add(s, an.CalculateValue(thisTime));
            }
            return ret;
        }
        public bool AddAnimation(string s, Animator an)
        {
            if (an == null)
                throw new ArgumentNullException("In MultiAnimator.AddAnimation(...) the animator was NULL.");
            if (animators.ContainsKey(s))
                return false;
            else
                animators.Add(s, an);
            return true;
        }

        public bool ReplaceAnimation(string s, Animator an)
        {
            if (an == null)
                throw new ArgumentNullException("In MultiAnimator.ReplaceAnimation(...) the animator was NULL.");
            if (!animators.ContainsKey(s))
                return false;
            animators.Remove(s);
            animators.Add(s, an);
            return true;
        }

        public Animator GetAnimation(string s)
        {
            Animator an = null;
            if (animators.TryGetValue(s, out an))
                return an;
            else
                throw new KeyNotFoundException("In MultiAnimator.GetAnimation(...) the animator was not found.");
        }

        public void RemoveAllAnimations()
        {
            animators.Clear();
        }
    }
}
