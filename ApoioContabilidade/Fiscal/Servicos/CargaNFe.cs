using ApoioContabilidade.Core;
using ApoioContabilidade.Fiscal.ACBR.Classes;
using ApoioContabilidade.Fiscal.Comum;
using ApoioContabilidade.Fiscal.Model;
using ApoioContabilidade.Models;
using ApoioCOntabilidade.Services;
/*ing PrjApiParceiro_C.AcessosServidor;
using PrjApiParceiro_C.Comum;
using PrjApiParceiro_C.Core;
using PrjApiParceiro_C.Fiscais.Classes;
using PrjApiParceiro_C.Fiscais.Model;
using PrjApiParceiro_C.Services;*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ApoioContabilidade.Fiscal.Servicos
{
    public static class ClassEventosNF
    {

        public static string CACAU_ = "  1";
        public static DataSet odataset = new DataSet();
        public static DataSet odata_disp = new DataSet();
        public static DataSet odata_disp_Fazendas = new DataSet();
        public static DataTable SomaSetor = null;

        static async public Task<Boolean> CargaNFeXML(DateTime data1, DateTime data2)
        {
            
            DataTable Mestre;
            DataTable ItensFis;
            ListaAdoCNPJ ocnpj;
            DataTable adoCNPJ;

            Boolean result = false;
            ocnpj = new ListaAdoCNPJ();
            adoCNPJ = CollectionHelper.ConvertTo<ADOCNPJ>(ocnpj.lista);
            Mestre = CollectionHelper.ConvertTo<NFiscal>(new List<NFiscal>());
            ItensFis = CollectionHelper.ConvertTo<ItensFis>(new List<ItensFis>());

  
            int ID_ItensFis = 0;
            List<string> Lista = new List<string>();
            List<string> Eventos = new List<string>();
            string anomes_ini = data1.ToString("yyyyMM").Substring(2);  // copy(FormatDateTime('yyyymm', FdtData1.Date), 3);
            string anomes_fim = data2.ToString("yyyyMM").Substring(2);

            List<string> sql = new List<string>();

            string str = "SELECT * FROM CATPROD";
            sql.Add(str);

            // adoPegNumero
            str = "SELECT data, valor, CAST(CAST (doc AS NUMERIC(19,4)) AS INT) AS NumNFiscal, emissor, codigof, quant, "
               + "e_orig, forn_cli, tp_ordem, tp_emissor FROM  nfiscal WHERE "
               + " (SUBSTR(DTOS(data),1,6) BETWEEN '" + data1.ToString("yyyyMM")
               + "' and '" + data2.ToString("yyyyMM") + "' AND (alltrim(emissor) = '20') ) ORDER BY NumNFiscal ";

            sql.Add(str);
            // oNfeLote.oTable
            str = "SELECT id, itensnf_id, dbo.fnLoteId(setor, safra, lote,prod) as lote_id, dta_ent, dta_emi, kg, prod, lote, setor, "
                + "safra, nfiscal, serie, forn_cli, e_orig, codigof, kg as kg_ant  " +
                   " FROM NFE_LOTE WHERE DTA_EMI BETWEEN CTOD('" + data1.ToString("MM/dd/yyyy")
                   + "')" +
               " and CTOD('" + data2.ToString("MM/dd/yyyy") + "')";
            sql.Add(str);
            // oNfeVenda
            str = "SELECT  id, id_venda, id_itens, safra, cont, prod, nfiscal, serie, dta_venda, "
                + "dta_emi, prod_tp, certif, firma, cnpj, quant_v, valor_v, punit_v, valor_fis, "
                + "quant_fis, punit_fis, e_orig, codigof, quant_v as quantv_ant " +
                "FROM NFE_VENDA WHERE DTA_EMI BETWEEN CTOD('" + data1.ToString("MM/dd/yyyy") + "')" +
                               " and CTOD('" + data2.ToString("MM/dd/yyyy") + "')";
            sql.Add(str);
            str = "SELECT* FROM FIRMA";
            sql.Add(str);



            odataset = await ApiServices.Api_QueryMulti(sql);
            DataTable qrCatProd = odataset.Tables[0];
            DataTable NfeLote = odataset.Tables[2];
            DataTable qrFirmas = odataset.Tables[odataset.Tables.Count - 1];

            odataset.Tables[0].TableName = "qrCatProd";
            odataset.Tables[1].TableName = "adoPegNumero";
            odataset.Tables[2].TableName = "NfeLote";
            odataset.Tables[3].TableName = "NfeVenda";
            odataset.Tables[4].TableName = "qrFirmas";
            odataset.Tables.Add(Mestre);
            odataset.Tables[odataset.Tables.Count - 1].TableName = "Mestre";
            odataset.Tables.Add(ItensFis);
            odataset.Tables[odataset.Tables.Count - 1].TableName = "ItensFis";
            //// Pegas as notas fiscais e de enventos fiscais no diretorio
            int incremento;
            foreach (var file in FindDiretorio.TraverseDirectory(Configura_XML_NF.PATHNFE, f => f.Extension == ".xml"))
            {
                incremento = 0;
                if (file.Name.Contains("-procNfe") || file.Name.Contains("-nfe-sign") || file.Name.Contains("-procEventoNfe") || file.Name.Contains("can-sign") || file.Name.Contains("cce-sign"))
                {
                    if (file.Name.Contains("-procEventoNfe") || file.Name.Contains("procEvento") || file.Name.Contains("can-sign") || file.Name.Contains("cce-sign"))
                    {
                        incremento = 7;// 7;
                        if (file.Name.Contains("can-sign") || file.Name.Contains("cce-sign") )
                        {
                            incremento = 6;
                        }
                        
                    }
                    string cUF = file.Name.Substring(incremento + 0, 2);
                    string aa = file.Name.Substring(incremento + 2, 2);
                    string mm = file.Name.Substring(incremento + 4, 2);
                    string cnpj = file.Name.Substring(incremento + 6, 14);
                    string modelo = file.Name.Substring(incremento + 20, 2);
                    string serie = file.Name.Substring(incremento + 22, 3);
                    string nf = file.Name.Substring(incremento + 25, 9);
                    if ((cUF == Configura_XML_NF.cUF_pad) && (cnpj == Configura_XML_NF.CNPJ_pad) &&
                        (((aa + mm).CompareTo(anomes_ini) >= 0) && ((aa + mm).CompareTo(anomes_fim) <= 0)))
                    {
                        if (incremento == 0) { Lista.Add(file.Name); }
                        else { Eventos.Add(file.Name); }
                    }
                }
            }




            // List<string> tags = new List<string>();
            int numid = 0;
            foreach (string nota in Lista)
            {
                try
                {
                    FiscalACBR.LoadFromFile(Configura_XML_NF.PATHNFE + "\\" + nota);
                    NotaXML notaXML = FiscalACBR.NotasFiscais[FiscalACBR.NotasFiscais.Count - 1];
                    // if (!(nota.CompareTo("29200114512735000109550010000015291000531005-nfe-sign.xml") == 0)) continue;
                    notaXML.NFePegElemento();
                    if (!((notaXML.Ide.dhEmi.Date >= data1) && (notaXML.Ide.dhEmi.Date <= data2))) continue;
                    foreach (var evento in Eventos)
                    {
                        if (nota.Substring(0, 35).CompareTo(evento.Substring(6, 35)) == 0)
                        {
                            notaXML.LerEventoXML(Configura_XML_NF.PATHNFE + "\\" + evento);
                        }
                    }
                    DataRow orow = Mestre.NewRow();
                    numid++;
                    orow["ID"] = numid;
                    orow["NOME"] = nota;
                    orow["NATOP"] = notaXML.Ide.natOp.ToUpper();

                    orow["DtaEmi"] = notaXML.Ide.dhEmi;

                    orow["NFISCAL"] = notaXML.Ide.nNF;
                    orow["SERIE"] = notaXML.Ide.serie;
                    orow["TPNF"] = Enum.GetName(typeof(TpcnTipoNFe), (notaXML.Ide.tpNF)).ToUpper().Substring(2, 1);
                    orow["IDDEST"] = Enum.GetName(typeof(TpcnDestinoOperacao), notaXML.Ide.idDest).ToUpper().Substring(2);
                    orow["FINNF"] = Enum.GetName(typeof(TpcnFinalidadeNFe), notaXML.Ide.finNFe).ToUpper().Substring(2);
                    orow["CODFAZ"] = "";
                    if (notaXML.Ide.tpNF == 0)
                    {
                        DataView dv = new DataView(adoCNPJ);
                        dv.RowFilter = "(TRIM(CODFAZ) = '') and (CNPJ = '" + notaXML.Emit.CNPJ + "')";
                        if (dv.Count > 0)
                        {
                            dv.RowFilter = "(TRIM(CODFAZ) <> '') and (CNPJ = '" + notaXML.Dest.CNPJ + "')";
                            if (dv.Count > 0)
                            {
                                orow["CODFAZ"] = dv[0].Row["CODFAZ"];
                                orow["xLgr"] = notaXML.Dest.enderDest.xLgr;
                            }
                        }
                    }
                    orow["DEST_CNPJ"] = notaXML.Dest.CNPJ;
                    if (orow["DEST_CNPJ"].ToString().Length > 11)
                    {
                        orow["DEST_TPDOC"] = "CNPJ";
                    }
                    else if (orow["DEST_CNPJ"].ToString().Length > 1)
                    {
                        orow["DEST_TPDOC"] = "CPF";
                    }
                    else orow["DEST_TPDOC"] = "";
                    if ((notaXML.Dest.idEstrangeiro != null) && (notaXML.Dest.idEstrangeiro != "")) { orow["DEST_CNPJ"] = notaXML.Dest.idEstrangeiro; }

                    // Dest.
                    orow["DEST_NOME"] = notaXML.Dest.xNome.ToUpper();
                    orow["FIRMA"] = "";
                    if (orow["TPNF"].ToString() == "S")
                    {
                        string tnumconta = pegNumCodigo(notaXML.Dest.CNPJ, notaXML.Dest.xNome, qrFirmas);
                        orow["FIRMA"] = tnumconta;
                    }
                    orow["Evento"] = "";
                    orow["ObsEvento"] = "";
                    foreach (var evento in notaXML.Eventos)
                    {
                        orow["Evento"] = evento.infEvento.detEvento.descEvento;
                        if (evento.infEvento.tpEvento == Convert.ToUInt32(EnumString.TpcnTpEvento[(int) TpcnTpEvento.teCancelamento]))
                        {
                            orow["ObsEvento"] = evento.infEvento.detEvento.xJust;
                        }
                        else
                        {
                            orow["ObsEvento"] = evento.infEvento.detEvento.xCorrecao ;
                        }

                        // orow["Evento"] = evento
                            /* ACBrNFe1.EventoNFe.idLote := ACBrNFe1.NotasFiscais.Count - 1;
                             for j := 0 to ACBrNFe1.EventoNFe.Evento.Count - 1 do
                                     begin
                               orow["Evento').AsString :=
                                 ACBrNFe1.EventoNFe.Evento.Items[j].InfEvento.DescEvento;
                             if (ACBrNFe1.EventoNFe.Evento.Items[j]
                               .InfEvento.tpEvento = teCancelamento) then
                             begin
                                orow["ObsEvento').AsString :=
                                UTF8Encode(ACBrNFe1.EventoNFe.Evento.Items[j]
                                 .InfEvento.detEvento.xJust);
                             end
                            else
                                 begin
                             orow["ObsEvento').AsString :=
                               UTF8Encode(ACBrNFe1.EventoNFe.Evento.Items[j]
                              .InfEvento.detEvento.xCorrecao);
                             end;
                           end;
                           */
                    }
                    notaXML.Eventos.Clear();
                    string prodfis = "";
                    string exclusivo = "";
                    decimal quant_prod = 0;
                    exclusivo = "";
                    string cfop_mesmo = "";
                    foreach (DetItem detalhe in notaXML.Det)
                    {
                        DataRow orowDet = ItensFis.NewRow();
                        ID_ItensFis++;
                        orowDet["ID"] = ID_ItensFis;
                        orowDet["ID_NF"] = notaXML.Ide.nNF;
                        DataView dv = new DataView(qrCatProd);
                        dv.RowFilter = "ST_FISC = '" + detalhe.prod.cProd + "'";
                        if (dv.Count > 0)
                        {
                            orowDet["cProd"] = dv[0].Row["COD"];

                            if (prodfis == "")
                            {
                                prodfis = dv[0].Row["COD"].ToString();
                                exclusivo = "S";
                            }
                            else if (prodfis.CompareTo(dv[0].Row["COD"].ToString()) != 0)
                            {
                                exclusivo = "N";
                                if (dv[0].Row["COD"].ToString() == CACAU_)
                                { prodfis = dv[0].Row["COD"].ToString(); }

                            }
                        }
                        orowDet["xProd"] = detalhe.prod.cProd + " " + detalhe.prod.xProd;
                        orowDet["cUNID"] = detalhe.prod.uCom;
                        orowDet["cPUNIT"] = detalhe.prod.vUnCom;
                        orowDet["cQuant"] = detalhe.prod.qCom;
                        quant_prod = quant_prod + detalhe.prod.qCom;
                        orowDet["cVlr"] = detalhe.prod.vProd;
                        orowDet["CFOP"] = detalhe.prod.CFOP;
                        cfop_mesmo = detalhe.prod.CFOP;
                        orowDet["InfAdPROD"] = detalhe.infAdProd;
                        ItensFis.Rows.Add(orowDet);
                    }
                    orow["PRODFIS"] = prodfis;
                    orow["CFOP"] = cfop_mesmo;

                    orow["PROD_EXC"] = exclusivo;
                    orow["vlrNF"] = notaXML.total.ICMSTot.vNF;

                    orow["Quant_NF"] = quant_prod;

                    orow["CRITICA"] =
                       PegCritica(orow, NfeLote); // rotina de desligar automaticamente notas canceladas de entrada

                    if ((orow["Evento"] != null) && (orow["Evento"].ToString() != ""))
                    {
                        if ((orow["TPNF"].ToString().Substring(0, 1) == "E") &&
                          (orow["CODFAZ"].ToString() != "") &&
                       (orow["PRODFIS"].ToString() == CACAU_) &&
                        (orow["Evento"].ToString().ToUpper().Substring(0, 4) == "CANC"))
                        {   // delete os registro do Itens NF que houverem sido CANCELADOS 
                            DataRow[] dr = null;
                            dr = NfeLote.Select("(ITENSNF_ID = " + orow["NFISCAL"].ToString() + ")");
                            foreach (DataRow row in dr)
                            {
                                NfeLote.Rows.Remove(row);
                            }

                        }
                    }
                    Mestre.Rows.Add(orow);
                }
                catch (Exception E) { MessageBox.Show("Erro NotaFiscal:" + nota + "  " + E.Message); throw; }

            }
            result = true;
            return result;
        }
        static public string PegCritica(DataRow rowMestre, DataTable nfeLote)
        {
            double totkg;
            string result = "";
            if (rowMestre["TPNF"].ToString() == "E")
            {
                if ((rowMestre["CODFAZ"].ToString().Trim() != "") &&
                 (rowMestre["PRODFIS"].ToString() == CACAU_))
                {
                    totkg = 0;
                    DataView dv = new DataView(nfeLote);
                    dv.RowFilter = "(ITENSNF_ID = " + rowMestre["NFISCAL"].ToString() + ")";
                    if (dv.Count > 0)
                    {
                        for (int i = 0; i < dv.Count; i++)
                        {
                            totkg = totkg + Convert.ToDouble(dv[i].Row["KG"]);
                        }
                    }

                    if (totkg == 0)
                    { result = "Nenhuma"; }

                    else if (totkg != Convert.ToDouble(rowMestre["QUANT_NF"]))
                    { result = "Incompleto"; }
                    else if (totkg == Convert.ToDouble(rowMestre["QUANT_NF"]))
                        result = "Completo";
                }
            }
            return result;
        }
        static private string pegNumCodigo(string codigo, string nome, DataTable qrFirma)
        {
            string cnpj = "";
            string firma = "";
            //  string desc= "";

            if (!((codigo == "") || (codigo == null)))
            { cnpj = codigo.Replace(".", ""); }

            if (cnpj.Trim() != "")
            {
                DataView dv = new DataView(qrFirma);
                dv.RowFilter = "CNPJ =  '" + cnpj.Trim() + "'";
                if (dv.Count > 0)
                {
                    firma = dv[0].Row["CONTABIL"].ToString();
                }
            }
            if (firma != "") { return firma; }
            foreach (DataRow orow in qrFirma.Rows)
            {
                if (orow["CONTABIL"].ToString() == "") continue;
                if (!(orow["DESCRI"].ToString().Contains(nome.Substring(0, 5)))) continue;
                firma = orow["CONTABIL"].ToString();
                break;
            }
            return firma;
        }

        static async public Task<bool> RecarregueDisponivel()
        {
            // List<string> sql = new List<string>();
            bool result = false;
            string query = "sp_numero=501";
            odata_disp = await ApiServices.Api_QuerySP(new List<string>(new string[]
                        { query }));
            if (odataset.Tables.Count > 0)
            {
                odata_disp.Tables[0].TableName = "Lote_disp";
                odata_disp.Tables[1].TableName = "Nfe_lote"; // estrutura vazia id == null
                odata_disp.Tables[2].TableName = "Venda_disp";
                odata_disp.Tables[3].TableName = "Nfe_Venda"; // estrutura vazia id == null
                result = true;
                try
                {
                    /* este procedimento passou a ser realizado no sql server 
                     * if (odata_disp.Tables["Lote_disp"].Rows.Count > 0)
                    {
                        odata_disp.Tables["Lote_disp"].AsEnumerable().Where(row => (!row.IsNull("acum_ant"))).ToList().ForEach(
                                        row =>
                                        {
                                            row.BeginEdit();
                                            row["SDOKG_ENT"] = Convert.ToDouble(row["KG_ENT"]) - Convert.ToDouble(row["ACUM_ANT"]);
                                            row.EndEdit();
                                        });
                    }*/
                    if (odata_disp.Tables["Venda_disp"].Rows.Count > 0)
                    {
                        odata_disp.Tables["Venda_disp"].AsEnumerable().Where(row => (!row.IsNull("acum_ant"))).ToList().ForEach(
                                        row =>
                                        {
                                            row.BeginEdit();
                                            row["SDOQUANT"] = Convert.ToDouble(row["QUANT"]) - Convert.ToDouble(row["ACUM_ANT"]);
                                            row.EndEdit();
                                        });
                                }
                }
                catch (Exception E) { throw; }
            }

            return result;
        }

        static async public Task<bool> Recarregue_FazendaDisponivel()
        {
            // List<string> sql = new List<string>();
            bool result = false;
            SomaSetor = new DataTable();
            SomaSetor.TableName = "SomaSetor";
            SomaSetor.Columns.Add("ID", Type.GetType("System.Int32"));
            SomaSetor.Columns.Add("SETOR", Type.GetType("System.String"));
            SomaSetor.Columns.Add("INICIO", Type.GetType("System.DateTime"));
            SomaSetor.Columns.Add("FIM", Type.GetType("System.DateTime"));
            SomaSetor.Columns.Add("QUANT", Type.GetType("System.Double"));

            string str = "SELECT  id, id_lote, tipoProd,setor, prod, safra, lote, apronte,dta_ent, benef, kg_ent," +
              "iif(dta_ent<>'',dta_ent,apronte) as dta_informe, " +
                 "iif(kg_ent=0,benef,kg_ent) as SDOKG_ENT,"
             + " (SELECT SUM(kg) from nfe_lote where (lote.ID_LOTE = lote_ID ) " +
             " and (nfiscal<>9999999) ) AS ACUM_ANT "
             + " FROM lote  WHERE " +
             " EXISTS (SELECT 1 AS Expr1  FROM  nfe_lote  WHERE (lote_ID = lote.ID_LOTE) AND (nfiscal <> 9999999)) "
             + " AND (prod = '  1') AND (benef <> 0) or " +
             " (prod = '  1') AND (benef <> 0) and (NOT EXISTS " +
             " (SELECT 1 AS Expr1 FROM nfe_lote nfe_lote_1  WHERE    (lote_ID = lote.ID_LOTE))) ";

            odata_disp_Fazendas = await ApiServices.Api_QueryMulti(new List<string>(new string[] { str }));
            if (odata_disp_Fazendas.Tables.Count > 0)
            {
                odata_disp_Fazendas.Tables.Add(SomaSetor);
                odata_disp_Fazendas.Tables[0].TableName = "LoteFazenda";
                result = true;
                try
                {
                    if (odata_disp_Fazendas.Tables["LoteFazenda"].Rows.Count > 0)
                       odata_disp_Fazendas.Tables["LoteFazenda"].AsEnumerable().Where(row => (!row.IsNull("acum_ant"))).ToList().ForEach(row => row["SDOKG_ENT"]
                                                                               = Convert.ToDouble(row["SDOKG_ENT"]) - Convert.ToDouble(row["ACUM_ANT"]));
                    foreach (DataRow orow in odata_disp_Fazendas.Tables["LoteFazenda"].AsEnumerable().
                        Where(row => row.Field<double>("SDOKG_ENT") != 0).OrderBy(row => row.Field<string>("SETOR")).ThenBy(row => row.Field<DateTime>("dta_informe")))
                    {
                        string setor = orow["Setor"].ToString();
                        setor = setor == " 1" ? " 2" : setor;
                        DataRow rowSetor = SomaSetor.AsEnumerable().Where(row => row.Field<string>("Setor") == setor).FirstOrDefault();
                        if (rowSetor == null)
                        {
                            rowSetor = SomaSetor.NewRow();
                            rowSetor["SETOR"] = setor;
                            rowSetor["QUANT"] = orow["SDOKG_ENT"];
                            rowSetor["INICIO"] = orow["dta_informe"];
                            rowSetor["FIM"] = orow["dta_informe"];
                            SomaSetor.Rows.Add(rowSetor);
                        }
                        else
                        {
                            rowSetor.BeginEdit();
                            rowSetor["QUANT"] = Convert.ToDouble(rowSetor["QUANT"]) + Convert.ToDouble(orow["SDOKG_ENT"]);
                            if (Convert.ToDateTime(rowSetor["INICIO"]).CompareTo(orow["dta_informe"]) > 0)
                            {
                                rowSetor["INICIO"] = orow["dta_informe"];
                            }
                            if (Convert.ToDateTime(rowSetor["FIM"]).CompareTo(orow["dta_informe"]) < 0)
                            {
                                rowSetor["FIM"] = orow["dta_informe"];
                            }
                            rowSetor.EndEdit();
                        }

                    }
                    SomaSetor.AcceptChanges();
             
                }
                catch (Exception E) { throw; }
            }
            return result;
        }
    }
}