using Microsoft.AspNetCore.Http;

namespace SimpliHR.WebUI.Modals.ITDeclarations;

public class ITDeclarationProofDocument
{
    public string ItDeclarationParticular { get; set; }
    public int ItDeclarationParticularId { get; set; }
    public IFormFile? DocumentProofFile { get; set; }
    public byte[]? DocumentProofByte { get; set; }
    public string DocumentName { get; set; }
    public string DocumentExtension { get; set; }
}
