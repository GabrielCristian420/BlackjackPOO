namespace BlackjackPOO
{
    public abstract class ReguliMiza
    {
        public abstract void ProceseazaMiza(Jucator jucator);
        public abstract void AplicaCastig(Jucator jucator, decimal multiplicator);
        public abstract decimal GetBalanta(Jucator jucator);
        public abstract bool AreBalantaSuficienta(Jucator jucator, decimal suma);
        public abstract void SeteazaBalanta(Jucator jucator, decimal valoare);
    }
}