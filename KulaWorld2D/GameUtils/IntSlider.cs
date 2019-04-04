using System;

namespace GameUtils
{
    /// <summary>
    /// Classe che rappresenta uno slider che resta tra un massimo ed un minimo. Si può incrementare e decrementare di una quantità fissa.
    /// La quantità di aumento e diminuzione è definita al momento della costruzione dell'oggetto.
    /// Al momento della costruzione, il valore dello slider è a metà tra il massimo ed il minimo specificato.
    /// </summary>
    public class IntSlider
    {
        private double val;
        private int max;
        private int min;
        private double delta;
        private int steps;

        private void ConstructorsMutualPart(int m, int M)
        {
            min = Math.Min(m, M);
            max = Math.Max(m, M);
            val = ((double)(max + min)) / (2.0);
        }

        /// <summary>
        /// Inizializza uno slider che ha i valori compresi tra m ed M, oppure tra M ed m. Il numero di gradazioni è 10.
        /// </summary>
        /// <param name="m">Massimo o minimo del valore dello slider</param>
        /// <param name="M">Massimo o minimo del valore dello slider</param>
        public IntSlider(int m, int M)
        {
            ConstructorsMutualPart(m, M);
            delta = ((double)(max - min)) / 10.0;
            steps = 10;
        }

        /// <summary>
        /// Inizializza uno slider che ha i valori compresi tra m ed M, oppure tra M ed m. Il numero di gradazioni è 10.
        /// </summary>
        /// <param name="m">Massimo o minimo del valore dello slider</param>
        /// <param name="M">Massimo o minimo del valore dello slider</param>
        /// <param name="steps">Numero di gradazioni dello slider</param>
        public IntSlider(int m, int M, int steps)
        {
            ConstructorsMutualPart(m, M);
            delta = (((double)max) - ((double)min)) / ((double)steps);
            this.steps = steps;
        }

        /// <summary>
        /// Restituisce il valore dello slider.
        /// </summary>
        public int Value
        {
            get { return (int)val; }
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
        public IntSlider Clone()
        {
            return new IntSlider(min, max, steps);
        }
    }
    
}
