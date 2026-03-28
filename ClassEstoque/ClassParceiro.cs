using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
//using System.Globalization;
//using System.Collections;

using ClassConexao;

namespace ClassEstoque
{
    class ClassParceiro
    {
        DataTable contratosparc;
        public ClassParceiro()
        { 
             OleDbCommand OleDbcomm;
             string strOleDb, path;
             
             path = TDataControlReduzido.Get_Path("PATRIMONIO");
            // strOleDb = "SELECT parceiro.cod as CODIGO, prgarea.ncontra, prgarea.cod, prgarea.gleba , QUADRAS.qua, parceiro.nome, prgarea.inicio, prgarea.fim, prgarea.ncontra " +
             //  " FROM " + path + "PARCEIRO, " + path + "PRGAREA " + path + "QUADRAS " +
             //  "WHERE   ( prgarea.cod = parceiro.cod) AND ( prgarea.gleba = QUADRAS.gleba)";
               // " AND" +
               // " (QUADRAS.fim = CTOD('  /  /  ')) ";  
             //(PROD = '" + oprod + "') AND
               //" AND " +
               //"( (CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") BETWEEN prgarea.inicio AND prgarea.fim) " +
              // " OR (CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") BETWEEN prgarea.inicio AND prgarea.fim) )";
                strOleDb = "SELECT cod , ncontra, gleba , qua, prod, inicio, fim, parc " +
                 " FROM "+ path + "PRGAREA WHERE (ALLTRIM(gleba) <> '') order by gleba,inicio";
             
             try
             {
                 OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                 OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);
                 OleDbda.TableMappings.Add("Table", "CONTRATOS");
                 DataSet result = new DataSet();
                 OleDbda.Fill(result);
                 OleDbda.Dispose();
                 contratosparc = result.Tables[0].Copy();
                 //fazendo de multiplas colunas primary key...
                
             //    contratosparc.PrimaryKey = new DataColumn[] {contratosparc.Columns["GLEBA"],contratosparc.Columns["QUA"], 
              //                           contratosparc.Columns["PROD"],contratosparc.Columns["INICIO"]};
                 // Or

                /* DataColumn[] keyColumn = new DataColumn[3];
                 keyColumn[0] = contratosparc.Columns["GLEBA"];
                 keyColumn[1] = contratosparc.Columns["PROD"];
                 keyColumn[2] = contratosparc.Columns["INICIO"];
                 contratosparc.PrimaryKey = keyColumn;*/


             }
             catch (Exception)
             {
                 throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
             }
         

        
        }
       public Boolean VerParceria(string quadra, string gleba, string produto, DateTime data)
        {
           DataRow[] rows = contratosparc.Select(//"(QUA ='" + quadra + "') " +
                                                  " (GLEBA ='" + gleba + "') " +
                                                  " AND (PROD ='" + produto + "') " +
                                                             "AND (INICIO <= '" + data.ToString("d") + "') "+
                                                             "AND (FIM >= '" + data.ToString("d") + "') "
                                                               );

           if (rows.Length > 0)
           {
               Boolean result = false;
               foreach (DataRow orow in rows.AsEnumerable())
               {
                   if ((orow["QUA"].ToString().Trim() == "")|| (orow["QUA"].ToString().Trim() == quadra.Trim()))
                   {
                       result = true;
                       break;
                   }
               }
               return result;
           }
           else
               return false;
        }

        

    }
}
