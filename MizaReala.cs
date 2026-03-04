using System;
using System.Windows.Forms;

namespace BlackjackPOO
{
    public class MizaReala : ReguliMiza
    {
        public override void ProceseazaMiza(Jucator jucator)
        {
            // 🔧 VALIDARE
            if (jucator.MizaCurenta <= 0)
                throw new InvalidOperationException("Miza trebuie să fie > 0!");

            if (!AreBalantaSuficienta(jucator, jucator.MizaCurenta))
                throw new InvalidOperationException($"Fonduri insuficiente! Balanta: {jucator.BalantaRON:F2} RON.");

            // 🔧 SCADE MIZA DIN BALANTĂ
            jucator.BalantaRON -= jucator.MizaCurenta;
            System.Diagnostics.Debug.WriteLine($"MizaReala: Scăzut {jucator.MizaCurenta}, rămas {jucator.BalantaRON}");
        }

        public override void AplicaCastig(Jucator jucator, decimal multiplicator)
        {
            // 🔧 RETURNEAZĂ MIZA + CÂȘTIGUL (2x la câștig, 1x la egal)
            decimal sumaCastig = jucator.MizaCurenta * multiplicator;
            jucator.BalantaRON += sumaCastig;
            System.Diagnostics.Debug.WriteLine($"MizaReala: Adăugat {sumaCastig}, total {jucator.BalantaRON}");
        }

        public override decimal GetBalanta(Jucator jucator)
        {
            return jucator.BalantaRON;
        }

        public override bool AreBalantaSuficienta(Jucator jucator, decimal suma)
        {
            if (jucator.BalantaRON >= suma)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void SeteazaBalanta(Jucator jucator, decimal valoare)
        {
            jucator.BalantaRON = valoare;
        }
    }
}