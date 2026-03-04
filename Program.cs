using System;
using System.Windows.Forms;

namespace BlackjackPOO
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // ✅ PREVENIRE multiple instanțe
           // bool nouaInstanta;
           // using (var mutex = new System.Threading.Mutex(true, "BlackjackPOOApp", out nouaInstanta))
          //  {
               // if (nouaInstanta)
             //   {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
              //  }
              //  else
              //  {
               //     MessageBox.Show("Aplicația este deja deschisă!", "Blackjack POO",
                //        MessageBoxButtons.OK, MessageBoxIcon.Information);
               // }
         //   }
        }
    }
}