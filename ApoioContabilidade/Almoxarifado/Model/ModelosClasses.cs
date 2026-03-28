using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Almoxarifado.Model
{
    public class InicioFim
    {
        public InicioFim()
        {
            errosInicioFim = new List<InicioFim>();
        }

        public DateTime inicio { get; set; }
        public DateTime fim { get; set; }

        public double quant { get; set; }
        public double valor { get; set; }

        public double custoMedio { get; set; }

        public double quantSaida { get; set; }
        public double valorSaida { get; set; }

        public double quantAcum { get; set; }
        public double valAcum { get; set; }

        public List<InicioFim> errosInicioFim;
        public string tipoErro { get; set; }

    }

    static public class Utils
    {
        static public void Combo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
                e.KeyChar = Char.ToUpper(e.KeyChar);

        }

        static public DateTime dataRef = new DateTime(2000, 1, 1);
    }



}
