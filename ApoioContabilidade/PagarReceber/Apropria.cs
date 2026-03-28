using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.PagarReceber
{
    // Classe Que servirá se for automatizar a Apropriaçao
    public class Apropria
    {
        public string desc2 { get; set; }
        public double valor { get; set; }
        public string historico { get; set; }
        public DateTime data { get; set; }
        public List<string> verifiqueNum { get; set; }
        public string setor { get; set; }
        public string num_mod { get; set; }
        public string codser { get; set; }
        public string centro { get; set; }
        public int icodser { get; set; }
        public string numconta_ap { get; set; }
        public double valor_ap { get; set; }
        public double mov_id_ap { get; set; }
        public Apropria()
        {
            verifiqueNum = new List<string>();
        }
    }
}

