using System.Windows.Forms;

namespace UIEssentials
{
    
    

    
    /// <summary>
    /// Questa classe è un wrapper che ingloba MouseEventArgs o KeyEventArgs
    /// </summary>
    public class KeyboardMouseEventArgs
    {
        /*Variabili di istanza:
         * kEvt = KeyEventArgs contenuto nel wrapper.
         * mEvt = MouseEventArgs contenuto nel wrapper.
         * isKeyEvt = booleano per decidere se il wrapper è stato inizializzato con un KeyEventArgs.
         */
        private KeyEventArgs kEvt;
        private MouseEventArgs mEvt;
        private KeyboardMouseEventID type;
        private bool isKeyEvt;

        /// <summary>
        /// Inizializza il wrapper con un KeyEventArgs e un ID che ne identifica il tipo.
        /// </summary>
        /// <param name="k">KeyEventArgs da inserire nel wrapper.</param>
        /// <param name="ID">Identificatore del tipo dell'evento inserito.</param>
        public KeyboardMouseEventArgs(KeyEventArgs k, KeyboardMouseEventID ID)
        {
            this.kEvt = k;
            this.type = ID;
            this.mEvt = null;
            this.isKeyEvt = true;
        }

        /// <summary>
        /// Inizializza il wrapper con un MouseEventArgs e un ID che ne identifica il tipo.
        /// </summary>
        /// <param name="m">MouseEventArgs da inserire nel wrapper.</param>
        /// <param name="ID">Identificatore del tipo dell'evento inserito.</param>
        public KeyboardMouseEventArgs(MouseEventArgs m, KeyboardMouseEventID ID)
        {
            this.mEvt = m;
            this.type = ID;
            this.kEvt = null;
            this.isKeyEvt = false;
        }

        /// <summary>
        /// Restituisce il tipo di evento contenuto.
        /// </summary>
        public KeyboardMouseEventID getEventType
        {
            get { return type; }
        }

        /// <summary>
        /// Identifica il tipo dell'evento contenuto. Restituisce true se l'evento contenuto è un keyevent.
        /// </summary>
        public bool isKeyEvent
        {
            get { return isKeyEvt; }
        }

        /// <summary>
        /// Metodo che indica se il KeyEventArgs contenuto è null o meno.
        /// </summary>
        /// <returns>true se il KeyEventArgs è null, false altrimenti.</returns>
        public bool isNullKeyEvent()
        {
            return (kEvt == null);
        }

        /// <summary>
        /// Metodo che indica se il MouseEventArgs contenuto è null o meno.
        /// </summary>
        /// <returns>true se il MouseEventArgs è null, false altrimenti.</returns>
        public bool isNullMouseEvent()
        {
            return (mEvt == null);
        }

        /// <summary>
        /// Restituisce il KeyEventArgs contenuto (può essere null).
        /// </summary>
        public KeyEventArgs keyEvent
        {
            get { return kEvt; }
        }

        /// <summary>
        /// Restituisce il MouseEventArgs contenuto (può essere null).
        /// </summary>
        public MouseEventArgs mouseEvent
        {
            get { return mEvt; }
        }
    }
    

}
