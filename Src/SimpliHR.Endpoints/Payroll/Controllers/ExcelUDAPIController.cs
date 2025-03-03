using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Infrastructure.Models.Employee;
using System.Data;
using System.Data.OleDb;
using System.Linq.Expressions;
using System.Net;
using System.IO;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Masters;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using SimpliHR.Infrastructure.Models.Login;

namespace SimpliHR.Endpoints.ExcelUploads;


[Route("api/[controller]/[action]")]
[ApiController]
public class ExcelUDAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExcelUDAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    //private IConfiguration _configuration;

    public ExcelUDAPIController(IUnitOfWork unitOfWork, ILogger<ExcelUDAPIController> logger, IMapper mapper, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _config = config;
        //_configuration = configuration;
        //IConfiguration configuration
        // _simpliDbContext = SimpliDbContext;
    }
    [HttpPost]
    public IActionResult SaveEmployeeSalaryDetails(EmployeesSalaryDetailsDTO inputDTO)
    {
        try
        {

            Expression<Func<EmployeesSalaryDetails, bool>> expression1 = a => a.UnitId == inputDTO.UnitId && a.EmployeeId == inputDTO.EmployeeId && a.SalaryComponentId == inputDTO.SalaryComponentId && a.SalaryMonth == inputDTO.SalaryMonth && a.SalaryYear == inputDTO.SalaryYear;
            if (!_unitOfWork.EmployeesSalaryDetails.Exists(expression1))
            {
                _unitOfWork.EmployeesSalaryDetails.AddAsync(_mapper.Map<EmployeesSalaryDetails>(inputDTO));
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            else
            {
                EmployeesSalaryDetailsDTO employeeSalaryIds = _mapper.Map<EmployeesSalaryDetailsDTO>(_unitOfWork.EmployeesSalaryDetails.GetAll(null, null, null).Result.Where(a => a.UnitId == inputDTO.UnitId && a.EmployeeId == inputDTO.EmployeeId && a.SalaryComponentId == inputDTO.SalaryComponentId && a.SalaryMonth == inputDTO.SalaryMonth && a.SalaryYear == inputDTO.SalaryYear).FirstOrDefault());
                inputDTO.EmployeeSalaryId = employeeSalaryIds.EmployeeSalaryId;
                inputDTO.AmtPerMonth = employeeSalaryIds.AmtPerMonth;
                inputDTO.WEF = employeeSalaryIds.WEF;
                _unitOfWork.EmployeesSalaryDetails.Update(_mapper.Map<EmployeesSalaryDetails>(inputDTO));
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee salary {nameof(SaveEmployeeSalaryDetails)}");
            throw;
        }
    }

    [HttpPost(Name = "GetEmployeeLeaveDetails")]
    public async Task<IActionResult> GetEmployeeSalaryDetails(int employeeId, int? UnitId, int? Year, int? Month)
    {
        EmployeesSalaryDetailsDTO SalaryList = new EmployeesSalaryDetailsDTO();

        var returnData = _mapper.Map<IList<EmployeesSalaryDetailsDTO>>(await _unitOfWork.EmployeesSalaryDetails.GetAll(null,
        expression: (p => p.EmployeeId == employeeId && p.UnitId == UnitId && p.SalaryYear == Year && p.SalaryMonth == Month && p.AmtPerMonth > 0), orderBy: (m => m.OrderBy(x => x.ComponentName))));
        IList<EmployeesSalaryDetailsDTO>? outputModel = new List<EmployeesSalaryDetailsDTO>();

        outputModel = returnData.Select(r => new EmployeesSalaryDetailsDTO
        {
            EmployeeSalaryId = r.EmployeeSalaryId,
            ComponentName = r.ComponentName,
            AmtPerMonth = r.AmtPerMonth,
            ArrearsAmt = r.ArrearsAmt
        }).ToList();

        // SalaryList.SalaryDetails = outputModel;
        return Ok(outputModel);


    }


    [HttpPost]
    public async Task<string> SalaryProcessing(SalaryProcessInputs inputVariables)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"EmpId", inputVariables.EmployeeId, DbType.Int32);
            parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            parms.Add(@"@Month", inputVariables.Month, DbType.Int32);
            parms.Add(@"@Year", inputVariables.Year, DbType.Int32);
            parms.Add(@"@FY", inputVariables.FY, DbType.String);
            try
            {
                var dset = await _unitOfWork.EmployeesSalaryProcessDetails.GetSPData("usp_SalaryCalculationCTCViaExl", parms);
            }
            catch (Exception ex) { return "Error while Processing the Salary."; }
            try
            {
                string returnMessage = "Salaries have been Successfully Processed";//await _unitOfWork.ManualPunches.SendManualPunchActionMail(userAction);

                return returnMessage;

            }
            catch (Exception ex) { return "Error while sending mail to user."; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while Processing the Salary {nameof(SalaryProcessing)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<List<EmployeeSalarySummary>> GetSalarySummery(SalaryProcessInputs inputVariables)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"EmpId", inputVariables.EmployeeId, DbType.Int32);
            parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            parms.Add(@"@Month", inputVariables.Month, DbType.String);
            parms.Add(@"@Year", inputVariables.Year, DbType.String);
            try
            {
                var result = await _unitOfWork.EmployeeSalarySummary.GetSPData("usp_GetEmployeeSalary", parms);
                // objResult = (SalarySummery)result;
                List<EmployeeSalarySummary>? objResultData = (List<EmployeeSalarySummary>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetSalarySummery)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<List<EmployeeSalarySummaryDTO>> GetProcessedSalaryDetails(SalaryProcessInputs inputVariables)
    {
        try
        {

            var parms = new DynamicParameters();
            //  parms.Add(@"EmpId", inputVariables.EmployeeId, DbType.Int32);
            parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            parms.Add(@"@Month", inputVariables.Month, DbType.Int32);
            parms.Add(@"@Year", inputVariables.Year, DbType.Int32);
            try
            {
                var result = _mapper.Map<List<EmployeeSalarySummaryDTO>>(await _unitOfWork.EmployeeSalarySummary.GetSPData("usp_GetEmployeeProcessedSalary", parms));
                // objResult = (SalarySummery)result;
                List<EmployeeSalarySummaryDTO>? objResultData = (List<EmployeeSalarySummaryDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetSalarySummery)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<List<EmployeesSalaryProcessDetails>> GetSalaryDetails(SalaryProcessInputs inputVariables)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"EmpId", inputVariables.EmployeeId, DbType.Int32);
            parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            parms.Add(@"@Month", inputVariables.Month, DbType.String);
            parms.Add(@"@Year", inputVariables.Year, DbType.String);
            try
            {
                var result = await _unitOfWork.EmployeesSalaryProcessDetails.GetSPData("usp_GetEmployeeSalaryDetails", parms);
                // objResult = (SalarySummery)result;
                List<EmployeesSalaryProcessDetails>? objResultData = (List<EmployeesSalaryProcessDetails>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }
            //try
            //{
            //    string returnMessage = "Success";//await _unitOfWork.ManualPunches.SendManualPunchActionMail(userAction);

            //   // return objResultData;

            //}
            //catch (Exception ex) { return null; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetSalarySummery)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<List<EmployeeSalarySummaryDTO>> UpdateSalaryDetails(SalaryParameters inputVariables)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeIds", inputVariables.EmployeeIds, DbType.String);
            parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            parms.Add(@"@ProcessMonth", inputVariables.ProcessMonth, DbType.String);
            parms.Add(@"@ProcessYear", inputVariables.ProcessYear, DbType.String);
            parms.Add(@"@ActualDays", inputVariables.ActualDays, DbType.String);
            parms.Add(@"@ActionRemarks", inputVariables.PayrollRemarks, DbType.String);
            parms.Add(@"@FixedSalaries", inputVariables.FixedSalary, DbType.String);
            parms.Add(@"@PayOutSalaries", inputVariables.PayOutSalary, DbType.String);
            parms.Add(@"@WorkingDays", inputVariables.NoOfDays, DbType.String);

            try
            {
                var result = _mapper.Map<List<EmployeeSalarySummaryDTO>>(await _unitOfWork.EmployeeSalarySummary.GetSPData("usp_UpdateEmployeeSalary", parms));
                // objResult = (SalarySummery)result;
                List<EmployeeSalarySummaryDTO>? objResultData = (List<EmployeeSalarySummaryDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }
            //try
            //{
            //    string returnMessage = "Success";//await _unitOfWork.ManualPunches.SendManualPunchActionMail(userAction);

            //   // return objResultData;

            //}
            //catch (Exception ex) { return null; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetSalarySummery)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<List<EmployeeSalarySummaryDTO>> FreezeSalaryDetails(SalaryParameters inputVariables)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeIds", inputVariables.EmployeeIds, DbType.String);
            parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            parms.Add(@"@ProcessMonth", inputVariables.ProcessMonth, DbType.String);
            parms.Add(@"@ProcessYear", inputVariables.ProcessYear, DbType.String);

            try
            {
                var result = _mapper.Map<List<EmployeeSalarySummaryDTO>>(await _unitOfWork.EmployeeSalarySummary.GetSPData("usp_FreezeEmployeeSalary", parms));
                // objResult = (SalarySummery)result;
                List<EmployeeSalarySummaryDTO>? objResultData = (List<EmployeeSalarySummaryDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }
            //try
            //{
            //    string returnMessage = "Success";//await _unitOfWork.ManualPunches.SendManualPunchActionMail(userAction);

            //   // return objResultData;

            //}
            //catch (Exception ex) { return null; }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(FreezeSalaryDetails)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<List<PaySlipDetailsDTO>> GetPaySlipDetails(int EmployeeId)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeId", EmployeeId, DbType.Int32);
            // parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            //parms.Add(@"@Month", inputVariables.Month, DbType.Int32);
            //parms.Add(@"@Year", inputVariables.Year, DbType.Int32);
            try
            {
                var result = _mapper.Map<List<PaySlipDetailsDTO>>(await _unitOfWork.PaySlipDetails.GetSPData("usp_GetEmployeeDetailsforPaySlip", parms));
                // objResult = (SalarySummery)result;
                List<PaySlipDetailsDTO>? objResultData = (List<PaySlipDetailsDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetSalarySummery)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<List<PaySlipComponentsDTO>> GetPaySlipComponents(SalaryProcessInputs inputVariables)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"@EmpId", inputVariables.EmployeeId, DbType.Int32);
            parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            parms.Add(@"@Month", inputVariables.Month, DbType.Int32);
            parms.Add(@"@Year", inputVariables.Year, DbType.Int32);
            try
            {
                var result = _mapper.Map<List<PaySlipComponentsDTO>>(await _unitOfWork.PaySlipComponents.GetSPData("usp_GetComponentsforPaySlip", parms));
                // objResult = (SalarySummery)result;
                List<PaySlipComponentsDTO>? objResultData = (List<PaySlipComponentsDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetSalarySummery)}");
            throw;
        }
    }

    public async Task<bool> EditExcelDataOfEmployee(string excelFilePath)
    {
        string constr = string.Empty;
        //OleDbConnectionStringBuilder sbConnection = new OleDbConnectionStringBuilder();
        //String strExtendedProperties = String.Empty;
        ////sbConnection.DataSource = excelFilePath;
        //if (Path.GetExtension(excelFilePath).Equals(".xls"))//for 97-03 Excel file
        //{

        //    constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelFilePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        //}
        //else if (Path.GetExtension(excelFilePath).Equals(".xlsx"))  //for 2007 Excel file
        //{
        constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelFilePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //}

        constr = string.Format(constr, excelFilePath);
        DataTable datatable = new DataTable();
        using (OleDbConnection excelconn = new OleDbConnection(constr))
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
                {

                    excelconn.Open();
                    cmd.Connection = excelconn;
                    DataTable excelschema;
                    excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string sheetname = string.Empty; //excelschema.Rows["Personal$"].ToString();
                    foreach (DataRow drSheet in excelschema.Rows)
                        if (drSheet["TABLE_NAME"].ToString().Contains("Personal$"))
                        {
                            sheetname = drSheet["TABLE_NAME"].ToString();
                            break;
                        }
                    excelconn.Close();

                    excelconn.Open();
                    cmd.CommandText = "SELECT * From [" + sheetname + "]";
                    adapterexcel.SelectCommand = cmd;
                    adapterexcel.Fill(datatable);
                    excelconn.Close();

                }
            }
        }
        bool isSaved = false;
        if (datatable.AsEnumerable().Select(r => r).ToList().Count > 0)
        {
            List<EmployeeMaster> listFromData = new List<EmployeeMaster>();
            listFromData = CommonHelper.ConvertToList<EmployeeMaster>(datatable);
            listFromData.ForEach(x => x.ModifiedOn = DateTime.Now);
            listFromData.ForEach(x => x.EmployeeId = _unitOfWork.EmployeeMaster.FindFirstByExpression(p => p.EmployeeCode == x.EmployeeCode).EmployeeId);
            isSaved = await _unitOfWork.EmployeeMaster.UpdateRange(listFromData);
            _unitOfWork.Save();
        }
        return isSaved;
    }

    public async Task<string> SaveExcelDataToEmployee(string excelFilePath)
    {
        string constr = string.Empty;
        bool isSaved = false;
        string returnMsg = string.Empty;
        constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelFilePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        constr = string.Format(constr, excelFilePath);
        try
        {
            Dictionary<string, DataTable> empMasterData = new Dictionary<string, DataTable>();
            using (OleDbConnection excelconn = new OleDbConnection(constr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
                    {
                        string sList = string.Empty;
                        excelconn.Open();
                        cmd.Connection = excelconn;
                        DataTable excelschema;
                        excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = string.Empty; //excelschema.Rows["Personal$"].ToString();
                        List<EmployeeMaster> listMasterData = new List<EmployeeMaster>();
                        foreach (DataRow drSheet in excelschema.Rows)
                        {
                            DataTable datatable = new DataTable();
                            sheetName = drSheet["TABLE_NAME"].ToString();
                            excelconn.Close();
                            if (sheetName.ToLower().Contains("_data"))
                            {
                                excelconn.Open();
                                cmd.CommandText = "SELECT * From [" + sheetName + "]";
                                adapterexcel.SelectCommand = cmd;
                                adapterexcel.Fill(datatable);
                                excelconn.Close();
                                empMasterData.Add(sheetName.ToLower().Replace("_data$", ""), datatable);
                                //if (drSheet["TABLE_NAME"].ToString().Contains("Master$"))
                                //{
                                //    listMasterData = CommonHelper.ConvertToList<EmployeeMaster>(datatable);

                                //}
                            }

                        }
                        returnMsg = await SaveSheetInfo(empMasterData);
                    }
                }
            }
        }
        catch (Exception ex)
        {

            returnMsg = $"Source: {ex.Source}({nameof(SaveExcelDataToEmployee)})\n{ex.Message}";
            string sLogPath = _config.GetValue<string>("LogFilePathName");
            CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", returnMsg);

        }

        return returnMsg;
    }

    public async Task<string> SaveSheetInfo(Dictionary<string, DataTable> empMasterData)
    {
        bool isSaved = false;
        string sEmpCode = string.Empty;
        string empCodeToAvoid = string.Empty;
        string sReturnMsg = "Success";
        string sFinalMsg = string.Empty;
        List<string> sheetNameList = new List<string>() { "master", "contactdetail", "bankdetail", "familydetail", "academicdetail", "experiencedetail", "certificationdetail", "referencedetail", "languagedetail" }; ;
        string sheetname = "master";

        List<EmployeeMaster> listMasterData = new List<EmployeeMaster>();
        //listMasterData = (empMasterData[sheetname]);
        foreach (var sheetName in sheetNameList)
        {
            DataTable dataTable = empMasterData[sheetName];
            switch (sheetName)
            {
                case "master":
                    EmployeeMaster empData = new EmployeeMaster();
                    //listMasterData.ForEach(x => x.CreatedOn = DateTime.Now);
                    listMasterData = CommonHelper.ConvertToList<EmployeeMaster>(dataTable);
                    if (listMasterData.Count == 0)
                        return "No data to upload";
                    listMasterData.ForEach(x =>
                    {
                        if (string.IsNullOrEmpty(x.EmailId) && string.IsNullOrEmpty(x.ContactNo))
                        {
                            empCodeToAvoid += empCodeToAvoid == "" ? x.EmployeeCode : ", " + x.EmployeeCode;
                            sReturnMsg = empCodeToAvoid;
                        }
                        else {
                            empData = _unitOfWork.EmployeeMaster.FindFirstByExpression(p => (x.EmailId != null && p.EmailId == x.EmailId) || (x.ContactNo != null && p.ContactNo == x.ContactNo));
                            if (empData != null)
                            {
                                empCodeToAvoid += empCodeToAvoid == "" ? empData.EmployeeCode : ", " + empData.EmployeeCode;
                                sReturnMsg = empCodeToAvoid;
                            }                               
                        }
                    });

                    if (!string.IsNullOrEmpty(empCodeToAvoid))
                        return $"Contact No and EmailId is missing or duplicate emailid or contact number found for Employee code : {sReturnMsg}. Fail to save file";

                    empCodeToAvoid = string.Empty;
                    sReturnMsg = string.Empty;

                    listMasterData.ForEach(x =>
                    {
                        empData = _unitOfWork.EmployeeMaster.FindFirstByExpression(p => p.EmployeeCode == x.EmployeeCode);
                        if (empData != null)
                            sEmpCode = empData.EmployeeCode;

                        if (string.IsNullOrEmpty(sEmpCode))
                        {
                            x.CreatedOn = DateTime.Now;
                        }
                        else
                        {
                            empCodeToAvoid += empCodeToAvoid == "" ? sEmpCode : ", " + sEmpCode;
                            sReturnMsg = empCodeToAvoid;
                        }
                    });

                    if (!string.IsNullOrEmpty(empCodeToAvoid))
                        return $"Employee code already exists : {sReturnMsg}. Fail to save file";

                    isSaved = await _unitOfWork.EmployeeMaster.AddRangeAsync(listMasterData.Where(p => p.EmployeeId == null || p.EmployeeId == 0).Select(x => x).ToList());
                    _unitOfWork.Save();
                    if (isSaved)
                    {
                        sReturnMsg = await SaveExcelEmployeeLoginInfo(listMasterData);
                    }

                    _unitOfWork.Save();
                    break;
                case "contactdetail":
                    if (listMasterData.Count == 0)
                        break;
                    List<EmployeeContactDetailDTO> listContactData = new List<EmployeeContactDetailDTO>();
                    listContactData = CommonHelper.ConvertToList<EmployeeContactDetailDTO>(dataTable);
                    //listContactData.ForEach(x => x.CreatedOn = DateTime.Now);
                    listContactData.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.EmployeeCode))
                        {
                            x.CreatedOn = DateTime.Now;
                            x.EmployeeId = listMasterData.Where(p => p.EmployeeCode == x.EmployeeCode).FirstOrDefault().EmployeeId;
                        }

                    });
                    isSaved = await _unitOfWork.EmployeeContactDetail.AddRangeAsync(_mapper.Map<List<EmployeeContactDetail>>(listContactData.Where(p => p.EmployeeId != null && p.EmployeeId > 0).Select(x => x).ToList()));
                    _unitOfWork.Save();
                    break;
                case "bankdetail":
                    if (listMasterData.Count == 0)
                        break;
                    List<EmployeeBankDetailDTO> listBankDetail = new List<EmployeeBankDetailDTO>();
                    listBankDetail = CommonHelper.ConvertToList<EmployeeBankDetailDTO>(dataTable);
                    //listBankDetail = CommonHelper.ConvertToList<EmployeeBankDetailDTO>(dataTable.Rows.Remove(();
                    listBankDetail.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.EmployeeCode))
                        {
                            x.CreatedOn = DateTime.Now;
                            x.EmployeeId = listMasterData.Where(p => p.EmployeeCode == x.EmployeeCode).FirstOrDefault().EmployeeId;
                        }
                    });
                    isSaved = await _unitOfWork.EmployeeBankDetail.AddRangeAsync(_mapper.Map<List<EmployeeBankDetail>>(listBankDetail.Where(p => p.EmployeeId != null && p.EmployeeId > 0).Select(x => x).ToList()));
                    _unitOfWork.Save();
                    break;
                case "familydetail":
                    if (listMasterData.Count == 0)
                        break;
                    List<EmployeeFamilyDetailDTO> listFamilyDatail = new List<EmployeeFamilyDetailDTO>();
                    listFamilyDatail = CommonHelper.ConvertToList<EmployeeFamilyDetailDTO>(dataTable);
                    listFamilyDatail.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.EmployeeCode))
                        {
                            x.CreatedOn = DateTime.Now;
                            x.EmployeeId = listMasterData.Where(p => p.EmployeeCode == x.EmployeeCode).FirstOrDefault().EmployeeId;
                        }
                    });
                    isSaved = await _unitOfWork.EmployeeFamilyDetail.AddRangeAsync(_mapper.Map<List<EmployeeFamilyDetail>>(listFamilyDatail.Where(p => p.EmployeeId != null && p.EmployeeId > 0).Select(x => x).ToList()));
                    _unitOfWork.Save();
                    break;
                case "academicdetail":
                    if (listMasterData.Count == 0)
                        break;
                    List<EmployeeAcademicDTO> listAcademicDetail = new List<EmployeeAcademicDTO>();
                    listAcademicDetail = CommonHelper.ConvertToList<EmployeeAcademicDTO>(dataTable);
                    listAcademicDetail.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.EmployeeCode))
                        {
                            x.CreatedOn = DateTime.Now;
                            x.EmployeeId = listMasterData.Where(p => p.EmployeeCode == x.EmployeeCode).FirstOrDefault().EmployeeId;
                        }
                    });

                    isSaved = await _unitOfWork.EmployeeAcademicDetail.AddRangeAsync(_mapper.Map<List<EmployeeAcademicDetail>>(listAcademicDetail.Where(p => p.EmployeeId != null && p.EmployeeId > 0).Select(x => x).ToList()));
                    _unitOfWork.Save();
                    break;
                case "experiencedetail":
                    if (listMasterData.Count == 0)
                        break;
                    List<EmployeeExperienceDetailDTO> listExperienceDetail = new List<EmployeeExperienceDetailDTO>();
                    listExperienceDetail = CommonHelper.ConvertToList<EmployeeExperienceDetailDTO>(dataTable);
                    listExperienceDetail.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.EmployeeCode))
                        {
                            x.CreatedOn = DateTime.Now;
                            x.EmployeeId = listMasterData.Where(p => p.EmployeeCode == x.EmployeeCode).FirstOrDefault().EmployeeId;
                        }
                    });
                    isSaved = await _unitOfWork.EmployeeExperienceDetail.AddRangeAsync(_mapper.Map<List<EmployeeExperienceDetail>>(listExperienceDetail.Where(p => p.EmployeeId != null && p.EmployeeId > 0).Select(x => x).ToList()));
                    _unitOfWork.Save();
                    break;
                case "certificationdetail":
                    if (listMasterData.Count == 0)
                        break;
                    List<EmployeeCertificationDetailDTO> listCertificationDetail = new List<EmployeeCertificationDetailDTO>();
                    listCertificationDetail = CommonHelper.ConvertToList<EmployeeCertificationDetailDTO>(dataTable);
                    listCertificationDetail.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.EmployeeCode))
                        {
                            x.CreatedOn = DateTime.Now;
                            x.EmployeeId = listMasterData.Where(p => x.EmployeeCode != null && p.EmployeeCode == x.EmployeeCode).FirstOrDefault().EmployeeId;
                        }
                    });
                    isSaved = await _unitOfWork.EmployeeCertificationDetail.AddRangeAsync(_mapper.Map<List<EmployeeCertificationDetail>>(listCertificationDetail.Where(p => p.EmployeeId != null && p.EmployeeId > 0).Select(x => x).ToList()));
                    _unitOfWork.Save();
                    break;
                case "referencedetail":
                    if (listMasterData.Count == 0)
                        break;
                    List<EmployeeReferenceDetailDTO> listReferenceDetail = new List<EmployeeReferenceDetailDTO>();
                    listReferenceDetail = CommonHelper.ConvertToList<EmployeeReferenceDetailDTO>(dataTable);
                    listReferenceDetail.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.EmployeeCode))
                        {
                            x.CreatedOn = DateTime.Now;
                            x.ReferenceOf = listMasterData.Where(p => p.EmployeeCode == x.EmployeeCode).FirstOrDefault().EmployeeId;
                        }
                    });
                    isSaved = await _unitOfWork.EmployeeReferenceDetail.AddRangeAsync(_mapper.Map<List<EmployeeReferenceDetail>>(listReferenceDetail.Where(p => p.ReferenceOf != null && p.ReferenceOf > 0).Select(x => x).ToList()));
                    _unitOfWork.Save();
                    break;
                case "languagedetail":
                    if (listMasterData.Count == 0)
                        break;
                    List<EmployeeLanguageDetailDTO> listLanguageDetail = new List<EmployeeLanguageDetailDTO>();
                    listLanguageDetail = CommonHelper.ConvertToList<EmployeeLanguageDetailDTO>(dataTable);
                    string yesStr = "Y,Yes";
                    listLanguageDetail.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.EmployeeCode))
                        {
                            if (x.EmployeeCode != null)
                            {
                                x.CreatedOn = DateTime.Now;
                                x.EmployeeId = listMasterData.Where(p => p.EmployeeCode == x.EmployeeCode).FirstOrDefault().EmployeeId;
                                x.Read = yesStr.Contains(string.IsNullOrEmpty(x.CanRead) ? "N" : x.CanRead) ? true : false;
                                x.Write = yesStr.Contains(string.IsNullOrEmpty(x.CanWrite) ? "N" : x.CanRead) ? true : false;
                                x.Speak = yesStr.Contains(string.IsNullOrEmpty(x.CanSpeak) ? "N" : x.CanRead) ? true : false;
                            }

                        }

                    });
                    isSaved = await _unitOfWork.EmployeeLanguageDetail.AddRangeAsync(_mapper.Map<IList<EmployeeLanguageDetail>>(listLanguageDetail.Where(p => p.EmployeeId != null && p.EmployeeId > 0 && p.LanguageId != null && p.LanguageId > 0).Select(x => x).ToList()));
                    _unitOfWork.Save();
                    break;
            }
        }


        return sReturnMsg;
    }

    public async Task<string> SaveExcelEmployeeLoginInfo(List<EmployeeMaster> listMasterData)
    {
        bool isSaved = false;
        try
        {
            List<LoginDetailDTO> loginDetailList = new List<LoginDetailDTO>();
            loginDetailList = await CreateLoginForEmployee(listMasterData);
            if (loginDetailList != null && loginDetailList.Count > 0)
            {
                isSaved = await _unitOfWork.LoginDetail.AddRangeAsync(_mapper.Map<IList<LoginDetail>>(loginDetailList).ToList());
                _unitOfWork.Save();
            }
            if (isSaved)
                return "Success";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return @"Error while saving login info {nameof(SaveEmployeeSalaryDetails)}";
    }

    public async Task<List<LoginDetailDTO>> CreateLoginForEmployee(List<EmployeeMaster> listMasterData)
    {
        List<LoginDetailDTO> loginDetailList = new List<LoginDetailDTO>();
        LoginDetailDTO loginDetail = new LoginDetailDTO();

        try
        {
            listMasterData.ForEach(x =>
            {
                loginDetail.LoginType = 2;
                loginDetail.UserName = x.EmailId;
                loginDetail.MobileNo = x.ContactNo;
                loginDetail.EmployeeId = _unitOfWork.EmployeeMaster.FindFirstByExpression(p=>p.EmployeeCode==x.EmployeeCode).EmployeeId;
                loginDetail.Password = CommonHelper.Encrypt(CommonHelper.RandomString());
                loginDetail.ClientId = x.ClientId;
                loginDetail.JoiningMailSent = true;
                loginDetail.IsActive = true;
                loginDetailList.Add(loginDetail);
            });

            //SaveLoginDetail(loginDetail);
            return loginDetailList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in CreateLoginForEmployee {nameof(CreateLoginForEmployee)}");
            throw;
        }

    }


    [HttpPost]
    public IActionResult SaveSattlementMaster(PayrollFullnFinalSettingsDTO inputDTO)
    {
        try
        {

            Expression<Func<PayrollFullnFinalSettings, bool>> expression1 = a => a.UnitId == inputDTO.UnitId;
            if (!_unitOfWork.PayrollFullnFinalSettings.Exists(expression1))
            {
                _unitOfWork.PayrollFullnFinalSettings.AddAsync(_mapper.Map<PayrollFullnFinalSettings>(inputDTO));
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            else
            {
                PayrollFullnFinalSettingsDTO FnFSettings = _mapper.Map<PayrollFullnFinalSettingsDTO>(_unitOfWork.PayrollFullnFinalSettings.GetAll(null, null, null).Result.Where(a => a.UnitId == inputDTO.UnitId && a.IsActive == true).FirstOrDefault());
                inputDTO.CreatedBy = FnFSettings.CreatedBy;
                inputDTO.CreatedOn = FnFSettings.CreatedOn;
                //inputDTO.WEF = employeeSalaryIds.WEF;
                _unitOfWork.PayrollFullnFinalSettings.Update(_mapper.Map<PayrollFullnFinalSettings>(inputDTO));
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee salary {nameof(SaveEmployeeSalaryDetails)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetSettlementMaster(PayrollFullnFinalSettingsDTO inputDTO)
    {
        try
        {
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            //inputDTO.SettingId = 0;
            PayrollFullnFinalSettingsDTO SettlementId = _mapper.Map<PayrollFullnFinalSettingsDTO>(_unitOfWork.PayrollFullnFinalSettings.GetAll(null, null, null).Result.Where(a => a.UnitId == inputDTO.UnitId && a.IsActive == true).FirstOrDefault());
            if (SettlementId == null)
                inputDTO.SettingId = 0;
            else
                inputDTO.SettingId = SettlementId.SettingId;

            PayrollFullnFinalSettingsDTO outputDTO = _mapper.Map<PayrollFullnFinalSettingsDTO>(await _unitOfWork.PayrollFullnFinalSettings.GetByIdAsync(inputDTO.SettingId));

            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);
            //  LoanPaymentDetailDTO loanDetails = _mapper.Map<LoanPaymentDetailDTO>(_unitOfWork.LoanPaymentDetail.GetAll(null, null, null).Result.Where(a => a.RepaymentId == inputs.RepaymetId).FirstOrDefault());
            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee Loan {nameof(GetSettlementMaster)}");
            throw;
        }
    }

    public async Task<byte[]> ReadFileFromLocation(string fileLocation)
    {
        string filePath = fileLocation;
        byte[] fileBytes;

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                fileBytes = br.ReadBytes((int)fs.Length);
            }
        }
        return fileBytes;
    }
    public async Task<Fom16DocViewModel> SaveForm16Data(string unZipFilePath, Fom16DocViewModel form16DocVM)
    {
        string allFileName = string.Empty, sQry = string.Empty;
        string[] arrFiles;
        string[] files = Directory.GetFiles(unZipFilePath);
        bool isEmpFound = false;
        EmployeeMaster empMaster = new EmployeeMaster();
        List<Form16DocDTO> ListOfform16DocNotMatched = new List<Form16DocDTO>();
        List<Form16DocDTO> ListOfform16DocAdd = new List<Form16DocDTO>();
        List<Form16DocDTO> ListOfform16DocEdit = new List<Form16DocDTO>();
        Form16DocDTO form16DocDTO = new Form16DocDTO();

        // Dictionary<dynamic,string> form16PDFDictionary = new Dictionary<dynamic,string>();
        // Loop through each file and print its name
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            byte[] pdfFile = await ReadFileFromLocation(file);
            arrFiles = Path.GetFileNameWithoutExtension(file).Split("_");
            if (arrFiles.Length <= 0)
                arrFiles = file.Split("-");
            if (arrFiles.Length > 0)
            {
                foreach (string fileNamePart in arrFiles)
                {
                    //Expression<Func<EmployeeMaster, bool>> expression = a => a.Pannumber.Trim().Replace(" ", "") == fileNamePart;
                    sQry = $"SELECT top 1 PANNumber,EmployeeId FROM EmployeeMaster WHERE PANNumber=@panNumber";
                    empMaster = (await _unitOfWork.PayrollEarningComponent.GetTableDataExec<EmployeeMaster>(sQry, new { @panNumber = fileNamePart })).FirstOrDefault(); ;
                    if (empMaster != null)
                    {
                        sQry = $"SELECT [FormId],[DocName],[DocAttachment],[PANNumber],[EmployeeId],[FinYear],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsActive] FROM [dbo].[Form16Docs] WHERE EmployeeId=@empId AND FinYear=@finYear";
                        form16DocDTO = (await _unitOfWork.PayrollEarningComponent.GetTableDataExec<Form16DocDTO>(sQry, new { @empId = empMaster.EmployeeId, @finYear = form16DocVM.Form16Doc.FinYear })).FirstOrDefault();
                        if (form16DocDTO == null)
                            ListOfform16DocAdd.Add(new Form16DocDTO { DocName = fileName, Pannumber = empMaster.Pannumber, EmployeeId = empMaster.EmployeeId, FinYear = form16DocVM.Form16Doc.FinYear, DocAttachment = pdfFile, CreatedBy = form16DocVM.Form16Doc.CreatedBy, CreatedOn = DateTime.Now });
                        else
                            ListOfform16DocEdit.Add(new Form16DocDTO { DocName = fileName, Pannumber = empMaster.Pannumber, EmployeeId = empMaster.EmployeeId, FinYear = form16DocDTO.FinYear, DocAttachment = pdfFile, ModifiedBy = form16DocVM.Form16Doc.ModifiedBy, ModifiedOn = form16DocVM.Form16Doc.ModifiedOn });
                        isEmpFound = true;
                        break;
                    }
                }
                if (!isEmpFound)
                    ListOfform16DocNotMatched.Add(new Form16DocDTO { DocName = fileName });
                isEmpFound = false;

            }

        }
        if (ListOfform16DocAdd.Count > 0)
        {
            sQry = "INSERT INTO Form16Docs(DocName,DocAttachment,PANNumber,EmployeeId,FinYear,CreatedBy,CreatedOn,IsActive)" +
                "VALUES(@DocName,@DocAttachment,@PANNumber,@EmployeeId,@FinYear,@CreatedBy,@CreatedOn,1)";
            await _unitOfWork.PayrollEarningComponent.ExecuteListData<Form16DocDTO>(ListOfform16DocAdd, sQry);

        }
        if (ListOfform16DocEdit.Count > 0)
        {
            sQry = "UPDATE Form16Docs SET DocName=@DocName,DocAttachment=@DocAttachment,PANNumber=@PANNumber" +
                ",ModifiedBy=@ModifiedBy,ModifiedOn=@ModifiedOn WHERE EmployeeId=@EmployeeId AND FinYear=@FinYear";
            await _unitOfWork.PayrollEarningComponent.ExecuteListData<Form16DocDTO>(ListOfform16DocEdit, sQry);
        }
        form16DocVM.Form16DocList = ListOfform16DocNotMatched;
        //Console.WriteLine(fileName);
        return form16DocVM;
    }

    public async Task<Fom16DocViewModel> Form16Search(Fom16DocViewModel form16DocVM)
    {
        string allFileName = string.Empty, sQry = string.Empty;
        EmployeeMaster empMaster = new EmployeeMaster();
        List<Form16DocDTO> ListOfform16Docs = new List<Form16DocDTO>();
        sQry = $"SELECT a.[FormId],a.[DocName],a.[DocAttachment],a.[PANNumber],b.[EmployeeId],b.EmployeeName,a.[FinYear],a.[CreatedBy],a.[CreatedOn],a.[ModifiedBy],a.[ModifiedOn],a.[IsActive] " +
            $" FROM [dbo].[Form16Docs] a INNER JOIN EmployeeMaster b ON a.EmployeeId=b.EmployeeId WHERE a.EmployeeId=@empId AND a.FinYear=(CASE WHEN @finYear='' THEN a.FinYear ELSE @finYear END)  AND a.IsActive=1 AND b.IsActive=1";
        form16DocVM.Form16DocList = await _unitOfWork.PayrollEarningComponent.GetTableDataExec<Form16DocDTO>(sQry, new { @empId = form16DocVM.Form16Doc.EmployeeId, @finYear = (form16DocVM.Form16Doc.FinYear == null ? "" : form16DocVM.Form16Doc.FinYear) }); ;
        //Console.WriteLine(fileName);
        return form16DocVM;
    }

    public async Task<SalaryProcessInputs> Form16EmployeeSearch(SalaryProcessInputs form16DocVM)
    {
        string allFileName = string.Empty, sQry = string.Empty;
        EmployeeMaster empMaster = new EmployeeMaster();
        List<Form16DocDTO> ListOfform16Docs = new List<Form16DocDTO>();
        sQry = $"SELECT a.[FormId],a.[DocName],a.[DocAttachment],a.[PANNumber],b.[EmployeeId],b.EmployeeName,a.[FinYear],a.[CreatedBy],a.[CreatedOn],a.[ModifiedBy],a.[ModifiedOn],a.[IsActive] " +
            $" FROM [dbo].[Form16Docs] a INNER JOIN EmployeeMaster b ON a.EmployeeId=b.EmployeeId WHERE a.EmployeeId=@empId AND a.FinYear=(CASE WHEN @finYear='' THEN a.FinYear ELSE @finYear END)  AND a.IsActive=1 AND b.IsActive=1";
        form16DocVM.Form16DocList = await _unitOfWork.PayrollEarningComponent.GetTableDataExec<Form16DocDTO>(sQry, new { @empId = form16DocVM.Form16Doc.EmployeeId, @finYear = (form16DocVM.Form16Doc.FinYear == null ? "" : form16DocVM.Form16Doc.FinYear) }); ;
        //Console.WriteLine(fileName);
        return form16DocVM;
    }
    public async Task<bool> DeleteForm16(int iFormId)
    {
        string sQry = "UPDATE Form16Docs SET IsActive=0 WHERE FormId=@formId";
        bool isDeleted = await _unitOfWork.PayrollEarningComponent.ExecuteQuery(sQry, new { @formId = iFormId });
        return isDeleted;
    }

}


