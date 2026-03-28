using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using ClassConexao;
using System.Data.OleDb;

namespace ClassLibTrabalho
{
    class Class_Produto_Rateio
    {
        static public DataSet GetCatProduto()
        {
            OleDbCommand oledbcomm;
            string stroledb, path;

            path = TDataControlReduzido.Get_Path("TRABALHO");
            stroledb = "SELECT        COD, NATURA, UNID_BENE, CONVERSAO, PARCERIA,REINVEST,DESC AS DESCRICAO " +
             "  FROM         " + path + "CATPROD" +
              " ORDER BY COD";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "CATPROD");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        static public DataSet GetLote(string safra, string setor)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;

            path = TDataControlReduzido.Get_Path("TRABALHO");
            stroledb = "SELECT SETOR, PROD, SAFRA, LOTE, U_BENEF, DATAINI, DATAFIM, NATURA, BENEF, TRANSP, APRONTE, FECHA, QUANT_FR, QUANT_PA, QUANT_RI, VQUANT_FR, VQUANT_PA, VLR_FR, VLR_PA, " +
                "     VLR_RI, B_RATEIO, B_VENDA " +
             "  FROM         " + path + "LOTE " +
             " WHERE (SAFRA = '" + safra + "') AND (SETOR = '" + setor + "') AND (prod = '  1') " +
              " ORDER BY LOTE";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "LOTE");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
       

        static public DataTable CalculeLote(DataTable prodtestemunha, DataTable Segue, DataTable parceiros, decimal parceria, decimal reinvest)
        {
            var dados_prodtestemunha =
                           from linha in prodtestemunha.AsEnumerable()
                           where (linha.Field<string>("SETOR") == " 9")
                           group linha by new
                           {
                               lote = linha.Field<string>("LOTE"),
                               setor = linha.Field<string>("SETOR"),
                               safra = linha.Field<string>("Safra")

                           } into g
                           orderby g.Key.lote
                           select new
                           {
                               lote = g.Key.lote,
                               setor = g.Key.setor,
                               safra = g.Key.safra,
                               caixas = g.Sum((linha => linha.Field<Decimal?>("caixas")))

                           };

            // DateTime tdatap = FieldbyName('APRONTE').asDateTime;
            foreach (var camposlote in dados_prodtestemunha)
            {
                if ((decimal)camposlote.caixas == 0) continue;
                DataRow[] rowsproduto = prodtestemunha.Select("(SETOR = '" + camposlote.setor + "') AND (LOTE = '" + camposlote.lote + "')", "QUA");
                System.Nullable<decimal> totalkgs =
                       (from linha in Segue.AsEnumerable()
                        where ((linha.Field<string>("SAFRA") == camposlote.safra) && (linha.Field<string>("LOTE") == camposlote.lote)
                        && (linha.Field<string>("SETOR") == camposlote.setor)
                         )
                        select linha.Field<decimal?>("QUANT")).Sum();


                var dates_segue = from linha in Segue.AsEnumerable()
                                  where ((linha.Field<string>("SAFRA") == camposlote.safra) &&
                                     (linha.Field<string>("LOTE") == camposlote.lote) &&
                                     (linha.Field<string>("SETOR") == camposlote.setor))
                                  orderby linha.Field<DateTime>("DATA")
                                  select linha;
                DateTime tdatap = DateTime.MinValue;
                if (dates_segue.Count() > 0)
                    tdatap = dates_segue.Last().Field<DateTime>("DATA");

                decimal totquant = 0;
                decimal totquant_fr = 0;
                decimal totquant_pa = 0;
                decimal totquant_ri = 0;
                decimal maiornquant = 0;
                decimal tot1 = (decimal)totalkgs;
                decimal tdif = tot1 / (decimal)camposlote.caixas;
                DataRow maiorrow = null;
                foreach (DataRow rowprod in rowsproduto)
                {
                    if (totalkgs == 0)
                    {
                        rowprod.BeginEdit();
                        rowprod["KGTOTAL"] = 0;
                        rowprod["KGEMPRESA"] = 0;
                        rowprod["KGPARCEIRO"] = 0;
                        rowprod["KGRI"] = 0;
                        rowprod["CODPARC"] = "";
                        rowprod["NCONTRA"] = "";
                        rowprod.EndEdit();
                        rowprod.AcceptChanges();
                        continue;
                    }


                    decimal tvalor = Math.Round(Convert.ToDecimal(rowprod["Caixas"]) * tdif, 2);

                    tot1 = tot1 - tvalor;
                    if (Convert.ToDecimal(rowprod["CAIXAS"]) > maiornquant)
                    {
                        maiornquant = Convert.ToDecimal(rowprod["CAIXAS"]);
                        maiorrow = rowprod;
                    }
                    decimal tquant_fr = 0;
                    decimal tquant_pa = 0;
                    decimal tquant_ri = 0;
                    string tcontraparc = "";
                    string tcodparc = "";
                    DataRow[] dataparc = parceiros.Select("QUA = " + "'" + rowprod["QUA"] + "'");
                    if (dataparc.Length > 0)
                    {
                        foreach (DataRow rparc in dataparc)
                        {
                            if (Convert.ToDateTime(rparc["Inicio"]).CompareTo(Convert.ToDateTime(rowprod["DATAE"])) > 0)
                                continue;
                            if (Convert.ToDateTime(rparc["Fim"]).CompareTo(Convert.ToDateTime(rowprod["DATAE"])) < 0)
                                continue;
                            tcontraparc = rparc["ncontra"].ToString();
                            tcodparc = rparc["codigo"].ToString();
                            break;
                        }
                    }
                    if (tcodparc != "")
                    {
                        if (parceria > 0)
                        {
                            tquant_pa = Math.Round((tvalor * parceria / 100), 3);
                            tquant_fr = (tvalor - tquant_pa);
                        }
                        if (reinvest > 0)
                        {
                            tquant_ri = Math.Round((tquant_pa * reinvest / 100), 3);
                            tquant_pa = tquant_pa - tquant_ri;
                        }
                    }
                    else
                        tquant_fr = tvalor;

                    rowprod.BeginEdit();
                    rowprod["KGTOTAL"] = tvalor;
                    rowprod["KGEMPRESA"] = tquant_fr;
                    rowprod["KGPARCEIRO"] = tquant_pa;
                    rowprod["KGRI"] = tquant_ri;
                    rowprod["CODPARC"] = tcodparc;
                    rowprod["NCONTRA"] = tcontraparc;
                    rowprod["DATAP"] = tdatap;
                    rowprod.EndEdit();
                    rowprod.AcceptChanges();
                    totquant = totquant + Convert.ToDecimal(rowprod["KGTOTAL"]);
                    totquant_fr = totquant_fr + Convert.ToDecimal(rowprod["KGEMPRESA"]);
                    totquant_pa = totquant_pa + Convert.ToDecimal(rowprod["KGPARCEIRO"]);
                    totquant_ri = totquant_ri + Convert.ToDecimal(rowprod["KGRI"]);
                }
                if (tot1 != 0)
                {
                    if (maiorrow != null)
                    {
                        decimal tvalor = Convert.ToDecimal(maiorrow["KGTOTAL"]) + tot1;
                        decimal tquant_fr = 0;
                        decimal tquant_pa = 0;
                        decimal tquant_ri = 0;
                        totquant_fr = totquant_fr - Convert.ToDecimal(maiorrow["KGEMPRESA"]);
                        totquant_pa = totquant_pa - Convert.ToDecimal(maiorrow["KGPARCEIRO"]);
                        totquant_ri = totquant_ri - Convert.ToDecimal(maiorrow["KGRI"]);
                        totquant = totquant - Convert.ToDecimal(maiorrow["KGTOTAL"]);
                        if (maiorrow["CODPARC"].ToString().Trim() != "")
                        {
                            tquant_pa = Math.Round((tvalor * parceria / 100), 3);
                            tquant_fr = tvalor - tquant_pa;
                            if (reinvest > 0)
                            {
                                tquant_ri = Math.Round((tquant_pa * reinvest / 100), 3);
                                tquant_pa = tquant_pa - tquant_ri;
                            }
                        }
                        else
                            tquant_fr = tvalor;
                        maiorrow.BeginEdit();
                        maiorrow["KGTOTAL"] = tvalor;
                        maiorrow["KGEMPRESA"] = tquant_fr;
                        maiorrow["KGPARCEIRO"] = tquant_pa;
                        maiorrow["KGRI"] = tquant_ri;
                        maiorrow.EndEdit();
                        maiorrow.AcceptChanges();
                        totquant = totquant + Convert.ToDecimal(maiorrow["KGTOTAL"]);
                        totquant_fr = totquant_fr + Convert.ToDecimal(maiorrow["KGEMPRESA"]);
                        totquant_pa = totquant_pa + Convert.ToDecimal(maiorrow["KGPARCEIRO"]);
                        totquant_ri = totquant_ri + Convert.ToDecimal(maiorrow["KGRI"]);
                    }
                }
            }
            return prodtestemunha;


        }





        static public DataSet TabelaProdutoAjustada(DataTable Produto, DataTable Segue, DataTable Produto_PT, DataTable parceiros)
        {
            //setor, gleba, qua, safra, lote, datae, nquant AS caixas,quant AS kgtotal, quant_fr AS kgempresa, quant_pa AS kgparceiro, quant_ri AS kgri " +
            try
            {
                DataTable Segue_PT = Segue.Copy();
                DataSet result = new DataSet();
                DataTable prodtestemunha = Produto.Copy();
                prodtestemunha.TableName = "Copia_Produto";
                DataSet dsCatProd = GetCatProduto();
                DataRow rowcatalogo = null;
                foreach (DataRow rowCatalogo in dsCatProd.Tables[0].AsEnumerable())
                {
                    if (rowCatalogo["COD"].ToString() == "  1") //produto cacau
                    {
                        rowcatalogo = rowCatalogo;
                        break;
                    }
                }
                DataRow[] orowdelete = Segue_PT.Select("(SETOR <> ' 9')");
                int ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                Segue_PT.AcceptChanges();

                orowdelete = Segue_PT.Select("(SAFRA <> '2011')");
                ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                Segue_PT.AcceptChanges();

                orowdelete = Segue_PT.Select("(SCS < 1)");
                ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                Segue_PT.AcceptChanges();


                orowdelete = Segue_PT.Select("(LOTE > ' 32')");
                ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                Segue_PT.AcceptChanges();


                orowdelete = Segue_PT.Select("(SCS = 61)");
                ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                Segue_PT.AcceptChanges();


                try
                {
                    // tira do segue
                    orowdelete = Segue.Select(" (SCS > 0)  AND (SETOR = ' 9') AND (SAFRA = '2011') AND (LOTE < ' 33')");
                    ind1 = 0;
                    while (orowdelete.Length > ind1)
                    {
                        if (Convert.ToDecimal(orowdelete[ind1]["SCS"]) < 61)
                            orowdelete[ind1].Delete();
                        ind1 = ind1 + 1;
                    }
                    Segue.AcceptChanges();
                }
                catch
                {
                    MessageBox.Show("Erro apagar segue");
                }

                orowdelete = Produto_PT.Select("(SETOR <> ' 9')");
                ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                Produto_PT.AcceptChanges();

                orowdelete = Produto_PT.Select("(SAFRA <> '2011')");
                ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                Produto_PT.AcceptChanges();

                orowdelete = Produto_PT.Select("(LOTE > ' 32')");
                ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                Produto_PT.AcceptChanges();

     
                var dados_lote =
                        from linha in Produto.AsEnumerable()
                        where ((linha.Field<string>("SETOR") == " 9")
                              && (linha.Field<string>("SAFRA") == "2011")
                              && (linha.Field<string>("LOTE").CompareTo(" 33") < 0))
                        group linha by new
                        {
                            lote = linha.Field<string>("LOTE"),
                            setor = linha.Field<string>("SETOR"),
                            safra = linha.Field<string>("Safra")
                        } into g
                        orderby g.Key.lote
                        select new
                        {
                            lote = g.Key.lote,
                            setor = g.Key.setor,
                            safra = g.Key.safra,
                            caixas = g.Sum((linha => linha.Field<Decimal?>("caixas"))),
                            kgtotal = g.Sum((linha => linha.Field<Decimal?>("kgtotal"))),
                            kgempresa = g.Sum((linha => linha.Field<Decimal?>("kgempresa"))),
                            kgparceiro = g.Sum((linha => linha.Field<Decimal?>("kgparceiro"))),
                            kgri = g.Sum((linha => linha.Field<Decimal?>("kgri")))
                        };

                string ultimolote = dados_lote.Last().lote;
                DataSet dsLote = GetLote(dados_lote.Last().safra, dados_lote.Last().setor);

                foreach (var camposlote in dados_lote)
                {
                    DataRow[] rowsproduto = prodtestemunha.Select("(SETOR = '" + camposlote.setor + "') AND (LOTE = '" + camposlote.lote + "')", "QUA");

                    DataRow[] rowsprodutopt = Produto_PT.Select("(SETOR = '" + camposlote.setor + "') AND (LOTE = '" + camposlote.lote + "')", "QUA");
                    if (rowsprodutopt.Length == 0) continue;
                    int ind = -1;
                    foreach (DataRow row_fino in rowsprodutopt)
                    {
                        ind = ind + 1;
                        while ((ind < rowsproduto.Length) &&
                            (rowsproduto[ind]["QUA"].ToString().CompareTo(row_fino["QUA"].ToString()) < 0))
                            ind = ind + 1;
                        if (ind == rowsproduto.Length) break;
                        if (rowsproduto[ind]["QUA"].ToString() == row_fino["QUA"].ToString())
                        {
                            if (Convert.ToDecimal(row_fino["Caixas"]) > Convert.ToDecimal(rowsproduto[ind]["Caixas"]))
                            {
                                MessageBox.Show("Erro Dados");
                                break;
                            }
                            rowsproduto[ind].BeginEdit();
                            rowsproduto[ind]["Caixas"] = Convert.ToDecimal(rowsproduto[ind]["Caixas"]) -
                                Convert.ToDecimal(row_fino["Caixas"]);
                            rowsproduto[ind].EndEdit();
                            rowsproduto[ind].AcceptChanges();
                            // if (Convert.ToDecimal(rowsproduto[ind]["Caixas"]) == 0)
                            //   rowsproduto[ind].Delete();
                        }
                    }
                }
                orowdelete = prodtestemunha.Select("(CAIXAS = 0)");
                ind1 = 0;
                while (orowdelete.Length > ind1)
                {
                    orowdelete[ind1].Delete();
                    ind1 = ind1 + 1;
                }
                prodtestemunha.AcceptChanges();


                decimal parceria = Convert.ToDecimal(rowcatalogo["PARCERIA"]);
                decimal reinvest = Convert.ToDecimal(rowcatalogo["REINVEST"]);

                Produto_PT = CalculeLote(Produto_PT, Segue_PT, parceiros, parceria, reinvest);

                foreach (DataRow rprod_pt in Produto_PT.Select("(SETOR = ' 9') AND (SAFRA = '2011')").AsEnumerable())
                {
                    rprod_pt["LOTE"] = (Convert.ToInt16(rprod_pt["LOTE"]) + 300).ToString();
                }

                foreach (DataRow segue_pt in Segue_PT.Select("(SETOR = ' 9') AND (SAFRA = '2011')").AsEnumerable())
                {
                    segue_pt["LOTE"] = (Convert.ToInt16(segue_pt["LOTE"]) + 300).ToString();
                }

                prodtestemunha = CalculeLote(prodtestemunha, Segue, parceiros, parceria, reinvest);

                foreach (DataRow rprod_pt in Produto_PT.Select("(SETOR = ' 9') AND (SAFRA = '2011')").AsEnumerable())
                {
                    prodtestemunha.ImportRow(rprod_pt);
                }
                foreach (DataRow segue_pt in Segue_PT.Select("(SETOR = ' 9') AND (SAFRA = '2011')").AsEnumerable())
                {
                    Segue.ImportRow(segue_pt);
                }

                result.Tables.Add(prodtestemunha);
                result.Tables.Add(Segue.Copy());

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
