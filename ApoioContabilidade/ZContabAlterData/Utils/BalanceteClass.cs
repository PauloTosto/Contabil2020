using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.ZContabAlterData.Utils
{
    public class BalanceteClass
    {
        
        public class BalancoCampos
        {
            public BalancoCampos()
            {
            }

            public string numconta;
            public decimal sdoAntCalc { get; set; }
            public decimal sdoAnt { get; set; }
            public decimal debito { get; set; }
            public decimal credito { get; set; }
            public decimal sdoAtual { get; set; }
           
            
        }


    }
}
