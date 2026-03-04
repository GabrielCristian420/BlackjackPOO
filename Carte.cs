using System;


namespace BlackjackPOO
{
    public class Carte
    {
        public string Valoare { get; private set; }
        public string Culoare { get; private set; }
        public int Punctaj { get; private set; }
        public char Simbol { get; private set; }

        public Carte(string valoare, string culoare)
        {
            Valoare = valoare;
            Culoare = culoare;
            Punctaj = CalculeazaPunctaj(valoare);
            Simbol = GetSimbol(culoare);
        }

        public Carte(string date)
        {
            // Validare: nu acceptăm texte goale sau prea scurte
            if (string.IsNullOrWhiteSpace(date) || date.Length < 2)
                throw new ArgumentException($"Carte invalidă: '{date}'");

            Valoare = date.Substring(0, date.Length - 1);
            char simbol = date[date.Length - 1];

            if (string.IsNullOrWhiteSpace(Valoare))
                throw new ArgumentException($"Valoare goală în '{date}'");

            Culoare = GetCuloareDinSimbol(simbol);
            Punctaj = CalculeazaPunctaj(Valoare);
            Simbol = simbol;
        }

        public string NumeFisier
        {
            get { return $"{Valoare}{GetSimbolASCII()}.png"; }
        }

        private char GetSimbolASCII()
        {
            switch (Culoare)
            {
                case "Inima": return 'H';
                case "Romb": return 'D';
                case "Trefla": return 'C';
                case "Pica": return 'S';
                default: return '?';
            }
        }

        private int CalculeazaPunctaj(string valoare)
        {
            if (valoare == "A") return 11;
            if (valoare == "J" || valoare == "Q" || valoare == "K") return 10;

            // Curățăm textul ca să rămână doar cifre
            string curat = "";
            for (int i = 0; i < valoare.Length; i++)
            {
                if (char.IsDigit(valoare[i]))
                    curat += valoare[i];
            }
            // Încercăm să transformăm textul "2" în numărul 2. Dacă nu merge, dăm 0.
            if (int.TryParse(curat, out int rezultat))
                return rezultat;

            return 0;
        }

        private char GetSimbol(string culoare)
        {
            if (culoare == "Inima") return '♥';
            if (culoare == "Romb") return '♦';
            if (culoare == "Trefla") return '♣';
            if (culoare == "Pica") return '♠';
            return '?';
        }

        private string GetCuloareDinSimbol(char simbol)
        {
            if (simbol == '♥') return "Inima";
            if (simbol == '♦') return "Romb";
            if (simbol == '♣') return "Trefla";
            if (simbol == '♠') return "Pica";
            return "Necunoscut";
        }

        public override string ToString()
        {
            return $"{Valoare}{Simbol}";
        }

        public string PentruRetea()
        {
            return $"{Valoare}{Simbol}";
        }
    }
}