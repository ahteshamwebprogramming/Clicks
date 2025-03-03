using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Helper;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Login;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Services.DBContext;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Net;
using System.Text;

//namespace ClientManagement.Controllers
//{
namespace SimpliHR.Endpoints.ClientManagement;
public class ClientController : ControllerBase
{

    private readonly SimpliDbContext _simpliDbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientController> _logger;
    private readonly IMapper _mapper;
    public ClientController(IUnitOfWork unitOfWork, ILogger<ClientController> logger, IMapper mapper, SimpliDbContext SimpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = SimpliDbContext;
    }


    //[HttpPost(Name = "GetClientByID")]
    //public List<ClientDTO> GetClientByID(int ClientId)
    //{
    //    try
    //    {
    //        // IList<ClientDTO> outputModel = new List<ClientDTO>();
    //        ClientDTO outputModel = new ClientDTO();
    //        outputModel.lstClient = (from client in _simpliDbContext.Clients
    //                                 join country in _simpliDbContext.CountryMasters on client.CountryId equals country.CountryId                                    
    //                                 where (client.IsActive == true && client.ClientId== ClientId)
    //                                 select new ClientDTO()
    //                                 {
    //                                     ClientId = client.ClientId,
    //                                     ClientName = client.ClientName,
    //                                     CompanyName = client.CompanyName,
    //                                     GSTN = client.GSTN,
    //                                     EmailId = client.EmailId,
    //                                     Address = client.Address,
    //                                     CityId = client.CityId,
    //                                     StateId = client.StateId,
    //                                     CountryId = client.CountryId,
    //                                     ContactNumber = client.ContactNumber,
    //                                     Pincode = client.Pincode
    //                                     //PoliciesLink = client.PoliciesLink,
    //                                     //DocumentLink = client.DocumentLink,
    //                                     //SupportLink = client.SupportLink,
    //                                     //ColorTheme = client.ColorTheme,
    //                                     //MenuStyle = client.MenuStyle,
    //                                     //FooterText = client.FooterText,
    //                                     //HeaderText = client.HeaderText,
    //                                     //CountryName = country.CountryName,
    //                                     //StateName = state.StateName,
    //                                     //CityName = city.CityName
    //                                 }).ToList();


    //        //  outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetPagedList(requestParams)).Where(p => p.IsActive == true).ToList();
    //        return outputModel.lstClient;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Clients {nameof(GetClientByID)}");
    //        throw;
    //    }
    //}


    [HttpPost]
    public async Task<IActionResult> GetClientByID(int inputDTO)
    {
        try
        {
            ClientDTO outputDTO = _mapper.Map<ClientDTO>(await _unitOfWork.Client.GetByIdAsync(inputDTO));
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
            _logger.LogError(ex, $"Error in retriving Get client by ID {nameof(GetClientByID)}");
            throw;
        }
    }


    //[HttpPost(Name = "GetClientID")]
    public dynamic GetClientID()
    {
        try
        {


            dynamic clientId = _simpliDbContext.Clients.OrderBy(item => item.ClientId).LastOrDefault().ClientId;

            return clientId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Client Id {nameof(GetClientID)}");
            throw;
        }
        finally
        {
            // _simpliDbContext.Dispose();
        }
    }

    public async Task<ClientDTO> GetClientDetails(int ClientId)
    {
        try
        {

            ClientDTO clientDetails = _mapper.Map<ClientDTO>(await _unitOfWork.Client.GetByIdAsync(ClientId));

            return clientDetails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Client Id {nameof(GetClientID)}");
            throw;
        }
    }


    public async Task<ClientSettingDTO> GetClientSettingDetails(int ClientId)
    {
        try
        {

            //ClientSettingDTO clientDetails = _mapper.Map<ClientSettingDTO>(_unitOfWork.ClientSetting.GetAll(null, null, null).Result.Where(x => x.ClientId == ClientId).FirstOrDefault());

            ClientSettingDTO clientDetails = _mapper.Map<ClientSettingDTO>(_unitOfWork.ClientSetting.FindFirstByExpression(x => x.ClientId == ClientId));

            return clientDetails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Client Id {nameof(ClientSettingDTO)}");
            throw;
        }
    }

    public async Task<UnitMasterDTO> GetClientUnitStatus(int? UnitId)
    {
        try
        {

            UnitMasterDTO unitDetails = _mapper.Map<UnitMasterDTO>(_unitOfWork.UnitMaster.GetAll(null, null, null).Result.Where(x => x.UnitID == UnitId).FirstOrDefault());

            return unitDetails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Unit status {nameof(GetClientUnitStatus)}");
            throw;
        }
    }

    [HttpPost(Name = "GetClients")]
    public async Task<IActionResult> GetClients(Core.Helper.RequestParams requestParams)
    {
        try
        {
            var returnData = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                             orderBy: (m => m.OrderBy(x => x.CompanyName))));
            IList<ClientDTO> outputModel = new List<ClientDTO>();

            outputModel = returnData.Select(r => new ClientDTO
            {
                ClientId = r.ClientId,
                ClientName = r.ClientName,
                CompanyName = r.CompanyName,
                GSTN = r.GSTN,
                EmailId = r.EmailId,
                ContactNumber = r.ContactNumber,
                Pincode = r.Pincode,
                CountryId = r.Country.CountryId,
                CountryName = r.Country.CountryName,
                StateId = r.State.StateId,
                StateName = r.State.StateName,
                CityId = r.City.CityId,
                CityName = r.City.CityName,
                IsActive = r.IsActive
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving clients {nameof(GetClients)}");
            throw;
        }
    }


    [HttpPost(Name = "GetAllClient")]
    public List<ClientDTO> GetAllClient(SimpliHR.Core.Helper.RequestParams requestParams)
    {
        try
        {
            // IList<ClientDTO> outputModel = new List<ClientDTO>();
            ClientDTO outputModel = new ClientDTO();
            outputModel.lstClient = (from client in _simpliDbContext.Clients
                                         //join clientsetting in _simpliDbContext.ClientSettings on client.ClientId equals clientsetting.ClientId
                                         // join country in _simpliDbContext.CountryMasters on client.CountryId equals country.CountryId
                                         // join state in _simpliDbContext.StateMasters on client.StateId equals state.StateId
                                         // join city in _simpliDbContext.CityMasters on client.CityId equals city.CityId
                                     where (client.IsActive == true)
                                     select new ClientDTO()
                                     {
                                         ClientId = client.ClientId,
                                         ClientName = client.ClientName,
                                         CompanyName = client.CompanyName,
                                         // GSTN = client.GSTN,
                                         EmailId = client.EmailId,
                                         ContactNumber = client.ContactNumber,
                                         // StateName = state.StateName,
                                         // CityName = city.CityName
                                     }).OrderByDescending(x => x.ClientId).ToList();


            //  outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetPagedList(requestParams)).Where(p => p.IsActive == true).ToList();
            return outputModel.lstClient;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Clients {nameof(GetClients)}");
            throw;
        }
    }



    [HttpPost(Name = "GetClientsSetting")]
    public List<ClientSettingDTO> GetClientsSetting(int ID)
    {
        try
        {

            ClientSettingDTO outputModel = new ClientSettingDTO();


            if (ID == 0)
            {
                outputModel.lstClientSetting = (from clientsetting in _simpliDbContext.ClientSettings
                                                join client in _simpliDbContext.Clients on clientsetting.ClientId equals client.ClientId
                                                where (client.IsActive == true)
                                                select new ClientSettingDTO()
                                                {
                                                    ClientSettingId = clientsetting.ClientSettingId,
                                                    ClientId = clientsetting.ClientId,
                                                    ClientName = client.CompanyName,
                                                    ClientLogo = clientsetting.ClientLogo,
                                                    ColorTheme = clientsetting.ColorTheme,
                                                    MenuStyle = clientsetting.MenuStyle,
                                                    PoliciesLink = clientsetting.PoliciesLink,
                                                    DocumentLink = clientsetting.DocumentLink,
                                                    SupportLink = clientsetting.SupportLink,
                                                    FooterText = clientsetting.FooterText,
                                                    HeaderText = clientsetting.HeaderText,
                                                    ModuleIds = clientsetting.ModuleIds,
                                                    IDTypes = clientsetting.IDTypes,
                                                    ProfileImage = clientsetting.ProfileImage


                                                }).OrderByDescending(x => x.ClientSettingId).ToList();

            }
            else
            {
                outputModel.lstClientSetting = (from clientsetting in _simpliDbContext.ClientSettings
                                                join client in _simpliDbContext.Clients on clientsetting.ClientId equals client.ClientId
                                                where (client.IsActive == true && clientsetting.ClientSettingId == ID)
                                                select new ClientSettingDTO()
                                                {
                                                    ClientSettingId = clientsetting.ClientSettingId,
                                                    ClientId = clientsetting.ClientId,
                                                    ClientName = client.CompanyName,
                                                    ClientLogo = clientsetting.ClientLogo,
                                                    ColorTheme = clientsetting.ColorTheme,
                                                    MenuStyle = clientsetting.MenuStyle,
                                                    PoliciesLink = clientsetting.PoliciesLink,
                                                    DocumentLink = clientsetting.DocumentLink,
                                                    SupportLink = clientsetting.SupportLink,
                                                    FooterText = clientsetting.FooterText,
                                                    HeaderText = clientsetting.HeaderText,
                                                    ModuleIds = clientsetting.ModuleIds,
                                                    IDTypes = clientsetting.IDTypes

                                                }).OrderByDescending(x => x.ClientSettingId).ToList();
            }
            return outputModel.lstClientSetting;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Clients {nameof(GetClientsSetting)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> GetClientsSettingID(int inputDTO)
    {
        try
        {
            ClientSettingDTO outputDTO = _mapper.Map<ClientSettingDTO>(await _unitOfWork.ClientSetting.GetByIdAsync(inputDTO));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, true);

            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Get client by ID {nameof(GetClientByID)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetClientsSettingIDAsNoTracking(int inputDTO)
    {
        try
        {

            ClientSettingDTO outputDTO = _mapper.Map<ClientSettingDTO>(_unitOfWork.ClientSetting.Find(x => x.ClientSettingId == inputDTO).FirstOrDefault());

            var res = _unitOfWork.ClientSetting.Find(x => x.ClientSettingId == inputDTO);



            //ClientSettingDTO outputDTO = _mapper.Map<ClientSettingDTO>(await _unitOfWork.ClientSetting.GetByIdAsync(inputDTO));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, true);

            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Get client by ID {nameof(GetClientByID)}");
            throw;
        }
    }

    [HttpPost(Name = "GetModules")]
    public List<ClientDTO> GetModules(SimpliHR.Core.Helper.RequestParams requestParams)
    {
        try
        {
            // IList<ClientDTO> outputModel = new List<ClientDTO>();
            ClientDTO outputModel = new ClientDTO();
            outputModel.lstClient = (from client in _simpliDbContext.Clients
                                     join module in _simpliDbContext.ClientModuleMappings on client.ClientId equals module.ClientId
                                     join modulemaster in _simpliDbContext.ModuleMasters on module.ModuleId equals modulemaster.ModuleId
                                     where (client.IsActive == true)
                                     select new ClientDTO()
                                     {
                                         CompanyName = client.CompanyName,

                                     }).ToList();


            //  outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetPagedList(requestParams)).Where(p => p.IsActive == true).ToList();
            return outputModel.lstClient;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Clients {nameof(GetClients)}");
            throw;
        }
    }

    [HttpPost]
    public string ValidateClientInfo(ClientDTO inputDTO)
    {
        string validMessage = string.Empty;
        StringBuilder messageBuilder = new StringBuilder();
        // inputDTO.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        Expression<Func<Client, bool>> expression = a => (a.IsActive == true && a.ClientId != inputDTO.ClientId);
        IList<ClientDTO> tableData = new List<ClientDTO>();
        tableData = _mapper.Map<IList<ClientDTO>>(_unitOfWork.Client.FindAllByExpression(expression));
        if (tableData.Where((a => (a.CompanyName != null && a.CompanyName.Trim().Replace(" ", "") == inputDTO.CompanyName.Trim().Replace(" ", "")))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate Company Number({inputDTO.CompanyName.Trim()}) found</br>");
        }
        //if (tableData.Where((a => (a.GSTN.Trim() ==  inputDTO.GSTN))).Count() != 0)
        //{
        //if (tableData.Where((a => (a.EmailId != null && a.EmailId.Trim() == inputDTO.EmailId.Trim()))).Count() != 0)
        //    messageBuilder.Append($"Duplicate GSTN Number({inputDTO.GSTN.Trim()}) found</br>");
        //}
        if (tableData.Where((a => (a.EmailId != null && a.EmailId.Trim() == inputDTO.EmailId.Trim()))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate EmailID({inputDTO.EmailId.Trim()}) found</br>");
        }
        if (tableData.Where((a => (a.ContactNumber != null && a.ContactNumber == inputDTO.ContactNumber))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate Contact Number({inputDTO.ContactNumber}) found</br>");
        }


        if (!messageBuilder.ToString().Trim().IsNullOrEmpty())
            validMessage = messageBuilder.ToString();

        return validMessage;
    }

    [HttpPost]
    public string ValidateClientSettingInfo(ClientSettingDTO inputDTO)
    {
        string validMessage = string.Empty;
        StringBuilder messageBuilder = new StringBuilder();
        // inputDTO.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        Expression<Func<ClientSetting, bool>> expression = a => (a.ClientSettingId != inputDTO.ClientSettingId);
        IList<ClientSettingDTO> tableData = new List<ClientSettingDTO>();
        tableData = _mapper.Map<IList<ClientSettingDTO>>(_unitOfWork.ClientSetting.FindAllByExpression(expression));
        if (tableData.Where((a => (a.ClientId != null && a.ClientId == inputDTO.ClientId))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate client ({inputDTO.ClientId}) found</br>");
        }
        //if (tableData.Where((a => (a.GSTN.Trim() ==  inputDTO.GSTN))).Count() != 0)
        //{
        //    messageBuilder.Append($"Duplicate GSTN Number({inputDTO.GSTN.Trim()}) found</br>");
        //}
        //if (tableData.Where((a => (a.ModuleIds.Trim() == inputDTO.ModuleIds.Trim()))).Count() != 0)
        //{
        //    messageBuilder.Append($"Duplicate Modules({inputDTO.ModuleIds.Trim()}) found</br>");
        //}
        //if (tableData.Where((a => (a.IDTypes == inputDTO.IDTypes))).Count() != 0)
        //{
        //    messageBuilder.Append($"Duplicate ID Types({inputDTO.IDTypes}) found</br>");
        //}


        if (!messageBuilder.ToString().Trim().IsNullOrEmpty())
            validMessage = messageBuilder.ToString();

        return validMessage;
    }

    [HttpPost]
    public string ValidateClientUnitInfo(UnitMasterDTO inputDTO)
    {
        string validMessage = string.Empty;
        StringBuilder messageBuilder = new StringBuilder();
        // inputDTO.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        Expression<Func<UnitMaster, bool>> expression = a => (a.IsActive == true && a.ClientId != inputDTO.ClientId);
        IList<UnitMasterDTO> tableData = new List<UnitMasterDTO>();
        tableData = _mapper.Map<IList<UnitMasterDTO>>(_unitOfWork.UnitMaster.FindAllByExpression(expression));
        //if (tableData.Where((a => (a.UnitName.Trim().Replace(" ", "") == inputDTO.UnitName.Trim().Replace(" ", "")))).Count() != 0)
        //{
        //    messageBuilder.Append($"Duplicate Unit Name({inputDTO.UnitName.Trim()}) found</br>");
        //}
        if (tableData.Where((a => (a.EmailId != null && a.EmailId.Trim() == inputDTO.EmailId.Trim()))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate EmailID({inputDTO.EmailId.Trim()}) found</br>");
        }
        if (tableData.Where((a => (a.GSTN != null && a.GSTN.Trim() == inputDTO.GSTN))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate GSTN Number({inputDTO.GSTN.Trim()}) found</br>");
        }
        if (tableData.Where((a => (a.TIN != null && a.TIN.Trim() == inputDTO.TIN))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate TIN Number({inputDTO.TIN.Trim()}) found</br>");
        }
        if (tableData.Where((a => (a.PanCard != null && a.PanCard.Trim() == inputDTO.PanCard))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate Pan Card({inputDTO.PanCard.Trim()}) found</br>");
        }




        if (!messageBuilder.ToString().Trim().IsNullOrEmpty())
            validMessage = messageBuilder.ToString();

        return validMessage;
    }

    [HttpPost]
    public IActionResult SaveClientDetails(ClientDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Expression<Func<Client, bool>> expression = a => a.CompanyName.Trim().Replace(" ", "") == inputDTO.CompanyName;
                // Expression<Func<Client, bool>> expression1 = a => a.GSTN.Trim() == inputDTO.GSTN || a.ContactNumber == inputDTO.ContactNumber;
                //  if (!_unitOfWork.Client.Exists(expression1))
                // {
                //var outputDTO = _mapper.Map<EmployeeMaster>(inputDTO);
                _unitOfWork.Client.AddAsync(_mapper.Map<Client>(inputDTO));
                //_unitOfWork.Client.AddAsync(inputDTO);
                _unitOfWork.Save();
                return Ok("Success");
                //  }
                // else
                //  return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving client {nameof(SaveClientDetails)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateClient(ClientDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //  Expression<Func<Client, bool>> expression = a => a.CompanyName.Trim().Replace(" ", "") == inputDTO.CompanyName && a.ClientId != inputDTO.ClientId;
                //// if (!_unitOfWork.Client.Exists(expression))
                // {
                _unitOfWork.Client.Update(_mapper.Map<Client>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
                //  }
                // else
                //  return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee updates {nameof(UpdateClient)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteClient(ClientDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Client outputMaster = _mapper.Map<Client>(await _unitOfWork.EmployeeMaster.GetByIdAsync(inputDTO.ClientId));
                outputMaster.IsActive = false;
                _unitOfWork.Client.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee {nameof(DeleteClient)}");
            throw;
        }
    }



    [HttpPost]
    public IActionResult SaveClientSettingDetails(ClientSettingDTO inputDTO)
    {
        try
        {
            //if (ModelState.IsValid)
            //{

            Expression<Func<ClientSetting, bool>> expression1 = a => a.ClientId == inputDTO.ClientId;
            if (!_unitOfWork.ClientSetting.Exists(expression1))
            {
                //var outputDTO = _mapper.Map<EmployeeMaster>(inputDTO);
                _unitOfWork.ClientSetting.AddAsync(_mapper.Map<ClientSetting>(inputDTO));
                //_unitOfWork.Client.AddAsync(inputDTO);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            else
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate Entry Found"));
            //  }
            // return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving client setting {nameof(SaveClientSettingDetails)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult UpdateClientSetting(ClientSettingDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<ClientSetting, bool>> expression = a => a.ClientSettingId != inputDTO.ClientSettingId;
                //if (!_unitOfWork.ClientSetting.Exists(expression))
                //{
                _unitOfWork.ClientSetting.Update(_mapper.Map<ClientSetting>(inputDTO));
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                //}
                //else
                //    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in client setting updates {nameof(UpdateClientSetting)}");
            throw;
        }
    }

    #region Unit Master


    [HttpPost(Name = "GetClientUnits")]
    public async Task<IActionResult> GetClientUnits(Core.Helper.RequestParams requestParams)
    {
        try
        {
            var returnData = _mapper.Map<IList<UnitMasterDTO>>(await _unitOfWork.UnitMaster.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                             orderBy: (m => m.OrderBy(x => x.UnitName))));
            IList<UnitMasterDTO> outputModel = new List<UnitMasterDTO>();

            outputModel = returnData.Select(r => new UnitMasterDTO
            {
                UnitID = r.UnitID,
                ClientId = r.ClientId,
                UnitName = r.UnitName,
                ClientName = r.Client.CompanyName,
                GSTN = r.GSTN,
                EmailId = r.EmailId,
                PanCard = r.PanCard,
                TIN = r.TIN,
                ContactNumber = r.ContactNumber,
                ContactPerson = r.ContactPerson,
                IsBlock = r.IsBlock,
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving client Units {nameof(GetClientUnits)}");
            throw;
        }
    }


    [HttpPost(Name = "GetClientUnitNameById")]
    public async Task<IActionResult> GetClientUnitNameById(int UnitId)
    {
        try
        {
            UnitMasterDTO res = _mapper.Map<UnitMasterDTO>(await _unitOfWork.UnitMaster.GetByIdWithChildrenAsync(UnitId));



            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving client Units {nameof(GetClientUnitById)}");
            throw;
        }
    }


    [HttpPost(Name = "GetClientUnitById")]
    public async Task<UnitMasterDTO> GetClientUnitById(int UnitId)
    {

        try
        {
            UnitMasterDTO outputDTO = _mapper.Map<UnitMasterDTO>(await _unitOfWork.UnitMaster.GetByIdWithChildrenAsync(UnitId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return outputDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetClientUnitById)}");
            throw;
        }

    }

    [HttpPost]
    public IActionResult SaveClientUnitDetails(UnitMasterDTO inputDTO)
    {
        try
        {
            //if (ModelState.IsValid)
            //{

            Expression<Func<UnitMaster, bool>> expression1 = a => a.UnitID == inputDTO.UnitID;
            if (!_unitOfWork.UnitMaster.Exists(expression1))
            {

                _unitOfWork.UnitMaster.AddAsync(_mapper.Map<UnitMaster>(inputDTO));
                _unitOfWork.Save();
                int unitID = _unitOfWork.UnitMaster.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.UnitID);

                return Ok(unitID.ToString());
                // return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            else
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate Entry Found"));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving client Unit {nameof(SaveClientUnitDetails)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult UpdateClientUnit(UnitMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<ClientSetting, bool>> expression = a => a.ClientSettingId != inputDTO.ClientSettingId;
                //if (!_unitOfWork.ClientSetting.Exists(expression))
                //{
                //  _unitOfWork.UnitMaster.Remove(_mapper.Map<UnitMaster>(inputDTO));
                _unitOfWork.UnitMaster.Update(_mapper.Map<UnitMaster>(inputDTO));
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                //}
                //else
                //    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in client unit updates {nameof(UpdateClientUnit)}");
            throw;
        }
    }


    public async Task<EmployeeMasterDTO> CreateDefaultDepartmentJobTitleRole(int unitId)
    {
        EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
        try
        {
            var deps = _unitOfWork.DepartmentMaster.FindFirstByExpression(x => x.UnitId == unitId && x.IsActive == true && (x.DepartmentCode == "HR" || x.DepartmentName == "Human Resource"));
            if (deps == null)
            {
                DepartmentMaster dTO = new DepartmentMaster();
                dTO.IsActive = true;
                dTO.DepartmentCode = "HR";
                dTO.DepartmentName = "Human Resource";
                dTO.UnitId = unitId;
                var depres = _unitOfWork.DepartmentMaster.Insert(dTO);
                _unitOfWork.Save();
                if (depres != null)
                {
                    employeeMasterDTO.DepartmentId = depres.DepartmentId;
                }
            }
            var Des = _unitOfWork.JobTitleMaster.FindFirstByExpression(x => x.IsActive == true && x.JobTitle == "HR Head" && x.UnitId == unitId);
            if (deps == null)
            {
                JobTitleMaster dTO = new JobTitleMaster();
                dTO.IsActive = true;
                dTO.UnitId = unitId;
                dTO.JobTitle = "HR Head";
                var jobtitle = _unitOfWork.JobTitleMaster.Insert(dTO);
                _unitOfWork.Save();
                if (jobtitle != null)
                {
                    employeeMasterDTO.JobTitleId = jobtitle.JobTitleId;
                }
            }
            var role = _unitOfWork.RoleMaster.FindFirstByExpression(x => x.IsActive == true && x.RoleName == "Admin" && x.UnitId == unitId);
            if (role == null)
            {
                RoleMaster dTO = new RoleMaster();
                dTO.IsActive = true;
                dTO.UnitId = unitId;
                dTO.RoleName = "Admin";
                dTO.RoleType = "A";
                var roleres = _unitOfWork.RoleMaster.Insert(dTO);
                _unitOfWork.Save();
                if (roleres != null)
                {
                    employeeMasterDTO.RoleId = roleres.RoleId;
                }
            }
            return employeeMasterDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving EmployeeUnitMapping {nameof(CreateDefaultDepartmentJobTitleRole)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeUnitMapping(EmployeeUnitsMappingDTO inputDTO)
    {
        try
        {
            //if (ModelState.IsValid)
            //{

            Expression<Func<EmployeeUnitsMapping, bool>> expression1 = a => a.UnitID == inputDTO.UnitID;
            if (!_unitOfWork.EmployeeUnitsMapping.Exists(expression1))
            {

                _unitOfWork.EmployeeUnitsMapping.AddAsync(_mapper.Map<EmployeeUnitsMapping>(inputDTO));
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            else
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate Entry Found"));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving EmployeeUnitMapping {nameof(SaveClientUnitDetails)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult GeyEmployeeID(string emailId)
    {
        try
        {
            int empID = _unitOfWork.EmployeeMaster.GetAll(null, null, null).Result.Where(x => x.EmailId == emailId).DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeId);
            return Ok(empID.ToString());

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in get Employee id {nameof(GeyEmployeeID)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult GetCllientUnits(string emailId)
    {
        IList<UnitMasterDTO> outputModel = new List<UnitMasterDTO>();
        try
        {
            var returnData = _unitOfWork.UnitMaster.GetAll(null, null, null).Result.Where(x => x.EmailId == emailId && x.IsActive == true).ToList();
            outputModel = returnData.Select(r => new UnitMasterDTO
            {
                UnitID = r.UnitID,
                UnitName = r.UnitName,
                Address = r.Address,
                Pincode = r.Pincode,
                CountryName = r.Country == null ? "" : r.Country.CountryName,
                StateName = r.State==null ? "" : r.State.StateName,
                CityName = r.City==null ? "" : r.City.CityName,
                ClientId = r.ClientId
            }).ToList();

            return Ok(outputModel);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in get Employee id {nameof(GeyEmployeeID)}");
            throw;
        }
    }

    [HttpPost]
    public dynamic CheckClientEmailExistorNot(int clientId)
    {
        try
        {
            var unitDetails = _unitOfWork.UnitMaster.GetAll(null, null, null).Result.Where(x => x.ClientId == clientId).Select(x => new { x.EmailId, x.ContactNumber }).FirstOrDefault();
            return unitDetails;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in getCheckClientEmailExistorNot {nameof(CheckClientEmailExistorNot)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<LoginDetailDTO> GetClientLoginDetailsByUnitId(int UnitId)
    {
        try
        {
            string query = $"Select * from LoginDetails where ClientId=(Select ClientId from UnitMaster where UnitID={UnitId}) and LoginType=1";
            var x = _unitOfWork.LoginDetail.GetWithRawSql(query);
            if (x != null && x.Count() > 0)
            {
                LoginDetailDTO loginDetailDTO = new LoginDetailDTO();
                loginDetailDTO = _mapper.Map<LoginDetailDTO>(x.FirstOrDefault());
                return loginDetailDTO;
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in getCheckClientEmailExistorNot {nameof(GetClientLoginDetailsByUnitId)}");
            throw;
        }
    }



    [HttpPost]
    public IActionResult BlockUnits(int unitId, int isBlock)
    {
        try
        {
            var clientUnitId = new Microsoft.Data.SqlClient.SqlParameter("@UnitId", unitId);
            var isBlocked = new Microsoft.Data.SqlClient.SqlParameter("@IsBlocked", isBlock);

            _unitOfWork.UnitMaster.ExecuteRawQuery("EXEC usp_BlockandunBlockUnits @UnitId,@IsBlocked", new[] { clientUnitId, isBlocked });

            //if (ModelState.IsValid)
            //{
            //    var Details = _unitOfWork.UnitMaster.GetAll(null, null, null).Result.Where(x => x.UnitID == unitId).FirstOrDefault();
            //    UnitMasterDTO inputDTO = new UnitMasterDTO();
            //    //Expression<Func<ClientSetting, bool>> expression = a => a.ClientSettingId != inputDTO.ClientSettingId;
            //    //if (!_unitOfWork.ClientSetting.Exists(expression))
            //    //{
            //    _unitOfWork.UnitMaster.Update(_mapper.Map<UnitMaster>(inputDTO));
            //   // _unitOfWork.UnitMaster.ExecuteRawQuery()
            //    _unitOfWork.Save();
            //    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            //    //}
            //    //else
            //    //    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            //}
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in client unit block {nameof(BlockUnits)}");
            throw;
        }
    }


    #endregion



}
//}
