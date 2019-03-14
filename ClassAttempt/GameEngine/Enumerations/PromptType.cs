namespace GameEngine.Enumerations
{
    public enum PromptType
    {
        Retry, //Premi qualsiasi tasto per riprovare (vinto/perso in prova, perso in partita normale)
        GoNext, //Premi qualsiasi tasto per continuare (vinto/perso in bonus, vinto in partita normale)
        End //Non ci sono più livelli da fare o game over
    }
}

