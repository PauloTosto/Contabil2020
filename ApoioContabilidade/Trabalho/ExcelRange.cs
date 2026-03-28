using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Trabalho
{
    public static  class ExcelRange
    {
        static private void PonhaNoExcel(Microsoft.Office.Interop.Excel._Worksheet oworksheet,
          int linha, string letra, object valor)
        {
            int via1 = linha;
            string linhaStr = via1.ToString();
            var range = oworksheet.get_Range(letra + linhaStr, letra + linhaStr);
            range.Value = valor;
        }

        static public void PonhaNoExcel2(Microsoft.Office.Interop.Excel._Worksheet oworksheet,
             int linha, string letra, object valor)
        {
            int via1 = linha;
            string linhaStr = via1.ToString();
            var range = oworksheet.get_Range(letra + linhaStr, letra + linhaStr);
            range.Value2 = valor;
        }
    }
}
