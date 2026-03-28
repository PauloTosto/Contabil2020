using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using ClassConexao;


namespace ClassLibTrabalho
{
    public class ClassCLTADIAN
    {
        static public DataTable dtCLTADIAN()
        {
            DataTable otable = new DataTable("CLTADIAN");
            otable.Columns.Add("DATA", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("SETOR", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CENTRO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("QUADRA", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("TRAB", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("TIPO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("NUM_MOD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CODSER", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("QUANT", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("ADIANT", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("VLR_HXS", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("VLR_HXA", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("VLR_HXN", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("FGTS", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("SALFAM", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("INSS", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("HX", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("HXS", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("HXA", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("HXN", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("HEFETIVA", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("PONTO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns[otable.Columns.Count - 1].MaxLength = 62;
            return otable;
        }
        static public OleDbDataAdapter CLTADIANConstruaAdaptador(string opath)
        {
            OleDbDataAdapter OleDbda = new OleDbDataAdapter();

            // Construção do insert padrao 
            string path = TDataControlReduzido.Get_Path(opath);

            OleDbCommand cmd = new OleDbCommand(
            "INSERT INTO " + path + "CLTADIAN ( " +
            " DATA,  SETOR,  CENTRO,  QUADRA,  TRAB,  TIPO, " +
            " NUM_MOD,  CODSER,  QUANT,  ADIANT,  VLR_HXS, " +
            " VLR_HXA,  VLR_HXN,  FGTS,  SALFAM,  INSS,  HX, " +
            " HXS,  HXA,  HXN,  HEFETIVA,  PONTO ) VALUES (  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());


            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@CENTRO", OleDbType.Char, 4, "CENTRO");
            cmd.Parameters.Add("@QUADRA", OleDbType.Char, 3, "QUADRA");
            cmd.Parameters.Add("@TRAB", OleDbType.Char, 4, "TRAB");
            cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@ADIANT", OleDbType.Numeric, 0, "ADIANT");
            cmd.Parameters.Add("@VLR_HXS", OleDbType.Numeric, 0, "VLR_HXS");
            cmd.Parameters.Add("@VLR_HXA", OleDbType.Numeric, 0, "VLR_HXA");
            cmd.Parameters.Add("@VLR_HXN", OleDbType.Numeric, 0, "VLR_HXN");
            cmd.Parameters.Add("@FGTS", OleDbType.Numeric, 0, "FGTS");
            cmd.Parameters.Add("@SALFAM", OleDbType.Numeric, 0, "SALFAM");
            cmd.Parameters.Add("@INSS", OleDbType.Numeric, 0, "INSS");
            cmd.Parameters.Add("@HX", OleDbType.Numeric, 0, "HX");
            cmd.Parameters.Add("@HXS", OleDbType.Numeric, 0, "HXS");
            cmd.Parameters.Add("@HXA", OleDbType.Numeric, 0, "HXA");
            cmd.Parameters.Add("@HXN", OleDbType.Numeric, 0, "HXN");
            cmd.Parameters.Add("@HEFETIVA", OleDbType.Numeric, 0, "HEFETIVA");
            cmd.Parameters.Add("@PONTO", OleDbType.Char, 62, "PONTO");
            OleDbda.InsertCommand = cmd;



            // Construção do altera padrao 

            cmd = new OleDbCommand(
           "UPDATE " + path + "CLTADIAN SET " +
           " DATA = ? ,  SETOR = ? ,  CENTRO = ? ,  QUADRA = ? , " +
           " TRAB = ? ,  TIPO = ? ,  NUM_MOD = ? ,  CODSER = ? , " +
           " QUANT = ? ,  ADIANT = ? ,  VLR_HXS = ? ,  VLR_HXA = ? , " +
           " VLR_HXN = ? ,  FGTS = ? ,  SALFAM = ? ,  INSS = ? , " +
           " HX = ? ,  HXS = ? ,  HXA = ? ,  HXN = ? ,  HEFETIVA = ? , " +
           " PONTO = ?  WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@CENTRO", OleDbType.Char, 4, "CENTRO");
            cmd.Parameters.Add("@QUADRA", OleDbType.Char, 3, "QUADRA");
            cmd.Parameters.Add("@TRAB", OleDbType.Char, 4, "TRAB");
            cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@ADIANT", OleDbType.Numeric, 0, "ADIANT");
            cmd.Parameters.Add("@VLR_HXS", OleDbType.Numeric, 0, "VLR_HXS");
            cmd.Parameters.Add("@VLR_HXA", OleDbType.Numeric, 0, "VLR_HXA");
            cmd.Parameters.Add("@VLR_HXN", OleDbType.Numeric, 0, "VLR_HXN");
            cmd.Parameters.Add("@FGTS", OleDbType.Numeric, 0, "FGTS");
            cmd.Parameters.Add("@SALFAM", OleDbType.Numeric, 0, "SALFAM");
            cmd.Parameters.Add("@INSS", OleDbType.Numeric, 0, "INSS");
            cmd.Parameters.Add("@HX", OleDbType.Numeric, 0, "HX");
            cmd.Parameters.Add("@HXS", OleDbType.Numeric, 0, "HXS");
            cmd.Parameters.Add("@HXA", OleDbType.Numeric, 0, "HXA");
            cmd.Parameters.Add("@HXN", OleDbType.Numeric, 0, "HXN");
            cmd.Parameters.Add("@HEFETIVA", OleDbType.Numeric, 0, "HEFETIVA");
            cmd.Parameters.Add("@PONTO", OleDbType.Char, 62, "PONTO");
            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;
            OleDbda.UpdateCommand = cmd;

            // Construção do deleta padrao 

           /* cmd = new OleDbCommand(
           " DELETE FROM " + path + " CLTADIAN" +
           " WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;
            OleDbda.DeleteCommand = cmd;*/
            cmd = new OleDbCommand(
           "DELETE FROM " + path + "CLTADIAN WHERE (DATA  BETWEEN ? AND ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@INICIO", OleDbType.DBDate, 0);
            cmd.Parameters.Add("@FIM", OleDbType.DBDate, 0);
            OleDbda.DeleteCommand = cmd;
           
            
            return OleDbda;
        }

    }
}

