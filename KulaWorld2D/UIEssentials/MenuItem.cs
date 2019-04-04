using GameUtils;
using System.Collections.Generic;

namespace UIEssentials
{

    
    /// <summary>
    /// MenuItem rappresenta una classe basilare per costruire Menù a gerarchia: MenuItem è una singola voce del Menù e
    /// può contenere riferimenti a eventuali elementi figli. Con tale MenuItem è possibile consultare i riferimenti ai
    /// figli, il riferimento all'eventuale padre, modificare valori associati alla singola voce e aggiungere altri figli.
    /// </summary>
    public class MenuItem
    {
        /*Variabili di istanza:
         children = Lista ai figli
         father = riferimento possibilmente nullo all'item padre
         descr = descrizione testuale associata all'item
         boolVal = valore booleano associato all'item
         intVal = valore intero associato all'item
         doubleVal = valore a doppia precisione associato all'item
         */
        private List<MenuItem> children;
        private MenuItem father;
        private string descr;
        private bool boolVal;
        private IntSlider intVal;
        private DoubleSlider doubleVal;
        private bool editable;
        private int typePtr = 2;

        /// <summary>
        /// Crea un MenuItem senza specificare nessun valore associato, i quali saranno impostati a valori di default, cioè:
        /// (description = ""; booleanValue = false; int32Value = 0; doubleValue = 0.0)
        /// </summary>
        /// <param name="f">Riferimento al MenuItem padre (può essere pure null)</param>
        public MenuItem(MenuItem f)
        {
            this.children = new List<MenuItem>();
            this.father = f;
            this.descr = "";
            this.boolVal = false;
            this.editable = false;
            this.intVal = new IntSlider(-1, 1, 2);
            this.doubleVal = new DoubleSlider(-1, 1, 2);
        }
        /// <summary>
        /// Crea un MenuItem senza specificare il riferimento al padre, che sarà null.
        /// </summary>
        /// <param name="s">Stringa associata al MenuItem</param>
        /// <param name="b">Booleano associato al MenuItem</param>
        /// <param name="i">Intero 32b associato al MenuItem</param>
        /// <param name="d">Numero a doppia precisione associato al MenuItem</param>
        public MenuItem(string s, bool b, IntSlider i, DoubleSlider d)
        {
            this.children = new List<MenuItem>();
            this.father = null;
            this.descr = s;
            this.boolVal = b;
            this.intVal = i;
            this.doubleVal = d;
            editable = (i != null) || (d != null);
        }
        /// <summary>
        /// Crea un MenuItem specificando tutto, valori e riferimento al padre.
        /// </summary>
        /// <param name="f">Riferimento al MenuItem padre (può essere pure null)</param>
        /// <param name="s">Stringa associata al MenuItem</param>
        /// <param name="b">Booleano associato al MenuItem</param>
        /// <param name="i">Intero 32b associato al MenuItem</param>
        /// <param name="d">Numero a doppia precisione associato al MenuItem</param>
        public MenuItem(MenuItem f, string s, bool b, IntSlider i, DoubleSlider d)
        {
            this.children = new List<MenuItem>();
            this.father = f;
            this.descr = s;
            this.boolVal = b;
            this.intVal = i;
            this.doubleVal = d;
            editable = (i != null) || (d != null);
        }

        /// <summary>
        /// Imposta o restituisce la descrizione testuale dell'elemento del menù.
        /// </summary>
        public string description
        {
            get { return descr; }
            set { descr = value; }
        }
        /// <summary>
        /// Imposta o restituisce lo slider del valore intero associato all'elemento del menù.
        /// </summary>
        public IntSlider int32Value
        {
            get { return intVal; }
        }
        /// <summary>
        /// Imposta o restituisce lo slider del valore a doppia precisione associato all'elemento del menù.
        /// </summary>
        public DoubleSlider doublePValue
        {
            get { return doubleVal; }
        }
        /// <summary>
        /// Imposta o restituisce il valore booleano associato all'elemento del menù.
        /// </summary>
        public bool booleanValue
        {
            get { return boolVal; }
            set { boolVal = value; }
        }
        /// <summary>
        /// Restituisce il riferimento all'elemento padre. Se tale riferimento non esiste, viene restituito null
        /// </summary>
        public MenuItem fatherItem
        {
            get { return father; }
        }
        /// <summary>
        /// Restituisce una lista di riferimenti ai figli del MenuItem.
        /// </summary>
        /// <returns>Lista di riferimenti ai figli del MenuItem (lista di MenuItem)</returns>
        public List<MenuItem> getChildren()
        {
            List<MenuItem> returnedCopy = new List<MenuItem>();
            foreach (MenuItem child in children)
                returnedCopy.Add(child);
            return returnedCopy;
        }
        /// <summary>
        /// Inserisce, come figlio, una DEEP copy del MenuItem specificato.
        /// </summary>
        /// <param name="child">MenuItem da inserire nella gerarchia come figlio.</param>
        public void appendChild(MenuItem child)
        {
            if (child != null)
            {
                IntSlider iS;
                DoubleSlider dS;
                if ((iS = child.int32Value) != null)
                    iS = iS.Clone();
                if ((dS = child.doublePValue) != null)
                    dS = dS.Clone();
                children.Add(new MenuItem(this, child.description, child.booleanValue, iS, dS));
            }
        }
        /// <summary>
        /// Aumenta di uno step il valore intero associato.
        /// </summary>
        /// <returns>Restituisce il valore intero correntemente associato all'item.</returns>
        public int? increaseIntValue()
        {
            if (intVal != null)
            {
                intVal.IncreaseValue();
                return intVal.Value;
            }
            else
                return null;
        }
        /// <summary>
        /// Decrementa di uno step il valore intero associato.
        /// </summary>
        /// <returns>Restituisce il valore intero correntemente associato all'item.</returns>
        public int? decreaseIntValue()
        {
            if (intVal != null)
            {
                intVal.DecreaseValue();
                return intVal.Value;
            }
            else return null;
        }
        /// <summary>
        /// Inverte il valore booleano associato.
        /// </summary>
        /// <returns>Restituisce il valore booleano correntemente associato all'item.</returns>
        public bool switchBoolValue()
        {
            boolVal = !boolVal;
            return boolVal;
        }
        /// <summary>
        /// Aumenta di uno step il valore a doppia precisione associato.
        /// </summary>
        /// <returns>Restituisce il valore a doppia precisione correntemente associato all'item.</returns>
        public double? increaseDoubleValue()
        {
            if (doubleVal != null)
            {
                doubleVal.IncreaseValue();
                return doubleVal.Value;
            }
            else return null;
        }
        /// <summary>
        /// Decrementa di uno step il valore a doppia precisione associato.
        /// </summary>
        /// <returns>Restituisce il valore a doppia precisione correntemente associato all'item.</returns>
        public double? decreaseDoubleValue()
        {
            if (doubleVal != null)
            {
                doubleVal.DecreaseValue();
                return doubleVal.Value;
            }
            else return null;
        }
        /// <summary>
        /// Imposta o restituisce se gli slider sono significativi o meno
        /// </summary>
        public bool isEditable
        {
            set { editable = value; }
            get { return editable; }
        }
        /// <summary>
        /// Imposta il tipo di valore significativo (scelto tra bool, double, integer)
        /// </summary>
        /// <param name="type">intero che indica: 0 per boolean, 1 per double, 2 per integer</param>
        public void setValueType(int type)
        {
            if (type >= 0 && type <= 2)
                this.typePtr = type;
        }

        /// <summary>
        /// Converte il menuitem in stringa.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            MenuItem i = this;
            string testo = "";
            if (i.isEditable)
            {
                if (this.typePtr == 0)
                    testo = i.description + ": " + i.booleanValue.ToString();
                else if (this.typePtr == 1)
                    testo = i.description + ": " + i.doublePValue.Value.ToString();
                else if (this.typePtr == 2)
                    testo = i.description + ": " + i.int32Value.Value.ToString();
            }
            else
                testo = i.description;
            return testo;
        }
    }
    

}
