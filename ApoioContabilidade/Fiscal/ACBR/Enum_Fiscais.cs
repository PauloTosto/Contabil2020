using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Fiscal.ACBR.Classes
{
    // enums vindos do AcBR (delphi)
    // Notas Fiscais de Entrada
   
    public enum TpcnindIEDest  { inContribuinte, inIsento, inNaoContribuinte }
    public enum TpcnCRT  { crtSimplesNacional, crtSimplesExcessoReceita, crtRegimeNormal }
    public enum TpcnIndicadorPagamento  { ipVista, ipPrazo, ipOutras }
    public enum TpcnTipoNFe  { tnEntrada, tnSaida } // usada 0 e 1
    public enum TpcnDestinoOperacao  { doInterna = 1, doInterestadual = 2, doExterior = 3}   // usada
    public enum TpcnECFModRef  { ECFModRefVazio, ECFModRef2B, ECFModRef2C, ECFModRef2D }
    public enum TpcnTipoImpressao  { tiSemGeracao , tiRetrato , tiPaisagem , tiSimplificado, tiNFCe, tiMsgEletronica, tiNFCeA4 }
    public enum TpcnTipoEmissao  { teNormal, teContingencia, teSCAN, teDPEC, teFSDA, teSVCAN, teSVCRS, teSVCSP, teOffLine }
    public enum TpcnTipoAmbiente  { taProducao, taHomologacao }
    public enum TpcnFinalidadeNFe  { fnNormal = 1, fnComplementar = 2, fnAjuste = 3, fnDevolucao = 4} // usada
    public enum TpcnConsumidorFinal  { cfNao, cfConsumidorFinal }
    public enum TpcnPresencaComprador  { pcNao, pcPresencial, pcInternet, pcTeleatendimento, pcEntregaDomicilio, pcOutros }
    public enum TpcnProcessoEmissao  { peAplicativoContribuinte, peAvulsaFisco, peAvulsaContribuinte, peContribuinteAplicativoFisco}
    // eventos
    public enum TpcnTipoAutor { taEmpresaEmitente, taEmpresaDestinataria, taEmpresa, taFisco, taRFB, taOutros }


    public enum TpcnTpEvento
    {
        teCCe, teCancelamento, teManifDestConfirmacao,
        teManifDestCiencia, teManifDestDesconhecimento, teManifDestOperNaoRealizada,
        teEncerramento, teEPEC, teInclusaoCondutor,
        teMultiModal, teRegistroPassagem, teRegistroPassagemBRId,
        teEPECNFe, teRegistroCTe, teRegistroPassagemNFeCancelado,
        teRegistroPassagemNFeRFID, teCTeCancelado, teMDFeCancelado,
        teVistoriaSuframa, tePedProrrog1, tePedProrrog2,
        teCanPedProrrog1, teCanPedProrrog2, teEventoFiscoPP1,
        teEventoFiscoPP2, teEventoFiscoCPP1, teEventoFiscoCPP2,
        teRegistroPassagemNFe, teConfInternalizacao, teCTeAutorizado,
        teMDFeAutorizado
    };

    public static class EnumString
    {
        public static string[] TpcnTpEvento = {
            "110110", "110111", "210200",
            "210210", "210220", "210240",
            "110112", "110113", "110114",
            "110160", "310620", "510620",
            "110140", "610600", "610501",
            "610550", "610601", "610611",
            "990900", "111500", "111501",
            "111502", "111503", "411500",
            "411501", "411502", "411503",
            "610500", "990910", "000000",
            "610610"
        };
    }
    //TpcnTpEventoString : array[0..30] of String =();



}
