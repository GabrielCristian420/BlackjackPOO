using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using System.Threading;
using System.Windows.Forms;

namespace BlackjackPOO
{
    public partial class Form1 : Form
    {
        private Joc joc;
        private ClientRetea clientRetea;
        private ServerRetea serverRetea;
        private Button btnHit, btnStand, btnNewGame, btnConnect, btnStartServer;
        private TextBox txtNume, txtIP, txtPort, txtMiza;
        private Label lblJucator, lblDealer, lblScorJucator, lblScorDealer, lblStatus, lblMiza, lblDebug;
        private Panel pnlJucator, pnlDealer;
        private List<PictureBox> cartiJucator;
        private List<PictureBox> cartiDealer;
        private List<Label> lblAltJucatori;
        private Panel pnlConexiune;
        private Label lblRezultat;
        private Thread serverThread;
        private bool joculCuMizaReala;

        public Form1()
        {
            InitializeComponent();
            InitializeazaInterfata();

            joculCuMizaReala = MessageBox.Show(
                "Vrei să joci cu MIZĂ REALĂ (RON)?\n\nDa = Bani reali\nNu = Statistică W-L-D",
                "SELECTEAZĂ MODUL", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            joc = new Joc(joculCuMizaReala);

            //  Setează vizibilitatea controalelor de miză
            lblMiza.Visible = joculCuMizaReala;
            txtMiza.Visible = joculCuMizaReala;

            // Configurează clientul de rețea (dar nu se conectează încă)
            clientRetea = new ClientRetea();
            clientRetea.OnMesajPrimit += ProceseazaMesajServer; // Când vine un mesaj, apelează "ProceseazaMesajServer"
            clientRetea.OnConectat += ClientRetea_OnConectat;

            this.FormClosing += Form1_FormClosing;

            //  Initializează interfața în funcție de modul ales
            ActualizeazaInterfata();
        }
        
        private void ClientRetea_OnConectat(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(ClientRetea_OnConectat), sender, e);
                return;
            }
            joc.ManaJucator.ResetStats();
            pnlConexiune.Visible = false;
            ActualizeazaInterfata();
            lblStatus.Text = $"Conectat ca {txtNume.Text}! Apasă 'JOC NOU' pentru a începe.";
        }
        private void SchimbaModJoc(bool modMizaReala)
        {
            // Actualizăm variabila locală
            joculCuMizaReala = modMizaReala;

            // Reconstruim logica jocului cu noile reguli
            // Salvăm numele curent ca să nu-l pierdem
            string numeTemp = txtNume.Text;

            joc = new Joc(joculCuMizaReala);
            joc.ManaJucator.Nume = numeTemp;
            joc.ManaJucator.ResetStats();

            //  Actualizăm Interfața (ascundem/arătăm controalele de miză)
            lblMiza.Visible = joculCuMizaReala;
            txtMiza.Visible = joculCuMizaReala;

            if (joculCuMizaReala)
                lblStatus.Text = "Mod Server: BANI REALI. Introdu miza și dă 'Joc Nou'.";
            else
                lblStatus.Text = "Mod Server: STATISTICI (W-L-D). Dă 'Joc Nou'.";
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            clientRetea?.Dispose();
            serverRetea?.Dispose();
        }

        private void InitializeazaInterfata()
        {
            this.Text = "Blackjack POO - Multiplayer";
            this.Size = new Size(900, 600);
            this.BackColor = Color.DarkGreen;
            this.StartPosition = FormStartPosition.CenterScreen;

            lblRezultat = new Label
            {
                Text = "",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                Size = new Size(600, 60),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            int centruX = (this.Width - lblRezultat.Width) / 2;
            lblRezultat.Location = new Point(centruX, 190);
            this.Controls.Add(lblRezultat);
            lblRezultat.BringToFront();

            pnlConexiune = new Panel
            {
                Size = new Size(880, 50),
                Location = new Point(10, 10),
                BackColor = Color.DarkSlateGray
            };

            txtNume = new TextBox { Text = "Jucator1", Location = new Point(10, 25), Size = new Size(70, 20) };
            txtIP = new TextBox { Text = "127.0.0.1", Location = new Point(90, 25), Size = new Size(70, 20) };
            txtPort = new TextBox { Text = "8888", Location = new Point(170, 25), Size = new Size(40, 20) };

            btnConnect = new Button
            {
                Text = "Conectează",
                Location = new Point(220, 23),
                Size = new Size(75, 25),
                BackColor = Color.LightBlue
            };
            btnConnect.Click += BtnConnect_Click;

            btnStartServer = new Button
            {
                Text = "Pornește Server",
                Location = new Point(305, 23),
                Size = new Size(100, 25),
                BackColor = Color.LightGreen
            };
            btnStartServer.Click += BtnStartServer_Click;

            pnlConexiune.Controls.AddRange(new Control[] {
                new Label() { Text = "Nume:", Location = new Point(10, 5), ForeColor = Color.White, Font = new Font("Arial", 8), AutoSize = true, BackColor = Color.Transparent },
                txtNume,
                new Label() { Text = "IP:", Location = new Point(90, 5), ForeColor = Color.White, Font = new Font("Arial", 8), AutoSize = true, BackColor = Color.Transparent },
                txtIP,
                new Label() { Text = "Port:", Location = new Point(170, 5), ForeColor = Color.White, Font = new Font("Arial", 8), AutoSize = true, BackColor = Color.Transparent },
                txtPort,
                btnConnect,
                btnStartServer
            });
            this.Controls.Add(pnlConexiune);

            pnlDealer = new Panel
            {
                Size = new Size(600, 150),
                Location = new Point(50, 70),
                BackColor = Color.Transparent
            };
            this.Controls.Add(pnlDealer);
            

            lblDealer = new Label
            {
                Text = "DEALER:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10)
            };
            pnlDealer.Controls.Add(lblDealer);

            pnlJucator = new Panel
            {
                Size = new Size(600, 150),
                Location = new Point(50, 240),
                BackColor = Color.Transparent
            };
            this.Controls.Add(pnlJucator);
            

            lblJucator = new Label
            {
                Text = "JUCĂTOR:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10)
            };
            pnlJucator.Controls.Add(lblJucator);

            // Panel pentru alți jucători
            Panel pnlAltii = new Panel
            {
                Size = new Size(200, 300),
                Location = new Point(670, 70),
                BackColor = Color.FromArgb(100, 0, 0, 0)
            };
            this.Controls.Add(pnlAltii);

            Label lblAltii = new Label
            {
                Text = "JUCĂTORI:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10)
            };
            pnlAltii.Controls.Add(lblAltii);

            lblAltJucatori = new List<Label>();
            for (int i = 0; i < 3; i++)
            {
                Label lbl = new Label
                {
                    Location = new Point(10, 40 + i * 70),
                    Size = new Size(180, 60),
                    BackColor = Color.FromArgb(100, 255, 255, 255),
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Arial", 8),
                    Text = $"Jucator {i + 2}\nAșteptând..."
                };
                pnlAltii.Controls.Add(lbl);
                lblAltJucatori.Add(lbl);
            }

            btnHit = new Button
            {
                Text = "HIT",
                Size = new Size(90, 45),
                Location = new Point(150, 420),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnHit.Click += BtnHit_Click;
            this.Controls.Add(btnHit);

            btnStand = new Button
            {
                Text = "STAND",
                Size = new Size(90, 45),
                Location = new Point(260, 420),
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnStand.Click += BtnStand_Click;
            this.Controls.Add(btnStand);

            btnNewGame = new Button
            {
                Text = "JOC NOU",
                Size = new Size(110, 45),
                Location = new Point(370, 420),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnNewGame.Click += BtnNewGame_Click;
            this.Controls.Add(btnNewGame);

            lblScorDealer = new Label
            {
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(500, 70),
                Size = new Size(150, 30)
            };
            this.Controls.Add(lblScorDealer);

            lblScorJucator = new Label
            {
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(500, 240),
                Size = new Size(150, 30)
            };
            this.Controls.Add(lblScorJucator);

            lblStatus = new Label
            {
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Yellow,
                Location = new Point(50, 490),
                Size = new Size(800, 30),
                Text = "Apasă 'JOC NOU' pentru a începe!"
            };
            this.Controls.Add(lblStatus);

            // Controale pentru miză reală
            lblMiza = new Label
            {
                Text = "Miză:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 525),
                AutoSize = true,
                Visible = false
            };
            this.Controls.Add(lblMiza);

            txtMiza = new TextBox
            {
                Text = "100",
                Location = new Point(100, 523),
                Size = new Size(60, 20),
                Visible = false
            };
            this.Controls.Add(txtMiza);

            cartiJucator = new List<PictureBox>();
            cartiDealer = new List<PictureBox>();
        }

        private void ActualizeazaInterfata()
        {
            lblScorJucator.Text = $"Scor: {joc.CalculeazaScor(joc.ManaJucator)}";
            if (joc.JocTerminat)
                lblScorDealer.Text = $"Scor: {joc.CalculeazaScor(joc.ManaDealer)}";
            else
                lblScorDealer.Text = "Scor: ?";

            btnHit.Enabled = !joc.JocTerminat;
            btnStand.Enabled = !joc.JocTerminat;
            btnNewGame.Enabled = true;

            // Actualizează balanta
            if (joculCuMizaReala)
                lblStatus.Text = $"Balanta: {joc.ManaJucator.BalantaRON:F2} RON";
            else
                lblStatus.Text = joc.ManaJucator.GetStatsWLD();

            // Adaugă status rând jucător
            if (!joc.JocTerminat)
                lblStatus.Text += " | Rândul tău";
            else if (!string.IsNullOrEmpty(joc.Rezultat))
                lblStatus.Text += $" | {joc.Rezultat}";

            // Rezultatul jocului
            if (joc.JocTerminat && !string.IsNullOrEmpty(joc.Rezultat))
            {
                lblRezultat.Text = joc.Rezultat;
                lblRezultat.Visible = true;
                lblRezultat.BringToFront();
            }
            else
            {
                lblRezultat.Visible = false;
            }

            DeseneazaCarti();
        }

        private void DeseneazaCarti()
        {
            
            foreach (var pb in cartiJucator) { pb.Image?.Dispose(); pb.Dispose(); }
            foreach (var pb in cartiDealer) { pb.Image?.Dispose(); pb.Dispose(); }

            // Șterge tot din paneluri
            pnlJucator.Controls.Clear();
            pnlDealer.Controls.Clear();

            pnlJucator.Controls.Add(lblJucator);
            pnlDealer.Controls.Add(lblDealer);

            cartiJucator.Clear();
            cartiDealer.Clear();

            for (int i = 0; i < joc.ManaJucator.NumarCarti; i++)
            {
                Carte carte = joc.ManaJucator.GetCarte(i);
                if (carte != null)
                {
                    string path = Path.Combine("CartiPNG", carte.NumeFisier);
                    if (File.Exists(path))
                    {
                        PictureBox pb = new PictureBox
                        {
                            Size = new Size(70, 100),
                            Location = new Point(10 + i * 75, 40),
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            BackColor = Color.Transparent,
                            Image = Image.FromFile(path),
                            Visible = true
                        };

                        pnlJucator.Controls.Add(pb);
                        cartiJucator.Add(pb);
                        pb.BringToFront();
                    }
                }
            }

            for (int i = 0; i < joc.ManaDealer.NumarCarti; i++)
            {
                Carte carte = joc.ManaDealer.GetCarte(i);
                if (carte != null)
                {
                    bool eCarteAscunsa = !joc.JocTerminat && i == 1;
                    string path = Path.Combine("CartiPNG", eCarteAscunsa ? "Back.png" : carte.NumeFisier);

                    if (File.Exists(path))
                    {
                        PictureBox pb = new PictureBox
                        {
                            Size = new Size(70, 100),
                            Location = new Point(10 + i * 75, 40),
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            BackColor = Color.Transparent,
                            Image = Image.FromFile(path),
                            Visible = true
                        };

                        pnlDealer.Controls.Add(pb);
                        cartiDealer.Add(pb);
                        pb.BringToFront();
                    }
                }
            }

            pnlJucator.Invalidate();
            pnlDealer.Invalidate();
            pnlJucator.Update();
            pnlDealer.Update();
        }

        private void BtnHit_Click(object sender, EventArgs e)
        {
            if (clientRetea.EsteConectat)
                clientRetea.TrimiteMesaj($"HIT|{clientRetea.Id}");
            else
            {
                joc.JucatorHit();
                ActualizeazaInterfata();
            }
        }

        private void BtnStand_Click(object sender, EventArgs e)
        {
            if (clientRetea.EsteConectat)
                clientRetea.TrimiteMesaj($"STAND|{clientRetea.Id}");
            else
            {
                joc.JucatorStand();
                ActualizeazaInterfata();
            }
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            lblRezultat.Visible = false;
            decimal miza = 1;

            if (joculCuMizaReala)
            {
                if (!decimal.TryParse(txtMiza.Text, out miza) || miza <= 0)
                {
                    MessageBox.Show("Introdu o miză validă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                joc.SetMiza(miza);
            }
            else
            {
                joc.SetMiza(1);
            }

            if (clientRetea.EsteConectat)
            {
                clientRetea.TrimiteMesaj($"NEW_GAME|{miza}|{clientRetea.Id}");
            }
            else
            {
                try
                { 
                    joc.ÎncepeJoc();
                    ActualizeazaInterfata();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Eroare Miza", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try { clientRetea.Conecteaza(txtIP.Text, int.Parse(txtPort.Text), txtNume.Text); }
            catch (Exception ex) { MessageBox.Show($"Eroare conexiune: {ex.Message}", "Eroare"); }
        }

        private void BtnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                bool serverCuMiza = MessageBox.Show("Serverul va folosi miză reală?", "Configurare server", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                serverRetea = new ServerRetea(serverCuMiza);
                serverThread = new Thread(serverRetea.Porneste) { IsBackground = true };
                serverThread.Start();
                MessageBox.Show($"Server pornit pe portul 8888 (Mod: {(serverCuMiza ? "MIZĂ REALĂ" : "W-L-D")})!", "Server Pornit");
            }
            catch (Exception ex) { MessageBox.Show($"Eroare server: {ex.Message}", "Eroare"); }
        }

        private void ProceseazaMesajServer(string mesaj)
        {
            // INVOKE: Verificăm dacă suntem pe firul secundar
            if (this.InvokeRequired)
            {
                // Ne re-apelăm pe noi înșine, dar pe firul principal
                this.Invoke(new Action<string>(ProceseazaMesajServer), mesaj);
                return; // Ieșim din firul secundar
            }

            if (mesaj.StartsWith("ID_ASSIGN|")) return;

            string[] parti = mesaj.Split('|');
            if (parti.Length == 0) return;

            switch (parti[0])
            {
                case "GAME_STATE":
                    if (parti.Length > 1)
                    {
                        // Serverul ne-a dat starea jocului. O băgăm în obiectul nostru local.
                        string[] subParti = new string[parti.Length - 1];
                        for (int i = 1; i < parti.Length; i++)
                        {
                            subParti[i - 1] = parti[i];
                        }

                        joc.ProceseazaStareRetea(subParti);

                        // Procesăm balanta dacă e cazul
                        if (parti.Length > 7)
                        {
                            if (decimal.TryParse(parti[7], out decimal balanta))
                            {
                                joc.SetBalanta(balanta);
                            }
                        }

                        ActualizeazaInterfata();
                    }
                    break;
                
                case "PLAYER_LIST":
                    ActualizeazaListaJucatori(parti);
                    break;
                case "MODE":
                    if (parti.Length > 1)
                    {
                        string modServer = parti[1];
                        bool esteModReal = (modServer == "REAL");

                        // Dacă modul serverului e diferit de ce am ales eu la pornire, schimbăm
                        if (esteModReal != joculCuMizaReala)
                        {
                            SchimbaModJoc(esteModReal);
                        }
                    }
                    break;
                case "MESSAGE":
                    if (parti.Length > 1) lblStatus.Text = parti[1];
                    break;
            }
        }

        private void ActualizeazaListaJucatori(string[] parti)
        {
            for (int i = 0; i < lblAltJucatori.Count; i++)
            {
                if (i + 1 < parti.Length && !string.IsNullOrEmpty(parti[i + 1]))
                    lblAltJucatori[i].Text = parti[i + 1];
                else
                    lblAltJucatori[i].Text = $"Jucator {i + 2}\nAșteptând...";
            }
        }
    }
}