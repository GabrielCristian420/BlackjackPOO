using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackPOO
{
    public class Pachet
    {
        private List<Carte> carti;
        private Random random; // Generator de numere aleatoare

        public int CartiRamase
        {
            get { return carti.Count; }
        }

        public Pachet()
        {
            carti = new List<Carte>();
            random = new Random();
            InitializeazaPachet();
        }

        // Creează cele 52 de cărți
        private void InitializeazaPachet()
        {
            string[] culori = { "Inima", "Romb", "Trefla", "Pica" };
            string[] valori = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            for (int i = 0; i < culori.Length; i++)
            {
                for (int j = 0; j < valori.Length; j++)
                {
                    carti.Add(new Carte(valori[j], culori[i]));
                }
            }
        }

        public void Amesteca()
        {
            // Algoritmul Fisher-Yates
            for (int i = carti.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                Carte temp = carti[i];
                carti[i] = carti[j];
                carti[j] = temp;
            }
        }

        public Carte ExtrageCarte()
        {
            if (carti.Count == 0)
            {
                InitializeazaPachet();
                Amesteca();
            }

            Carte carte = carti[0];
            carti.RemoveAt(0);
            return carte;
        }
    }
}
