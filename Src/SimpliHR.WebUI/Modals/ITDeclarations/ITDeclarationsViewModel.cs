using SimpliHR.Infrastructure.Models.ITDeclaration;
using SimpliHR.Infrastructure.Models.Master;

namespace SimpliHR.WebUI.Modals.ITDeclarations;

public class ITDeclarationsViewModel
{
    public string FY { get; set; }
    public string Regime { get; set; }
    public IList<ItDeclarationHouseRentDetailDTO> ItDeclarationHouseRentDetailList { get; set; }
    public ItDeclarationHomeLoanDetailDTO ItDeclarationHomeLoanDetailDTO { get; set; }
    public IList<ItDeclarationLentOutPropertyDetailDTO> ItDeclarationLentOutPropertyDetailList { get; set; }
    public ItDeclarationOtherSourceOfIncomeDTO ItDeclarationOtherSourceOfIncomeDTO { get; set; }
    public List<ItDeclaration80CinvestmentDTO> ItDeclaration80CinvestmentList { get; set; }
    public List<ItDeclaration80DexemptionDTO> ItDeclaration80DexemptionList { get; set; }
    public List<ItDeclarationOtherInvestmentExemptionDTO> ItDeclarationOtherInvestmentExemptionList { get; set; }
    public ItDeclarationPreviousEmployementDTO ItDeclarationPreviousEmployementDTO { get; set; }
    public List<Investment80CmasterDTO> Investment80CmasterDTOs { get; set; }
    public List<Exemptions80DDTO> exemptions80DDTOs { get; set; }
    public List<OtherInvestmentExemptionDTO> otherInvestmentExemptionDTOs { get; set; }
}
