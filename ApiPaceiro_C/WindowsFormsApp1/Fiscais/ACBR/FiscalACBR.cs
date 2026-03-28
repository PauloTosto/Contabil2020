using PrjApiParceiro_C.Fiscais.ACBR;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PrjApiParceiro_C.Fiscais.Classes
{
    static public class FiscalACBR
    {
        static public List<NotaXML> NotasFiscais = new List<NotaXML>();
        static public void LoadFromFile(string PathNFE)
        {
            NotaXML notaXML = new NotaXML();
            try
            {
                notaXML.pathNFE = PathNFE;
                notaXML.NFE = new XmlDocument();
                notaXML.NFE.Load(PathNFE);
                NotasFiscais.Add(notaXML);
            }
            catch (Exception)
            {
                // throw;
            }
        }

        //static public  
    }
    public class NotaXML
    {
        public string pathNFE { get; set; }
        public XmlDocument NFE { get; set; }
        public List<XmlDocument> EventosXML { get; set; }
        public List<XmlDocument> EventosCorrecaoXML { get; set; }
        public List<evento> Eventos { get; set; }
        // public List<eventoCorrecao> EventosCorrecao { get; set; }
        public ide Ide { get; set; }
        public emit Emit { get; set; }
        public dest Dest { get; set; }
        public List<DetItem> Det { get; set; }
        public Total total { get; set; } 
        public NotaXML()
        {
            EventosXML = new List<XmlDocument>();
            Eventos = new List<evento>();
           // EventosCorrecao = new List<eventoCorrecao>();
            Ide = new ide();
            Emit = new emit();
            Dest = new dest();
            Det = new List<DetItem>();
            total = new Total();
        }
        public void NFePegElemento()
        {
            // XmlNamespaceManager nsmgr = new XmlNamespaceManager(NFE.NameTable);
            // nsmgr.AddNamespace("nf", "http://www.portalfiscal.inf.br/nfe");

            XmlElement root = NFE.DocumentElement;
            try
            {
                var oide = root.GetElementsByTagName("ide");
                var ide_des = oide.DeserializeList<ide>();
                if ((ide_des != null) && (ide_des.Count > 0))
                { this.Ide = ide_des[0]; }
            }
            catch (Exception) { }
            try
            {
                var oemit = root.GetElementsByTagName("emit");
                var emit_des = oemit.DeserializeList<emit>();
                if ((emit_des != null) && (emit_des.Count > 0))
                { this.Emit = emit_des[0]; }

            }
            catch (Exception) { }
            try
            {
                var odest = root.GetElementsByTagName("dest");
                var dest_des = odest.DeserializeList<dest>();
                if ((dest_des != null) && (dest_des.Count > 0))
                { this.Dest = dest_des[0]; }

            }
            catch (Exception) { }
            try
            {
                /*var odet = root.GetElementsByTagName("det");
                var det_des = odet.DeserializeList<DetItem>();
                if ((det_des != null) && (det_des.Count > 0))
                { this.Det.Add(det_des[0]); }*/
                var odet = root.GetElementsByTagName("det");
                var det_des = odet.DeserializeList<DetItem>();
                if ((det_des != null) && (det_des.Count > 0))
                {
                    this.Det = det_des; 
                }

            }
            catch (Exception E)   {  throw; }
            try
            {
                var ototal = root.GetElementsByTagName("total");
                var total_des = ototal.DeserializeList<Total>();
                if ((total_des != null) && (total_des.Count > 0))
                { this.total = total_des[0]; }
            }
            catch (Exception E) { throw; }
        }
        public evento Eventos_Desserialize(XmlDocument doceventoXML)
        {
            evento evento = new evento();
            XmlElement root = doceventoXML.DocumentElement;
            try
            {
                var oevento = root.GetElementsByTagName("evento");
                var evento_des = oevento.DeserializeList<evento>();
                if ((evento_des != null) && (evento_des.Count > 0))
                { evento = evento_des[0]; }
            }
            catch (Exception) { }
            return evento;
        }

        /*public eventoCorrecao EventosCorrecao_Desserialize(XmlDocument doceventoXML)
        {
            eventoCorrecao evento = new eventoCorrecao();
            XmlElement root = doceventoXML.DocumentElement;
            try
            {
                var oevento = root.GetElementsByTagName("evento");
                var evento_des = oevento.DeserializeList<eventoCorrecao>();
                if ((evento_des != null) && (evento_des.Count > 0))
                { evento = evento_des[0]; }
            }
            catch (Exception) { }
            return evento;
        }
        */

        public void LerEventoXML(string PathNFE)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(PathNFE);
                EventosXML.Add(doc);
                Eventos.Add(Eventos_Desserialize(doc));
            }
            catch (Exception)
            {
                // throw;
            }
        }
        /*public void LerEventoCorrecaoXML(string PathNFE)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(PathNFE);
                EventosCorrecaoXML.Add(doc);
                EventosCorrecao.Add(EventosCorrecao_Desserialize(doc));
            }
            catch (Exception)
            {
                // throw;
            }
        }*/



    }
    /*
    
     */
    public class ide
    {
        public int cUF { get; set; }
        public int cNF { get; set; }
        public string natOp { get; set; }
        public int mod { get; set; }
        public int indPag { get; set; }   // TpcnIndicadorPagamento;
        public int serie { get; set; }
        public int nNF { get; set; }
        public DateTime dhEmi { get; set; }
        public DateTime dhSaiEnt { get; set; }
        public int tpNF { get; set; }
        public int idDest { get; set; }
        public int cMunFG { get; set; }
      //  public NFrefCollection NFref { get; set; }

        public int tpImp { get; set; } // TpcnTipoImpressao
        public int tpEmis { get; set; } // TpcnTipoEmissao
         public int cDV { get; set; }
        public  int tpAmb { get; set; } //TpcnTipoAmbiente
        public int finNFe { get; set; } //TpcnFinalidadeNFe
        public  int indFinal { get; set; } //TpcnConsumidorFinal
        public int indPres { get; set; } // TpcnPresencaComprador
        public int procEmi { get; set; } //TpcnProcessoEmissao
        public string verProc { get; set; }
        public DateTime dhCont { get; set; }
        public string xJust { get; set; }

        public ide()
        {
            //NFref = new NFrefCollection();
        }
    }
   
    public class emit
    {
        public string CNPJ { get; set; }
        public string xNome { get; set; }
        public string xFant { get; set; }
        public enderEmit enderEmit { get; set; }
        public string IE { get; set; }
        public string IEST { get; set; }
        public string CNAE { get; set; }
        public int CRT { get; set; } //TpcnCRT
        public emit()
        {
            enderEmit = new enderEmit();
        }
    }

    public class dest
    {
        public string CNPJ { get; set; }
        public string idEstrangeiro { get; set; }
        public string xNome { get; set; }
        public enderDest enderDest { get; set; }
        public int indIEDest { get; set; } //TpcnindIEDest
        public string FIE { get; set; }
        public string FISUF { get; set; }
        public string FIM { get; set; }
        public string Femail { get; set; }
        public dest()
        {
            enderDest = new enderDest();
        }
    }
    public class enderEmit
    {
        public string xLgr { get; set; }
        public string nro { get; set; } // pode ser int 
        public string xCpl { get; set; }
        public string xBairro { get; set; }
        public int cMun { get; set; }
        public string xMun { get; set; }
        public string UF { get; set; }
        public int CEP { get; set; }
        public int cPais { get; set; }
        public string xPais { get; set; }
    }
    public class enderDest
    {
        public string xLgr { get; set; }
        public string nro { get; set; } // pode ser int 
        public string xCpl { get; set; }
        public string xBairro { get; set; }
        public int cMun { get; set; }
        public string xMun { get; set; }
        public string UF { get; set; }
        public int CEP { get; set; }
        public int cPais { get; set; }
        public string xPais { get; set; }
        public string fone { get; set; }
    }

    // Classe dos DETALHES DAS NOTAS FISCAIS DE ENTRADA


  
    public class DetItem
    {
        public Prod prod { get; set; }
        public Imposto imposto { get; set; }
        public decimal pDevol { get; set; }
        public decimal vIPIDevol { get; set; }

        public string infAdProd { get; set; }
    }



    public class Prod
    {
        public string cProd { get; set; }
        public int nItem { get; set; }
        public string cEAN { get; set; }
        public string xProd { get; set; }
        public string NCM { get; set; }
        public string EXTIPI { get; set; }
        public string CFOP { get; set; }
        public string uCom { get; set; }
        public decimal qCom { get; set; }
        public double vUnCom { get; set; }
        public decimal vProd { get; set; }
        public string uTrib { get; set; }
        public decimal qTrib { get; set; }
        public double vUnTrib { get; set; }
        public decimal vFrete { get; set; }
        public decimal vSeg { get; set; }
        public decimal vDesc { get; set; }
        public decimal vOutro { get; set; }

        public int IndTot { get; set; } //TpcnIndicadorTotal
        public List<DI> DI { get; set; }
        public string xPed { get; set; }
        public string nItemPed { get; set; }
        public List<DetExport> detExport { get; set; }

        public List<VeicProd> veiProd { get; set; }
        public List<Med> med { get; set; }
        public List<Arma> arma { get; set; }
        public Comb comb { get; set; } 
        public string nRECOPI { get; set; }
        public string nFCI { get; set; }
        public List<NVE> NVE { get; set; }
        public string CEST { get; set; }
        public Prod()
        {
            DI = new List<DI>();
            detExport = new List<DetExport>();
            veiProd = new List<VeicProd>();
            med = new List<Med>();
            arma = new List<Arma>();
            NVE = new List<NVE>();
        }
    }


    public class DI
    {
        public string nDi { get; set; }
        public DateTime dDi { get; set; }
        public string xLocDesemb { get; set; }
        public string UFDesemb { get; set; }
        public DateTime dDesemb { get; set; }
        public int tpViaTransp { get; set; } //TpcnTipoViaTransp
        public decimal vAFRMM { get; set; }
        public int tpIntermedio { get; set; } //TpcnTipoIntermedio
        public string CNPJ { get; set; }
        public string UFTerceiro { get; set; }
        public string cExportador { get; set; }
        public List<Adi> adi { get; set; }
        public DI()
        { adi = new List<Adi>(); 
        }
    }
    public class Adi
    {
        public int nAdicao { get; set; }
        public int nSeqAdi { get; set; }
        public string cFabricante { get; set; }
        public decimal vDescDI { get; set; }
        public string nDraw { get; set; }
    }

    public class DetExport
    {
        public string nDraw { get; set; }
        public string nRE { get; set; }
        public string chNFe { get; set; }
        public decimal qExport { get; set; }
    }

    public class VeicProd
    {
        public int tpOP { get; set; } // TpcnTipoOperacao;
        public string chassi { get; set; }
        public string cCor { get; set; }
        public string pot { get; set; }
        public string Cilin { get; set; }
        public string pesoL { get; set; }
        public string pesoB { get; set; }
        public string nSerie { get; set; }
        public string tpComb { get; set; }
        public string nMotor { get; set; }
         public string CMT { get; set; }
        public string dist { get; set; }
        public int anoMod { get; set; }
        public int anoFab { get; set; }
        public string tpPint { get; set; }
        public int tpVeic { get; set; }
        public int espVeic { get; set; }
        public string VIN { get; set; }
        public int condVeic { get; set; } //TpcnCondicaoVeiculo
        public string cMod { get; set; }
        public string cCorDENATRAN { get; set; }
        public int lota { get; set; }
        public int tpRest { get; set; }

    }

    public class Med
    {
        public string nLote { get; set; }
        public decimal qLote { get; set; }
        public DateTime dFab { get; set; }
        public DateTime dVal { get; set; }
        public decimal vPMC { get; set; }
    }

    public class Arma
    {
        public int tpArma { get; set; } //TpcnTipoArma;
        public string nSerie { get; set; }
        public string nCano { get; set; }
        public string descr { get; set; }
    }

    public class Comb
    {
        public int cProdANP { get; set; }
        public decimal pMixGN { get; set; }
        public string CODIF { get; set; }
        public decimal qTemp { get; set; }
        public string UFcons { get; set; }
        public CIDE CIDE { get; set; }
        public ICMSComb ICMS { get; set; }
        //ICMSInter
        public ICMSInter ICMSInter { get; set; }
        public ICMSCons ICMSCons { get; set; }
        public Encerrante encerrante { get; set; }
    }
    public class CIDE {
        public decimal qBCProd { get; set; }
        public decimal vAliqProd { get; set; }
        public decimal vCIDE { get; set; }
    }

    public class ICMSComb
    {
        public decimal vBCICMS { get; set; }
        public decimal vICMS { get; set; }
        public decimal vBCICMSST { get; set; }
        public decimal vICMSST { get; set; }
    }
    public class ICMSInter
    {
        public decimal vBCICMSSTDest { get; set; }
        public decimal vICMSSTDest { get; set; }
    }
    public class ICMSCons
    {
        public decimal vBCICMSSTCons { get; set; }
        public decimal vICMSSTCons { get; set; }
        public string UFcons { get; set; }
    }


    public class Encerrante
    {
        public int nBico { get; set; }
        public int nBomba { get; set; }
        public int nTanque { get; set; }
        public decimal vEncIni { get; set; }
        public decimal vEncFin { get; set; }
    }
    
    public class NVE
    {
        public string Nve { get; set; }
    }
    //  //////// fim de PROD
    /// <summary>
    ///  Inicio Class TIMPOSTO
    /// </summary>
    public class Imposto {
        public decimal vTotTrib { get; set; }
        public ICMS ICMS { get; set; }
        public IPI IPI { get; set; }
        public II II { get; set; }
        public PIS PIS { get; set; }
        public PISST PISST { get; set; }
        public COFINS COFINS { get; set; }
        public COFINSST COFINSST { get; set; }
        public ISSQN ISSQN { get; set; }
        public ICMSUFDest ICMSUFDest { get; set; }
    }
    public class ICMS
    {
        public int orig { get; set; } // TpcnOrigemMercadoria;          //N11
        public int CST { get; set; } // TpcnCSTIcms;                    //N12
        public int CSOSN { get; set; } // TpcnCSOSNIcms;                //N12a
        public int modBC { get; set; } // TpcnDeterminacaoBaseIcms;     //N13
        public decimal pRedBC { get; set; } //N14
        public decimal vBC { get; set; } //N15
        public decimal pICMS { get; set; } //N16
        public decimal vICMS { get; set; } //N17
        public int modBCST { get; set; } // TpcnDeterminacaoBaseIcmsST; //N18
        public decimal pMVAST { get; set; } //N19
        public decimal pRedBCST { get; set; } //N20
        public decimal vBCST { get; set; } //N21
        public decimal pICMSST { get; set; } //N22
        public decimal vICMSST { get; set; } //N23
        public string UFST { get; set; } //N24
        public decimal pBCOp { get; set; } //N25
        public decimal vBCSTRet { get; set; } //N26
        public decimal vICMSSTRet { get; set; } //N27
        public int motDesICMS { get; set; } //   TpcnMotivoDesoneracaoICMS; //N28
        public decimal pCredSN { get; set; } ////N29
        public decimal vCredICMSSN { get; set; } //N30
        public decimal vBCSTDest { get; set; } //N31
        public decimal vICMSSTDest { get; set; } //N32
        public decimal vICMSDeson { get; set; }
        public decimal vICMSOp { get; set; }
        public decimal pDif { get; set; }
        public decimal vICMSDif { get; set; }
    }

    public class IPI
    {
        public string clEnq { get; set; }
        public string CNPJProd { get; set; }
        public string cSelo { get; set; }
        public int qSelo { get; set; }
        public string cEnq { get; set; }
        public int CST { get; set; } // TpcnCstIpi;
        public decimal vBC { get; set; }
        public decimal qUnid { get; set; }
        public decimal vUnid { get; set; }
        public decimal pIPI { get; set; }
        public decimal vIPI { get; set; }
    }

    public class II
    {
        public decimal vBc { get; set; }
        public decimal vDespAdu { get; set; }
        public decimal vII { get; set; }
        public decimal vIOF { get; set; }
    }
    public class PIS
    {
        public int CST { get; set; } // TpcnCstPis;
        public decimal vBC { get; set; }
        public decimal pPIS { get; set; }
        public decimal vPIS { get; set; }
        public decimal qBCProd { get; set; }
        public decimal vAliqProd { get; set; }
    }
    public class PISST
    {
        public decimal vBc { get; set; }
        public decimal pPis { get; set; }
        public decimal qBCProd { get; set; }
        public decimal vAliqProd { get; set; }
        public decimal vPIS { get; set; }
    }
    public class COFINS
    {
        public int CST { get; set; } //TpcnCstCofins;
        public decimal vBC { get; set; }
        public decimal pCOFINS { get; set; }
        public decimal vCOFINS { get; set; }
        public decimal vBCProd { get; set; }
        public decimal vAliqProd { get; set; }
        public decimal qBCProd { get; set; }
    }
    public class COFINSST
    {
        public decimal vBC { get; set; }
        public decimal pCOFINS { get; set; }
        public decimal qBCProd { get; set; }
        public decimal vAliqProd { get; set; }
        public decimal vCOFINS { get; set; }
    }
    public class ISSQN
    {
        public decimal vBC { get; set; }
        public decimal vAliq { get; set; }
        public decimal vISSQN { get; set; }
        public int cMunFG { get; set; }
        public string cListServ { get; set; }
        public int cSitTrib { get; set; } //    TpcnISSQNcSitTrib;
        public decimal vDeducao { get; set; }
        public decimal vOutro { get; set; }
        public decimal vDescIncond { get; set; }
        public decimal vDescCond { get; set; }
        public int indISSRet { get; set; } //    TpcnindISSRet;
        public decimal vISSRet { get; set; }
        public int indISS { get; set; } //   TpcnindISS;
        public string cServico { get; set; }
        public int cMun { get; set; }
        public int cPais { get; set; }
        public string nProcesso { get; set; }
        public int indIncentivo { get; set; } //FindIncentivo: TpcnindIncentivo;
    }
    public class ICMSUFDest
    {
        public decimal vBCUFDest { get; set; }
        public decimal pFCPUFDest { get; set; }
        public decimal pICMSUFDest { get; set; }
        public decimal pICMSInter { get; set; }
        public decimal pICMSInterPart { get; set; }
        public decimal vFCPUFDest { get; set; }
        public decimal vICMSUFDest { get; set; }
        public decimal vICMSUFRemet { get; set; }
    }


    /// <summary>
    /// /
    /// </summary>
    public class NFrefCollection
    {
        List<NFrefCollectionItem> NFrefs { get; set; }
        public NFrefCollection()
        {
            NFrefs = new List<NFrefCollectionItem>();
        }
    }
    public class NFrefCollectionItem
    {
        public string refNFe { get; set; }
        public string refCTe { get; set; }
        public RefNF RefNF { get; set; }
        public RefECF RefECF { get; set; }
        public RefNFP RefNFP { get; set; }
        public NFrefCollectionItem()
        {
            RefNF = new RefNF();
            RefECF = new RefECF();
            RefNFP = new RefNFP();
        }
    }


    public class RefNF
    {
        public int cUF { get; set;}
        public string AAMM { get; set; }
        public string CNPJ { get; set; }
        public int modelo { get; set; }
        public int serie { get; set; }
        public int nNF { get; set; } 
    }
    public class RefNFP
    {
        public int cUF { get; set; }
        public string AAMM { get; set; }
        public string CNPJCPF { get; set; }
        public string IE { get; set; }
        public int modelo { get; set; }
        public int serie { get; set; }
        public int nNF { get; set; }
    }
    
    public class RefECF
    {
        public int modelo { get; set; } //TpcnECFModRef
        public string nECF { get; set; }
        public string nCOO { get; set; }
    }

    // Eventos CLass
    


    /// <summary>
    /// CLASS TOTAL da NFE
    /// </summary>

    public class Total {
       public ICMSTot ICMSTot { get; set; }
       public ISSQNtot ISSQNtot { get; set; }
      public  retTrib retTrib { get; set; }
    }

    public class ICMSTot
    {
        public decimal vBC { get; set; }
        public decimal vICMS { get; set; }
        public decimal vICMSDeson { get; set; }
        public decimal vFCPUFDest { get; set; }
        public decimal vICMSUFDest { get; set; }
        public decimal vICMSUFRemet { get; set; }
        public decimal vBCST { get; set; }
        public decimal vST { get; set; }
        public decimal vProd { get; set; }
        public decimal vFrete { get; set; }
        public decimal vSeg { get; set; }
        public decimal vDesc { get; set; }
        public decimal vII { get; set; }
        public decimal vIPI { get; set; }
        public decimal vPIS { get; set; }
        public decimal vCOFINS { get; set; }
        public decimal vOutro { get; set; }
        public decimal vNF { get; set; }
        public decimal vTotTrib { get; set; }
    }
    public class ISSQNtot
    {
        public decimal vServ { get; set; }
        public decimal vBC { get; set; }
        public decimal vISS { get; set; }
        public decimal vPIS { get; set; }
        public decimal vCOFINS { get; set; }
        public DateTime dCompet { get; set; }
        public decimal vDeducao { get; set; }
        public decimal vOutro { get; set; }
        public decimal vDescIncond { get; set; }
        public decimal vDescCond { get; set; }
        public decimal vISSRet { get; set; }
        public int cRegTrib { get; set; } // TpcnRegTribISSQN;
    }

    public class retTrib
    {
        public decimal vRetPIS { get; set; }
        public decimal vRetCOFINS { get; set; }
        public decimal vRetCSLL { get; set; }
        public decimal vBCIRRF { get; set; }
        public decimal vIRRF { get; set; }
        public decimal vBCRetPrev { get; set; }
        public decimal vRetPrev { get; set; }
    }
 


    // FIM TTOTAL 
    // Agradeço à https://stackoverflow.com/questions/48652270/how-to-deserialize-a-node-in-a-large-document-using-xmlserializer
    public static partial class XmlNodeExtensions
    {
        public static List<T> DeserializeList<T>(this XmlNodeList nodes)
        {
            return nodes.Cast<XmlNode>().Select(n => n.Deserialize<T>()).ToList();
        }

        public static T Deserialize<T>(this XmlNode node)
        {
            if (node == null)
                return default(T);
            var serializer = XmlSerializerFactory.Create(typeof(T), node.LocalName, node.NamespaceURI);
            using (var reader = new XmlNodeReader(node))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }



    public static class XmlSerializerFactory
    {
        // To avoid a memory leak the serializer must be cached.
        // https://stackoverflow.com/questions/23897145/memory-leak-using-streamreader-and-xmlserializer
        // This factory taken from 
        // https://stackoverflow.com/questions/34128757/wrap-properties-with-cdata-section-xml-serialization-c-sharp/34138648#34138648

        readonly static Dictionary<Tuple<Type, string, string>, XmlSerializer> cache;
        readonly static object padlock;

        static XmlSerializerFactory()
        {
            padlock = new object();
            cache = new Dictionary<Tuple<Type, string, string>, XmlSerializer>();
        }

        public static XmlSerializer Create(Type serializedType, string rootName, string rootNamespace)
        {
            if (serializedType == null)
                throw new ArgumentNullException();
            if (rootName == null && rootNamespace == null)
                return new XmlSerializer(serializedType);
            lock (padlock)
            {
                XmlSerializer serializer;
                var key = Tuple.Create(serializedType, rootName, rootNamespace);
                if (!cache.TryGetValue(key, out serializer))
                    cache[key] = serializer = new XmlSerializer(serializedType, new XmlRootAttribute { ElementName = rootName, Namespace = rootNamespace });
                return serializer;
            }
        }
    }

}
