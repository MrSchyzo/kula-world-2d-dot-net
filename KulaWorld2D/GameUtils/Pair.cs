using System;

namespace GameUtils
{
    /// <summary>
    /// Questa classe rappresenta una coppia immutabile di valori generici.
    /// </summary>
    /// <typeparam name="T">Tipo istanziato dei due elementi della coppia</typeparam>
    [Serializable()]
    public struct Pair<T> : IComparable<Pair<T>> where T : IComparable
    {
        public T first, second;

        public T First { get => first; }
        public T Second { get => second; }

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
        /// Restituisce un intero > 0 se l'oggetto in input è più piccolo, un intero minore 0 se l'oggetto è più grande e un intero = o se l'oggetto è uguale.
        /// </summary>
        /// <param name="obj">Oggetto comparabile in input</param>
        /// <returns></returns>
        public int CompareTo(Pair<T> other)
        {
            int compare = First.CompareTo(other.First);
            if (compare == 0)
                return Second.CompareTo(other.Second);
            else
                return compare;
        }
    }
    
}
