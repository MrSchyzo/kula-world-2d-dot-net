namespace ResourcesBasics
{
    #region ResourceItem: Classe basilare per creare file "logici"
    /// <summary>
    /// Classe principale per incorporare una risorsa: è possibile caricarne e scaricarne il contenuto.
    /// </summary>
    public abstract class ResourceItem
    {
        /// <summary>
        /// Carica il contenuto indicato dalla stringa. Se fosse già stata caricata un'altra risorsa, quest'ultima viene scaricata.
        /// </summary>
        /// <param name="Path">Stringa che indica il percorso in cui ottenere la risorsa.</param>
        /// <returns>true se e solo se il caricamento va a buon fine.</returns>
        public abstract bool Load(string Path);

        /// <summary>
        /// Scarica il contenuto attualmente caricato. Se non c'è contenuto caricato, questo metodo non fa nulla.
        /// </summary>
        public abstract void Unload();
    }
    #endregion
}
