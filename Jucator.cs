using System;
using System.Collections.Generic;

namespace BlackjackPOO
{
    public class Jucator
    {
        private List<Carte> carti = new List<Carte>();

        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
        public string Nume { get; set; }
        public decimal BalantaRON { get; set; } = 1000;
        public decimal MizaCurenta { get; set; }
        public int Win { get; set; } = 0;
        public int Lose { get; set; } = 0;
        public int Draw { get; set; } = 0;

        public int NumarCarti
        {
            get { return carti.Count; }
        }
        public void ResetStats()
        {
            Win = 0;
            Lose = 0;
            Draw = 0;
            MizaCurenta = 0;
            GolesteMana();
        }
        public void AdaugaCarte(Carte carte)
        {
            if (carte != null) carti.Add(carte);
        }

        public void GolesteMana()
        {
            carti.Clear();
        }

        public Carte GetCarte(int index)
        {
            if (index >= 0 && index < carti.Count) return carti[index];
            return null;
        }

        public int CalculeazaScor()
        {
            int scor = 0, numarAsi = 0;
            for (int i = 0; i < carti.Count; i++)
            {
                scor += carti[i].Punctaj;
                if (carti[i].Valoare == "A") numarAsi++;
            }
            while (scor > 21 && numarAsi > 0)
            {
                scor -= 10; numarAsi--;
            }
            return scor;
        }

        public bool EsteBlackjack()
        {
            return NumarCarti == 2 && CalculeazaScor() == 21;
        }
        public bool EsteBust()
        {
            return CalculeazaScor() > 21;
        }

        // Reconstruiește mâna pe baza datelor primite de la server
        public void IncarcaDinRetea(string dateCarti)
        {
            System.Diagnostics.Debug.WriteLine($"IncarcaDinRetea: '{dateCarti}'");

            GolesteMana();
            if (!string.IsNullOrEmpty(dateCarti))
            {
                string[] cartiArray = dateCarti.Split(',');
                for (int i = 0; i < cartiArray.Length; i++)
                {
                    if (!string.IsNullOrEmpty(cartiArray[i]))
                    {
                        try
                        {
                            AdaugaCarte(new Carte(cartiArray[i]));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Eroare carte: '{cartiArray[i]}' - {ex.Message}");
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"IncarcaDinRetea rezultat: {NumarCarti} carti");
        }

        // Transformă cărțile în text pentru a fi trimise prin rețea : "10H,KD,2C"
        public string PentruRetea()
        {
            List<string> cartiStr = new List<string>();
            for (int i = 0; i < carti.Count; i++)
            {
                cartiStr.Add(carti[i].PentruRetea());
            }
            return string.Join(",", cartiStr);
        }

        public string GetStatsWLD()
        {
            return $"W-L-D: {Win}-{Lose}-{Draw}";
        }
    }
}