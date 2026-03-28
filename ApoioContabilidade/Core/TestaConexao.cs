using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Core
{
    
    public static class TestaConexao
    {
        public static bool CheckForInternetConnection(string Servidor)
        {
            /*if (Servidor.Contains("."))
            {
                System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(Servidor);
                Servidor = entry.HostName;
            }*/
            bool Sucesso = false;

            try
            {
                Ping myPing = new Ping();
                String host = Servidor;
                byte[] buffer = new byte[32];
                int timeout = 30000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                {
                    Sucesso = true;
                };

            }
            catch (Exception)
            {

                Sucesso = false;
            }
            
            return Sucesso;

            /*try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://" + Servidor))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }*/
        }


        public static string ReturnHost(string server)
        {
            string host = "";
            if (!(server.Contains(".")))
            {
                System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(server.ToUpper());

                if (entry.AddressList.Count() > 0)
                {
                    host = entry.AddressList[entry.AddressList.Count() - 1].ToString();
                }
                else host = "";
            }
            else { host = server; }

            return host;
        }

    }
    public static class ConexaoAtual
    {
        public static string SERVIDOR_IP = "Servidor";
    }


}
