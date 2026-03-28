using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Trabalho.ServicesTrab
{
    static public class Util
    {
        static public DateTime DiadoPonto(DateTime tdata, DayOfWeek tconst)
        {
            DateTime result = tdata;
            while (result.DayOfWeek != tconst)
            {
                result = result.AddDays(-1);
            }
            return result;
        }
        static public DateTime DiaFinaldoPonto(DateTime tdata, DayOfWeek tconst)
        {
            DateTime result = tdata;
            while (result.DayOfWeek != tconst)
            {
                result = result.AddDays(1);
            }
            return result;
        }
    }
}
