using System;

namespace GameUtils
{
    #region IntSlider: Classe che rappresenta uno slider che resta tra un massimo ed un minimo. Si può incrementare e decrementare di una quantità fissa.
    #endregion

    #region DoubleSlider: Classe che rappresenta uno slider che resta tra un massimo ed un minimo. Si può incrementare e decrementare di una quantità fissa.
    /// <summary>
    /// Classe che rappresenta uno slider che resta tra un massimo ed un minimo. Si può incrementare e decrementare di una quantità fissa.
    /// La quantità di aumento e diminuzione è definita al momento della costruzione dell'oggetto.
    /// Al momento della costruzione, il valore dello slider è a metà tra il massimo ed il minimo specificato.
    /// </summary>
    public class DoubleSlider
    {
        private double val;
        private double max;
        private double min;
        private double delta;
        private int steps;

        private void ConstructorsMutualPart(double m, double M)
        {
            min = Math.Min(m, M);
            max = Math.Max(m, M);
            val = (max + min) / (2.0);
        }

        /// <summary>
        /// Genera un oggetto con valori numerici regolabili da un minimo e un massimo.
        /// </summary>
        /// <param name="m">Minimo o massimo dello slider</param>
        /// <param name="M">Minimo o massimo dello slider</param>
        public DoubleSlider(double m, double M)
        {
            ConstructorsMutualPart(m, M);
            delta = (max - min) / 10.0;
            this.steps = 10;
        }

        /// <summary>
        /// Genera un oggetto con valori numerici regolabili da un minimo e un massimo.
        /// </summary>
        /// <param name="m">Minimo o massimo dello slider</param>
        /// <param name="M">Minimo o massimo dello slider</param>
        /// <param name="steps">Numero di gradazioni dello slider</param>
        public DoubleSlider(double m, double M, int steps)
        {
            ConstructorsMutualPart(m, M);
            delta = (max - min) / ((double)steps);
            this.steps = steps;
        }

        /// <summary>
        /// Restituisce il valore dello slider.
        /// </summary>
        public double Value
        {
            get { return val; }
        }

        /// <summary>
        /// Aumenta il valore dello slider di uno step.
        /// </summary>
        public void IncreaseValue()
        {
            if ((val += delta) > max)
                val = max;
        }

        /// <summary>
        /// Decrementa il valore dello slider di uno step.
        /// </summary>
        public void DecreaseValue()
        {
            if ((val -= delta) < min)
                val = min;
        }

        /// <summary>
        /// Restituisce una deep copy dello slider corrente. (Stato del valore escluso)
        /// </summary>
        public DoubleSlider Clone()
        {
            return new DoubleSlider(min, max, steps);
        }
    }
    #endregion
}
