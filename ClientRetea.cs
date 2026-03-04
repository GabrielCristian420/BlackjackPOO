using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BlackjackPOO
{
    public class ClientRetea : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;
        private bool conectat;
        private bool disposed = false;

        public string NumeJucator { get; private set; }
        public string Id { get; private set; } = "LOCAL";

        public delegate void MesajPrimitHandler(string mesaj);
        public event MesajPrimitHandler OnMesajPrimit;
        public event EventHandler OnConectat;

        public bool EsteConectat
        {
            get { return conectat && client != null && client.Connected; }
        }

        public ClientRetea()
        {
            conectat = false;
        }

        public void Conecteaza(string ip, int port, string nume)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
                stream = client.GetStream();
                conectat = true;
                NumeJucator = nume;

                TrimiteMesaj($"NUME|{nume}");
                Thread threadAsculta = new Thread(AscultaMesaje);
                threadAsculta.IsBackground = true;
                threadAsculta.Start();

                OnConectat?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                conectat = false;
                throw new Exception($"Eroare la conectare: {ex.Message}");
            }
        }

        public void TrimiteMesaj(string mesaj)
        {
            if (EsteConectat && stream.CanWrite)
            {
                try
                {
                    // Adăugăm \0 ca delimitator pentru a preveni lipirea pachetelor TCP
                    byte[] data = Encoding.UTF8.GetBytes(mesaj + "\0");
                    stream.Write(data, 0, data.Length);
                }
                catch
                {
                    conectat = false;
                }
            }
        }

        private void AscultaMesaje()
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            string bufferString = ""; //  Stocăm datele parțiale

            try
            {
                while (EsteConectat && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bufferString += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    int indexLinie;

                    // Extragem mesaj cu mesaj pe baza delimitatorului \0
                    while ((indexLinie = bufferString.IndexOf('\0')) >= 0)
                    {
                        string mesajComplet = bufferString.Substring(0, indexLinie);
                        bufferString = bufferString.Substring(indexLinie + 1);

                        if (mesajComplet.StartsWith("ID_ASSIGN|"))
                        {
                            string[] parti = mesajComplet.Split('|');
                            if (parti.Length > 1)
                                Id = parti[1];
                            continue;
                        }

                        OnMesajPrimit?.Invoke(mesajComplet);
                    }
                }
            }
            catch
            {
                conectat = false;
            }
        }

        public void Deconecteaza()
        {
            conectat = false;
            stream?.Close();
            client?.Close();
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
                    Deconecteaza();
                }
                disposed = true;
            }
        }

        ~ClientRetea()
        {
            Dispose(false);
        }
    }
}