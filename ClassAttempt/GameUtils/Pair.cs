using System;

namespace GameUtils
{
    #region Pair<T>: Coppia immutabile di valori generici (è pure comparabile)
    /// <summary>
    /// Questa classe rappresenta una coppia immutabile di valori generici.
    /// </summary>
    /// <typeparam name="T">Tipo istanziato dei due elementi della coppia</typeparam>
    [Serializable()]
    public class Pair<T> : IComparable where T : IComparable
    {
        private T first;
        private T second;

        /// <summary>
        /// Inizializza una coppia immutabile di valori confrontabili.
        /// </summary>
        /// <param name="firstVal">Primo valore della coppia</param>
        /// <param name="secondVal">Secondo valore della coppia</param>
        public Pair(T firstVal, T secondVal)
        {
            first = firstVal;
            second = secondVal;
        }

        /// <summary>
        /// Restituisce il primo valore della coppia
        /// </summary>
        public T FirstValue
        {
            get { return first; }
        }

        /// <summary>
        /// Restituisce il secondo valore della coppia
        /// </summary>
        public T SecondValue
        {
            get { return second; }
        }

        /// <summary>
        /// Restituisce un intero > 0 se l'oggetto in input è più piccolo, un intero minore 0 se l'oggetto è più grande e un intero = o se l'oggetto è uguale.
        /// </summary>
        /// <param name="obj">Oggetto comparabile in input</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Pair<T> other = (Pair<T>)obj;
            int compare;
            if ((compare = first.CompareTo(other.FirstValue)) == 0)
                return second.CompareTo(other.SecondValue);
            else
                return compare;
        }
    }
    #endregion
}
