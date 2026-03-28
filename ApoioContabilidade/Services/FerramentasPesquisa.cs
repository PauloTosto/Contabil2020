using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.PagarReceber.ServicesLocais
{
    static public class PesquisaFuncoes
    {
        // originalmente no TDataControlContab
        // ALGUMAS FUNCOES IRÂO RETORNAR UMA STRING QUE È GERALMENTE UM PEDAÇO DE CÓDIGO SQL PARA PESQUISA DEIRETA NO SERVIDOR
        // OUTRASS RETORNAM BOOLEANOS QUE SERVIRÃO PARA SELECIONAR REGISTROS EM CODIGO C# 
        static public Boolean fPassa_Contab2(LinhaSolucao oLinha, DataRow odatarow, PesquisaGenerico opesquisa)
        {
            bool classificado = false;

            if ((oLinha.dado[0] == "") || (oLinha.dado[0] == "*")) return classificado;
            bool invertaSelecao = false;
            if ((oLinha.dado[0].Length > 0) && (oLinha.dado[0].Substring(0, 1).ToUpper() == "N"))
                invertaSelecao = true;
            if (opesquisa == null)
            {
                if ((Boolean)odatarow["TP_FIN"] == true)
                {
                    if ((string)odatarow["TIPO"] == "P")
                    {
                        if (((EBancoNormalizado((string)odatarow["CREDITO"], opesquisa, true)))//|| (((string)odatarow["CREDITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                            classificado = true;
                    }
                    else
                        if (((EBancoNormalizado((string)odatarow["DEBITO"], opesquisa, true)))// || (((string)odatarow["DEBITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
                else
                {
                    if (((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])) || (((string)odatarow["CREDITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                        classificado = true;
                    if ((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"]) || (((string)odatarow["DEBITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
            }
            else
            {
                if ((Boolean)odatarow["TP_FIN"] == true)
                {
                    if ((string)odatarow["TIPO"] == "P")
                    {                                          //  || (((string)odatarow["CREDITO"]).Trim() == "00"))
                        if (((EBancoNormalizado((string)odatarow["CREDITO"], opesquisa, true)))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                            classificado = true;
                    }
                    else
                        if (((EBancoNormalizado((string)odatarow["DEBITO"], opesquisa, true)))//|| (((string)odatarow["DEBITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
                else
                {
                    if (((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])) || (((string)odatarow["CREDITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                        classificado = true;
                    if ((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"]) || (((string)odatarow["DEBITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
            }
            if (invertaSelecao) return !classificado;
            else
            return classificado;

        }
        static public bool EBancoNormalizado(string conta, PesquisaGenerico opesquise, Boolean contabil)
        {
            Boolean result = false;
            if (conta.Trim().Length != 2) return result;
            int intconta;
            try
            { intconta = Convert.ToInt16(conta.Trim()); }
            catch
            {
                return result;
            }
            if (intconta == 0) return result;
            conta = intconta.ToString("D2");

            DataRow[] linhas = opesquise.PesqLinhas("BANCOS", new string[1] { "NBANCO" }, new string[1] { conta });
            if (linhas.Length == 0) return result;
            if (contabil && (Convert.ToString(linhas[0]["CONTAB"]).Trim() == ""))
                return result;
            return true;
        }
        static public Boolean CompareModelo(LinhaSolucao oLinha, DataRow odatarow, PesquisaGenerico opesquisa)
        {
            bool classificado = false;

            if (odatarow.Table.Columns.Contains(oLinha.campo))
            {
                classificado = (oLinha.dado[0].Trim() == odatarow[oLinha.campo].ToString().Trim());
            }


            return classificado;
        }

        static public Boolean CompareGenericoExato(LinhaSolucao oLinha, DataRow odatarow, PesquisaGenerico opesquisa)
        {
            bool classificado = false;

            string[] dados = oLinha.dado[0].Trim().Split(Convert.ToChar("/"));
            if (odatarow.Table.Columns.Contains(oLinha.campo))
            {
                foreach (string dado in dados)
                {
                    // SITUAÇÂO OR (/) SE UM DOS DADOS FOR VERDADEIRO
                    if (dado.Trim() == odatarow[oLinha.campo].ToString().Trim())
                    {
                        classificado = true;
                    }
                }
            }
            return classificado;
        }

        static public Boolean CompareGenericoAprox(LinhaSolucao oLinha, DataRow odatarow, PesquisaGenerico opesquisa)
        {
            bool classificado = false;

            string[] dados = oLinha.dado[0].Trim().Split(Convert.ToChar("/"));
            if (odatarow.Table.Columns.Contains(oLinha.campo))
            {
                foreach (string dado in dados)
                {
                    // SITUAÇÂO OR (/) SE UM DOS DADOS FOR VERDADEIRO
                    if (odatarow[oLinha.campo].ToString().Trim().Contains(dado.Trim()))
                    {
                        classificado = true;
                    }
                }
            }
            return classificado;
        }




        /*
         *  static public string CompareValor(LinhaSolucao oLinha)
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
        static public Boolean CompareValor(LinhaSolucao oLinha, DataRow odatarow, PesquisaGenerico opesquisa)
        {
            bool classificado = false;

            if ((oLinha.dado[0] == "") || (oLinha.dado[1] == "") || (oLinha.campo.Trim() == ""))
                return true;


            try
            {
                double valorcomparar = Convert.ToDouble(odatarow[oLinha.campo]);
                double ovalor = Convert.ToDouble(oLinha.dado[1].Trim());
                if (oLinha.dado[0].ToString().Trim() == "=")
                {
                    classificado = (ovalor == valorcomparar);
                }
                else if (oLinha.dado[0].ToString().Trim() == ">")
                {
                    classificado = (valorcomparar > ovalor);
                }
                else if (oLinha.dado[0].ToString().Trim() == "<")
                {
                    classificado = (valorcomparar < ovalor);
                }
            }
            catch
            {
                classificado = false;
            }
            return classificado;
        }

    }
    static public class RetornaCodigoSql
    {
        static public string CompareValor(LinhaSolucao oLinha)
        {
            string result = "";
            if ((oLinha.dado[0] == "") || (oLinha.dado[1] == "") || (oLinha.campo.Trim() == ""))
                return result;
            string valor = "";
            try
            {
                double ovalor = Convert.ToDouble(oLinha.dado[1].Trim());
                valor = FormatDoubleGravar(ovalor); //2DECIMAIS
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
        static public string ComparePeriodoLote(LinhaSolucao oLinha, DataRow odatarow, PesquisaGenerico opesquisa)
        {
            string result = "";
            if ((oLinha.dado[0] == "") || (oLinha.dado[1] == "") || (oLinha.campo.Trim() == ""))
                return result;
                
                if (oLinha.dado[0].Substring(0, 1) == "1")
                {
                string ostrsql = "( SUBS(DTOS(APRONTE),1,8) >= ''";// +
                     
                        /*  formatdatetime('yyyymmdd', TDateTimePicker(oEdite[linha, 1])
                          .Date) + ''') ' + ' AND ( SUBS(DTOS(APRONTE),1,8) <= ''' +
                     formatdatetime('yyyymmdd', TDateTimePicker(oEdite[linha, 2])
                       .Date) + ''') ';*/
                }
                else if (oLinha.dado[0].Substring(0, 1) == "2")
                { }
              /*ostrsql:= '( SUBS(DTOS(TRANSP),1,8) >= ''' +
               formatdatetime('yyyymmdd', TDateTimePicker(oEdite[linha, 1])
               .Date) + ''') ' + ' AND ( SUBS(DTOS(TRANSP),1,8) <= ''' +
               formatdatetime('yyyymmdd', TDateTimePicker(oEdite[linha, 2])
               .Date) + ''') ';
                end;
                string valor = "";
                try
                {
                    double ovalor = Convert.ToDouble(oLinha.dado[1].Trim());
                    valor = FormatDoubleGravar(ovalor); //2DECIMAIS
                }
                catch
                {
                    throw;
                }

                string comparador = oLinha.dado[0].Trim();
                result = oLinha.campo.Trim() + " " + comparador + " " + valor;
                if (result.Length > 0)
                { result = "  AND (" + result + ")"; }
            }
            else if (linguagem.Trim() == "F")
            {
            
            
            }
              */

                return result;
        }


        /*
         * function ComparePeriodoLote(Pagina_Pesq: TTabSheet1; linha: Integer;
  Linguagem: string): TList;
var
  i: Integer;
  tnumero: double;
  ostrsql: string;
begin
  result := TList.Create;
  with Pagina_Pesq do
    begin
      if (Variavel[linha, 0] = '') or (trim(copy(Variavel[linha, 0], 1, 1)) = '')
      then
        exit;
      if Linguagem = '' then
        begin

          if copy(Variavel[linha, 0], 1, 1) = '1' then
            begin
              ostrsql := '( SUBS(DTOS(APRONTE),1,8) >= ''' +
                formatdatetime('yyyymmdd', TDateTimePicker(oEdite[linha, 1])
                .Date) + ''') ' + ' AND ( SUBS(DTOS(APRONTE),1,8) <= ''' +
                formatdatetime('yyyymmdd', TDateTimePicker(oEdite[linha, 2])
                .Date) + ''') ';
            end
          else if copy(Variavel[linha, 0], 1, 1) = '2' then
            begin
              ostrsql := '( SUBS(DTOS(TRANSP),1,8) >= ''' +
                formatdatetime('yyyymmdd', TDateTimePicker(oEdite[linha, 1])
                .Date) + ''') ' + ' AND ( SUBS(DTOS(TRANSP),1,8) <= ''' +
                formatdatetime('yyyymmdd', TDateTimePicker(oEdite[linha, 2])
                .Date) + ''') ';
            end;
        end  else if Linguagem = 'F' then // linaguagem do adodataset filter
        begin

          if copy(Variavel[linha, 0], 1, 1) = '1' then
            begin
              ostrsql := '( APRONTE >= #' + formatdatetime('dd/mm/yyyy',
                TDateTimePicker(oEdite[linha, 1]).Date) + '#) ' +
                ' AND ( APRONTE <= #' + formatdatetime('dd/mm/yyyy',
                TDateTimePicker(oEdite[linha, 2]).Date) + '#) ';
            end
          else if copy(Variavel[linha, 0], 1, 1) = '2' then
            begin
              ostrsql := '( TRANSP >= #' + formatdatetime('dd/mm/yyyy',
                TDateTimePicker(oEdite[linha, 1]).Date) + '#) ' +
                ' AND ( TRANSP <= #' + formatdatetime('dd/mm/yyyy',
                TDateTimePicker(oEdite[linha, 2]).Date) + '#) ';

            end;

        end;
      result.Add(TFilter.Create('', 'strsql_usuario', ostrsql));
    end;

end;

        */
        static public string exameTransf_Fin(LinhaSolucao oLinha)
        {
            string result = "";
            if ((oLinha.dado[0].Trim() == "") || (oLinha.dado[0].Trim() == "*"))
                return result;
            string ostrsql =
            " ((trim(doc) <> 'SIST_RURAL') AND (trim(debito) <> '') AND EXISTS "
            + "(SELECT desc2 FROM BANCOS WHERE MOV_FIN.debito = desc2) OR " +
            " (trim(doc) <> 'SIST_RURAL') AND (trim(debito) <> '') AND EXISTS "
            + " (SELECT desc2 FROM BANCOS B1 WHERE MOV_FIN.debito = '*' + substring(desc2, 1, 24)) OR "
            + " (trim(doc) <> 'SIST_RURAL') AND (trim(credito) <> '') AND EXISTS "
            + " (SELECT desc2 FROM BANCOS B2 WHERE MOV_FIN.credito = desc2) OR " +
            " (trim(doc) <> 'SIST_RURAL') AND (trim(credito) <> '') AND EXISTS "
            + " (SELECT desc2 FROM BANCOS B3 WHERE MOV_FIN.credito = '*' + substring(desc2, 1, 24)) ) ";

            if (oLinha.dado[0].Substring(0,1) == "-")
            {
               ostrsql = " not ( " + ostrsql + ") ";
            }
            result = " AND " + ostrsql;
            return result;
        }

        static public string passaAprop_Fin(LinhaSolucao oLinha)
        {
            string result = "";
            if ((oLinha.dado[0].Trim() == "") || (oLinha.dado[0].Trim() == "*"))
                return result;
            string ostrsql =  "((outro_id <> 0) and " +
                   "( Exists(select mov_id from ctacentr where (mov_fin.outro_id = ctacentr.mov_id) ) "
                    + " OR Exists(select mov_id from movest where (mov_fin.outro_id = movest.mov_id) )) ) ";
            if (oLinha.dado[0].Substring(0, 1) == "N")
            {
                ostrsql = " not ( " + ostrsql + ") ";
            }
            result = ostrsql;
            return result;
        }

        static public Dictionary<string,string> passa_Contab_Fin(LinhaSolucao oLinha)
        {
            Dictionary<string, string> odic = new Dictionary<string, string>();
            if ((oLinha.dado[0].Trim() == "") || (oLinha.dado[0].Trim() == "*"))
                return odic;

            string campo = oLinha.campo;
            char[] separador = new char[] { '/' };

            string[] campos = campo.Split(separador);
            

            string strReceberFin = "Exists(select desc2, numconta  from placon where (mov_fin.credito = desc2) and(substring(numconta, 6, 3) <> '000')) "
                   + " and((substring(mov_fin.Debito, 1, 2) = '00')   or((LEN(dbo.ALLTRIM(mov_fin.debito)) = 2)  and "
                   + " Exists(select nbanco, contab  from bancos where "
                   + " (mov_fin.debito = IIF(LEN(dbo.ALLTRIM(str(bancos.nbanco, 2))) = 2, str(bancos.nbanco, 2) + '                       ', "
                   + " '0' + str(bancos.nbanco, 1) + '                       ')) "
                   + "  and(dbo.ALLTRIM(CONTAB) <> '')))) ";

            string strPagarFin =
                      "Exists(select desc2, numconta  from placon where (mov_fin.DEBITO = desc2) and(substring(numconta, 6, 3) <> '000')) "
                 + " and((substring(mov_fin.CREDITO, 1, 2) = '00')   or((LEN(dbo.ALLTRIM(mov_fin.CREDITO)) = 2)  and "
                 + " Exists(select nbanco, contab  from bancos where "
                 + " (mov_fin.CREDITO = IIF(LEN(dbo.ALLTRIM(str(bancos.nbanco, 2))) = 2, str(bancos.nbanco, 2) + '                       ', "
                 + " '0' + str(bancos.nbanco, 1) + '                       ')) "
                 + "  and(dbo.ALLTRIM(CONTAB) <> '')))) ";


            if (oLinha.dado[0].Substring(0, 1) == "N")
            {
                strReceberFin = " not ( " + strReceberFin + ") ";
                strPagarFin = " not ( " + strPagarFin + ") ";
            }
            else if (oLinha.dado[0].Substring(0, 1) == "S")
            {
                strReceberFin = " ( " + strReceberFin + ") ";
                strPagarFin = "  ( " + strPagarFin + ") ";
            }
            


            foreach(string chave in campos)
            {
                if (chave.ToUpper().Trim() == "DEBITO")
                {
                    odic.Add(chave.ToUpper().Trim(), strReceberFin);
                }
                if (chave.ToUpper().Trim() == "CREDITO")
                {
                    odic.Add(chave.ToUpper().Trim(), strPagarFin);
                }
            }



            return odic;
        }



        /*
         * function fPassa_Contab_Fin(Pagina_Pesq: TTabSheet1; linha: Integer): TList;
var
  i, ind: Integer;
  ofiltro: TFilter;
  strReceberFin, strPagarFin: string;
  strReceber, strPagar: string;
  Campos: TStrings;
  tcampo: string;
begin
  result := TList.Create;
  with Pagina_Pesq do
    begin

      if (Variavel[linha, 0] = '') or (Variavel[linha, 0] = '*') then
        exit;

      tcampo := campo[linha];
      Campos := TStringList.Create;
      ind := pos('/', tcampo);
      while (ind <> 0) do
        begin
          Campos.Add(copy(tcampo, 1, (ind - 1)));
          delete(tcampo, 1, ind);
          ind := pos('/', tcampo);
        end;
      if tcampo <> '' then
        Campos.Add(tcampo);

      strReceberFin :=
        ' Exists (select desc2, numconta  from placon where (mov_fin.credito = desc2) and (substr(numconta,6,3)<>''000'')) '
        + ' and (  (substr(mov_fin.Debito,1,2) = ''00'')   or  ( ( LEN(ALLTRIM(mov_fin.debito)) = 2)  and  '
        + ' Exists ( select nbanco,contab  from bancos where ' +
        ' ( mov_fin.debito = IIF( LEN(ALLTRIM(str(bancos.nbanco,2) )  )  =2, str(bancos.nbanco,2)+''                       '', '
        + ' ''0''+str(bancos.nbanco,1)+''                       '') ) ' +
        ' and  (ALLTRIM(CONTAB) <> '''') ) )  ) ';

      strPagarFin :=
        ' Exists (select desc2, numconta  from placon where (mov_fin.debito = desc2) and (substr(numconta,6,3)<>''000'')) '
        + ' and (  (substr(mov_fin.credito,1,2) = ''00'')   or  ( ( LEN(ALLTRIM(mov_fin.credito)) = 2)  and  '
        + ' Exists ( select nbanco,contab  from bancos where ' +
        ' ( mov_fin.credito = IIF( LEN(ALLTRIM(str(bancos.nbanco,2) )  )  =2, str(bancos.nbanco,2)+''                       '', '
        + ' ''0''+str(bancos.nbanco,1)+''                       '') ) ' +
        ' and  (ALLTRIM(CONTAB) <> '''') ) ) ) ';

      if (copy(Variavel[linha, 0], 1, 1) = 'N') then
        begin
          strReceberFin := ' not ( ' + strReceberFin + ' ) ';
          strPagarFin := ' not ( ' + strPagarFin + ' ) ';
        end;

      result.Clear();
      for i := 0 to (Campos.count - 1) do
        begin
          if (uppercase(trim(Campos[i])) = 'DEBITO') then
            result.Add(TFilter.Create(Campos[i], 'strsql_usuario',
              strReceberFin))
          else if (uppercase(trim(Campos[i])) = 'CREDITO') then
            result.Add(TFilter.Create(Campos[i], 'strsql_usuario',
              strPagarFin));
        end;

    end;
end;

         * */


        // NAO É NECESSÁRIA ESTA FUNCÃO -- SPLIT resolve
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




        static public string FormatDoubleGravar(double ovalor)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            string result = ovalor.ToString("G", ci);
            return result;
        }
        static public string FormatDoubleGravar(double ovalor, int num)
        {
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            string result = Math.Round(ovalor,num).ToString("G", ci);
            return result;
        }

    }
}