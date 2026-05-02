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


namespace ClassLibTrabalho//WFAEstatiscaTrabalho
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

    public class TDataControlReduzido
    {
        static public TConnectionPooling ConnectionPooling = new TConnectionPooling();
        static public Boolean Add_Diretorio(TDiretorio Diretorio)
        {
            OdbcCommand OdbcComm;
            string str;
            OdbcComm = null;
            str = "INSERT INTO DIRETORI (SISTEMA,CAMINHO) VALUES('" +
                         Diretorio.Sistema + "','" +
                         Diretorio.Caminho + "')";

            try
            {
                OdbcComm = new OdbcCommand(str, ConnectionPooling.GetConnection());
                return (OdbcComm.ExecuteNonQuery() == 1);
            }
            finally
            {
                if (OdbcComm != null)
                    OdbcComm.Dispose();
            };

        }
        static public Boolean Update_Diretorio(TDiretorio Diretorio)
        {
            OdbcCommand OdbcComm;
            string str;
            OdbcComm = null;
            str = "UPDATE DIRETORI SET " +
                 " CAMINHO ='" + Diretorio.Caminho + "'" +
                " WHERE SISTEMA = '" + Diretorio.Sistema + "'";
            try
            {
                OdbcComm = new OdbcCommand(str, ConnectionPooling.GetConnection());
                return (OdbcComm.ExecuteNonQuery() == 1);
            }
            finally
            {
                if (OdbcComm != null)
                    OdbcComm.Dispose();
            };

        }
        static public OdbcDataReader Get_Diretorios(string Diretorio)
        {

            OdbcCommand ODBCComm;
            OdbcDataReader ODBCRead;
            string strODBC;
            if (Diretorio == "")
            { strODBC = "SELECT SISTEMA,CAMINHO FROM DIRETORI"; }
            else
                strODBC = "SELECT SISTEMA,CAMINHO FROM DIRETORI WHERE SISTEMA LIKE '" + Diretorio + "%'";

            ODBCComm = new OdbcCommand(strODBC, ConnectionPooling.GetConnection());

            try
            {

                ODBCRead = ODBCComm.ExecuteReader();
                return ODBCRead;
            }
            finally
            {
                ODBCComm.Dispose();

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





        static string FormatDoubleGravar(double ovalor)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            string result = ovalor.ToString("N2", ci);
            return result;
        }
        static string FormatDoubleGravar(double ovalor, int num)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            string result = ovalor.ToString("N" + num.ToString().Trim(), ci);
            return result;
        }
    

   
        static public string Get_Path(string Sistema)
        {

            OdbcCommand odbccomm;
            OdbcDataReader odbcread;
            string strodbc, result = string.Empty;

            if (Sistema == string.Empty)
                return string.Empty;
            strodbc = "SELECT SISTEMA,CAMINHO FROM DIRETORI WHERE SISTEMA LIKE '" + Sistema.Trim().ToUpper() + "%'";
            odbccomm = new OdbcCommand(strodbc, ConnectionPooling.GetConnection());
            odbcread = odbccomm.ExecuteReader();
            try
            {


                while (odbcread.Read() == true)
                    result = odbcread["CAMINHO"].ToString().Trim();

                odbcread.Close();

            }
            finally
            {
                odbcread.Close();
                odbccomm.Dispose();

            }
            return result;
        }
        // ExcelNomePlanilha e LeiaExcel removidos: nao tinham nenhum caller (todos os
        // consumidores chamam ClassConexao.TDataControlReduzido). A versao em ClassConexao
        // ja foi migrada para nao depender de Excel Interop.

        static public int AlteraTabela(string diretorio, string tabela, string campo, string tipo)
        {
            OdbcCommand odbccomm;
            string strodbc, path;
            path = Get_Path(diretorio);
            strodbc = "ALTER TABLE " + path + tabela + " ADD " + campo + " " + tipo;// "PLACON ADD NOVOCOD CHAR(30)";
            try
            {
                odbccomm = new OdbcCommand(strodbc, ConnectionPooling.GetConnection());
                odbccomm.Connection.Close();
                odbccomm.Connection.Open();

                int num = odbccomm.ExecuteNonQuery();
                odbccomm.Connection.Close();

                return num;

            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + tabela));
            }
        }


     
    }
}
