using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using ClassConexao;
namespace ClassEstoque
{
    class Classcadest
    {
        static public DataTable dtcadest()
        {
            DataTable otable = new DataTable("cadest");
            otable.Columns.Add("COD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DESC", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 40;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("FORN", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 40;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("UNID", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("QUANT", System.Type.GetType("System.Int32"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("PMEDIO", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("PUNIT", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("DATA_CAD", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("DATA", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("VALOR", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("TP_MAT", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            return otable;
        }
        static public Boolean SQLCreateTablecadest()
        {
            string stroledb = " CREATE TABLE cadest(" +
            " COD C(4), " +
            " DESC C(40), " +
            " FORN C(40), " +
            " UNID C(3), " +
            " QUANT I, " +
            " PMEDIO N(11,4), " +
            " PUNIT N(11,2), " +
            " VALOR N(12,2), " +
            " TP_MAT C(2) )";
            return true;
        }
        static public OleDbDataAdapter cadestConstruaAdaptador(string path)
        {
            OleDbDataAdapter OleDbda = new OleDbDataAdapter();
            path = TDataControlReduzido.Get_Path(path);
            //construçao do //select
            OleDbCommand cmd = new OleDbCommand(
            "SELECT  " +   " COD,  DESC,  FORN,  UNID,  QUANT,  PMEDIO,  PUNIT, " +
            " DATA_CAD,  DATA,  VALOR,  TP_MAT FROM " +path + "cadest " +
            " WHERE  ( DATA BETWEEN ? AND ? )  ORDER BY COD", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("inicio", OleDbType.DBDate);
            cmd.Parameters.Add("fim", OleDbType.DBDate);
            OleDbda.SelectCommand = cmd;
            
            
            
            // Construção do insert padrao 

             cmd = new OleDbCommand(
            "INSERT INTO " + path + "cadest ( " +
            " COD,  `DESC`,  FORN,  UNID,  QUANT,  PMEDIO,  PUNIT, " +
            " DATA_CAD,  DATA,  VALOR,  TP_MAT ) VALUES (  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());


            cmd.Parameters.Add("@COD", OleDbType.Char, 4, "COD");
            cmd.Parameters.Add("@DESC", OleDbType.Char, 40, "DESC");
            cmd.Parameters.Add("@FORN", OleDbType.Char, 40, "FORN");
            cmd.Parameters.Add("@UNID", OleDbType.Char, 3, "UNID");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@PMEDIO", OleDbType.Numeric, 0, "PMEDIO");
            cmd.Parameters.Add("@PUNIT", OleDbType.Numeric, 0, "PUNIT");
            cmd.Parameters.Add("@DATA_CAD", OleDbType.DBDate, 0, "DATA_CAD");
            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@VALOR", OleDbType.Numeric, 0, "VALOR");
            cmd.Parameters.Add("@TP_MAT", OleDbType.Char, 2, "TP_MAT");
            OleDbda.InsertCommand = cmd;



            // Construção do altera padrao 

            cmd = new OleDbCommand(
           "UPDATE " + path + "cadest SET " +
           "  COD = ? , `DESC` = ? , FORN = ? ,  UNID = ? , " +
           " QUANT = ? ,  PMEDIO = ? ,  PUNIT = ? ,  DATA_CAD = ? , " +
           " DATA = ? ,  VALOR = ? ,  TP_MAT = ?  WHERE ( COD = ? )" +
           "", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@COD2", OleDbType.Char, 40, "COD");
            cmd.Parameters.Add("@DESC", OleDbType.Char, 40, "DESC");
            cmd.Parameters.Add("@FORN", OleDbType.Char, 40, "FORN");
            cmd.Parameters.Add("@UNID", OleDbType.Char, 3, "UNID");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@PMEDIO", OleDbType.Numeric, 0, "PMEDIO");
            cmd.Parameters.Add("@PUNIT", OleDbType.Numeric, 0, "PUNIT");
            cmd.Parameters.Add("@DATA_CAD", OleDbType.DBDate, 0, "DATA_CAD");
            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@VALOR", OleDbType.Numeric, 0, "VALOR");
            cmd.Parameters.Add("@TP_MAT", OleDbType.Char, 2, "TP_MAT");
            cmd.Parameters.Add("@COD", OleDbType.Char,4 , "COD").SourceVersion = DataRowVersion.Original;
            OleDbda.UpdateCommand = cmd;

            // Construção do deleta padrao 

            cmd = new OleDbCommand(
           " DELETE FROM " + path + "cadest" +
           " WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;
            OleDbda.DeleteCommand = cmd;
            return OleDbda;
        }
    }
}

