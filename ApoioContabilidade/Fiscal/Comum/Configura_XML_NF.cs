using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Fiscal.Comum
{
    public static class Configura_XML_NF
    {
        public static string cUF_pad = "29";
        public static string PATHNFE = @"X:\RECEPÇÃO\Notas Fiscais\MLSA";
        public static string CNPJ_pad = "14512735000109";
        public static string fileConfigXML = "ConfigXml.txt";
    }

    public static class Configura_Almox_Saidas
    {
        public static object RG_SOITENS = 0;
        public static object INS_CK_SETOREXCLUSIVE = false;
        public static object INS_CK_SDONEGATIVO = false;
        public static object INS_STR_DEPOSITO = "";
        public static object INS_CK_SUGERE = true;
        public static object INS_INI_DTAPESQUISA = DateTime.Now.AddYears(-4);
        public static object fileConfigInsumosSaidas = "ConfigInsumosSaidas.txt";
    }

}
