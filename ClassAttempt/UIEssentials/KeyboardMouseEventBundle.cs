using System.Collections.Generic;
using System.Linq;

namespace UIEssentials
{
    #region KeyboardMouseEventArgs: Wrapper che ingloba MouseEventArgs o KeyEventArgs
    #endregion

    #region KeyboardMouseEventBundle: una coda FIFO di eventi che possono essere relativi al mouse oppure alla tastiera.
    /// <summary>
    /// Questa classe è una coda FIFO di eventi che possono essere relativi al mouse oppure alla tastiera.
    /// Si può aggiungere eventi in coda e estrarli dalla testa.
    /// </summary>
    public class KeyboardMouseEventBundle
    {
        private List<KeyboardMouseEventArgs> evtList;

        /// <summary>
        /// Inizializza una nuova coda vuota di eventi.
        /// </summary>
        public KeyboardMouseEventBundle()
        {
            this.evtList = new List<KeyboardMouseEventArgs>();
        }

        /// <summary>
        /// Aggiunge un evento in fondo alla coda.
        /// </summary>
        /// <param name="e">Evento da inserire.</param>
        public void addEvent(KeyboardMouseEventArgs e)
        {
            if ((e != null) && (!e.isNullKeyEvent() || !e.isNullMouseEvent()))
                evtList.Add(e);
        }

        /// <summary>
        /// Estrae l'evento in testa alla coda. Può restituire pure null.
        /// </summary>
        /// <returns>Un KeyboardEventArgs, se la coda non è vuota, null altrimenti.</returns>
        public KeyboardMouseEventArgs extractEvent()
        {
            if (evtList.Count != 0)
            {
                KeyboardMouseEventArgs output = evtList.ElementAt<KeyboardMouseEventArgs>(0);
                evtList.RemoveAt(0);
                return output;
            }
            else
                return null;
        }
    }
    #endregion

}
