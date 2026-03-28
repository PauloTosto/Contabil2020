using Newtonsoft.Json;
using PrjApiParceiro_C.Core;
using PrjApiParceiro_C.Fiscais.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace PrjApiParceiro_C.AcessosServidor
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
                            odataset = retDataset[1];
                            for (int i = 0; i < odataset.Tables.Count; i++)
                            {
                                if (odataset.Tables[i].Columns.Count == 0)
                                {
                                    DataTable otable = odataset.Tables[i];
                                    DataTable otableSchema = retDataset[0].Tables[i];
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
                            }
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
                            odataset = retDataset[1];
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
                            }
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
            { // inclusão e exclusão
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


        static private string APIConstruaInclusao(DataRow orow, string tablename, List<string> excluafields)
        {
            // O ID FICA AUTO_INC pelo SQL SERVER
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-En", false);
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
                        else dado = "'" + orow[col.ColumnName].ToString() + "'";
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
        static private string APIConstruaAlteracao(DataRow orow, string campopesquisa, string tablename, List<string> excluafields = null)
        {
            string result = "";
            string vlrcampopesquisa = "";
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF     
            result = "UPDATE " + tablename + " SET ";
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-En", false);

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
                        else dado = "'" + orow[col.ColumnName].ToString() + "'";
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
            CultureInfo.CurrentCulture = new CultureInfo("en-En", false);
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
                            else dado = "'" + orow[col.ColumnName].ToString() + "'";
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

    }

    /*public static class ApiServices_antigo
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
            url = "http://" + host + ":5000/api/SelectML/CSharp?query=" + queryGeral;
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        var fileJsonString = await response.Content.ReadAsStringAsync();
                        odataset = JsonConvert.DeserializeObject<DataSet>(fileJsonString);

                    }
                }
            }
            return odataset;
        }
        // função para manter coerencia entre scripts DBF e SQLSERVER




        public static async Task<DataSet> Api_QuerySP(string querySP)
        {
            DataSet odataset = new DataSet();


            string url = "";
            string host = TestaConexao.ReturnHost(ConexaoAtual.SERVIDOR_IP.ToUpper());
            if (host == "")
            {
                MessageBox.Show("Host não Localizado");
                return odataset;
            }
            url = "http://" + host + ":5000/api/SelectML/SP?query=" + querySP;
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        var fileJsonString = await response.Content.ReadAsStringAsync();
                        odataset = JsonConvert.DeserializeObject<DataSet>(fileJsonString);

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
            if (tipo == 9) {
                url = "/api/SelectML/editeCSharp?modo=inclue";
            }
            else if ((tipo == 0) || (tipo == 1)) { // inclusão e exclusão
                url = "/api/SelectML/editeCSharp?modo=altera";
            }
           
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                //client.DefaultRequestHeaders.Accept.Clear();
                // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    //Insere_Edita teste = new Insere_Edita();
                    //teste.query = str;
                    //var newUserJson = JsonConvert.SerializeObject(teste);
                    // StringContent content;
                    try
                    {
                        // content = new StringContent(str, System.Text.Encoding.UTF8, "application/json");
                        var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("query", str)  });
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
                catch (Exception) {  }

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


        static public async Task<bool> OpereRegistroServidorAsync(DataRow orow, string tablename, List<string> excluafields = null)
        {
            bool result = false;
            string retorno = await APIInsereAlteraRegistro(orow, tablename, excluafields);
            int tid = -1;
            try { tid = Convert.ToInt32(retorno); } catch (Exception) { }
            if (tid <= 0) return result; // insucesso
            result = true;
            if (orow.RowState == DataRowState.Added) { orow["ID"] = tid; }
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
        }


        static private async Task<string> APIInsereAlteraRegistro(DataRow orow, string tablename, List<string> excluafields = null)
        {
            string str = "0"; // retorno 0 significa insucesso na operação
            int tipo = -1;
            if (orow.RowState == DataRowState.Added)
            {
                str = APIConstruaInclusao(orow, tablename, excluafields);
                if (str != "")
                    tipo = 9;

            }
            else
           if (orow.RowState == DataRowState.Modified)
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


        static private string APIConstruaInclusao(DataRow orow, string tablename, List<string> excluafields)
        {
            // O ID FICA AUTO_INC pelo SQL SERVER
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-En", false);
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF
            string result = "INSERT INTO " + tablename + " ( ";
            try
            {
                string virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() == "ID") continue;
                    if ((excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
                    result = result + virgula;
                    result = result + '[' + col.ColumnName + ']';
                    if (virgula == "") virgula = ",";
                }
                result = result + " )  OUTPUT INSERTED.ID VALUES ( ";
                virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (col.ColumnName.ToUpper() == "ID") continue;
                    if ((excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
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
                        if (orow[col.ColumnName] == null) { dado = "''"; }
                        else dado = "'" + orow[col.ColumnName].ToString() + "'";
                    }
                    else if (tipo == Type.GetType("System.Boolean"))
                    {
                        if (orow[col.ColumnName] == null) { dado = "0"; }
                        else
                        {
                            if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                            else { dado = "0"; }
                        }
                    }
                    else
                    {
                        if (orow[col.ColumnName] == null) { dado = "0"; }

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
        static private string APIConstruaAlteracao(DataRow orow, string campopesquisa, string tablename, List<string> excluafields)
        {
            string result = "";
            string vlrcampopesquisa = "";
            if (tablename.ToUpper().Contains(".DBF")) { tablename.ToUpper().Replace(".DBF", ""); }  // eliminar .DBF     
            result = "UPDATE " + tablename + " SET ";
            var sdefault = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-En", false);

            try
            {
                string virgula = "";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if ((excluafields.Count > 0) && (excluafields.Contains(col.ColumnName.ToUpper()))) continue;
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
                        if (orow[col.ColumnName] == null) { dado = "''"; }
                        else dado = "'" + orow[col.ColumnName].ToString() + "'";
                    }
                    else if (tipo == Type.GetType("System.Boolean"))
                    {
                        if (orow[col.ColumnName] == null) { dado = "0"; }
                        else
                        {
                            if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                            else { dado = "0"; }
                        }
                    }
                    else
                    {
                        if (orow[col.ColumnName] == null) { dado = "0"; }

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
            CultureInfo.CurrentCulture = new CultureInfo("en-En", false);
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
                                if (orow[col.ColumnName] == null) { dado = "''"; }
                                else
                                    dado = "'" + Convert.ToDateTime(orow[col.ColumnName]).ToString("yyyy-MM-dd") + "'";
                            }
                            catch (Exception) { dado = "''"; }
                        }
                        else
                        if (tipo == Type.GetType("System.String"))
                        {
                            if (orow[col.ColumnName] == null) { dado = "''"; }
                            else dado = "'" + orow[col.ColumnName].ToString() + "'";
                        }
                        else if (tipo == Type.GetType("System.Boolean"))
                        {
                            if (orow[col.ColumnName] == null) { dado = "0"; }
                            else
                            {
                                if (Convert.ToBoolean(orow[col.ColumnName])) { dado = "1"; }
                                else { dado = "0"; }
                            }
                        }
                        else
                        {
                            if (orow[col.ColumnName] == null) { dado = "0"; }

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

    }*/

}