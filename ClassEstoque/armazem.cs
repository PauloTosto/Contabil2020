using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using ClassConexao;

namespace ClassEstoque
{
    class Classarmazem
    {

        static public DataTable armazemselect()
        
        { 
       
          OleDbCommand OleDbcomm;
             string strOleDb, path;
             
             path = TDataControlReduzido.Get_Path("ESTOQUE");
                strOleDb = "SELECT        cod, nome_dep, saldo, desc2, contab "+
                 " FROM "+ path + "ARMAZEM order by cod";
             
             try
             {
                 OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                 OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);
                 OleDbda.TableMappings.Add("Table", "ARMAZEM");
                 DataSet result = new DataSet();
                 OleDbda.Fill(result);
                 OleDbda.Dispose();
                 return result.Tables[0];
         

             }
             catch (Exception)
             {
                 throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
             }
         

        
        }
        
        
        static public DataTable dtarmazem()
        {
            DataTable otable = new DataTable("armazem");
            otable.Columns.Add("COD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("NOME_DEP", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 30;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("SALDO", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("DESC2", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 25;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CONTAB", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 8;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            return otable;
        }
        static public Boolean SQLCreateTablearmazem()
        {
            string stroledb = " CREATE TABLE armazem(" +
            " COD C(2), " +
            " NOME_DEP C(30), " +
            " SALDO N(11,2), " +
            " DESC2 C(25), " +
            " CONTAB C(8) )";
            return true;
        }
        static public OleDbDataAdapter armazemConstruaAdaptador(string path)
        {
            OleDbDataAdapter OleDbda = new OleDbDataAdapter();

            // Construção do insert padrao 

            OleDbCommand cmd = new OleDbCommand(
            "INSERT INTO " + path + "armazem ( " +
            " COD,  NOME_DEP,  SALDO,  DESC2,  CONTAB ) VALUES (  ?,  ?, " +
            " ?,  ?,  ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());


            cmd.Parameters.Add("@COD", OleDbType.Char, 2, "COD");
            cmd.Parameters.Add("@NOME_DEP", OleDbType.Char, 30, "NOME_DEP");
            cmd.Parameters.Add("@SALDO", OleDbType.Numeric, 0, "SALDO");
            cmd.Parameters.Add("@DESC2", OleDbType.Char, 25, "DESC2");
            cmd.Parameters.Add("@CONTAB", OleDbType.Char, 8, "CONTAB");
            OleDbda.InsertCommand = cmd;



            // Construção do altera padrao 

            cmd = new OleDbCommand(
           "UPDATE " + path + "armazem SET " +
           " COD = ? ,  NOME_DEP = ? ,  SALDO = ? ,  DESC2 = ? , " +
           " CONTAB = ?  WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@COD", OleDbType.Char, 2, "COD");
            cmd.Parameters.Add("@NOME_DEP", OleDbType.Char, 30, "NOME_DEP");
            cmd.Parameters.Add("@SALDO", OleDbType.Numeric, 0, "SALDO");
            cmd.Parameters.Add("@DESC2", OleDbType.Char, 25, "DESC2");
            cmd.Parameters.Add("@CONTAB", OleDbType.Char, 8, "CONTAB");
            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;
            OleDbda.UpdateCommand = cmd;

            // Construção do deleta padrao 

            cmd = new OleDbCommand(
           " DELETE FROM " + path + "armazem" +
           " WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;
            OleDbda.DeleteCommand = cmd;
            return OleDbda;
        }
    }
}

