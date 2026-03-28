using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using ClassConexao;

namespace ClassEstoque
{
    class Classmovest
    {
        static public DataTable dtmovest()
        {
            DataTable otable = new DataTable("movest");
            otable.Columns.Add("DATA", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("COD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("TIPO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("QUANT", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("PUNIT", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("DEPOSITO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("FORN", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 25;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("SETOR", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("GLEBA", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("QUADRA", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("VALOR", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("DOC", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 15;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DATAC", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("TIPO2", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CODSER", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("TIPO_CAT", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("PROD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("NUM_MOD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CENTRO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 8;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("OBS", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 25;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("MOV_ID", System.Type.GetType("System.Int32"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("QUANT_PA", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("QUANT_FR", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("VLR_PA", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("VLR_FR", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("MUDA_INT", System.Type.GetType("System.Boolean"));
            otable.Columns.Add("NUMERO", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            return otable;
        }
        static public Boolean SQLCreateTablemovest()
        {
            string stroledb = " CREATE TABLE movest(" +
            " COD C(4), " +
            " TIPO C(1), " +
            " QUANT N(19,4), " +
            " PUNIT N(19,4), " +
            " DEPOSITO C(2), " +
            " FORN C(25), " +
            " SETOR C(2), " +
            " GLEBA C(4), " +
            " QUADRA C(3), " +
            " VALOR N(19,4), " +
            " DOC C(15), " +
            " TIPO2 C(1), " +
            " CODSER C(3), " +
            " TIPO_CAT C(1), " +
            " PROD C(3), " +
            " NUM_MOD C(2), " +
            " CENTRO C(8), " +
            " OBS C(25), " +
            " MOV_ID I, " +
            " QUANT_PA N(19,4), " +
            " QUANT_FR N(19,4), " +
            " VLR_PA N(19,4), " +
            " VLR_FR N(19,4), " +
            " MUDA_INT L, " +
            " NUMERO N(19,4) )";
            return true;
        }
        static public OleDbDataAdapter movestConstruaAdaptador(string path)
        {
            OleDbDataAdapter OleDbda = new OleDbDataAdapter();

            
            path = TDataControlReduzido.Get_Path(path);

            OleDbCommand cmd = new OleDbCommand(
                "SELECT  DATA,  COD,  TIPO,  QUANT,  PUNIT,  DEPOSITO, " +
           " FORN,  SETOR,  GLEBA,  QUADRA,  VALOR,  DOC, " +
           " DATAC,  TIPO2,  CODSER,  TIPO_CAT,  PROD,  NUM_MOD, " +
           " CENTRO,  OBS,  MOV_ID,  QUANT_PA,  QUANT_FR, " +
           " VLR_PA,  VLR_FR,  MUDA_INT,  NUMERO "+
           "FROM " + path + "movest " +
            "WHERE DATA BETWEEN ? AND ? "
            , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("inicio", OleDbType.DBDate);
            cmd.Parameters.Add("fim", OleDbType.DBDate);
            OleDbda.SelectCommand = cmd;

            // Construção do insert padrao 
             cmd = new OleDbCommand(
            "INSERT INTO " + path + "movest ( " +
            " DATA,  COD,  TIPO,  QUANT,  PUNIT,  DEPOSITO, " +
            " FORN,  SETOR,  GLEBA,  QUADRA,  VALOR,  DOC, " +
            " DATAC,  TIPO2,  CODSER,  TIPO_CAT,  PROD,  NUM_MOD, " +
            " CENTRO,  OBS,  MOV_ID,  QUANT_PA,  QUANT_FR, " +
            " VLR_PA,  VLR_FR,  MUDA_INT,  NUMERO ) VALUES (  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?" +
            " )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());


            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@COD", OleDbType.Char, 4, "COD");
            cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@PUNIT", OleDbType.Numeric, 0, "PUNIT");
            cmd.Parameters.Add("@DEPOSITO", OleDbType.Char, 2, "DEPOSITO");
            cmd.Parameters.Add("@FORN", OleDbType.Char, 25, "FORN");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@GLEBA", OleDbType.Char, 4, "GLEBA");
            cmd.Parameters.Add("@QUADRA", OleDbType.Char, 3, "QUADRA");
            cmd.Parameters.Add("@VALOR", OleDbType.Numeric, 0, "VALOR");
            cmd.Parameters.Add("@DOC", OleDbType.Char, 15, "DOC");
            cmd.Parameters.Add("@DATAC", OleDbType.DBDate, 0, "DATAC");
            cmd.Parameters.Add("@TIPO2", OleDbType.Char, 1, "TIPO2");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@TIPO_CAT", OleDbType.Char, 1, "TIPO_CAT");
            cmd.Parameters.Add("@PROD", OleDbType.Char, 3, "PROD");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CENTRO", OleDbType.Char, 8, "CENTRO");
            cmd.Parameters.Add("@OBS", OleDbType.Char, 25, "OBS");
            cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 0, "MOV_ID");
            cmd.Parameters.Add("@QUANT_PA", OleDbType.Numeric, 0, "QUANT_PA");
            cmd.Parameters.Add("@QUANT_FR", OleDbType.Numeric, 0, "QUANT_FR");
            cmd.Parameters.Add("@VLR_PA", OleDbType.Numeric, 0, "VLR_PA");
            cmd.Parameters.Add("@VLR_FR", OleDbType.Numeric, 0, "VLR_FR");
            cmd.Parameters.Add("@MUDA_INT", OleDbType.Boolean, 2, "MUDA_INT");
            cmd.Parameters.Add("@NUMERO", OleDbType.Numeric, 0, "NUMERO");
            OleDbda.InsertCommand = cmd;



            // Construção do altera padrao 

            cmd = new OleDbCommand(
           "UPDATE " + path + "movest SET " +
           " DATA = ? ,  COD = ? ,  TIPO = ? ,  QUANT = ? , " +
           " PUNIT = ? ,  DEPOSITO = ? ,  FORN = ? ,  SETOR = ? , " +
           " GLEBA = ? ,  QUADRA = ? ,  VALOR = ? ,  DOC = ? , " +
           " DATAC = ? ,  TIPO2 = ? ,  CODSER = ? ,  TIPO_CAT = ? , " +
           " PROD = ? ,  NUM_MOD = ? ,  CENTRO = ? ,  OBS = ? , " +
           " MOV_ID = ? ,  QUANT_PA = ? ,  QUANT_FR = ? , " +
           " VLR_PA = ? ,  VLR_FR = ? ,  MUDA_INT = ?  " +
           " WHERE ( NUMERO = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@COD", OleDbType.Char, 4, "COD");
            cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@PUNIT", OleDbType.Numeric, 0, "PUNIT");
            cmd.Parameters.Add("@DEPOSITO", OleDbType.Char, 2, "DEPOSITO");
            cmd.Parameters.Add("@FORN", OleDbType.Char, 25, "FORN");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@GLEBA", OleDbType.Char, 4, "GLEBA");
            cmd.Parameters.Add("@QUADRA", OleDbType.Char, 3, "QUADRA");
            cmd.Parameters.Add("@VALOR", OleDbType.Numeric, 0, "VALOR");
            cmd.Parameters.Add("@DOC", OleDbType.Char, 15, "DOC");
            cmd.Parameters.Add("@DATAC", OleDbType.DBDate, 0, "DATAC");
            cmd.Parameters.Add("@TIPO2", OleDbType.Char, 1, "TIPO2");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@TIPO_CAT", OleDbType.Char, 1, "TIPO_CAT");
            cmd.Parameters.Add("@PROD", OleDbType.Char, 3, "PROD");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CENTRO", OleDbType.Char, 8, "CENTRO");
            cmd.Parameters.Add("@OBS", OleDbType.Char, 25, "OBS");
            cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 0, "MOV_ID");
            cmd.Parameters.Add("@QUANT_PA", OleDbType.Numeric, 0, "QUANT_PA");
            cmd.Parameters.Add("@QUANT_FR", OleDbType.Numeric, 0, "QUANT_FR");
            cmd.Parameters.Add("@VLR_PA", OleDbType.Numeric, 0, "VLR_PA");
            cmd.Parameters.Add("@VLR_FR", OleDbType.Numeric, 0, "VLR_FR");
            cmd.Parameters.Add("@MUDA_INT", OleDbType.Boolean, 2, "MUDA_INT");
            cmd.Parameters.Add("@NUMERO", OleDbType.Numeric, 0, "NUMERO").SourceVersion = DataRowVersion.Original; 
            
            OleDbda.UpdateCommand = cmd;

            // Construção do deleta padrao 

            cmd = new OleDbCommand(
           " DELETE FROM " + path + "movest" +
           " WHERE ( NUMERO = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("@NUMERO", OleDbType.Numeric, 10, "NUMERO").SourceVersion = DataRowVersion.Original;
            OleDbda.DeleteCommand = cmd;
            return OleDbda;
        }
    }
}

