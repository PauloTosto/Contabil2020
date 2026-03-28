using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
//using ClassFiltroEdite;

namespace ClassConexao//ClassLibTrabalho//WFAEstatiscaTrabalho
{
    static class TSession
    {
        public static TDataControlReduzido DataControl()
        {
            return new TDataControlReduzido();
        }
    }
    enum TState { stEdit, stInsert }
    public class TDiretorio
    {
        private TState FState;
        public
        string FSistema, FCaminho;
        public
       Boolean Update()
        {
            return TDataControlReduzido.Update_Diretorio(this); //TSession.DataControl().Update_Diretorio(this); ;

        }
        Boolean Add()
        {
            return TDataControlReduzido.Add_Diretorio(this);
        }

        TState State
        {
            get { return FState; }
            set { FState = value; }
        }
        public string Sistema
        {
            get { return FSistema; }
            set { FSistema = value; }
        }
        public string Caminho
        {
            get { return FCaminho; }
            set { FCaminho = value; }
        }


    }

    public static class ListaCaminhos
    {
        static public Dictionary<string, string> Paths = new Dictionary<string, string>();
        static public string GetPath(string key)
        {
            string result = "";
            if (Paths.ContainsKey(key.Trim()))
                result = Paths[key];
            return result;
        }

    }

    public class TDataControlReduzido
    {
        
      
        
        static public TConnectionPooling ConnectionPooling = new TConnectionPooling();
        static public Boolean Add_Diretorio(TDiretorio Diretorio)
        {
            OleDbCommand OleDbComm;
            string str;
            OleDbComm = null;
            str = "INSERT INTO DIRETORI (SISTEMA,CAMINHO) VALUES('" +
                         Diretorio.Sistema + "','" +
                         Diretorio.Caminho + "')";

            try
            {
                OleDbComm = new OleDbCommand(str, ConnectionPooling.GetConnectionOleDb());
                return (OleDbComm.ExecuteNonQuery() == 1);
            }
            finally
            {
                if (OleDbComm != null)
                    OleDbComm.Dispose();
            };

        }
        static public Boolean Update_Diretorio(TDiretorio Diretorio)
        {
            OleDbCommand OleDbComm;
            string str;
            OleDbComm = null;
            str = "UPDATE DIRETORI SET " +
                 " CAMINHO ='" + Diretorio.Caminho + "'" +
                " WHERE SISTEMA = '" + Diretorio.Sistema + "'";
            try
            {
                OleDbComm = new OleDbCommand(str, ConnectionPooling.GetConnectionOleDb());
                return (OleDbComm.ExecuteNonQuery() == 1);
            }
            finally
            {
                if (OleDbComm != null)
                    OleDbComm.Dispose();
            };

        }
        static public OleDbDataReader Get_Diretorios(string Diretorio)
        {

            OleDbCommand OleDbComm;
            OleDbDataReader OleDbRead;
            string strOleDb;
            if (Diretorio == "")
            { strOleDb = "SELECT SISTEMA,CAMINHO FROM DIRETORI"; }
            else
                strOleDb = "SELECT SISTEMA,CAMINHO FROM DIRETORI WHERE SISTEMA LIKE '" + Diretorio + "%'";

            OleDbComm = new OleDbCommand(strOleDb, ConnectionPooling.GetConnectionOleDb());

            try
            {

                OleDbRead = OleDbComm.ExecuteReader();
                return OleDbRead;
            }
            finally
            {
                OleDbComm.Dispose();

            }
        }

        static public string FormatDataGravar(DateTime odatetime)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            string result = odatetime.ToString("d", ci);
            result = "'" + result + "'";
            return result;
        }
        static public string DataVazia()
        {
            return "'  /  /  '";//result;
        }

        static public string FormatDataGravarExtenso(DateTime odatetime)
        {
            string result = odatetime.ToString("yyyyMMdd");//, ci);
            return result;
        }
        static public string FormatNumero(Single valor, int total)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            // ci.DateTimeFormat.;
            string result = valor.ToString("F02", ci);
            while (result.Length < total)
                result = "0" + result;
            return result;
        }
        static public string FormatNumero(int valor, int total)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            // ci.DateTimeFormat.;
            string result = valor.ToString().Trim();
            while (result.Length < total)
                result = "0" + result;
            return result;
        }


        static public string preenchaespaco(string campo, int total)
        {
            string resultado = "";
            resultado = campo;
            while (resultado.Length < total)
                resultado += " ";
            if (resultado.Length > total)
                resultado = resultado.Substring(0, total);
            return resultado;

        }

        static public string zeroaesquerda(string campo, int total)
        {
            string resultado = "";
            resultado = campo.Trim();
            while (resultado.Length < total)
                resultado = "0" + resultado;
            //if (resultado.Length > total)
            //   resultado = resultado.Substring(0, total);
            return resultado;

        }



        static public string SoNumerico(string campo)
        {
            string resultado = "";
            for (int i = 0; i < campo.Length; i++)
            {
                //char ochar = Convert.ToChar(campo.Substring(i,1));
                if (campo.Substring(i, 1) == "-") continue;
                // if (Char.IsDigit(campo,i))
                resultado += campo.Substring(i, 1);
            }

            return resultado;

        }

        static public string Reformate(string campo)
        {
            string resultado = "";
            for (int i = 0; i < campo.Length; i++)
            {
                //char ochar = Convert.ToChar(campo.Substring(i,1));
                if (campo.Substring(i, 1) == ".")
                {
                    resultado += ",";
                    continue;
                }
                if (campo.Substring(i, 1) == ",")
                {
                    resultado += ".";
                    continue;
                }

                resultado += campo.Substring(i, 1);
            }

            return resultado;

        }




        static public DateTime UltimoDiaMes(DateTime odata)
        {
            int mes = odata.Month;
            while (odata.Month == mes)
            {
                odata = odata.AddDays(1);
            }
            return odata.AddDays(-1);
        }

        static public DateTime PrimeiroDiaMes(DateTime odata)
        {
            int decr = odata.Day - 1;
            return odata.AddDays(-decr);
        }



        /*
        Está em MiscelaniaS
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
        //Classes de Apoio na Construção de Aplicativos 
        */
        public static DataColumn Coluna(Type otipo, string onome, int omaxlength, Boolean oreadonly)
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

   
        static public string Get_Path(string Sistema)
        {
            string result = string.Empty;
            result = ListaCaminhos.GetPath(Sistema) +"\\";
            if (result != "") return result;
            OleDbCommand OleDbcomm;
            OleDbDataReader OleDbread;
            string strOleDb;

            if (Sistema == string.Empty)
                return string.Empty;
            strOleDb = "SELECT SISTEMA,CAMINHO FROM DIRETORI WHERE SISTEMA LIKE '" + Sistema.Trim().ToUpper() + "%'";
            OleDbcomm = new OleDbCommand(strOleDb, ConnectionPooling.GetConnectionOleDb());
            OleDbread = OleDbcomm.ExecuteReader();
            try
            {
                while (OleDbread.Read() == true)
                    result = OleDbread["CAMINHO"].ToString().Trim();

                OleDbread.Close();

            }
            finally
            {
                OleDbread.Close();
                OleDbcomm.Dispose();

            }
            return result;
        }
        static public string ExcelNomePlanilha(string Path_Excel)
        {
            
             Microsoft.Office.Interop.Excel.Application ExcelObj = new Microsoft.Office.Interop.Excel.Application();

            try
            {

                if (ExcelObj == null)
                {
                    MessageBox.Show("ERROR: EXCEL não pôde ser iniciado!");
                    return "";
                }
                // ExcelObj.Visible = true;                                          // read only
                Microsoft.Office.Interop.Excel.Workbook oWorkbook = ExcelObj.Workbooks.Open(Path_Excel, 0, false, 5,
                  "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t",
                     true, true, 0, true, 0, 0);

                try
                {
                    Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;

                    string nome = ((Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1)).Name;
                    oWorkbook.Close(false, Path_Excel, null);
                    ExcelObj.Quit();
                    return nome;
                }
                catch (Exception)
                {
                    throw;
                }

            }
            catch (Exception)
            {
                throw;
            }
           
        }
        public static DataTable LeiaExcel(string path_excel, string nomeplanilha, string query = "")
        {
            string strOleDb, connectionstring;
            OleDbCommand OleDbcomm;
            OleDbDataAdapter OleDbda;
            DataSet odataset;
            OleDbConnection excelOleDbconnection;
            DataTable result = null;
            try
            {
                if (query != "") { strOleDb = query; }
                else
                {
                    strOleDb = " select * " +
                  " from " + "`" + nomeplanilha + "$`";
                }
                // ATENÇÃO É NECESSARIO QUE NA MAQUINA ESTEJA INSTALADO O
               //  AccessDatabaseEngine que pode ser o de 32bits versão 2010

                connectionstring = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = "+ path_excel + "; Extended Properties = 'Excel 12.0 Xml;HDR=YES'";

                //Driver={Microsoft Text Driver(*.txt; *.csv)};DBQ= ;//"Driver={Microsoft Text Driver(*.txt; *.csv)};DQB=c:\teste"
                excelOleDbconnection = new OleDbConnection(connectionstring);
                excelOleDbconnection.Open();
                OleDbcomm = new OleDbCommand(strOleDb, excelOleDbconnection);



                OleDbda = new OleDbDataAdapter(OleDbcomm);


                OleDbda.TableMappings.Add("Table", nomeplanilha + "$");
                odataset = new DataSet();
                OleDbda.Fill(odataset);

                result = odataset.Tables[nomeplanilha + "$"];

                // verifica se existe todos os campos requeridos


                excelOleDbconnection.Close();
                return result;
            }
            catch (Exception E)
            {
                throw new Exception("Erro leitura Excel");
            }

        }

        // construindo delegates....
        // Delegate que construi como pedaço de código SQL
        /* Está EM Miiscelania (ClassFiltroEdite
         * static public string CompareValor(LinhaSolucao oLinha)
        {
            string result = "";
            if ((oLinha.dado[0] == "") || (oLinha.dado[1] == "") || (oLinha.campo.Trim() == ""))
                return result;
            string valor = "";
            try
            {
                double ovalor = Convert.ToDouble(oLinha.dado[1].Trim());
                valor = TDataControlReduzido.FormatDoubleGravar(ovalor); //2DECIMAIS
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
        */



        static public List<string> StringparaLista(string campo)
        {
            List<string> ostring = new List<string>();

            campo.Trim();
            int i = campo.IndexOf("/");
            while (i > 0)
            {
                ostring.Add(campo.Substring(0, i));
                campo = campo.Remove(0, i + 1);
                i = campo.IndexOf("/");
            }
            if (campo.Length > 0)
                ostring.Add(campo);

            return ostring;
        }

        static public string ConstruaSql(string campo, List<string> dados)
        {
            string result = "";
            List<string> vetdados = new List<string>();
            List<string> vetcampos = StringparaLista(campo);
            for (int i = 0; i < dados.Count; i++)
            {
                List<string> vetprov = StringparaLista(dados[i]);
                for (int j = 0; j < vetprov.Count; j++)
                { vetdados.Add(vetprov[j]); }
            }

            for (int i = 0; i < vetcampos.Count; i++)
            {
                for (int j = 0; j < vetdados.Count; j++)
                {
                    if (result != "") result += " OR ";
                    result += "(" + vetcampos[i] + " LIKE " + "'%" + vetdados[j] + "%')";
                }
            }
            if (result.Length > 0) result = "  AND (" + result + ")";
            return result;
        }



        static public int AlteraTabela(string diretorio, string tabela, string campo, string tipo)
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = Get_Path(diretorio);
            strOleDb = "ALTER TABLE " + path + tabela + " ADD " + campo + " " + tipo;// "PLACON ADD NOVOCOD CHAR(30)";
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, ConnectionPooling.GetConnectionOleDb());
                OleDbcomm.Connection.Close();
                OleDbcomm.Connection.Open();

                int num = OleDbcomm.ExecuteNonQuery();
                OleDbcomm.Connection.Close();

                return num;

            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + tabela));
            }
        }


     
    }
    public class DateDifference
    {
        /// <summary>
        /// defining Number of days in month; index 0=> january and 11=> December
        /// february contain either 28 or 29 days, that's why here value is -1
        /// which wil be calculate later.
        /// </summary>
        private int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// contain from date
        /// </summary>
        private DateTime fromDate;

        /// <summary>
        /// contain To Date
        /// </summary>
        private DateTime toDate;

        /// <summary>
        /// this three variable for output representation..
        /// </summary>
        private int year;
        private int month;
        private int day;

        public DateDifference(DateTime d1, DateTime d2)
        {
            int increment;

            if (d1 > d2)
            {
                this.fromDate = d2;
                this.toDate = d1;
            }
            else
            {
                this.fromDate = d1;
                this.toDate = d2;
            }

            /// 
            /// Day Calculation
            /// 
            increment = 0;

            if (this.fromDate.Day > this.toDate.Day)
            {
                increment = this.monthDay[this.fromDate.Month - 1];

            }
            /// if it is february month
            /// if it's to day is less then from day
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(this.fromDate.Year))
                {
                    // leap year february contain 29 days
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }
            if (increment != 0)
            {
                day = (this.toDate.Day + increment) - this.fromDate.Day;
                increment = 1;
            }
            else
            {
                day = this.toDate.Day - this.fromDate.Day;
            }

            ///
            ///month calculation
            ///
            if ((this.fromDate.Month + increment) > this.toDate.Month)
            {
                this.month = (this.toDate.Month + 12) - (this.fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                this.month = (this.toDate.Month) - (this.fromDate.Month + increment);
                increment = 0;
            }

            ///
            /// year calculation
            ///
            this.year = this.toDate.Year - (this.fromDate.Year + increment);

        }

        public override string ToString()
        {
            //return base.ToString();
            return this.year + " Year(s), " + this.month + " month(s), " + this.day + " day(s)";
        }

        public int Years
        {
            get
            {
                return this.year;
            }
        }

        public int Months
        {
            get
            {
                return this.month;
            }
        }

        public int Days
        {
            get
            {
                return this.day;
            }
        }

    }

}
