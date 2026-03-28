using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Financeiro.Models
{
    class ModelComboBoxSource
    {
    }

    public class Bancos
    {
        private string nbanco;
        private string descri;

        public string DESCRI
        {
            get { return descri; }
            set { descri = value; }
        }

        public string NBANCO
        {
            get { return nbanco; }
            set { nbanco = value; }
        }
    }

    public class PlaconDesc2
    {
        private string desc2;

        public string DESC2
        {
            get { return desc2; }
            set { desc2 = value; }
        }
     }


}
