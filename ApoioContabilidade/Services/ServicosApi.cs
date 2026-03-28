using ApoioContabilidade.Core;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ApoioContabilidade.Models
{
    public static class ApiServices
    {
        public static async Task<DataSet> Api_QueryMulti(List<String> sqlStrings)
        {
            DataSet odataset = new DataSet();

            string queryGeral = "";
            foreach (var q in sqlStrings)
            {
                string pontovirgula = Compatib_DBF_SQL.replaceSqlDBO(q);
                if (!(pontovirgula.EndsWith(";"))) { pontovirgula = pontovirgula + ";"; }
                queryGeral = queryGeral + pontovirgula;
            }
            queryGeral = Compatib_DBF_SQL.ptencode(queryGeral);

            string url = "";
            string host = TestaConexao.ReturnHost(ConexaoAtual.SERVIDOR_IP.ToUpper());
            if (host == "")
            {
                MessageBox.Show("Host não Localizado");
                return odataset;
            }

            var builder = new UriBuilder("http://" + host + ":5000/api/SelectML/CSharp?");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["query"] = queryGeral;
            builder.Query = query.ToString();
            url = builder.ToString();

            // url = "http://" + host + ":5000/api/SelectML/CSharp?query=" + queryGeral;
            try
            {


                using (var client = new HttpClient())
                {

                    using (var response = await client.GetAsync(url))
                    {

                        if (response.IsSuccessStatusCode)
                        {
                            var fileJsonString = await response.Content.ReadAsStringAsync();
                            List<DataSet> retDataset = JsonConvert.DeserializeObject<List<DataSet>>(fileJsonString);
                            odataset = ConfigureTabelasRetornoServidor(retDataset);

                        }
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Erro Acesso Servidor");
            }
            return odataset;
        }




        // função para manter coerencia entre scripts DBF e SQLSERVER




        public static async Task<DataSet> Api_QuerySP(List<string> lstquery)
        {
            DataSet odataset = new DataSet();

            string querySP = "";
            string amper = "";
            foreach (string param in lstquery)
            {
                querySP = querySP + amper + param;
                if (amper == "") amper = ";";
            }


            string url = "";
            string host = TestaConexao.ReturnHost(ConexaoAtual.SERVIDOR_IP.ToUpper());
            if (host == "")
            {
                MessageBox.Show("Host não Localizado");
                return odataset;
            }
            var builder = new UriBuilder("http://" + host + ":5000/api/SelectML/SP?");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["query"] = querySP;
            builder.Query = query.ToString();
            url = builder.ToString();

            //url = "http://" + host + ":5000/api/SelectML/SP?query=" + querySP;
            try
            {


                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(url))
                    {

                        if (response.IsSuccessStatusCode)
                        {
                            var fileJsonString = await response.Content.ReadAsStringAsync();
                            List<DataSet> retDataset = JsonConvert.DeserializeObject<List<DataSet>>(fileJsonString);
                            odataset = ConfigureTabelasRetornoServidor(retDataset);

                        }

                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro Acesso Servidor");

            }
            return odataset;
        }


        /*public static async Task<DataSet> Api_QuerySPSIMPLES(string strquery)
        {
            DataSet odataset = new DataSet();

            string querySP = "";
            string amper = "";
            foreach (string param in lstquery)
            {
                querySP = querySP + amper + param;
                if (amper == "") amper = ";";
            }


            string url = "";
            string host = TestaConexao.ReturnHost(ConexaoAtual.SERVIDOR_IP.ToUpper());
            if (host == "")
            {
                MessageBox.Show("Host não Localizado");
                return odataset;
            }
            var builder = new UriBuilder("http://" + host + ":5000/api/SelectML/SP?");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["query"] = querySP;
            builder.Query = query.ToString();
            url = builder.ToString();

            //url = "http://" + host + ":5000/api/SelectML/SP?query=" + querySP;
            try
            {


                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(url))
                    {

                        if (response.IsSuccessStatusCode)
                        {
                            var fileJsonString = await response.Content.ReadAsStringAsync();
                            List<DataSet> retDataset = JsonConvert.DeserializeObject<List<DataSet>>(fileJsonString);
                            odataset = ConfigureTabelasRetornoServidor(retDataset);

                        }

                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro Acesso Servidor");

            }
            return odataset;
        }
        */


        // O Json JsonConvert.DeserializeObject<List<DataSet>>(fileJsonString) não deserializa bem o DATASET ,
        // Porque além de não determinar o tamanho maximo dos campos strings, caso um DateTime venha no primeiro 
        // registro como null, considera este campo (que deveria ser DateTime) como string,

        static private DataSet NovoConfigureTabelasRetornoServidor(List<DataSet> retDataset)
        {
            DataSet result = new DataSet();

            //DataSet resultFinal = new DataSet();


            DataSet dsSchema = retDataset[0];
            DataSet dsDados = retDataset[1];
            // cria as tabelas conforme o esquema que veio do provedor de dados
            for (int i = 0; i < dsSchema.Tables.Count; i++)
            {
                DataTable otable = new DataTable();
                DataTable otableSchema = dsSchema.Tables[i];
                try
                {
                    foreach (DataRow row in otableSchema.Rows)
                    {

                        DataColumn mydatacolumn = new DataColumn();
                        mydatacolumn.ColumnName = row["ColumnName"].ToString().ToUpper();
                        mydatacolumn.ReadOnly = false;

                        if (row["DataType"].ToString().ToUpper() == "SYSTEM.STRING")
                        {
                            mydatacolumn.DataType = System.Type.GetType("System.String");
                            mydatacolumn.MaxLength = Convert.ToInt16(row["ColumnSize"]);

                        }
                        else
                        {
                            if (row["DataType"].ToString().ToUpper() == "SYSTEM.DECIMAL")
                            {
                                mydatacolumn.DataType = Type.GetType("System.Double");
                            }
                            else
                                mydatacolumn.DataType = Type.GetType(row["DataType"].ToString());
                        }
                        // mydatacolumn.
                        otable.Columns.Add(mydatacolumn);
                    }
                    otable.TableName = "Table" + (i + 1).ToString();
                    result.Tables.Add(otable);

                }
                catch (Exception E)
                {
                    MessageBox.Show("Erro configurar Dataset");
                    throw;
                }
            }
            for (int i = 0; i < dsDados.Tables.Count; i++)
            {
                if (dsDados.Tables[i].Columns.Count == 0) // Casos em que a tabela veio vazia pois não tinha nenhum dado valido para esta query no servidor
                {
                }
                else
                {
                    try
                    {
                        List<string> colName = new List<string>();
                        DataTable origem = dsDados.Tables[i];
                        DataTable destino = result.Tables[i];
                        foreach (DataColumn ocol in origem.Columns)
                        {
                            if ((destino.Columns[ocol.ColumnName].DataType != ocol.DataType)
                                && (destino.Columns[ocol.ColumnName].DataType == Type.GetType("System.DateTime"))
                                && (ocol.DataType == Type.GetType("System.String"))
                                )
                            {
                                colName.Add(ocol.ColumnName);
                            }
                        }
                        if (colName.Count > 0)
                        {
                            foreach (string coluna in colName)
                            {
                                foreach (DataRow orow in origem.AsEnumerable().Where(row =>
                                          (!row.IsNull(coluna)) && row.Field<string>(coluna).Length > 0)
                                    )
                                {
                                    var cultura = CultureInfo.CurrentCulture;
                                    try
                                    {
                                        CultureInfo.CurrentCulture = new CultureInfo("en-US");
                                        DateTime dataus = Convert.ToDateTime(orow[coluna]);
                                        CultureInfo.CurrentCulture = cultura;
                                        string dataNacional = dataus.ToString();
                                        orow.BeginEdit();
                                        orow[coluna] = dataNacional;
                                        orow.EndEdit();
                                        orow.AcceptChanges();
                                    }
                                    catch (Exception E)
                                    {
                                        MessageBox.Show("Erro ao TRansformar DateTime " + E.Message);
                                        throw;

                                    }
                                    finally
                                    {
                                        CultureInfo.CurrentCulture = cultura;
                                    }
                                }

                            }
                            // DataTable tabNova = result.Tables[i].Clone();
                            dsDados.Tables[i].AsEnumerable().CopyToDataTable(result.Tables[i], LoadOption.OverwriteChanges);
                            result.Tables[i].AcceptChanges();
                            // tabNova.AcceptChanges();
                            // resultFinal.Tables.Add(tabNova);
                        }
                        else
                        {
                            dsDados.Tables[i].AsEnumerable().CopyToDataTable(result.Tables[i], LoadOption.OverwriteChanges);
                            result.Tables[i].AcceptChanges();


                            /*   DataTable otableSchema = dsSchema.Tables[i];
                               foreach (DataRow row in otableSchema.Rows)
                               {

                                   string coluna = row["ColumnName"].ToString().ToUpper();

                                   if (row["DataType"].ToString().ToUpper() == "SYSTEM.STRING")
                                   {
                                       if (dsDados.Tables[i].Columns.Contains(coluna))
                                           dsDados.Tables[i].Columns[coluna].MaxLength = Convert.ToInt16(row["ColumnSize"]);
                                   }
                               }
                               resultFinal.Tables.Add(dsDados.Tables[i].Copy());
                              */
                        }

                    }
                    catch (Exception E)
                    {

                        throw;
                    }

                }
            }
            return result;
        }


        // Modifique este procedimento em 6/02/2021
        //
        static private DataSet ConfigureTabelasRetornoServidor(List<DataSet> retDataset)
        {
            DataSet odataset = retDataset[1];
            for (int i = 0; i < odataset.Tables.Count; i++)
            {
                if (odataset.Tables[i].Columns.Count == 0)
                {
                    DataTable otable = odataset.Tables[i];
                    DataTable otableSchema = retDataset[0].Tables[i];
                    try
                    {
                        foreach (DataRow row in otableSchema.Rows)
                        {

                            DataColumn mydatacolumn = new DataColumn();
                            mydatacolumn.ColumnName = row["ColumnName"].ToString().ToUpper();
                            mydatacolumn.ReadOnly = false;

                            if (row["DataType"].ToString().ToUpper() == "SYSTEM.STRING")
                            {
                                mydatacolumn.DataType = System.Type.GetType("System.String");
                                mydatacolumn.MaxLength = Convert.ToInt16(row["ColumnSize"]);

                            }
                            else
                            {
                                if (row["DataType"].ToString().ToUpper() == "SYSTEM.DECIMAL")
                                {
                                    mydatacolumn.DataType = Type.GetType("System.Double");
                                }
                                else
                                    mydatacolumn.DataType = Type.GetType(row["DataType"].ToString());
                            }
                            // mydatacolumn.
                            otable.Columns.Add(mydatacolumn);
                        }
                    }
                    catch (Exception E)
                    {
                        throw;
                    }
                }
                else
                {

                    DataTable otableSchema = retDataset[0].Tables[i];
                    foreach (DataRow row in otableSchema.Rows)
                    {

                        string colName = row["ColumnName"].ToString().ToUpper();

                        if (row["DataType"].ToString().ToUpper() == "SYSTEM.STRING")
                        {
                            if (odataset.Tables[i].Columns.Contains(colName))
                                odataset.Tables[i].Columns[colName].MaxLength = Convert.ToInt16(row["ColumnSize"]);
                        }
                    }
                }

            }


            for (int i = 0; i < odataset.Tables.Count; i++)
            {
                if (odataset.Tables[i].Columns.Count == 0) // Casos em que a tabela veio vazia pois não tinha nenhum dado valido para esta query no servidor
                {
                }
                else
                {
                    try
                    {
                        if (odataset.Tables[i].Rows.Count > 0)
                        {
                            DataRow orow = odataset.Tables[i].Rows[0];
                            foreach (DataColumn ocol in orow.Table.Columns)
                            {
                                if (ocol.DataType == Type.GetType("System.DateTime"))
                                {
                                    if (Convert.ToDateTime(orow[ocol.ColumnName]) == DateTime.MinValue)
                                    {
                                        orow.BeginEdit();
                                        orow[ocol.ColumnName] = DBNull.Value;
                                        orow.EndEdit();
                                        orow.AcceptChanges();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("Erro Conversão Dados " + E.Message);
                        throw;
                    }
                }
            }
            return odataset;
        }
        public static async Task<int> PostApi(string str, int tipo)
        {
            int retorno = -1;
            string url = "";
            string host = TestaConexao.ReturnHost(ConexaoAtual.SERVIDOR_IP.ToUpper());
            if (host == "")
            {
                MessageBox.Show("Host não Localizado");
                return retorno;
            }
            string uri = "http://" + host + ":5000";
            if (tipo == 9)
            {
                url = "/api/SelectML/editeCSharp?modo=inclue";
            }
            else if ((tipo == 0) || (tipo == 1))
            { // altera e exclusão
                url = "/api/SelectML/editeCSharp?modo=altera";
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                try
                {
                    try
                    {
                        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("query", str) });
                        var response = await client.PostAsync(url, content);
                        if (response.IsSuccessStatusCode)
                        {
                            string resultContent = await response.Content.ReadAsStringAsync();
                            try { retorno = Convert.ToInt32(resultContent); }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception) { }
                }
                catch (Exception) { }

            }
            return retorno;
        }


    }

    // função para manter coerencia entre scripts DBF e SQLSERVER
    static public class Compatib_DBF_SQL
    {
        static public string replaceSqlDBO(string sqlString)
        {
            string funcao, nova;
            funcao = "";
            nova = "";
            if (sqlString.ToUpper().Contains("=>"))
            {
                funcao = "=>";
                nova = ">=";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            if (sqlString.ToUpper().Contains("VAL("))
            {
                funcao = "VAL(";
                nova = "dbo.VAL(";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }

            if (sqlString.ToUpper().Contains("CTOD("))
            {
                funcao = "CTOD(";
                nova = "dbo.CTOD(";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            if (sqlString.ToUpper().Contains("ALLTRIM("))
            {
                if (!(sqlString.ToUpper().Contains("DBO.ALLTRIM(")))
                {

                    funcao = "ALLTRIM(";
                    nova = "dbo.ALLTRIM(";
                    sqlString = sqlString.ToUpper().Replace(funcao, nova);
                }
            }

            if (sqlString.ToUpper().Contains("DTOS("))
            {
                if (!(sqlString.ToUpper().Contains("DBO.DTOS(")))
                {
                    funcao = "DTOS(";
                    nova = "dbo.DTOS(";
                    sqlString = sqlString.ToUpper().Replace(funcao, nova);
                }
            }
            if (sqlString.ToUpper().Contains(".DBF"))
            {

                funcao = ".DBF";
                nova = "";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            if (sqlString.ToUpper().Contains("SUBS("))
            {
                funcao = "SUBS(";
                nova = "SUBSTRING(";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }

            if (sqlString.ToUpper().Contains("SUBSTR("))
            {
                funcao = "SUBSTR(";
                nova = "SUBSTRING(";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            if (sqlString.ToUpper().Contains(".T."))
            {
                funcao = ".T.";
                nova = "1";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            if (sqlString.ToUpper().Contains(".F."))
            {
                funcao = ".F.";
                nova = "0";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            return sqlString;
        }
        static public string ptencode(string sqlString)
        {
            string funcao, nova;
            if (sqlString.ToUpper().Contains("#"))
            {
                funcao = "#";
                nova = "%23";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            if (sqlString.ToUpper().Contains("+"))
            {
                funcao = "+";
                nova = "%2B";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            if (sqlString.ToUpper().Contains(";"))
            {
                funcao = ";";
                nova = "%3B";
                sqlString = sqlString.ToUpper().Replace(funcao, nova);
            }
            return sqlString;
        }

    }
    static public class Prepara_Sql
    {


        static public async Task<bool> OpereRegistroServidorAsync(DataRow orow, DataRowState rowEstado, string tablename, List<string> excluafields = null)
        {
            bool result = false;
            string retorno = await APIInsereAlteraRegistro(orow, rowEstado, tablename, excluafields);
            int tid = -1;
            try { tid = Convert.ToInt32(retorno); } catch (Exception) { }
            if (tid <= 0) return result; // insucesso
            result = true;
            if (rowEstado == DataRowState.Added) { orow["ID"] = tid; }
            return result;
        }
        static public async Task<bool> OpereIncluaRegistrosServidorAsync(DataRow[] orows, string tablename, List<string> excluafields = null)
        {
            bool result = false;
            List<string> lstString = APIConstruaInclusaoDiversos(orows, tablename, excluafields);
            string slqInsertRegistros = "";
            foreach (string linha in lstString)
            {
                slqInsertRegistros = slqInsertRegistros + linha + ";";
            }
            DataSet dsRetorno_Campos = null;
            try
            {
                dsRetorno_Campos = await ApiServices.Api_QueryMulti(lstString);

                if (dsRetorno_Campos.Tables.Count != orows.Length)
                {
                    MessageBox.Show("Erro ao COLAR registros");
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao COLAR registros");
                return false;
            }
            result = true;
            for (int i = 0; i < dsRetorno_Campos.Tables.Count; i++)
            {
                DataTable otable = dsRetorno_Campos.Tables[i];
                DataRow orowCurrente = orows[i];
                DataRow rowRetorno = otable.Rows[0];
                try
                {
                    foreach (DataColumn ocol in otable.Columns)
                    {
                        orowCurrente[ocol.ColumnName] = rowRetorno[ocol.ColumnName];
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        static public async Task<int> OpereIncluaRegistrosMultiplosServidorAsync(DataRow[] orows, string tablename, List<string> excluafields = null)
        {
            int result = -1;
            List<string> lstString = APIConstruaInclusaoDiversos(orows, tablename, excluafields);
            string slqInsertRegistros = "";
            foreach (string linha in lstString)
            {
                slqInsertRegistros = slqInsertRegistros + linha + ";";
            }
            try
            {

                result = await ApiServices.PostApi(slqInsertRegistros, 9);

            }
            catch (Exception)
            {
                MessageBox.Show("Erro Gravar  registros");
                return result;
            }
            
            return result;
        }



        static public async Task<bool> OpereIncluaRegistroServidorAsync_diversosMAX(
            DataRow orow, string tablename, List<string> fieldsMAX, List<string> excluafields = null)
        {

            bool result = false;
            List<string> lstString = APIConstruaInclusaoDiversosMAX_Generico(orow, tablename, excluafields, fieldsMAX);
            //string slqInsertRegistros = "";
            DataSet dsRetorno_Campos = null;
            try
            {

                dsRetorno_Campos = await ApiServices.Api_QueryMulti(lstString);

                if (dsRetorno_Campos.Tables.Count == 0)
                {
                    MessageBox.Show("Erro ao Incluir registro");
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao Incluir registro");
                return false;
            }
            result = true;
            DataTable otable = dsRetorno_Campos.Tables[0];
            DataRow rowRetorno = otable.Rows[0];
            try
            {

                foreach (DataColumn ocol in otable.Columns)
                {
                    orow[ocol.ColumnName] = rowRetorno[ocol.ColumnName];

                }
                orow.AcceptChanges();
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }



        static public async Task<bool> OpereDeleteRegistroServidorAsync(DataRow orow, string tablename, List<string> excluafields = null)
        {
            bool result = false;
            string retorno = await APIDeleteRegistro(orow, tablename, excluafields);
            int tid = -1;
            try { tid = Convert.ToInt32(retorno); } catch (Exception) { }
            if (tid <= 0) return result; // insucesso
            result = true;
            return result;
            /*if result then
            begin
              tid:= DmAdoFinan.AtualizaQueries.IndexOf(uppercase(nometabela));
            if (tid > -1) then
                TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[tid])
          .atualizado := false;
            end;
            */
        }


        static private async Task<string> APIInsereAlteraRegistro(DataRow orow, DataRowState rowEstado, string tablename, List<string> excluafields = null)
        {
            string str = "0"; // retorno 0 significa insucesso na operação
            int tipo = -1;
            if (rowEstado == DataRowState.Added)
            {
                str = APIConstruaInclusao(orow, tablename, excluafields);
                if (str != "")
                    tipo = 9;

            }
            else
           if (rowEstado == DataRowState.Modified)
            {
                str = APIConstruaAlteracao(orow, "ID", tablename, excluafields);
                if (str != "")
                    tipo = 0;
            }
            if (tipo == -1) { return str; }
            int retorno = await ApiServices.PostApi(str, tipo); // inclusão retorna o novo ID....Alteração retorno numero de linhas afetadas
            return retorno.ToString();
        }

        static private async Task<string> APIDeleteRegistro(DataRow orow, string tablename, List<string> excluafields = null)
        {
            string str = "0"; // retorno 0 significa insucesso na operação
            int tipo = -1;
            str = APIConstruaDeleteRegistro(orow, "ID", tablename);
            if (str != "") tipo = 1;
            if (tipo == -1) { return str; }
            int retorno = await ApiServices.PostApi(str, tipo);
            return retorno.ToString();
        }
        static public async Task<int> APIDeleteTodosRegistrosTabela(string tablename, string condicao)
        {
            string str = "DELETE FROM "+tablename;
            if (condicao != "")
            {
                str = str + " WHERE " + condicao;
            }
            
            int tipo = 1;
            int retorno = await ApiServices.PostApi(str, tipo);
            return retorno;
        }



        static public string APIConstruaInclusao(DataRow orow, string tablename, List<string> excluafields)
        {
            // O ID FICA AUTO_INC pelo SQL SERVER
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-us", false);
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF
            string result = "INSERT INTO " + tablename + " ( ";
            try
            {
                string virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() == "ID") continue;
                    if ((excluafields != null) && (excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    result = result + virgula;
                    result = result + '[' + col.ColumnName + ']';
                    if (virgula == "") virgula = ",";
                }
                result = result + " )  OUTPUT INSERTED.ID VALUES ( ";
                virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() == "ID") continue;
                    if ((excluafields != null) && (excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    Type tipo = col.DataType;
                    string dado = "''";

                    if (tipo == Type.GetType("System.DateTime"))
                    {
                        try
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                            else
                                dado = "'" + Convert.ToDateTime(orow[col.ColumnName]).ToString("yyyy-MM-dd") + "'";
                        }
                        catch (Exception) { dado = "''"; }
                    }
                    else
                    if (tipo == Type.GetType("System.String"))
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                        else
                        {
                            string dadoCorrigido = orow[col.ColumnName].ToString();
                            if (dadoCorrigido.ToUpper().Contains("'"))
                            {
                                string funcao = "'";
                                string nova = "''";
                                dadoCorrigido = dadoCorrigido.ToUpper().Replace(funcao, nova);
                            }
                            if (dadoCorrigido.Length > col.MaxLength)
                            {
                                if (col.MaxLength > 0)
                                    dadoCorrigido = dadoCorrigido.Substring(0, col.MaxLength);
                            }
                            dado = "'" + dadoCorrigido + "'";
                        }
                    }
                    else if (tipo == Type.GetType("System.Boolean"))
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "0"; }
                        else
                        {
                            if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                            else { dado = "0"; }
                        }
                    }
                    else
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "0"; }

                        else
                        {
                            try
                            {
                                dado = orow[col.ColumnName].ToString();
                                if (dado == "") { dado = "0"; }
                            }
                            catch (Exception) { dado = "0"; }
                        }
                    }
                    result = result + virgula;
                    result = result + dado;
                    if (virgula == "") virgula = ", ";
                }
                result = result + " )";
            }
            catch (Exception E) { result = ""; }
            finally { CultureInfo.CurrentCulture = sdefault; }
            return result;
        }
        static private string APIConstruaEspecialInclusaoCLTPONTO(DataRow orow, string tablename, List<string> excluafields)
        {
            // O ID FICA AUTO_INC pelo SQL SERVER
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-us", false);
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF
            string result = "INSERT INTO " + tablename + " ( ";
            try
            {
                string virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() == "ID") continue;
                    if ((excluafields != null) && (excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    result = result + virgula;
                    result = result + '[' + col.ColumnName + ']';
                    if (virgula == "") virgula = ",";
                }
                // OUTPUT INSERTED.ID
                result = result + " )   VALUES ( ";
                virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() == "ID") continue;
                    if ((excluafields != null) && (excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    Type tipo = col.DataType;
                    string dado = "''";

                    if (col.ColumnName.ToUpper() == "NREG")
                    {
                        dado = "(SELECT MAX(NREG) + 1 FROM CLTPONTO)";
                    }
                    else
                    {

                        if (tipo == Type.GetType("System.DateTime"))
                        {
                            try
                            {
                                if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                                else
                                    dado = "'" + Convert.ToDateTime(orow[col.ColumnName]).ToString("yyyy-MM-dd") + "'";
                            }
                            catch (Exception) { dado = "''"; }
                        }
                        else
                        if (tipo == Type.GetType("System.String"))
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                            else
                            {
                                string dadoCorrigido = orow[col.ColumnName].ToString();
                                if (dadoCorrigido.Length > col.MaxLength)
                                {
                                    if (col.MaxLength > 0)
                                        dadoCorrigido = dadoCorrigido.Substring(0, col.MaxLength);
                                }
                                dado = "'" + dadoCorrigido + "'";
                            }
                        }
                        else if (tipo == Type.GetType("System.Boolean"))
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "0"; }
                            else
                            {
                                if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                                else { dado = "0"; }
                            }
                        }
                        else
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "0"; }

                            else
                            {
                                try
                                {
                                    dado = orow[col.ColumnName].ToString();
                                    if (dado == "") { dado = "0"; }
                                }
                                catch (Exception) { dado = "0"; }
                            }
                        }
                    }
                    result = result + virgula;
                    result = result + dado;
                    if (virgula == "") virgula = ", ";
                }
                result = result + " )";
            }
            catch (Exception E) { result = ""; }
            finally { CultureInfo.CurrentCulture = sdefault; }
            return result;
        }
        static private string APIConstruaEspecialInclusao_COlOCANDO_MAXs(DataRow orow, string tablename, List<string> excluafields, List<string> fieldsMAX)
        {
            // O ID FICA AUTO_INC pelo SQL SERVER
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-us", false);
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF
            string result = "INSERT INTO " + tablename + " ( ";
            try
            {
                string virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() == "ID") continue;
                    if ((excluafields != null) && (excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    result = result + virgula;
                    result = result + '[' + col.ColumnName + ']';
                    if (virgula == "") virgula = ",";
                }
                // OUTPUT INSERTED.ID
                result = result + " )   VALUES ( ";
                virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() == "ID") continue;
                    if ((excluafields != null) && (excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    Type tipo = col.DataType;
                    string dado = "''";

                    if (fieldsMAX.Contains(col.ColumnName.ToUpper()))
                    {
                        dado = "(SELECT MAX(" + col.ColumnName.ToUpper() + ") + 1 FROM " + tablename + ")";
                    }
                    else
                    {

                        if (tipo == Type.GetType("System.DateTime"))
                        {
                            try
                            {
                                if (orow.IsNull(col.ColumnName)) { dado = "null"; }
                                else
                                    dado = "'" + Convert.ToDateTime(orow[col.ColumnName]).ToString("yyyy-MM-dd") + "'";
                            }
                            catch (Exception) { dado = "null"; }
                        }
                        else
                        if (tipo == Type.GetType("System.String"))
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                            else
                            {
                                string dadoCorrigido = orow[col.ColumnName].ToString();
                                if (dadoCorrigido.Length > col.MaxLength)
                                {
                                    if (col.MaxLength > 0)
                                        dadoCorrigido = dadoCorrigido.Substring(0, col.MaxLength);
                                }
                                dado = "'" + dadoCorrigido + "'";
                            }
                        }
                        else if (tipo == Type.GetType("System.Boolean"))
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "0"; }
                            else
                            {
                                if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                                else { dado = "0"; }
                            }
                        }
                        else
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "0"; }

                            else
                            {
                                try
                                {
                                    dado = orow[col.ColumnName].ToString();
                                    if (dado == "") { dado = "0"; }
                                }
                                catch (Exception) { dado = "0"; }
                            }
                        }
                    }
                    result = result + virgula;
                    result = result + dado;
                    if (virgula == "") virgula = ", ";
                }
                result = result + " )";
            }
            catch (Exception E) { result = ""; }
            finally { CultureInfo.CurrentCulture = sdefault; }
            return result;
        }



        /*
         
         (SELECT  MAX(NREG) +1 FROM CLTPONTO ) );
         SELECT MAX(ID) as ID, MAX(NREG) as NREG FROM CLTPONTO

         */

        static private string APIConstruaAlteracao(DataRow orow, string campopesquisa, string tablename, List<string> excluafields = null)
        {
            string result = "";
            string vlrcampopesquisa = "";
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF     
            result = "UPDATE " + tablename + " SET ";
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-us", false);

            try
            {
                string virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if ((excluafields != null) && (excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    Type tipo = col.DataType;
                    string dado = "''";

                    if (tipo == Type.GetType("System.DateTime"))
                    {
                        try
                        {
                            if (orow[col.ColumnName] == null) { dado = "''"; }
                            else
                                dado = "'" + Convert.ToDateTime(orow[col.ColumnName]).ToString("yyyy-MM-dd") + "'";
                        }
                        catch (Exception) { dado = "''"; }
                    }
                    else
                    if (tipo == Type.GetType("System.String"))
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                        else
                        {
                            string dadoCorrigido = orow[col.ColumnName].ToString();
                            if (dadoCorrigido.Length > col.MaxLength)
                            {

                                if (col.MaxLength > 0)
                                    dadoCorrigido = dadoCorrigido.Substring(0, col.MaxLength);


                            }
                            dado = "'" + dadoCorrigido + "'";
                        }


                    }
                    else if (tipo == Type.GetType("System.Boolean"))
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "0"; }
                        else
                        {
                            if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                            else { dado = "0"; }
                        }
                    }
                    else
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "0"; }

                        else
                        {
                            try
                            {
                                dado = orow[col.ColumnName].ToString();
                                if (dado == "") { dado = "0"; }
                            }
                            catch (Exception) { dado = "0"; }
                        }
                    }

                    if (col.ColumnName.ToUpper() == campopesquisa.ToUpper())
                    {
                        vlrcampopesquisa = dado;
                        continue;
                    }
                    result = result + virgula;
                    result = result + "[" + col.ColumnName + "] = " + dado;
                    if (virgula == "") virgula = ",";
                }
                result = result + " WHERE ( " + campopesquisa + " = " +
                                    vlrcampopesquisa + " )";

            }
            catch (Exception E) { result = ""; }
            finally { CultureInfo.CurrentCulture = sdefault; }
            return result;
        }
        static private string APIConstruaAlteracao_SoAlterados(DataRow orow, string campopesquisa, string tablename, List<string> excluafields = null)
        {
            string result = "";
            string vlrcampopesquisa = "";
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF     
            result = "UPDATE " + tablename + " SET ";
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-us", false);

            try
            {
                string virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if ((excluafields != null) && (excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    if (orow[col, DataRowVersion.Current].ToString() == orow[col, DataRowVersion.Original].ToString()) continue;

                    Type tipo = col.DataType;
                    string dado = "''";

                    if (tipo == Type.GetType("System.DateTime"))
                    {
                        try
                        {
                            if (orow[col.ColumnName] == null) { dado = "''"; }
                            else
                                dado = "'" + Convert.ToDateTime(orow[col.ColumnName]).ToString("yyyy-MM-dd") + "'";
                        }
                        catch (Exception) { dado = "''"; }
                    }
                    else
                    if (tipo == Type.GetType("System.String"))
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                        else
                        {
                            string dadoCorrigido = orow[col.ColumnName].ToString();
                            if (dadoCorrigido.Length > col.MaxLength)
                            {

                                if (col.MaxLength > 0)
                                    dadoCorrigido = dadoCorrigido.Substring(0, col.MaxLength);


                            }
                            dado = "'" + dadoCorrigido + "'";
                        }


                    }
                    else if (tipo == Type.GetType("System.Boolean"))
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "0"; }
                        else
                        {
                            if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                            else { dado = "0"; }
                        }
                    }
                    else
                    {
                        if (orow.IsNull(col.ColumnName)) { dado = "0"; }

                        else
                        {
                            try
                            {
                                dado = orow[col.ColumnName].ToString();
                                if (dado == "") { dado = "0"; }
                            }
                            catch (Exception) { dado = "0"; }
                        }
                    }

                    if (col.ColumnName.ToUpper() == campopesquisa.ToUpper())
                    {
                        vlrcampopesquisa = dado;
                        continue;
                    }
                    result = result + virgula;
                    result = result + "[" + col.ColumnName + "] = " + dado;
                    if (virgula == "") virgula = ",";
                }
                result = result + " WHERE ( " + campopesquisa + " = " +
                                    vlrcampopesquisa + " )";

            }
            catch (Exception E) { result = ""; }
            finally { CultureInfo.CurrentCulture = sdefault; }
            return result;
        }





        static private string APIConstruaDeleteRegistro(DataRow orow, string campopesquisa, string tablename)
        {
            string result = "";
            string vlrcampopesquisa = "";
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF     
            result = "DELETE FROM " + tablename;
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-us", false);
            try
            {
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() != campopesquisa.ToUpper()) continue;
                    {

                        Type tipo = col.DataType;
                        string dado = "''";

                        if (tipo == Type.GetType("System.DateTime"))
                        {
                            try
                            {
                                if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                                else
                                    dado = "'" + Convert.ToDateTime(orow[col.ColumnName]).ToString("yyyy-MM-dd") + "'";
                            }
                            catch (Exception) { dado = "''"; }
                        }
                        else
                        if (tipo == Type.GetType("System.String"))
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "''"; }
                            else
                            {
                                string dadoCorrigido = orow[col.ColumnName].ToString();
                                if (dadoCorrigido.Length > col.MaxLength)
                                {
                                    if (col.MaxLength > 0)
                                        dadoCorrigido = dadoCorrigido.Substring(0, col.MaxLength);
                                }
                                dado = "'" + dadoCorrigido + "'";
                            }


                        }
                        else if (tipo == Type.GetType("System.Boolean"))
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "0"; }
                            else
                            {
                                if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                                else { dado = "0"; }
                            }
                        }
                        else
                        {
                            if (orow.IsNull(col.ColumnName)) { dado = "0"; }

                            else
                            {
                                try
                                {
                                    dado = orow[col.ColumnName].ToString();
                                    if (dado == "") { dado = "0"; }
                                }
                                catch (Exception) { dado = "0"; }
                            }
                        }
                        vlrcampopesquisa = dado;

                    }
                }
                result = result + " WHERE ( " + campopesquisa + " = " +
                                    vlrcampopesquisa + " )";

            }
            catch (Exception E) { result = ""; }
            finally { CultureInfo.CurrentCulture = sdefault; }
            return result;
        }

        static private List<string> APIConstruaInclusaoDiversos(DataRow[] orows, string tablename, List<string> excluafields)
        {
            List<string> lstString = new List<string>();
            foreach (DataRow orow in orows)
            {
                string linha;
                if (tablename == "CLTPONTO")
                    linha = APIConstruaEspecialInclusaoCLTPONTO(orow, tablename, excluafields) + ";"
                        + "SELECT MAX(ID) as ID, MAX(NREG) as NREG FROM CLTPONTO";

                else
                    linha = APIConstruaInclusao(orow, tablename, excluafields);


                lstString.Add(linha);
            }
            return lstString;

        }
        static private List<string> APIConstruaInclusaoDiversosMAX_Generico(DataRow orow, string tablename,
                List<string> excluafields,
                List<string> fieldsMAX)
        {
            List<string> lstString = new List<string>();
            string linha;
            if (fieldsMAX.Count == 0) return lstString;

            linha = APIConstruaEspecialInclusao_COlOCANDO_MAXs(orow, tablename, excluafields, fieldsMAX) + ";";
            lstString.Add(linha);
            string retornoMaxs = "SELECT MAX(ID) as ID";
            foreach (string campo in fieldsMAX)
            {
                retornoMaxs = retornoMaxs + ", ";
                retornoMaxs = retornoMaxs + "MAX(" + campo + ") as " + campo;
            }
            retornoMaxs = retornoMaxs + " FROM " + tablename;
            lstString.Add(retornoMaxs);
            return lstString;
        }
    }
}