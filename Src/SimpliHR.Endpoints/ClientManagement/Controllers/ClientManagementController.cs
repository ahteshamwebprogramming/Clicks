using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Services.DBContext;


namespace SimpliHR.Endpoints.ClientManagement;

[Route("api/[controller]/[action]")]
[ApiController]
public class ClientManagementController : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientManagementController> _logger;
    private readonly IMapper _mapper;
    public ClientManagementController(IUnitOfWork unitOfWork, ILogger<ClientManagementController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetClient(ClientDTO inputDTO)
    {
        try
        {
            ClientDTO outputDTO = _mapper.Map<ClientDTO>(await _unitOfWork.Client.GetByIdAsync(inputDTO.ClientId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Client {nameof(GetClient)}");
            throw;
        }
    }

    [HttpPost(Name = "GetClients")]
    public async Task<IActionResult> GetClients(SimpliHR.Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<ClientDTO> outputModel = new List<ClientDTO>();
            outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.EmployeeMaster.GetPagedList(requestParams)).Where(p => p.IsActive == true).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Clients {nameof(GetClients)}");
            throw;
        }
    }
}

