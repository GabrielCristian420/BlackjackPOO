using System;
using System.Collections.Generic;

namespace BlackjackPOO
{
    public class Joc
    {
        protected Jucator jucatorPrincipal;
        protected Jucator dealer;
        protected Pachet pachet;
        protected bool jocTerminat;
        protected string rezultat;
        protected ReguliMiza _reguliMiza;

        public virtual Jucator ManaJucator { get { return jucatorPrincipal; } }
        public virtual Jucator ManaDealer { get { return dealer; } }
        public virtual bool JocTerminat { get { return jocTerminat; } }
        public virtual string Rezultat { get { return rezultat; } }

        public Joc(bool cuMizaReala)
        {
            jucatorPrincipal = new Jucator { Nume = "Player1" };
            dealer = new Jucator { Nume = "Dealer" };
            pachet = new Pachet();
            jocTerminat = true;
            rezultat = "";
            _reguliMiza = cuMizaReala ? (ReguliMiza)new MizaReala() : new MizaFictiva();
        }

        public virtual void ÎncepeJoc()
        {
            // FIX: Validam miza si dam drumul la eroare daca nu ai bani (FARA MessageBox.Show pe server)
            _reguliMiza.ProceseazaMiza(jucatorPrincipal);

            pachet = new Pachet();
            pachet.Amesteca();
            jucatorPrincipal.GolesteMana();
            dealer.GolesteMana();
            jocTerminat = false;
            rezultat = "";

            jucatorPrincipal.AdaugaCarte(pachet.ExtrageCarte());
            dealer.AdaugaCarte(pachet.ExtrageCarte());
            jucatorPrincipal.AdaugaCarte(pachet.ExtrageCarte());
            dealer.AdaugaCarte(pachet.ExtrageCarte());

            if (jucatorPrincipal.EsteBlackjack() || dealer.EsteBlackjack())
            {
                jocTerminat = true;
                DeterminaCastigator();
            }
        }

        public void SetMiza(decimal miza)
        {
            jucatorPrincipal.MizaCurenta = miza;
        }

        public void SetBalanta(decimal balanta)
        {
            _reguliMiza.SeteazaBalanta(jucatorPrincipal, balanta);
        }

        public virtual void JucatorHit()
        {
            if (!jocTerminat)
            {
                jucatorPrincipal.AdaugaCarte(pachet.ExtrageCarte());
                if (jucatorPrincipal.EsteBust())
                {
                    jocTerminat = true;
                    DeterminaCastigator();
                }
            }
        }

        public virtual void JucatorStand()
        {
            if (!jocTerminat)
            {
                JoacaDealer();
                DeterminaCastigator();
            }
        }

        protected virtual void JoacaDealer()
        {
            while (dealer.CalculeazaScor() < 17)
            {
                dealer.AdaugaCarte(pachet.ExtrageCarte());
            }
            jocTerminat = true;
        }

        protected virtual void DeterminaCastigator()
        {
            int scorJucator = jucatorPrincipal.CalculeazaScor();
            int scorDealer = dealer.CalculeazaScor();

            decimal multiplicator = 0;
            if (jucatorPrincipal.EsteBust())
            {
                rezultat = "AI DEPĂȘIT 21! DEALER-UL CÂȘTIGĂ!";
                multiplicator = 0;
            }
            else if (dealer.EsteBust())
            {
                rezultat = "DEALER-UL A DEPĂȘIT 21! AI CÂȘTIGAT!";
                multiplicator = 2;
            }
            else if (scorJucator > scorDealer)
            {
                rezultat = "AI CÂȘTIGAT!";
                multiplicator = 2;
            }
            else if (scorJucator < scorDealer)
            {
                rezultat = "DEALER-UL CÂȘTIGĂ!";
                multiplicator = 0;
            }
            else
            {
                rezultat = "EGALITATE!";
                multiplicator = 1;
            }

            _reguliMiza.AplicaCastig(jucatorPrincipal, multiplicator);
        }

        public virtual int CalculeazaScor(Jucator jucator)
        {
            return jucator?.CalculeazaScor() ?? 0;
        }

        public virtual string GetStatus()
        {
            if (jocTerminat)
            {
                return $"Joc terminat. Balanta: {_reguliMiza.GetBalanta(jucatorPrincipal):F2}";
            }
            else
            {
                return $"Rândul tău. Balanta: {_reguliMiza.GetBalanta(jucatorPrincipal):F2}";
            }
        }

        public virtual void ProceseazaStareRetea(string[] parti)
        {
            if (parti.Length >= 5)
            {
                dealer.IncarcaDinRetea(parti[0]);
                jucatorPrincipal.IncarcaDinRetea(parti[2]);
                jocTerminat = parti[4] == "TERMINAT";

                if (jocTerminat && parti.Length > 5)
                {
                    rezultat = parti[5];
                }
                if (parti.Length > 9)
                {
                    if (int.TryParse(parti[7], out int w)) jucatorPrincipal.Win = w;
                    if (int.TryParse(parti[8], out int l)) jucatorPrincipal.Lose = l;
                    if (int.TryParse(parti[9], out int d)) jucatorPrincipal.Draw = d;
                }
            }
        }

        public virtual string GetStareRetea()
        {
            decimal balanta = _reguliMiza.GetBalanta(jucatorPrincipal);
            return $"{dealer.PentruRetea()}|{CalculeazaScor(dealer)}|{jucatorPrincipal.PentruRetea()}|{CalculeazaScor(jucatorPrincipal)}|{(jocTerminat ? "TERMINAT" : "ACTIV")}|{rezultat}|{balanta:F2}|{jucatorPrincipal.Win}|{jucatorPrincipal.Lose}|{jucatorPrincipal.Draw}";
        }
    }
}