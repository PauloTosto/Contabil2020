using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassFiltroEdite
{
    public static class Miscelania
    {

        // construindo delegates....
        // Delegate que construi como pedaço de código SQL
        static public string CompareValor(LinhaSolucao oLinha)
        {
            string result = "";
            if ((oLinha.dado[0] == "") || (oLinha.dado[1] == "") || (oLinha.campo.Trim() == ""))
                return result;
            string valor = "";
            try
            {
                double ovalor = Convert.ToDouble(oLinha.dado[1].Trim());
                valor = FormatDoubleGravar(ovalor); //2DECIMAIS
            }
            catch
            {
                throw;
            }

            string comparador = oLinha.dado[0].Trim();
            result = oLinha.campo.Trim() + " " + comparador + " " + valor;
            if (result.Length > 0)
            { result = "  AND (" + result + ")"; }
            return result;
        }
        static public string FormatDoubleGravar(double ovalor)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            string result = ovalor.ToString("N2", ci);
            return result;
        }
        static public string FormatDoubleGravar(double ovalor, int num)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            string result = ovalor.ToString("N" + num.ToString().Trim(), ci);
            return result;
        }
    }
}
