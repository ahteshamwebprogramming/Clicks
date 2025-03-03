using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using SimpliHR.Endpoints.EmployeeSocialActivity;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Page;
using HtmlAgilityPack;

namespace SimpliHR.WebUI.Controllers.EmployeeSocialActivity;

public class EmployeeNewsController : Controller
{
    private readonly MastersKeyValueController _mastersKeyValueController;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    private readonly EmployeeNewsAPIController _employeeNewsAPIController;

    public EmployeeNewsController(MastersKeyValueController mastersKeyValueController, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnv, EmployeeNewsAPIController employeeNewsAPIController)
    {
        _mastersKeyValueController = mastersKeyValueController;
        this._hostingEnv = hostingEnv;
        _employeeNewsAPIController = employeeNewsAPIController;

    }

    //[Route("/EmployeeNews/EmployeeNewsList/{eNewsId?}")]


    public async Task<IActionResult> EmployeeNews()
    {
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeNewsViewModel viewModel = new EmployeeNewsViewModel();
        var res = await _employeeNewsAPIController.EmployeeNewsList(empSession.UnitId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.EmployeeNewsList = (List<EmployeeNewsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                foreach (var item in viewModel.EmployeeNewsList)
                {
                    item.encEmployeeNewsId = CommonHelper.EncryptURLHTML(item.EmployeeNewsId.ToString());
                }
            }
        }

        return View(viewModel);
    }

    [Route("/EmployeeNews/AddNews/{eNewsId?}")]
    public async Task<IActionResult> AddNews(string? eNewsId)
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            EmployeeNewsViewModel employeeNewsViewModel = new EmployeeNewsViewModel();
            employeeNewsViewModel.NewsCategoryTags = await _mastersKeyValueController.NewsCategoryTagKeyValueUnitWise(empSession.UnitId ?? default(int));

            if (!(employeeNewsViewModel.NewsCategoryTags != null && employeeNewsViewModel.NewsCategoryTags.Count() != 0))
            {
                Error error = new Error();
                error.Heading = "News Category Tags Not Found";
                error.Message = "Please add news category tags before accessing this screen";
                error.ButtonMessage = "Go To Back To Previous Page";
                error.ButtonURL = "javascript:history.go(-1)";
                return View("../Page/Error", error);
            }

            if (eNewsId != null)
            {
                int NewsId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eNewsId));
                var EmployementNewsRes = await _employeeNewsAPIController.EmployeeNewsById(NewsId);
                if (EmployementNewsRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementNewsRes).StatusCode == 200)
                    {
                        employeeNewsViewModel.employeeNewsDTO = (EmployeeNewsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementNewsRes).Value;
                        if (employeeNewsViewModel.employeeNewsDTO != null)
                            employeeNewsViewModel.employeeNewsDTO.encEmployeeNewsId = eNewsId;
                    }
                }
                var EmployementNewsFileUploadRes = await _employeeNewsAPIController.EmployeeNewsFilesByNewsId(NewsId);
                if (EmployementNewsFileUploadRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementNewsFileUploadRes).StatusCode == 200)
                    {
                        employeeNewsViewModel.EmployeeNewsFileUploadList = (List<EmployeeNewsFileUploadDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementNewsFileUploadRes).Value;
                    }
                }
            }
            return View(employeeNewsViewModel);
        }
        catch (Exception ex)
        {
            Error error = new Error();
            error.Heading = "Error has occurred";
            error.Message = ex.Message;
            error.ButtonMessage = "Go To previous page";
            error.ButtonURL = "javascript:history.go(-1)";
            return View("../Page/Error", error);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveNews(EmployeeNewsViewModel dataVM)
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            if (dataVM == null || dataVM.employeeNewsDTO == null)
            {
                return BadRequest("Data is not in the valid format");
            }

            int EmployeeNewsId = 0;
            List<int> FileUploadIds = new List<int>();
            string optType = "";

            #region Save EmployeeNews

            if (dataVM.employeeNewsDTO.encEmployeeNewsId != null && dataVM.employeeNewsDTO.encEmployeeNewsId != "")
            {
                dataVM.employeeNewsDTO.EmployeeNewsId = Convert.ToInt32(CommonHelper.DecryptURLHTML(dataVM.employeeNewsDTO.encEmployeeNewsId));
                dataVM.employeeNewsDTO.ModifiedDate = DateTime.Now;
                dataVM.employeeNewsDTO.ModifiedBy = empSession.EmployeeId;
                optType = "Update";
            }
            else
            {
                dataVM.employeeNewsDTO.IsActive = true;
                dataVM.employeeNewsDTO.UnitId = empSession.UnitId ?? default(int);
                dataVM.employeeNewsDTO.CreatedDate = DateTime.Now;
                dataVM.employeeNewsDTO.CreatedBy = empSession.EmployeeId;
                optType = "Insert";
            }
            var employeeNewsRes = await _employeeNewsAPIController.SaveEmployeeNews(dataVM.employeeNewsDTO);
            if (employeeNewsRes != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeNewsRes).StatusCode == 200)
                {
                    EmployeeNewsDTO? ead = (EmployeeNewsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)employeeNewsRes).Value;
                    if (ead != null)
                    {
                        if (optType == "Update")
                            dataVM.employeeNewsDTO.EmployeeNewsId = ead.EmployeeNewsId;
                        else
                            dataVM.employeeNewsDTO.EmployeeNewsId = EmployeeNewsId = ead.EmployeeNewsId;
                    }
                    else
                    {
                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                    }
                }
                else
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeNewsRes).Value == null)
                    {
                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                    }
                    else
                    {
                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)employeeNewsRes).Value.ToString());
                    }
                }
            }
            #endregion

            #region Attachments and Upload

            try
            {
                if (dataVM != null && dataVM.ThumbnailImage != null && dataVM.Upload != null)
                {
                    var base64Data = dataVM.ThumbnailImage.Split(',')[1];
                    if (CommonHelper.IsBase64String(base64Data))
                    {
                        var bytes = Convert.FromBase64String(base64Data);
                        foreach (var upload in dataVM.Upload)
                        {
                            if (upload.Length > 0)
                            {
                                EmployeeNewsFileUploadDTO employeeNewsFileUploadDTO = new EmployeeNewsFileUploadDTO();
                                var uploadName = Path.GetFileName(upload.FileName);
                                var uploadExtension = Path.GetExtension(uploadName);
                                string FilePathWithoutRoot = Path.Combine("EmployeeNewsFileUploads", "Upload", dataVM.employeeNewsDTO.EmployeeNewsId.ToString());
                                string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                if (!Directory.Exists(FilePath))
                                    Directory.CreateDirectory(FilePath);
                                var filePath = Path.Combine(FilePath, uploadName);
                                employeeNewsFileUploadDTO.IsActive = true;
                                employeeNewsFileUploadDTO.CreatedDate = DateTime.Now;
                                employeeNewsFileUploadDTO.CreatedBy = empSession.EmployeeId;
                                employeeNewsFileUploadDTO.UploadType = "Upload";
                                employeeNewsFileUploadDTO.UploadedFileName = uploadName;
                                employeeNewsFileUploadDTO.UploadedFilePath = FilePathWithoutRoot;
                                employeeNewsFileUploadDTO.UploadedFileExtension = uploadExtension;
                                employeeNewsFileUploadDTO.EmployeeNewsId = dataVM.employeeNewsDTO.EmployeeNewsId;
                                var employeeNewsFileUploadRes = await _employeeNewsAPIController.SaveEmployeeNewsFile(employeeNewsFileUploadDTO);
                                if (employeeNewsFileUploadRes != null)
                                {
                                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeNewsFileUploadRes).StatusCode == 200)
                                    {
                                        EmployeeNewsFileUploadDTO? eafud = (EmployeeNewsFileUploadDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)employeeNewsFileUploadRes).Value;
                                        if (eafud != null)
                                        {
                                            employeeNewsFileUploadDTO.EmployeeNewsFileUploadsId = eafud.EmployeeNewsFileUploadsId;
                                            FileUploadIds.Add(eafud.EmployeeNewsFileUploadsId);
                                        }
                                        else
                                        {
                                            throw new Exception("Some error has occurred. Please refresh the page and try again");
                                        }
                                    }
                                    else
                                    {
                                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeNewsFileUploadRes).Value == null)
                                        {
                                            throw new Exception("Some error has occurred. Please refresh the page and try again");
                                        }
                                        else
                                        {
                                            throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)employeeNewsFileUploadRes).Value.ToString());
                                        }
                                    }
                                }

                                System.IO.File.WriteAllBytes(filePath, bytes);
                                //using (FileStream fs = System.IO.File.Create(filePath))
                                //{
                                //    upload.CopyTo(fs);
                                //}
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Thumbnail Image not found");
                    }
                }
                else
                {
                    if (dataVM.ExistingThumbnail == true)
                    {
                        return Ok("Success");
                    }
                    throw new Exception("Thumbnail Image not found");
                }
            }
            catch (Exception e)
            {
                await _employeeNewsAPIController.HardDeleteEmployeeNews(EmployeeNewsId, FileUploadIds);
                return BadRequest(e.Message);
            }

            #endregion

            return Ok("Success");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



    [HttpPost]
    public async Task<IActionResult> DeleteUploadedFile([FromBody] EmployeeNewsFileUploadDTO inputDTO)
    {
        try
        {

            if (inputDTO != null)
            {
                var res = await _employeeNewsAPIController.DeleteEmployeeNewsFilesById(inputDTO.EmployeeNewsFileUploadsId);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> DeleteRecord([FromBody] EmployeeNewsDTO inputDTO)
    {
        if (inputDTO != null)
        {
            if (inputDTO.encEmployeeNewsId != null)
            {
                int EmployeeNewsId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encEmployeeNewsId));
                var res = await _employeeNewsAPIController.DeleteEmployeeNewsById(EmployeeNewsId);
                return res;
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to fetch Data");
            }
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Invalid Data");
        }
    }



    public async Task<IActionResult> EmployeeNewsList(string? v, string? e)
    {
        EmployeeNewsDTO employeeNewsDTO = new EmployeeNewsDTO();
        employeeNewsDTO.encEmployeeNewsId = e;
        employeeNewsDTO.Source = v;
        return View(employeeNewsDTO);
    }
    public async Task<IActionResult> NewsListPartialView([FromBody] EmployeeNewsViewModel inputDTO)
    {
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeNewsViewModel viewModel = new EmployeeNewsViewModel();
        viewModel = await getNewList(empSession, inputDTO);

        return PartialView("_employeeNewsList/_newsList", viewModel);
    }
    public async Task<IActionResult> NewsCardPartialView([FromBody] EmployeeNewsViewModel inputDTO)
    {
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeNewsViewModel viewModel = new EmployeeNewsViewModel();
        viewModel = await getNewList(empSession, inputDTO);
        return PartialView("_employeeNewsList/_newsCard", viewModel);
    }


    [Route("/EmployeeNews/NewsDetails/{eNewsId?}/{source?}")]
    public async Task<IActionResult> NewsDetailsPartialView(string? eNewsId, string source)
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            EmployeeNewsViewModel employeeNewsViewModel = new EmployeeNewsViewModel();

            if (eNewsId != null)
            {
                int NewsId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eNewsId));
                var EmployementNewsRes = await _employeeNewsAPIController.EmployeeNewsDetailsById(NewsId);
                if (EmployementNewsRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementNewsRes).StatusCode == 200)
                    {
                        employeeNewsViewModel.employeeNewsDTO = (EmployeeNewsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementNewsRes).Value;
                        if (employeeNewsViewModel.employeeNewsDTO != null)
                        {
                            employeeNewsViewModel.employeeNewsDTO.encEmployeeNewsId = eNewsId;
                            employeeNewsViewModel.employeeNewsDTO.Source = source;
                        }

                    }
                }
            }
            return PartialView("_employeeNewsList/_newsDetails", employeeNewsViewModel);
        }
        catch (Exception ex)
        {
            Error error = new Error();
            error.Heading = "Error has occurred";
            error.Message = ex.Message;
            error.ButtonMessage = "Go To previous page";
            error.ButtonURL = "javascript:history.go(-1)";
            return View("../Page/Error", error);
        }
    }


    public async Task<EmployeeNewsViewModel> getNewList(EmployeeMasterDTO? empSession, EmployeeNewsViewModel inputDTO)
    {
        EmployeeNewsViewModel viewModel = new EmployeeNewsViewModel();
        #region News Tags
        var resNewsCategoryTags = await _employeeNewsAPIController.NewsCategoryTagsForDashboard(empSession.UnitId ?? default(int));
        if (resNewsCategoryTags != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resNewsCategoryTags).StatusCode == 200)
            {
                viewModel.NewsCategoryTagsList = (List<NewsCategoryTagMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resNewsCategoryTags).Value;
            }
        }
        #endregion
        var res = await _employeeNewsAPIController.EmployeeNewsListForViewAll(empSession.UnitId ?? default(int), inputDTO);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.EmployeeNewsList = (List<EmployeeNewsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                foreach (var item in viewModel.EmployeeNewsList)
                {
                    item.encEmployeeNewsId = CommonHelper.EncryptURLHTML(item.EmployeeNewsId.ToString());
                    var doc = new HtmlDocument();
                    doc.LoadHtml(item.Article);
                    item.Article = doc.DocumentNode.InnerText;                    
                }
            }
        }

        EmployeeNewsPageDetails pageDetails = new EmployeeNewsPageDetails();
        pageDetails.TotalRecords = await _employeeNewsAPIController.EmployeeNewsListCountViewAll(empSession.UnitId ?? default(int), inputDTO);
        pageDetails.PageSize = (inputDTO != null && inputDTO.PageDetails != null && inputDTO.PageDetails.PageSize != null) ? inputDTO.PageDetails.PageSize : 6;
        pageDetails.PageNumber = (inputDTO != null && inputDTO.PageDetails != null && inputDTO.PageDetails.PageNumber != null) ? inputDTO.PageDetails.PageNumber : 1;
        pageDetails.TotalPages = (int)Math.Ceiling((double)pageDetails.TotalRecords / pageDetails.PageSize ?? default(int));

        viewModel.PageDetails = pageDetails;

        viewModel.NewsCategoryId = inputDTO.NewsCategoryId;
        viewModel.NewsSearchKeyword = inputDTO.NewsSearchKeyword;
        return viewModel;
    }
}
