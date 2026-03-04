using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BlackjackPOO
{
    public class ServerRetea : IDisposable
    {
        private TcpListener listener;
        private List<ClientHandler> clienti = new List<ClientHandler>();
        private Dictionary<string, Joc> jocuriJucatori = new Dictionary<string, Joc>();
        private bool disposed = false;
        private bool cuMizaReala;

        public ServerRetea(bool cuMizaReala)
        {
            this.cuMizaReala = cuMizaReala;
        }

        public void Porneste()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, 8888);
                listener.Start();
                Console.WriteLine($"Server pornit pe portul 8888 (Mod: {(cuMizaReala ? "MIZĂ REALĂ" : "W-L-D")})");

                while (!disposed)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientHandler handler = new ClientHandler(client, this);
                    lock (clienti) clienti.Add(handler);

                    Thread clientThread = new Thread(handler.HandleClient) { IsBackground = true };
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                if (!disposed) Console.WriteLine($"Eroare server: {ex.Message}");
            }
        }

        public void ProceseazaMesaj(ClientHandler client, string mesaj)
        {
            if (string.IsNullOrWhiteSpace(mesaj)) return;

            string[] parti = mesaj.Split('|');
            if (parti.Length == 0) return;

            switch (parti[0])
            {
                case "NUME":
                    client.Nume = parti.Length > 1 ? parti[1] : "Jucator";
                    client.Id = Guid.NewGuid().ToString().Substring(0, 8);

                    if (!jocuriJucatori.ContainsKey(client.Id))
                    {
                        jocuriJucatori[client.Id] = new Joc(cuMizaReala);
                        jocuriJucatori[client.Id].ManaJucator.Nume = client.Nume;
                        jocuriJucatori[client.Id].ManaJucator.Id = client.Id;
                    }

                    client.TrimiteMesaj($"ID_ASSIGN|{client.Id}");
                    client.TrimiteMesaj($"MODE|{(cuMizaReala ? "REAL" : "VIRTUAL")}");
                    TrimiteTuturor($"PLAYER_LIST|{GetListaJucatori()}");
                    break;

                case "NEW_GAME":
                    if (parti.Length > 2)
                    {
                        decimal miza = decimal.Parse(parti[1]);
                        string jucatorId = parti[2];

                        if (jocuriJucatori.ContainsKey(jucatorId))
                        {
                            var joc = jocuriJucatori[jucatorId];
                            try
                            {
                                
                                joc.SetMiza(miza);
                                joc.ÎncepeJoc();

                                string stare = joc.GetStareRetea();
                                client.TrimiteMesaj($"GAME_STATE|{stare}");
                                TrimiteTuturor($"PLAYER_LIST|{GetListaJucatori()}");
                            }
                            catch (Exception ex)
                            {
                                // Trimitem un mesaj pe ecran clientului in caz de lipsa de fonduri
                                client.TrimiteMesaj($"MESSAGE|{ex.Message}");
                            }
                        }
                    }
                    break;

                case "HIT":
                    if (parti.Length > 1)
                    {
                        string jucatorId = parti[1];
                        if (jocuriJucatori.ContainsKey(jucatorId))
                        {
                            var joc = jocuriJucatori[jucatorId];
                            joc.JucatorHit();
                            client.TrimiteMesaj($"GAME_STATE|{joc.GetStareRetea()}");
                            if (joc.JocTerminat)
                            {
                                TrimiteTuturor($"PLAYER_LIST|{GetListaJucatori()}");
                            }
                        }
                    }
                    break;

                case "STAND":
                    if (parti.Length > 1)
                    {
                        string jucatorId = parti[1];
                        if (jocuriJucatori.ContainsKey(jucatorId))
                        {
                            var joc = jocuriJucatori[jucatorId];
                            joc.JucatorStand();
                            client.TrimiteMesaj($"GAME_STATE|{joc.GetStareRetea()}");
                            TrimiteTuturor($"PLAYER_LIST|{GetListaJucatori()}");
                        }
                    }
                    break;
            }
        }

        private string GetListaJucatori()
        {
            List<string> infoJucatori = new List<string>();

           
            lock (clienti)
            {
                for (int i = 0; i < clienti.Count; i++)
                {
                    var client = clienti[i];
                    if (!string.IsNullOrEmpty(client.Id))
                    {
                        if (jocuriJucatori.ContainsKey(client.Id))
                        {
                            var jucator = jocuriJucatori[client.Id].ManaJucator;
                            if (cuMizaReala)
                                infoJucatori.Add($"{client.Nume}\nBal: {jucator.BalantaRON:F2}");
                            else
                                infoJucatori.Add($"{client.Nume}\n{jucator.GetStatsWLD()}");
                        }
                        else
                        {
                            if (cuMizaReala)
                                infoJucatori.Add($"{client.Nume}\nBal: 1000.00");
                            else
                                infoJucatori.Add($"{client.Nume}\nW-L-D: 0-0-0");
                        }
                    }
                }
            }

            while (infoJucatori.Count < 3)
            {
                int indexLiber = infoJucatori.Count + 2;
                infoJucatori.Add($"Jucator {indexLiber}\nAșteptând...");
            }

            return string.Join("|", infoJucatori);
        }

        public void TrimiteTuturor(string mesaj)
        {
            lock (clienti)
            {
                for (int i = clienti.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        if (clienti[i].EsteConectat)
                            clienti[i].TrimiteMesaj(mesaj);
                    }
                    catch
                    {
                        clienti.RemoveAt(i);
                    }
                }
            }
        }

        public void EliminaClient(ClientHandler client)
        {
            lock (clienti)
            {
                if (!string.IsNullOrEmpty(client.Id) && jocuriJucatori.ContainsKey(client.Id))
                    jocuriJucatori.Remove(client.Id);

                clienti.Remove(client);
                TrimiteTuturor($"PLAYER_LIST|{GetListaJucatori()}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    listener?.Stop();
                    lock (clienti)
                    {
                        foreach (var client in clienti) client.Dispose();
                        clienti.Clear();
                    }
                }
                disposed = true;
            }
        }

        ~ServerRetea()
        {
            Dispose(false);
        }
    }

    public class ClientHandler : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;
        private ServerRetea server;
        private bool disposed = false;

        public string Nume { get; set; }
        public string Id { get; set; }
        public bool EsteConectat
        {
            get { return client != null && client.Connected; }
        }

        public ClientHandler(TcpClient client, ServerRetea server)
        {
            this.client = client;
            this.stream = client.GetStream();
            this.server = server;
        }

        public void HandleClient()
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            string bufferString = ""; 
            try
            {
                while (!disposed && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bufferString += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    int indexLinie;

                    // Extragem mesajele bazat pe \0
                    while ((indexLinie = bufferString.IndexOf('\0')) >= 0)
                    {
                        string mesajComplet = bufferString.Substring(0, indexLinie);
                        bufferString = bufferString.Substring(indexLinie + 1);
                        server.ProceseazaMesaj(this, mesajComplet);
                    }
                }
            }
            catch { }
            finally
            {
                server.EliminaClient(this);
                Dispose();
            }
        }

        public void TrimiteMesaj(string mesaj)
        {
            if (!disposed && stream.CanWrite)
            {
                // Delimitator
                byte[] data = Encoding.UTF8.GetBytes(mesaj + "\0");
                stream.Write(data, 0, data.Length);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    stream?.Close();
                    client?.Close();
                }
                disposed = true;
            }
        }

        ~ClientHandler()
        {
            Dispose(false);
        }
    }
}