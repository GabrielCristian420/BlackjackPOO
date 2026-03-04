using System;
using System.Windows.Forms;

namespace BlackjackPOO
{
    public class MizaFictiva : ReguliMiza
    {
        public override void ProceseazaMiza(Jucator jucator)
        {
            jucator.MizaCurenta = 0;
        }

        public override void AplicaCastig(Jucator jucator, decimal multiplicator)
        {
            // 🔥 ACTUALIZEAZĂ STATISTICILE INDIVIDUALE
            if (multiplicator == 2m) jucator.Win++;
            else if (multiplicator == 1m) jucator.Draw++;
            else jucator.Lose++;

            
        }

        public override decimal GetBalanta(Jucator jucator)
        {
            return 0; 
        }

        public override bool AreBalantaSuficienta(Jucator jucator, decimal suma)
        {
            return true; 
        }
        public override void SeteazaBalanta(Jucator jucator, decimal valoare)
        {
          
        }
    }
}