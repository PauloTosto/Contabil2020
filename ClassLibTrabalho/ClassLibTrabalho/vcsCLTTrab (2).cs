using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using ClassConexao;

namespace ClassLibTrabalho


{
    
    public class TAtualizaFolha
    {
        public const DayOfWeek _DIADASEMANA = DayOfWeek.Thursday;
        public const DayOfWeek _DIAFIMDASEMANA = DayOfWeek.Wednesday;
        DataSet oTabelasAux;
        Dictionary<string, TCLTCodigo> FCLTCodigo;

        public DateTime inicio, fim;
        public Dictionary<string, TCLTCodigo> CLTCodig 
        { get 
           {
               return FCLTCodigo;
           } 
        }

        public DateTime Inicio
        {
            get  { return inicio;}
            set { inicio = value; }
        }
        public DateTime Fim
        {
            get { return fim; }
            set { fim = value; }
        }


         public DataTable Modelo 
        { get 
           {
            return oTabelasAux.Tables["MODELO"];
           } 
        }

         public DataTable Servico
         {
             get
             {
                 return oTabelasAux.Tables["SERVIC"];
             }
         }

        public TAtualizaFolha(DateTime oinicio, DateTime ofim)
        {
            inicio = oinicio;
            fim = ofim;
            oTabelasAux = TDataDivs.TabelasCLT();
            FCLTCodigo = GetDict_CLTCodigo();

        }





        public class TCLTCodigo : Object
        {
            public string indcod;
            public string desccod;
            public double diarias; //currency
            public double horas50;
            public double horas100;
            public double horasfe;
            public double horas_efetivas;
            public string compati;
            public double quant;
        }


        public class TObjPonto:Object
       {
           public DateTime semana;
           public DateTime inicio_semana;
           public DateTime fim_semana;
           public Boolean temdomingo;
           public int inddomingo;
           public decimal horas_padrao; //currency
           public int ndias;
           public int diainicial;
           public int diafinal;
       }
  


        public static DateTime DiaDoPonto(DateTime tdata, DayOfWeek tconst)
        {
            DateTime result = tdata;
            while (result.DayOfWeek != tconst)   //Wednesday"
                   result = result.AddDays(-1);
            return result;
        }

        public static DataTable ProvQuadra()
        {
            DataTable result = new DataTable("PVQUEM1");
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "SETOR", 2, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "CENTRO", 4, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "BL", 3, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "TIPO", 1, false));   // "D"/"E"
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "NUM_MOD", 3, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "CODSER", 3, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(DateTime), "PINICIO", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(DateTime), "PFIM", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(DateTime), "DTAINICIO", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(DateTime), "DTAFIM", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Single), "HORAS", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Single), "DIARIAS", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Single), "QUANT", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Single), "PRODUT", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Single), "AREA", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Single), "PRODAREA", 0, false));
            return result;
        }

        public static Dictionary<DateTime, TAtualizaFolha.TObjPonto> Peg_Ponto_LongoPeriodo(DateTime tdata, DateTime tdata2)
        {
            Dictionary<DateTime, TAtualizaFolha.TObjPonto> result = new Dictionary<DateTime,TObjPonto>();
            DateTime fim = DiaDoPonto(tdata2, _DIAFIMDASEMANA);
            DateTime inicio = tdata;
            tdata = DiaDoPonto(tdata, _DIADASEMANA);
            TObjPonto oponto;
            while (tdata < fim)
            {
                oponto = new TObjPonto();
                oponto.semana = tdata;
                oponto.temdomingo = false;
                oponto.inddomingo = 0;
                oponto.inicio_semana = tdata;
                oponto.horas_padrao = 44;
                oponto.ndias = 0;
                oponto.fim_semana = oponto.inicio_semana.AddDays(6);
                if (inicio > oponto.inicio_semana)
                    oponto.inicio_semana = inicio;
                if (fim < oponto.fim_semana)
                    oponto.fim_semana = fim;
                oponto.ndias = (oponto.fim_semana - oponto.inicio_semana).Days+1;
                oponto.diainicial = (7 - oponto.ndias) + 1;
                if ((oponto.fim_semana == fim) && (oponto.ndias != 7)) oponto.diainicial = 1;
                oponto.diafinal = oponto.diainicial + oponto.ndias - 1;
                while (((oponto.inicio_semana.AddDays(oponto.inddomingo)) <= oponto.fim_semana) && (!oponto.temdomingo))
                {
                    if (oponto.inicio_semana.AddDays(oponto.inddomingo).DayOfWeek == DayOfWeek.Sunday)
                    {
                        oponto.temdomingo = true;
                        break;
                    }
                    oponto.inddomingo += 1;
                }

                if (!oponto.temdomingo) oponto.inddomingo = 0;
                else
                    oponto.inddomingo = oponto.diainicial + oponto.inddomingo;

                result.Add(tdata.Date, oponto);
                tdata = tdata.AddDays(7);
            }
            ICollection<TObjPonto> pontos =  result.Values;
            oponto = pontos.First();
            if (oponto.temdomingo)
            {
                int j = 0;
                oponto.horas_padrao = 0M;
                while ((oponto.inicio_semana.AddDays(j)) <= oponto.fim_semana)
                {
                    if (oponto.inicio_semana.AddDays(j).DayOfWeek != DayOfWeek.Sunday)
                        if (oponto.inicio_semana.AddDays(j).DayOfWeek != DayOfWeek.Saturday)
                            oponto.horas_padrao = oponto.horas_padrao + 4;
                        else
                            oponto.horas_padrao = oponto.horas_padrao + 8;
                    j += 1;
                }
            }
            oponto = pontos.Last();
            if (oponto.temdomingo)
            {
                int j = 0;
                oponto.horas_padrao = 0M;
                while ((oponto.inicio_semana.AddDays(j)) <= oponto.fim_semana)
                {
                    if (oponto.inicio_semana.AddDays(j).DayOfWeek != DayOfWeek.Sunday)
                        if (oponto.inicio_semana.AddDays(j).DayOfWeek != DayOfWeek.Saturday)
                            oponto.horas_padrao = oponto.horas_padrao + 4;
                        else
                            oponto.horas_padrao = oponto.horas_padrao + 8;
                    j += 1;
                }
            }
           
            return result;
        }

        public static TObjPonto Peg_PontoDireto(DateTime tdata,TObjPonto [] Pontos)
        {
            TObjPonto result = null;
            for (int i = Pontos.Length-1; i >=0; i--)
            {
                if (tdata.Date == Pontos[i].semana.Date)
                {
                    result = Pontos[i];
                    break;
                }
            }
            return result;
        }

        public csQuadras DadosProdQuadra(string tquadra, string tcodser, string tnum, DateTime dtaponto, DateTime dtafim)
        {
            string tcult="";
            string tunid = ServicoUnid(tcodser);
            if (tunid.Substring(1, 2) == "PL")
                tunid = "PL";
            else
                if (tunid.Substring(1, 3) == "TAR")
                    tunid = "TAR";
                else
                    if (tunid.Substring(1, 2) == "CX")
                        tunid = "CX";
            if (tnum.Trim() != "") { tcult = ModeloCultura(tnum); };
            csQuadras result = new csQuadras(tquadra,tcodser, tnum, tunid, tcult, dtaponto, dtafim);
            return result;
        }

        public string ServicoUnid(string fcodser)
        {
            DataRow orow = Servico.Rows.Find(new object[] { fcodser});
            if (orow == null) return "";
            return orow["UNID"].ToString();

        }
        public string ModeloCultura(string fnum)
        {
            DataRow[] orow = Modelo.Select("(NUM = '" + fnum + "') AND (COD = '')");
             //Modelo.Rows.Find(new object [] {fnum,""}); 
            if (orow == null) return "";
            return orow[0]["DESC"].ToString();
        }


        public DataSet PegDadosEstatistica(DateTime inicio, DateTime fim)
        {
            //Dictionary<string, TTrab> ListTrab = new Dictionary<string, TTrab>();

            DateTime linicio = TAtualizaFolha.DiaDoPonto(inicio, TAtualizaFolha._DIADASEMANA);//ENCodeDate(ano,mes,1);
            DateTime lfim = TAtualizaFolha.DiaDoPonto(fim, TAtualizaFolha._DIAFIMDASEMANA);  //UltimoDiaMes(tdata);

            //rever...
            return TDataDivs.Get_CLTPontosPeriodo(linicio, lfim, "D");

        }


        static public Dictionary<string, TAtualizaFolha.TCLTCodigo> GetDict_CLTCodigo()
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            strOleDb = "SELECT  cltcodig.* FROM " + path + "CLTCODIG"; //+
            Dictionary<string, TAtualizaFolha.TCLTCodigo> result = new Dictionary<string, TAtualizaFolha.TCLTCodigo>();
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbDataReader leitor = OleDbcomm.ExecuteReader();

                if (leitor.HasRows)
                {
                    TAtualizaFolha.TCLTCodigo oCodigo;
                    while (leitor.Read())
                    {
                        if (Convert.IsDBNull(leitor["INDCOD"])) continue;
                        oCodigo = new TAtualizaFolha.TCLTCodigo();
                        oCodigo.desccod = leitor["DESCCOD"].ToString();
                        oCodigo.indcod = leitor["INDCOD"].ToString();
                        oCodigo.diarias = Convert.ToDouble(leitor["FATOR"]); // 3  horas normais
                        oCodigo.horas_efetivas = Convert.ToDouble(leitor["HORAS"]); // 4 horas efetivas
                        oCodigo.horas50 = Convert.ToDouble(leitor["X_HORAS"]); // 5 horas 50
                        oCodigo.horas100 = Convert.ToDouble(leitor["NOTURNO"]); // 6  horas 100
                        oCodigo.compati = leitor["COMPATI"].ToString();  // 7
                        oCodigo.horasfe = Convert.ToDouble(leitor["H_NORMAL"]); // 8 horas Feriado
                        result.Add(oCodigo.indcod, oCodigo);
                    }
                }
                leitor.Close();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }



/*        public static DataColumn Coluna(Type otipo, string onome, int omaxlength, Boolean oreadonly)
        {
            DataColumn ocoluna = new DataColumn();
            try
            {
                ocoluna.DataType = otipo;
                ocoluna.ColumnName = onome;
                if (!((Convert.IsDBNull(omaxlength)) || (omaxlength == 0)))
                    ocoluna.MaxLength = omaxlength;
                if (!Convert.IsDBNull(oreadonly))
                    ocoluna.ReadOnly = oreadonly;
                return ocoluna;
            }
            catch (Exception)
            {

                throw new Exception("Erro Criação Coluna");
            }

        }
        */

        

        }
    public class csQuadras
    {
        string fquadra, fcodser, fnum, funid, fcult;
        DateTime finicio, ffim;
        double farea, fprod, fprod_area;
        //public string quadra { get { return fquadra; } set { fquadra = value; } }
        //public string codser { get { return fcodser; } set { fcodser = value; } }
        //public string num { get { return fnum; } set { fnum = value; } }
        public string unidade { get { return funid; } }
        public string cult { get { return fcult; } }
        public DateTime inicio { get { return finicio; } }
        public DateTime fim { get { return ffim; } }
        public double area
        {
            get
            {
                if (fquadra.Trim() == "") return 0;
                if (farea == 0)
                    farea = Math.Round((2.333F * TDataDivs.peghectares(fquadra)), 4);
                return farea;
            }
        }
        public double producao
        {
            get
            {
                if (funid.Trim() != "CX")
                    fprod = 0;
                else
                    if (fprod == 0)
                        fprod = TDataDivs.pegcaixas(fquadra, finicio, ffim);
                return fprod;
            }
        }
        public double prod_area
        {
            get
            {
                if ((producao != 0) && (area != 0))
                    fprod_area = (producao / area);  // caixas/tarefas
                else fprod_area = 0;
                return fprod_area;
            }
        }
        public double quant
        {
            get
            {
                double result = 0;
                if ((funid.Trim() == "PL") && (fcult.Trim() != ""))
                    result = TDataDivs.pegplantas(fquadra, fcult);
                else
                    if (funid.Trim() == "TAR")
                        result = area;
                    else
                        if (funid.Trim() == "CX")
                            result = producao;
                return result;
            }
        }
        public csQuadras(string tquadra, string tcodser, string tnum, string tunid, string tcult, DateTime dtaponto, DateTime dtafim)
        {
            fquadra = tquadra;
            fcodser = tcodser;
            fnum = tnum;
            finicio = dtaponto;
            ffim = dtafim;
            funid = tunid;
            farea = 0;
            fcult = "";
            fprod_area = 0;
            fprod = 0;
            if (fcodser.Trim() == "")
                fcult = "";
            fcult = tcult;
        }

       
    }
}
