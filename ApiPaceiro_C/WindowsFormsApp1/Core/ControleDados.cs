using System.Collections.Generic;
using System.IO;





namespace PrjApiParceiro_C.Core
{
  
    static public class AcessosStream
    {
        static public Dictionary<string, string> LeiaLista(string filename)
        {
            // Read and show each line from the file. 
            Dictionary<string, string> oresultado = new Dictionary<string, string>();
            string line = "";
            using (StreamReader sr = new StreamReader(filename))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("="))
                    {
                        int pos = line.IndexOf("=");
                        string chave = line.Substring(0, pos);
                        string valor = line.Substring(pos + 1);
                        oresultado.Add(chave, valor);
                    }
                }
            }
            return oresultado;
        }
        static public void WriteLista(string filename, Dictionary<string, string> oLista)
        {

            FileMode oenum = FileMode.Truncate;
            if (File.Exists(filename) == false)
                oenum = FileMode.CreateNew;

            using (FileStream fs = new FileStream(filename, oenum))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (var olinha in oLista)
                        writer.WriteLine(olinha.Key + "=" + olinha.Value);
                }
                fs.Close();
            }

        }

    }


}
