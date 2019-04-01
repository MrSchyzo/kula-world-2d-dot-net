using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameUtils
{
    #region ExtensionMethods: classe statica per avere metodi di deep cloning tra oggetti serializzabili.
    /// <summary>
    /// Classe statica che contiene un metodo per fare la deep copy di oggetti serializzabili.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Fa una deepcopy dell'oggetto serializzabile.
        /// </summary>
        /// <typeparam name="T">Tipo di oggetto da serializzare</typeparam>
        /// <param name="a">Oggetto da serializzare</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
    #endregion
}
