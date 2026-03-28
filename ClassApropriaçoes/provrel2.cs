using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO; // novo
using ClassConexao; //novo

namespace ClassApropriaçoes
{
    class Classprovrel3
    {
        static public DataTable dtprovrel3()
        {
            DataTable otable = new DataTable("provrel3");
            otable.Columns.Add("DBCR", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("ORIGEM", System.Type.GetType("System.Int16"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("SAIDA", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("ENTRADA", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("SALDO", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("QUANT", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("ENCARGOS", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("NEGOCIO", System.Type.GetType("System.Int16"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("SETOR", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CENTRO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
          
            otable.Columns.Add("QUADRA", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
           
            otable.Columns.Add("NUM_MOD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CODSER", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CULT", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("PROD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("NATUREZA", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 13;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("ICODSER", System.Type.GetType("System.Int16"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("AREA", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("PROD_ANT1", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("PARC_ANT1", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("AREA_ANT1", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("PROD_ANT2", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("PARC_ANT2", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("AREA_ANT2", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;
            otable.Columns.Add("FINO", System.Type.GetType("System.Int16"));
             otable.Columns[otable.Columns.Count - 1].DefaultValue = 0;


            //Add('IPROrel3','setor+centro+num_mod+codser',[ixExpression]);
        //    Add('IPROREL3','setor+centro+prod',[ixExpression]);

            //
            return otable;
        }
        static public Boolean SQLCreateTableprovrel3(string path)
        {

            OleDbCommand OleDbcomm;
          
            path = TDataControlReduzido.Get_Path(path);
            
            string stroledb = " CREATE TABLE " + path+"provrel3(" +
            " DBCR C(1), " +
            " ORIGEM I, " +
            " SAIDA N(19,4), " +
            " ENTRADA N(19,4), " +
            " SALDO N(19,4), " +
            " QUANT N(19,4), " +
            " ENCARGOS N(19,4), " +
            " NEGOCIO I, " +
            " SETOR C(3), " +
            " CENTRO C(4), " +
            " QUADRA C(3), " +
            " NUM_MOD C(3), " +
            " CODSER C(3), " +
            " CULT C(3), " +
            " PROD C(3), " +
            " NATUREZA C(13), " +
            " ICODSER I, " +
            " AREA N(19,4), " +
            " PROD_ANT1 N(19,4), " +
            " PARC_ANT1 N(19,4), " +
            " AREA_ANT1 N(19,4), " +
            " PROD_ANT2 N(19,4), " +
            " PARC_ANT2 N(19,4), " +
            " AREA_ANT2 N(19,4), "  +
            " FINO I )";
            


            try
            {

                if (File.Exists(path + "PROVREL3.DBF"))
                {
                    //if (TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().State == ConnectionState.Open)
                      //  TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Close();
                    File.Delete(path + "PROVREL3.DBF");
                }
                {
                    OleDbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                    try
                    {
                        OleDbcomm.ExecuteNonQuery();
                    }
                    catch (Exception)
                    { throw; }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "PROVREL3"));
            }


            return true;
        }
        static public OleDbDataAdapter provrel3ConstruaAdaptador(string path)
        {
            OleDbDataAdapter OleDbda = new OleDbDataAdapter();

            // Construção do insert padrao 

            path = TDataControlReduzido.Get_Path(path);


            OleDbCommand cmd = new OleDbCommand(
           "SELECT   " +
           " DBCR,  ORIGEM,  SAIDA,  ENTRADA,  SALDO,  QUANT, " +
           " ENCARGOS,  NEGOCIO,  SETOR,  CENTRO, QUADRA,  NUM_MOD, " + 
           " CODSER,  CULT,  PROD,  NATUREZA,  ICODSER,  AREA, " +
           " PROD_ANT1,  PARC_ANT1,  AREA_ANT1,  PROD_ANT2, " +
           " PARC_ANT2,  AREA_ANT2, FINO  FROM " + path + "provrel3 ", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            //
            OleDbda.SelectCommand = cmd; 
            
            
            
            
            cmd = new OleDbCommand(
            "INSERT INTO " + path + "provrel3 ( " +
            " DBCR,  ORIGEM,  SAIDA,  ENTRADA,  SALDO,  QUANT, " +
            " ENCARGOS,  NEGOCIO,  SETOR,  CENTRO, QUADRA, NUM_MOD, " +
            " CODSER,  CULT,  PROD,  NATUREZA,  ICODSER,  AREA, " +
            " PROD_ANT1,  PARC_ANT1,  AREA_ANT1,  PROD_ANT2, " +
            " PARC_ANT2,  AREA_ANT2,FINO ) VALUES (    ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?, ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ? ,?,?,?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("@DBCR", OleDbType.Char, 1, "DBCR");
            cmd.Parameters.Add("@ORIGEM", OleDbType.Integer, 0, "ORIGEM");
            cmd.Parameters.Add("@SAIDA", OleDbType.Numeric, 0, "SAIDA");
            cmd.Parameters.Add("@ENTRADA", OleDbType.Numeric, 0, "ENTRADA");
            cmd.Parameters.Add("@SALDO", OleDbType.Numeric, 0, "SALDO");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@ENCARGOS", OleDbType.Numeric, 0, "ENCARGOS");
            cmd.Parameters.Add("@NEGOCIO", OleDbType.Integer, 0, "NEGOCIO");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 3, "SETOR");
            cmd.Parameters.Add("@CENTRO", OleDbType.Char, 4, "CENTRO");
            cmd.Parameters.Add("@QUADRA", OleDbType.Char, 3, "QUADRA");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 3, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@CULT", OleDbType.Char, 3, "CULT");
            cmd.Parameters.Add("@PROD", OleDbType.Char, 3, "PROD");
            cmd.Parameters.Add("@NATUREZA", OleDbType.Char, 13, "NATUREZA");
            cmd.Parameters.Add("@ICODSER", OleDbType.Integer, 0, "ICODSER");
            cmd.Parameters.Add("@AREA", OleDbType.Numeric, 0, "AREA");
            cmd.Parameters.Add("@PROD_ANT1", OleDbType.Numeric, 0, "PROD_ANT1");
            cmd.Parameters.Add("@PARC_ANT1", OleDbType.Numeric, 0, "PARC_ANT1");
            cmd.Parameters.Add("@AREA_ANT1", OleDbType.Numeric, 0, "AREA_ANT1");
            cmd.Parameters.Add("@PROD_ANT2", OleDbType.Numeric, 0, "PROD_ANT2");
            cmd.Parameters.Add("@PARC_ANT2", OleDbType.Numeric, 0, "PARC_ANT2");
            cmd.Parameters.Add("@AREA_ANT2", OleDbType.Numeric, 0, "AREA_ANT2");
            cmd.Parameters.Add("@FINO", OleDbType.Integer, 0, "FINO");
            OleDbda.InsertCommand = cmd;



            // Construção do altera padrao 

            cmd = new OleDbCommand(
           "UPDATE " + path + "provrel3 SET " +
           " DBCR = ? ,  ORIGEM = ? ,  SAIDA = ? ,  ENTRADA = ? , " +
           " SALDO = ? ,  QUANT = ? ,  ENCARGOS = ? ,  NEGOCIO = ? , " +
           " SETOR = ? ,  CENTRO = ? , QUADRA = ? ,  NUM_MOD = ? ,  CODSER = ? , " + //
           " CULT = ? ,  PROD = ? ,  NATUREZA = ? ,  ICODSER = ? , " +
           " AREA = ? ,  PROD_ANT1 = ? ,  PARC_ANT1 = ? , " +
           " AREA_ANT1 = ? ,  PROD_ANT2 = ? ,  PARC_ANT2 = ? , " + // 
           " AREA_ANT2 = ? , FINO = ? WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@DBCR", OleDbType.Char, 1, "DBCR");
            cmd.Parameters.Add("@ORIGEM", OleDbType.Numeric, 0, "ORIGEM");
            cmd.Parameters.Add("@SAIDA", OleDbType.Numeric, 0, "SAIDA");
            cmd.Parameters.Add("@ENTRADA", OleDbType.Numeric, 0, "ENTRADA");
            cmd.Parameters.Add("@SALDO", OleDbType.Numeric, 0, "SALDO");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@ENCARGOS", OleDbType.Numeric, 0, "ENCARGOS");
            cmd.Parameters.Add("@NEGOCIO", OleDbType.Numeric, 0, "NEGOCIO");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 3, "SETOR");
            cmd.Parameters.Add("@CENTRO", OleDbType.Char, 4, "CENTRO");
             cmd.Parameters.Add("@QUADRA", OleDbType.Char, 3, "QUADRA");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 3, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@CULT", OleDbType.Char, 3, "CULT");
            cmd.Parameters.Add("@PROD", OleDbType.Char, 3, "PROD");
            cmd.Parameters.Add("@NATUREZA", OleDbType.Char, 13, "NATUREZA");
            cmd.Parameters.Add("@ICODSER", OleDbType.Numeric, 0, "ICODSER");
            cmd.Parameters.Add("@AREA", OleDbType.Numeric, 0, "AREA");
            cmd.Parameters.Add("@PROD_ANT1", OleDbType.Numeric, 0, "PROD_ANT1");
            cmd.Parameters.Add("@PARC_ANT1", OleDbType.Numeric, 0, "PARC_ANT1");
            cmd.Parameters.Add("@AREA_ANT1", OleDbType.Numeric, 0, "AREA_ANT1");
            cmd.Parameters.Add("@PROD_ANT2", OleDbType.Numeric, 0, "PROD_ANT2");
            cmd.Parameters.Add("@PARC_ANT2", OleDbType.Numeric, 0, "PARC_ANT2");
            cmd.Parameters.Add("@AREA_ANT2", OleDbType.Numeric, 0, "AREA_ANT2");
            cmd.Parameters.Add("@FINO", OleDbType.Numeric, 0, "FINO");

            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;
            OleDbda.UpdateCommand = cmd;

            // Construção do deleta padrao 

            cmd = new OleDbCommand(
           " DELETE FROM " + path + " provrel3" +
           " WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;
            OleDbda.DeleteCommand = cmd;
            return OleDbda;
        }
    }
}

