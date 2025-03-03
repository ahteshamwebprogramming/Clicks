using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using System.Linq.Expressions;
using SimpliHR.Infrastructure.Models.Login;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using SimpliHR.Services.DBContext;
using Azure.Core;
using SimpliHR.Infrastructure.Models.Employee;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SimpliHR.Infrastructure.Models.ClientManagement;
using Newtonsoft.Json;

namespace SimpliHR.Endpoints.Login;

[Route("api/[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public LoginController(IUnitOfWork unitOfWork, ILogger<LoginController> logger, IMapper mapper, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _config = config;
    }

    //[HttpPost]
    //public LoginDetailDTO GetLoginDetail(string employeeCode)
    //{
    //    try
    //    {
    //        //Call [GetLoginDetail] SP
    //        LoginDetail loginDetail = _unitOfWork.LoginDetail.GetWithRawSql(@"GetLoginDetail @p0", new[] { employeeCode }).FirstOrDefault();
    //        //Return LoginDetail Object to caller

    //        return _mapper.Map <LoginDetailDTO>(loginDetail);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Get Raw SQL {nameof(GetLoginDetail)}");
    //        throw;
    //    }
    //}


    [HttpPost]
    public async Task<IActionResult> GetLoginDetail(LoginDetailDTO loginDTO)
    {
        try
        {
            Expression<Func<LoginDetail, bool>> expression = u => (u.UserName.ToLower() == loginDTO.UserName.ToLower() || u.MobileNo == loginDTO.MobileNo) && (loginDTO.EncryptedPassword != null ? u.Password == loginDTO.EncryptedPassword : true);
            LoginDetail loginDetail = _unitOfWork.LoginDetail.Find(expression).FirstOrDefault();
            LoginDetailDTO outputDTO = _mapper.Map<LoginDetailDTO>(loginDetail);

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
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult GetLoginDetails(LoginDetailDTO loginDTO)
    {
        try
        {
            Expression<Func<LoginDetail, bool>> expression = u => (u.UserName.Trim().ToLower() == loginDTO.UserName.Trim().ToLower() || u.MobileNo.Trim() == loginDTO.UserName.Trim()) && (loginDTO.EncryptedPassword != null ? u.Password == loginDTO.EncryptedPassword : true) && u.LoginType !=3;
            LoginDetail loginDetail = _unitOfWork.LoginDetail.Find(expression).FirstOrDefault();
            LoginDetailDTO outputDTO = _mapper.Map<LoginDetailDTO>(loginDetail);

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
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetails)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult CheckLoginPasswordExists(ChangePasswordDTO inputDTO)
    {
        try
        {
            Expression<Func<LoginDetail, bool>> expression = u => u.EmployeeId == inputDTO.EmployeeId && (inputDTO.OldPasswordEncrypted != null ? u.Password.ToLower() == inputDTO.OldPasswordEncrypted : true);
            LoginDetail loginDetail = _unitOfWork.LoginDetail.Find(expression).FirstOrDefault();
            LoginDetailDTO outputDTO = _mapper.Map<LoginDetailDTO>(loginDetail);
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
            {
                //outputDTO.Password = inputDTO.NewPassword;
                //outputDTO.IsPasswordSetByUser = true;
                //bool success = _unitOfWork.LoginDetail.UpdateDbEntry(_mapper.Map<LoginDetail>(outputDTO), "Password,IsPasswordSetByUser");

                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);
            }

            outputDTO.HttpMessage = httpMessage;

            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult CheckEmailExists(ForgetPasswordDTO inputDTO)
    {
        try
        {
            Expression<Func<LoginDetail, bool>> expression = u => u.UserName == inputDTO.EmployeeEmail && u.IsActive == true;
            bool isExists = _unitOfWork.LoginDetail.Exists(expression);

            if (isExists)
            {
                EmployeeMasterDTO? employeeMaster = _mapper.Map<EmployeeMasterDTO>(_unitOfWork.EmployeeMaster.GetWithRawSql("select * from EmployeeMaster where emailid='" + inputDTO.EmployeeEmail + "' and isactive=1").FirstOrDefault());
                return Ok(employeeMaster);
            }
            else
            {
                return null;
            }

            return Ok(isExists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<bool> UpdateLogin(LoginDetailDTO inputDTO, string sProperties)
    {
        try
        {
            //LoginDetail inputProperties = _mapper.Map<LoginDetail>(properties);

            bool success = _unitOfWork.LoginDetail.UpdateDbEntry(_mapper.Map<LoginDetail>(inputDTO), sProperties);

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee updates {nameof(UpdateLogin)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult GetNewJoineeLoginDetail(LoginDetailDTO loginDTO)
    {
        try
        {
            Expression<Func<LoginDetail, bool>> expression = u => u.UserName.ToLower() == loginDTO.UserName.ToLower() && (loginDTO.EncryptedPassword != null ? u.Password == loginDTO.EncryptedPassword : true) && u.LoginType == loginDTO.LoginType;
            LoginDetail loginDetail = _unitOfWork.LoginDetail.Find(expression).FirstOrDefault();
            LoginDetailDTO outputDTO = _mapper.Map<LoginDetailDTO>(loginDetail);

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
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }

    [HttpGet(Name = "GetLoginDetails")]
    public async Task<IActionResult> GetLoginDetails()
    {
        try
        {
            IEnumerable<LoginDetail> loginDTO = await _unitOfWork.LoginDetail.GetAll(null, null);
            IList<LoginDetailDTO> loginViewModel = _mapper.Map<IList<LoginDetailDTO>>(loginDTO);

            return Ok(loginViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Details {nameof(GetLoginDetails)}");
            throw;
        }
    }
    public LoginDetailDTO GetLoginByEmployeeId(int employeeId)
    {
        try
        {
            LoginDetailDTO loginDTO = _mapper.Map<LoginDetailDTO>(_unitOfWork.LoginDetail.GetAll(null, x => x.EmployeeId == employeeId, null).Result.FirstOrDefault());
            //IList<LoginDetailDTO> loginViewModel = _mapper.Map<IList<LoginDetailDTO>>(loginDTO);

            return loginDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Details {nameof(GetLoginDetails)}");
            throw;
        }

    }



    [HttpPost]
    public async Task<IActionResult> SaveLoginDetail(LoginDetailDTO loginDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<LoginDetail, bool>> expression = u => u.UserName == loginDTO.UserName;
                if (!_unitOfWork.LoginDetail.Exists(expression))
                {
                    var loginViewModel = _mapper.Map<LoginDetail>(loginDTO);
                    loginViewModel.MobileNo = _unitOfWork.EmployeeMaster.FindFirstByExpression(x => x.EmployeeId == loginViewModel.EmployeeId).ContactNo;
                    _unitOfWork.LoginDetail.AddAsync(loginViewModel);
                    _unitOfWork.Save();
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                }
                else
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Login Detail {nameof(SaveLoginDetail)}");
            throw;
        }
    }



    [HttpPost]
    public async Task<IActionResult> UpdateLoginDetail(LoginDetailDTO loginDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<LoginDetail, bool>> expression = u => u.UserName == loginDTO.UserName && u.LoginId != loginDTO.LoginId;
                if (!_unitOfWork.LoginDetail.Exists(expression))
                {
                    var loginViewModel = _mapper.Map<LoginDetail>(loginDTO);
                    _unitOfWork.LoginDetail.Update(loginViewModel);
                    _unitOfWork.Save();
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                }
                else
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Login Detail updates {nameof(UpdateLoginDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLoginDetail(LoginDetailDTO loginDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                LoginDetail loginDetail = _mapper.Map<LoginDetail>(await _unitOfWork.LoginDetail.GetByIdAsync(loginDTO.LoginId));
                loginDetail.IsActive = false;
                _unitOfWork.LoginDetail.Update(loginDetail);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Login Detail delete {nameof(UpdateLoginDetail)}");
            throw;
        }
    }

    public async Task<LoginDetailDTO> UpldateLoginForFinalSubmit(EmployeeMasterDTO inputData)
    {
        LoginDetailDTO loginDetail = GetLoginByEmployeeId(inputData.EmployeeId);
        if (loginDetail != null)
        {
            loginDetail.EncryptedPassword = loginDetail.Password;
            loginDetail.Password = CommonHelper.Decrypt(loginDetail.EncryptedPassword);
            bool sendMail = false;
            try
            {
                if (loginDetail != null)
                {
                    loginDetail.EmployeeId = inputData.EmployeeId;
                    if ((loginDetail.JoiningMailSent == null ? false : loginDetail.JoiningMailSent) == false)
                        sendMail = true;
                    if (loginDetail.LoginType == 3)
                    {
                        loginDetail.LoginType = 2;
                        sendMail = false;
                        UpdateLogin(loginDetail, "LoginType");
                    }
                }
                else
                {
                    loginDetail = CreateLoginForEmployee(inputData);
                    sendMail = true;
                }
                if (sendMail)
                {
                    // inputData.EmployeeMastersKeyValues = await _mastersKeyValueController.IdTypeKeyValue(true);



                    //var un = HttpContext.Session.GetString("unit");
                    //UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
                    bool mailSent = MailHelper.SendLoginDetailMail(inputData, loginDetail, inputData.UnitMaster);
                    loginDetail.JoiningMailSent = mailSent;
                    UpdateLogin(loginDetail, "JoiningMailSent");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpldateLoginForFinalSubmit {nameof(UpldateLoginForFinalSubmit)}");
                throw;
            }
        }
        else
        {
            loginDetail=new LoginDetailDTO();
            loginDetail.DisplayMessage = "Could not generate login information. Contact admin for more details";
        }
        return loginDetail;
    }
    public LoginDetailDTO CreateLoginForEmployee(EmployeeMasterDTO employeeDTO)
    {
        LoginDetailDTO loginDetail = new LoginDetailDTO();
        try
        {

            loginDetail.LoginType = 2;
            loginDetail.UserName = employeeDTO.EmailId;
            loginDetail.EmployeeId = employeeDTO.EmployeeId;
            loginDetail.Password = CommonHelper.Encrypt(CommonHelper.RandomString());
            loginDetail.ClientId = employeeDTO.ClientId; //Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
            loginDetail.IsActive = true;
            SaveLoginDetail(loginDetail);
            return loginDetail;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in CreateLoginForEmployee {nameof(CreateLoginForEmployee)}");
            throw;
        }

    }


    private string GenerateToken(LoginDetailDTO user, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        _ = int.TryParse(_config["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);
        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
                new Claim(ClaimTypes.Role,role)
            };
        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}

