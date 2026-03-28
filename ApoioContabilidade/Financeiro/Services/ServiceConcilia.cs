using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Financeiro.Services
{
    public static class ServiceConcilia
    {

        static private void PesquisaMaisRacional(DataTable bancoExtrato, DataTable mlsaExtrato, Int32 numConcilia = 0)
        {
            // FORNECEDORES COM PAGAMENTOS NA MESMA DATA DO LANÇAMENTO
            var fornecedores = (from gr in mlsaExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                                && (row.Field<Int32>("CONCILIA") == 0)
                                 && (row.Field<string>("DOC_FISC").Trim() != ""))
                                group gr by new
                                {
                                    data = gr.Field<DateTime>("DATA"),
                                    lanc = gr.Field<string>("LANC").Trim()
                                } into g
                                select new
                                {
                                    data = g.Key.data,
                                    lanc = g.Key.lanc,
                                    valor = g.Sum(row => row.Field<Decimal>("VALOR"))
                                });
            if (fornecedores != null)
            {
                foreach (var obj in fornecedores)
                {
                    DateTime data = obj.data;
                    if (data.DayOfWeek == DayOfWeek.Saturday)
                        data = data.AddDays(-1);
                    else if (data.DayOfWeek == DayOfWeek.Sunday)
                        data = data.AddDays(-2);

                    Decimal valor = obj.valor;
                    string lanc = obj.lanc;
                    List<string> lstCodHistBB = new List<string>();
                    lstCodHistBB.Add("109"); // boleto
                    lstCodHistBB.Add("002"); // cheque
                    lstCodHistBB.Add("470"); // transf enviada
                    lstCodHistBB.Add("144"); // transf agendada
                    var orowsBBForn = bancoExtrato.AsEnumerable().Where(row => (row.Field<Decimal>("VALOR") == valor) && (row.Field<Int32>("CONCILIA") == 0)
                                     && (row.Field<DateTime>("DATA").CompareTo(data) == 0)
                                     && lstCodHistBB.Contains(row.Field<String>("CODHISTORICO").Trim())

                                     );
                    if (orowsBBForn == null) continue;
                    if (orowsBBForn.Count() == 1)
                    {
                        //&& (row.Field<double>("OUTRO_ID") != 0)
                        var orows = mlsaExtrato.AsEnumerable().Where(row => (row.Field<string>("DOC_FISC").Trim() != "") && (row.Field<Int32>("CONCILIA") == 0)
                              && (row.Field<string>("DBCR").Trim() == "D") && (row.Field<Int32>("CONCILIA") == 0)
                                  // && (row.Field<double>("OUTRO_ID") != 0)
                                  && (row.Field<string>("LANC").ToString().Trim() == obj.lanc)
                                && (row.Field<DateTime>("DATA").CompareTo(obj.data) == 0));

                        if (orows == null) continue;
                        if (orows.Count() == 0) continue;
                        DataRow orowBB = orowsBBForn.FirstOrDefault();
                        numConcilia++;
                        orowBB.BeginEdit();
                        orowBB["CONCILIA"] = numConcilia;
                        orowBB.BeginEdit();
                        orowBB.AcceptChanges();

                        foreach (DataRow orow in orows)
                        {
                            orow.BeginEdit();
                            orow["CONCILIA"] = numConcilia;
                            orow["HISTBB"] = orowBB["HISTORICO"];
                            orow["DATABB"] = orowBB["DATA"];

                            orow.BeginEdit();
                            orow.AcceptChanges();
                        }
                    }
                }
            }

            // FORNECEDORES COM PAGAMENTOS EM DATAS POSTERIORES
            var fornecedores_atraso = (from gr in mlsaExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                                && (row.Field<Int32>("CONCILIA") == 0)
                                 && (row.Field<string>("DOC_FISC").Trim() != ""))
                                       group gr by new
                                       {
                                           data = gr.Field<DateTime>("DATA"),
                                           lanc = gr.Field<string>("LANC").Trim()
                                       } into g
                                       select new
                                       {
                                           data = g.Key.data,
                                           lanc = g.Key.lanc,
                                           valor = g.Sum(row => row.Field<Decimal>("VALOR"))
                                       });
            if (fornecedores_atraso != null)
            {
                foreach (var obj in fornecedores)
                {
                    DateTime data = obj.data;
                    if (data.DayOfWeek == DayOfWeek.Saturday)
                        data = data.AddDays(-1);
                    else if (data.DayOfWeek == DayOfWeek.Sunday)
                        data = data.AddDays(-2);

                    Decimal valor = obj.valor;
                    string lanc = obj.lanc;
                    List<string> lstCodHistBB = new List<string>();
                    lstCodHistBB.Add("109"); // boleto
                    lstCodHistBB.Add("002"); // cheque
                    lstCodHistBB.Add("470"); // transf enviada
                    lstCodHistBB.Add("144"); // transf agendada
                    var orowsBBForn = bancoExtrato.AsEnumerable().Where(row => (row.Field<Decimal>("VALOR") == valor) && (row.Field<Int32>("CONCILIA") == 0)
                                     && (row.Field<DateTime>("DATA").CompareTo(data) != 0)
                                     && lstCodHistBB.Contains(row.Field<String>("CODHISTORICO").Trim())

                                     );
                    if (orowsBBForn == null) continue;
                    if (orowsBBForn.Count() == 1)
                    {
                        //&& (row.Field<double>("OUTRO_ID") != 0)
                        var orows = mlsaExtrato.AsEnumerable().Where(row => (row.Field<string>("DOC_FISC").Trim() != "") && (row.Field<Int32>("CONCILIA") == 0)
                              && (row.Field<string>("DBCR").Trim() == "D") && (row.Field<Int32>("CONCILIA") == 0)
                                  // && (row.Field<double>("OUTRO_ID") != 0)
                                  && (row.Field<string>("LANC").ToString().Trim() == obj.lanc)
                                && (row.Field<DateTime>("DATA").CompareTo(obj.data) == 0));

                        if (orows == null) continue;
                        if (orows.Count() == 0) continue;
                        DataRow orowBB = orowsBBForn.FirstOrDefault();
                        numConcilia++;
                        orowBB.BeginEdit();
                        orowBB["CONCILIA"] = numConcilia;
                        orowBB.BeginEdit();
                        orowBB.AcceptChanges();

                        foreach (DataRow orow in orows)
                        {
                            orow.BeginEdit();
                            orow["CONCILIA"] = numConcilia;
                            orow["HISTBB"] = orowBB["HISTORICO"];
                            orow["DATABB"] = orowBB["DATA"];

                            orow.BeginEdit();
                            orow.AcceptChanges();
                        }
                    }
                }
            }


            var transferencias =
                (from gr in (bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                               && (row.Field<string>("CODHISTORICO").Trim() == "470")))
                 group gr by new
                 {
                     codHist = gr.Field<string>("CODHISTORICO"),
                     ndoc = gr.Field<string>("NUMERODOCUMENTO"),
                     detalhe = gr.Field<string>("DETALHAHISTORICO").Trim()
                 } into g
                 select new
                 {

                     codhist = g.Key.codHist,
                     ndoc = g.Key.ndoc,
                     detalhe = g.Key.detalhe,
                     lstData = g.Select(orow => orow.Field<DateTime>("DATA")).ToList(),
                     lstID = g.Select(orow => orow.Field<Int32>("ID")).ToList()
                 });


            if (numConcilia == 0)
            {
                foreach (DataRow orow in mlsaExtrato.Rows)
                {
                    orow.BeginEdit();
                    orow["CONCILIA"] = 0;
                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                bancoExtrato.AcceptChanges();
                foreach (DataRow orow in bancoExtrato.Rows)
                {
                    orow.BeginEdit();
                    orow["CONCILIA"] = 0;


                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                bancoExtrato.AcceptChanges();
            }
            /// PAGAMENTO DAS FOLHAS 
            /// 

            // ENCONTRE ToDOS OS CANDIDATOS A FOLHA NOS LANÇAMENTOS DA MLSA
            string labProvFolha = "PROV # FOLHA";
            var pagFolha_Agrupados =

                      (from gr in mlsaExtrato.AsEnumerable().Where(row =>
                  (((row.Field<string>("DOC_FISC").Trim().Length > 5) &&
                             (row.Field<string>("DOC_FISC").Substring(0, 5) == "SIST_"))
                             || (row.Field<string>("LANC").Trim() == labProvFolha.Trim()))
                              && (row.Field<Int32>("CONCILIA") == 0)
                                  )
                       group gr by new
                       {
                           data = gr.Field<DateTime>("DATA")
                       } into g
                       select new
                       {
                           data = g.Key.data,
                           valor = g.Sum(orow => orow.Field<Decimal>("VALOR")),
                           lstvalor = g.Select(orow => orow.Field<Decimal>("VALOR")).ToList(),
                           lstID = g.Select(orow => orow.Field<Double>("MOV_ID")).ToList()
                       });
            var orowsBB = bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("CODHISTORICO").Trim() == "250") && row.Field<Int32>("CONCILIA") == 0);
            if (orowsBB != null)
            {
                foreach (DataRow orowBB in orowsBB)
                {
                    DateTime data = Convert.ToDateTime(orowBB["DATA"]);
                    Decimal valor = Convert.ToDecimal(orowBB["VALOR"]);
                    // pegar a proxima data de folha (se houver)
                    var dadoEncontrou = pagFolha_Agrupados.Where(a => a.valor == valor).Select(a => a.lstID);
                    if (dadoEncontrou != null)
                    {
                        List<Double> lista = dadoEncontrou.FirstOrDefault();

                        if (lista == null) continue;

                        var orows = mlsaExtrato.AsEnumerable().Where(row =>
                            (((row.Field<string>("DOC_FISC").Trim().Length > 5) &&
                                       (row.Field<string>("DOC_FISC").Substring(0, 5) == "SIST_"))
                                       || (row.Field<string>("LANC").Trim() == labProvFolha.Trim()))
                              && (row.Field<Int32>("CONCILIA") == 0)
                                         && (row.Field<DateTime>("DATA").CompareTo(data) >= 0)
                                         && lista.Contains(row.Field<Double>("MOV_ID"))
                                         );

                        if (orows == null) return;
                        numConcilia++;

                        orowBB.BeginEdit();
                        orowBB["CONCILIA"] = numConcilia;
                        orowBB.BeginEdit();
                        orowBB.AcceptChanges();

                        foreach (DataRow orow in orows)
                        {

                            orow.BeginEdit();
                            orow["CONCILIA"] = numConcilia;
                            orow["HISTBB"] = orowBB["HISTORICO"];
                            orow["DATABB"] = orowBB["DATA"];
                            orow.BeginEdit();
                            orow.AcceptChanges();
                        }
                    }
                }
            }
            //// CHEQUES
            // CHECA DIRETAMENTE O NUMERO DO CHEQUE
            var cheques =
                  (from gr in (bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                  && (row.Field<string>("CODHISTORICO").Trim() == "002") && (row.Field<Int32>("CONCILIA") == 0)))
                   select new
                   {
                       codHist = gr.Field<string>("CODHISTORICO"),
                       chequenum = "CH" + Convert.ToUInt32(gr.Field<string>("NUMERODOCUMENTO")).ToString().Trim(),
                       data = gr.Field<DateTime>("DATA"),
                       valor = gr.Field<Decimal>("VALOR"),
                       id = gr.Field<Int32>("ID")
                   });
            foreach (var ocheque in cheques)
            {
                var orowBB = bancoExtrato.AsEnumerable().Where(row => (row.Field<Int32>("ID") == ocheque.id) && row.Field<Int32>("CONCILIA") == 0).FirstOrDefault();
                if (orowBB != null)
                {
                    var orows = mlsaExtrato.AsEnumerable().Where(row =>
                         (row.Field<string>("DOC").Trim() == ocheque.chequenum.Trim())
                          && (row.Field<Int32>("CONCILIA") == 0)
                                     );

                    if (orows == null) return;
                    numConcilia++;

                    orowBB.BeginEdit();
                    orowBB["CONCILIA"] = numConcilia;
                    orowBB.BeginEdit();
                    orowBB.AcceptChanges();

                    foreach (DataRow orow in orows)
                    {

                        orow.BeginEdit();
                        orow["CONCILIA"] = numConcilia;
                        orow["HISTBB"] = orowBB["HISTORICO"];
                        orow["DATABB"] = orowBB["DATA"];
                        orow.BeginEdit();
                        orow.AcceptChanges();
                    }
                }
            }

            /// PAGAMENTO DE IMPOSTOS
            /// 1 ETAPA RELACAO 1 a 1 
            var impostos_MesmoValor = bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                  && (row.Field<string>("CODHISTORICO").Trim() == "375") && (row.Field<Int32>("CONCILIA") == 0));

            foreach (DataRow orowBB in impostos_MesmoValor)
            {

                Decimal valor = Convert.ToDecimal(orowBB["VALOR"]);



                var orow = mlsaExtrato.AsEnumerable().Where(row =>
                         ((row.Field<string>("LANC").Trim() == "CTA_PG # IMPOSTOS")
                          || (row.Field<string>("LANC").Trim() == "21139026")
                         || (row.Field<string>("FORN").Trim().Contains("CAIXA") && row.Field<string>("FORN").Trim().Contains("FEDERAL")))
                          && (row.Field<Int32>("CONCILIA") == 0)
                          && (row.Field<Decimal>("VALOR") == valor)).FirstOrDefault();

                if (orow == null) continue;

                numConcilia++;

                orowBB.BeginEdit();
                orowBB["CONCILIA"] = numConcilia;
                orowBB.BeginEdit();
                orowBB.AcceptChanges();

                orow.BeginEdit();
                orow["CONCILIA"] = numConcilia;
                orow["HISTBB"] = orowBB["HISTORICO"];
                orow["DATABB"] = orowBB["DATA"];
                orow.BeginEdit();
                orow.AcceptChanges();
            }


            var impostosMLSA =
                      (from gr in
                           mlsaExtrato.AsEnumerable().Where(row =>
                         ((row.Field<string>("LANC").Trim() == "CTA_PG # IMPOSTOS")
                         || (row.Field<string>("FORN").Trim().Contains("CAIXA") && row.Field<string>("FORN").Trim().Contains("FEDERAL")))
                          && (row.Field<Int32>("CONCILIA") == 0)
                       )
                       group gr by new
                       {
                           data = gr.Field<DateTime>("DATA")
                       } into g
                       select new
                       {
                           data = g.Key.data,
                           valor = g.Sum(orow => orow.Field<Decimal>("VALOR")),
                           lstvalor = g.Select(orow => orow.Field<Decimal>("VALOR")).ToList(),
                           lstID = g.Select(orow => orow.Field<Double>("MOV_ID")).ToList()
                       });

            var impostos = bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                  && (row.Field<string>("CODHISTORICO").Trim() == "375") && (row.Field<Int32>("CONCILIA") == 0));
            foreach (DataRow orowBB in impostos)
            {

                Decimal valor = Convert.ToDecimal(orowBB["VALOR"]);

                var rowMatch = impostosMLSA.Where(a => a.valor == valor);
                if ((rowMatch == null) || (rowMatch.Count() == 0)) continue;
                var oImpMlsa = rowMatch.FirstOrDefault();

                var orows = mlsaExtrato.AsEnumerable().Where(row =>

                        oImpMlsa.lstID.Contains(row.Field<Double>("MOV_ID"))
                            && (row.Field<Int32>("CONCILIA") == 0)

                                     // &&  (row.Field<DateTime>("DATA").CompareTo(oimposto.data) >= 0)
                                     );

                if (orows == null) continue;
                if (orows.Count() == 0) continue;

                numConcilia++;

                orowBB.BeginEdit();
                orowBB["CONCILIA"] = numConcilia;
                orowBB.BeginEdit();
                orowBB.AcceptChanges();

                foreach (DataRow orow in orows)
                {

                    orow.BeginEdit();
                    orow["CONCILIA"] = numConcilia;
                    orow["HISTBB"] = orowBB["HISTORICO"];
                    orow["DATABB"] = orowBB["DATA"];
                    orow.BeginEdit();
                    orow.AcceptChanges();
                }

            }



            /// INSS 
            /// var impostosMLSA =
            var inss_Mlsa = (from gr in
                           mlsaExtrato.AsEnumerable().Where(row =>
                         (row.Field<string>("LANC").Trim() == "IMP # INSS")
                          // || (row.Field<string>("FORN").Trim().Contains("CAIXA") && row.Field<string>("FORN").Trim().Contains("FEDERAL")))
                          && (row.Field<Int32>("CONCILIA") == 0)
                       )
                             group gr by new
                             {
                                 data = gr.Field<DateTime>("DATA")
                             } into g
                             select new
                             {
                                 data = g.Key.data,
                                 valor = g.Sum(orow => orow.Field<Decimal>("VALOR")),
                                 lstvalor = g.Select(orow => orow.Field<Decimal>("VALOR")).ToList(),
                                 lstID = g.Select(orow => orow.Field<Double>("MOV_ID")).ToList()
                             });

            var inssBB = bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                  && (row.Field<string>("CODHISTORICO").Trim() == "196") && (row.Field<Int32>("CONCILIA") == 0));
            foreach (DataRow orowBB in inssBB)
            {
                Decimal valor = Convert.ToDecimal(orowBB["VALOR"]);

                var rowMatch = inss_Mlsa.Where(a => a.valor == valor);
                if ((rowMatch == null) || (rowMatch.Count() == 0)) continue;
                var oImpMlsa = rowMatch.FirstOrDefault();

                var orows = mlsaExtrato.AsEnumerable().Where(row =>

                        oImpMlsa.lstID.Contains(row.Field<Double>("MOV_ID"))
                            && (row.Field<Int32>("CONCILIA") == 0)

                                     // &&  (row.Field<DateTime>("DATA").CompareTo(oimposto.data) >= 0)
                                     );

                if (orows == null) continue;
                if (orows.Count() == 0) continue;

                numConcilia++;

                orowBB.BeginEdit();
                orowBB["CONCILIA"] = numConcilia;
                orowBB.BeginEdit();
                orowBB.AcceptChanges();

                foreach (DataRow orow in orows)
                {
                    orow.BeginEdit();
                    orow["CONCILIA"] = numConcilia;
                    orow["HISTBB"] = orowBB["HISTORICO"];
                    orow["DATABB"] = orowBB["DATA"];
                    orow.BeginEdit();
                    orow.AcceptChanges();
                }

            }



            // Tarifas varios lançamentos no contra 1 mlsa <verificar a conta ativa do bb)
            // CTA_PG # BRASIL
            var tarifasBB =
                  (from gr in (bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                  && (row.Field<string>("CODHISTORICO").Trim() == "170") && (row.Field<Int32>("CONCILIA") == 0)))
                   group gr by new
                   {
                       data = gr.Field<DateTime>("DATA")
                   } into g
                   select new
                   {
                       data = g.Key.data,
                       valor = g.Sum(orow => orow.Field<Decimal>("VALOR")),
                       lstvalor = g.Select(orow => orow.Field<Decimal>("VALOR")).ToList(),
                       lstID = g.Select(orow => orow.Field<Int32>("ID")).ToList()
                   });

            foreach (var otarifas in tarifasBB)
            {
                var orows = mlsaExtrato.AsEnumerable().Where(row =>
                        (row.Field<string>("LANC").Trim() == "CTA_PG # BRASIL")
                         && (row.Field<Int32>("CONCILIA") == 0)
                         && (row.Field<Decimal>("VALOR") == otarifas.valor)
                         && (row.Field<DateTime>("DATA").CompareTo(otarifas.data) == 0)
                         );

                if (orows != null)
                {
                    // nos lançamentos de MLSA deverá ser somente um 
                    if (orows.Count() != 1) continue;

                    List<Int32> lista = otarifas.lstID;

                    var orowBB = bancoExtrato.AsEnumerable().Where(row => otarifas.lstID.Contains(row.Field<Int32>("ID")) && row.Field<Int32>("CONCILIA") == 0);
                    numConcilia++;
                    // só um 
                    string hist = "";
                    DateTime data = new DateTime();
                    foreach (DataRow orow in orowBB)
                    {

                        orow.BeginEdit();
                        orow["CONCILIA"] = numConcilia;
                        hist = orow["HISTORICO"].ToString();
                        data = Convert.ToDateTime(orow["DATA"]);
                        orow.BeginEdit();
                        orow.AcceptChanges();
                    }
                    foreach (DataRow orow in orows)
                    {

                        orow.BeginEdit();
                        orow["CONCILIA"] = numConcilia;
                        orow["HISTBB"] = hist;
                        orow["DATABB"] = data;
                        orow.BeginEdit();
                        orow.AcceptChanges();
                    }

                }
            }


            // &&  (row.Field<DateTime>("DATA").CompareTo(oimposto.data) >= 0)
            var bbGiro =
                  (from gr in (bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "D")
                  && (row.Field<string>("CODHISTORICO").Trim() == "177") && (row.Field<Int32>("CONCILIA") == 0)))
                   select new
                   {
                       data = gr.Field<DateTime>("DATA"),
                       valor = gr.Field<Decimal>("VALOR"),
                       id = gr.Field<Int32>("ID")
                   });
            foreach (var ogiro in bbGiro)
            {
                var orowBB = bancoExtrato.AsEnumerable().Where(row => (row.Field<Int32>("ID") == ogiro.id) && row.Field<Int32>("CONCILIA") == 0).FirstOrDefault();
                if (orowBB != null)
                {
                    var orows = mlsaExtrato.AsEnumerable().Where(row =>
                         ((row.Field<string>("LANC").Trim() == "CTA_PG # BRASIL")
                         || (row.Field<string>("LANC").Trim() == "BCO_BRASIL # CURTO"))
                          && (row.Field<Int32>("CONCILIA") == 0)
                          && (row.Field<DateTime>("DATA") == ogiro.data)
                                     // &&  (row.Field<DateTime>("DATA").CompareTo(oimposto.data) >= 0)
                                     );

                    if (orows == null) continue;
                    if (ogiro.valor != orows.Sum(orow => orow.Field<Decimal>("VALOR"))) continue;

                    numConcilia++;

                    orowBB.BeginEdit();
                    orowBB["CONCILIA"] = numConcilia;
                    orowBB.BeginEdit();
                    orowBB.AcceptChanges();

                    foreach (DataRow orow in orows)
                    {
                        orow.BeginEdit();
                        orow["CONCILIA"] = numConcilia;
                        orow["HISTBB"] = orowBB["HISTORICO"];
                        orow["DATABB"] = orowBB["DATA"];

                        orow.BeginEdit();
                        orow.AcceptChanges();
                    }
                }
            }


            // LIQUIDAÇÂO CAMBIO

            var liqCambio =
                  (from gr in (bancoExtrato.AsEnumerable().Where(row => (row.Field<string>("DBCR").Trim() == "C")
                  && (row.Field<string>("CODHISTORICO").Trim() == "656") && (row.Field<Int32>("CONCILIA") == 0)))
                   select new
                   {
                       data = gr.Field<DateTime>("DATA"),
                       valor = gr.Field<Decimal>("VALOR"),
                       id = gr.Field<Int32>("ID")
                   });
            foreach (var oliq in liqCambio)
            {
                var orowBB = bancoExtrato.AsEnumerable().Where(row => (row.Field<Int32>("ID") == oliq.id) && row.Field<Int32>("CONCILIA") == 0).FirstOrDefault();
                if (orowBB != null)
                {
                    var orows = mlsaExtrato.AsEnumerable().Where(row =>
                         ((row.Field<string>("LANC").Trim() == "CTA_RC # BRASIL")
                         || (row.Field<string>("LANC").Trim().Contains("VALRHONA")))
                          && (row.Field<Int32>("CONCILIA") == 0)
                          && (row.Field<DateTime>("DATA") == oliq.data)
                                     // &&  (row.Field<DateTime>("DATA").CompareTo(oimposto.data) >= 0)
                                     );

                    if (orows == null) continue;
                    if (oliq.valor != orows.Sum(orow => orow.Field<Decimal>("VALOR"))) continue;

                    numConcilia++;

                    orowBB.BeginEdit();
                    orowBB["CONCILIA"] = numConcilia;
                    orowBB.BeginEdit();
                    orowBB.AcceptChanges();

                    foreach (DataRow orow in orows)
                    {
                        orow.BeginEdit();
                        orow["CONCILIA"] = numConcilia;
                        orow["HISTBB"] = orowBB["HISTORICO"];
                        orow["DATABB"] = orowBB["DATA"];

                        orow.BeginEdit();
                        orow.AcceptChanges();
                    }
                }
            }

            // 





            // POR ULTIMO A RELAÇÂO 1 para 1 com base no VALOR E DATA
            mlsaExtrato.AcceptChanges();
            // GERAL
            foreach (DataRow orow in mlsaExtrato.Rows)
            {
                DateTime data = Convert.ToDateTime(orow["DATA"]);
                if (data.DayOfWeek == DayOfWeek.Saturday)
                    data = data.AddDays(-1);
                else if (data.DayOfWeek == DayOfWeek.Sunday)
                    data = data.AddDays(-2);
                Decimal valor = Convert.ToDecimal(orow["VALOR"]);
                string DBCR = Convert.ToString(orow["DBCR"]);
                var rowsBB = bancoExtrato.AsEnumerable().Where(row => (row.Field<DateTime>("DATA").CompareTo(data) >= 0) && (row.Field<Decimal>("VALOR") == valor)
                && (row.Field<string>("DBCR") == DBCR));
                if (rowsBB == null) continue;
                if (rowsBB.Count() == 1)
                {
                    DataRow orowBB = rowsBB.FirstOrDefault();
                    numConcilia++;
                    orow.BeginEdit();
                    orow["CONCILIA"] = numConcilia;
                    orow["HISTBB"] = orowBB["HISTORICO"];
                    orow["DATABB"] = orowBB["DATA"];
                    orow.BeginEdit();
                    orow.AcceptChanges();
                    orowBB.BeginEdit();
                    orowBB["CONCILIA"] = numConcilia;
                    orowBB.BeginEdit();
                    orowBB.AcceptChanges();
                }
            }
            mlsaExtrato.AcceptChanges();
            bancoExtrato.AcceptChanges();



        }


        public static  void criaAsRelaçoesdeConciliacao(DataTable bancoExtrato, DataTable mlsaExtrato)
        {

            Int32 numConcilia = 0;
            // Inicializa os valores Dos campos No MLSAEXTRATO
            foreach (DataRow orow in mlsaExtrato.Rows)
            {
                orow.BeginEdit();
                orow["CONCILIA"] = 0;
                orow["DATABB"] = new DateTime();
                orow["HISTBB"] = "";
                orow.EndEdit();
            }

            // LANCAMENTOS DO BB DIVERSOS QUE PARA NÓS Só RESULTA EM 1 LANÇAMENTO
            var documentosBB = (from gr in bancoExtrato.AsEnumerable().Where(row =>
                            (row.Field<Int32>("CONCILIA") == 0) && (row.Field<Decimal>("VALOR") != 0)
                             )
                                group gr by new
                                {
                                    data = gr.Field<DateTime>("DATA"),
                                    docBB = gr.Field<string>("numeroDocumento").Trim()
                                } into g
                                select new
                                {
                                    data = g.Key.data,
                                    docBB = g.Key.docBB,
                                    valor = g.Sum(row => row.Field<string>("DBCR").Trim() == "D" ? row.Field<Decimal>("VALOR") : (row.Field<Decimal>("VALOR") * -1))
                                });
            if (documentosBB != null)
            {
                foreach (var obj in documentosBB)
                {
                    DateTime data = obj.data;
                    if (data.DayOfWeek == DayOfWeek.Saturday)
                        data = data.AddDays(-1);
                    else if (data.DayOfWeek == DayOfWeek.Sunday)
                        data = data.AddDays(-2);

                    Decimal valor = obj.valor;
                    string tipo = "D";
                    if (valor < 0)
                    {
                        valor = valor * -1;
                        tipo = "C";
                    }
                    string docBB = obj.docBB;
                    var orowsmlsa = mlsaExtrato.AsEnumerable().Where(row => (row.Field<Decimal>("VALOR") == valor) && (row.Field<Int32>("CONCILIA") == 0)
                                     && (row.Field<DateTime>("DATA").CompareTo(data) == 0) &&
                                     (row.Field<string>("DBCR").Trim() == tipo));
                    if (orowsmlsa == null) continue;
                    if (orowsmlsa.Count() == 1)
                    {
                        //&& (row.Field<double>("OUTRO_ID") != 0)
                        var orows = bancoExtrato.AsEnumerable().Where(row => (row.Field<Int32>("CONCILIA") == 0)
                                  && (row.Field<string>("numeroDocumento").ToString().Trim() == obj.docBB.Trim())
                                && (row.Field<DateTime>("DATA").CompareTo(obj.data) == 0));

                        if (orows == null) continue;
                        if (orows.Count() == 0) continue;
                        DataRow orowmlsa = orowsmlsa.FirstOrDefault();
                        numConcilia++;
                        orowmlsa.BeginEdit();
                        orowmlsa["CONCILIA"] = numConcilia;
                        orowmlsa["HISTBB"] = "divs docBB" + obj.docBB.Trim();
                        orowmlsa["DATABB"] = obj.data;

                        orowmlsa.BeginEdit();
                        orowmlsa.AcceptChanges();

                        foreach (DataRow orow in orows)
                        {
                            orow.BeginEdit();
                            orow["CONCILIA"] = numConcilia;

                            orow.BeginEdit();
                            orow.AcceptChanges();
                        }
                    }
                }
            }

            mlsaExtrato.AcceptChanges();
            bancoExtrato.AcceptChanges();
            PesquisaMaisRacional(bancoExtrato, mlsaExtrato, numConcilia);
        }






    }
}
