using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.EmployeeSocialActivity;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Performance;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Page;
using SimpliHR.Infrastructure.Models.Performace;

namespace SimpliHR.WebUI.Controllers.EmployeeSocialActivity;

public class EmployeeAccouncementController : Controller
{
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeAnnouncementAPIController _employeeAnnouncementAPIController;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public EmployeeAccouncementController(MastersKeyValueController mastersKeyValueController, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnv, EmployeeAnnouncementAPIController employeeAnnouncementAPIController)
    {
        _mastersKeyValueController = mastersKeyValueController;
        this._hostingEnv = hostingEnv;
        _employeeAnnouncementAPIController = employeeAnnouncementAPIController;
    }
    public async Task<IActionResult> EmployeeAnnouncements()
    {
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeAnnouncementViewModel viewModel = new EmployeeAnnouncementViewModel();
        var res = await _employeeAnnouncementAPIController.EmployeeAnnouncementList(empSession.UnitId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.EmployeeAnnouncementWithChildList = (List<EmployeeAnnouncementWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                foreach (var item in viewModel.EmployeeAnnouncementWithChildList)
                {
                    item.encEmployeeAnnouncementId = CommonHelper.EncryptURLHTML(item.EmployeeAnnouncementId.ToString());
                }
            }
        }

        return View(viewModel);
    }

    public async Task<IActionResult> GetPollData([FromBody] EmployeeAnnouncementDTO inputDTO)
    {
        try
        {
            List<SurveyPollViewModel> surveyPollViewModels = new List<SurveyPollViewModel>();
            string? eAnnouncementId = inputDTO.encEmployeeAnnouncementId;
            if (eAnnouncementId != null)
            {
                int AnnouncementId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eAnnouncementId));
                #region SurveyPolls


                var SurveyPollQuestionsRes = await _employeeAnnouncementAPIController.SurveyPollQuestions(AnnouncementId);
                if (SurveyPollQuestionsRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollQuestionsRes).StatusCode == 200)
                    {
                        List<SurveyPollsQuestionDTO>? surveyPollsQuestionDTOs = (List<SurveyPollsQuestionDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollQuestionsRes).Value;
                        if (surveyPollsQuestionDTOs != null && surveyPollsQuestionDTOs.Any())
                        {
                            foreach (var item in surveyPollsQuestionDTOs)
                            {
                                SurveyPollViewModel surveyPollViewModel = new SurveyPollViewModel();
                                surveyPollViewModel.PollQuestion = item;
                                var SurveyPollOptionsRes = await _employeeAnnouncementAPIController.SurveyPollOptions(item.SurveyPollQuestionId);
                                if (SurveyPollOptionsRes != null)
                                {
                                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollOptionsRes).StatusCode == 200)
                                    {
                                        surveyPollViewModel.PollOptions = (List<SurveyPollOptionDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollOptionsRes).Value;
                                    }
                                }
                                var SurveyPollResponsesRes = await _employeeAnnouncementAPIController.SurveyPollResponses(item.SurveyPollQuestionId);
                                if (SurveyPollResponsesRes != null)
                                {
                                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollResponsesRes).StatusCode == 200)
                                    {
                                        surveyPollViewModel.PollResponses = (List<PollResponseDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollResponsesRes).Value;
                                    }
                                }
                                surveyPollViewModels.Add(surveyPollViewModel);
                            }
                        }
                    }
                }

                #endregion
            }

            return Ok(surveyPollViewModels);

        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [Route("/EmployeeAccouncement/AddEmployeeAnnouncements/{eAnnouncementId?}")]
    //[Route("/EmployeeAccouncement/AddEmployeeAnnouncements")]
    public async Task<IActionResult> AddEmployeeAnnouncements(string? eAnnouncementId)
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

            EmployeeAnnouncementViewModel employeeAnnouncementViewModel = new EmployeeAnnouncementViewModel();
            employeeAnnouncementViewModel.Departments = await _mastersKeyValueController.DepartmentKeyValueUnitWise(empSession.UnitId ?? default(int));
            employeeAnnouncementViewModel.AnnouncementTypes = await _mastersKeyValueController.AnnouncementTypeKeyValueUnitWise(empSession.UnitId ?? default(int));


            if (!(employeeAnnouncementViewModel.Departments != null && employeeAnnouncementViewModel.Departments.Count() != 0))
            {
                Error error = new Error();
                error.Heading = "Departments Not Found";
                error.Message = "Please add departments before accessing this screen";
                error.ButtonMessage = "Go To Back To Previous Page";
                error.ButtonURL = "javascript:history.go(-1)";
                return View("../Page/Error", error);
            }
            if (!(employeeAnnouncementViewModel.AnnouncementTypes != null && employeeAnnouncementViewModel.AnnouncementTypes.Count() != 0))
            {
                Error error = new Error();
                error.Heading = "Announcement Types Not Found";
                error.Message = "Please add announcement types before accessing this screen";
                error.ButtonMessage = "Go To Back To Previous Page";
                error.ButtonURL = "javascript:history.go(-1)";
                return View("../Page/Error", error);
            }

            if (eAnnouncementId != null)
            {
                int AnnouncementId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eAnnouncementId));
                var EmployementAnnouncementRes = await _employeeAnnouncementAPIController.EmployeeAnnouncementById(AnnouncementId);
                if (EmployementAnnouncementRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementRes).StatusCode == 200)
                    {
                        employeeAnnouncementViewModel.employeeAnnouncementDTO = (EmployeeAnnouncementDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementRes).Value;
                        if (employeeAnnouncementViewModel.employeeAnnouncementDTO != null)
                            employeeAnnouncementViewModel.employeeAnnouncementDTO.encEmployeeAnnouncementId = eAnnouncementId;
                    }
                }
                var EmployementAnnouncementFileUploadRes = await _employeeAnnouncementAPIController.EmployeeAnnouncementFilesByAnnouncementId(AnnouncementId);
                if (EmployementAnnouncementFileUploadRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementFileUploadRes).StatusCode == 200)
                    {
                        employeeAnnouncementViewModel.EmployeeAnnouncementFileUploadList = (List<EmployeeAnnouncementFileUploadDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementFileUploadRes).Value;
                    }
                }
                List<SurveyPollViewModel> surveyPollViewModels = new List<SurveyPollViewModel>();


                var SurveyPollQuestionsRes = await _employeeAnnouncementAPIController.SurveyPollQuestions(AnnouncementId);

                if (SurveyPollQuestionsRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollQuestionsRes).StatusCode == 200)
                    {
                        List<SurveyPollsQuestionDTO>? surveyPollsQuestionDTOs = (List<SurveyPollsQuestionDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollQuestionsRes).Value;
                        if (surveyPollsQuestionDTOs != null && surveyPollsQuestionDTOs.Any())
                        {
                            foreach (var item in surveyPollsQuestionDTOs)
                            {
                                SurveyPollViewModel surveyPollViewModel = new SurveyPollViewModel();
                                surveyPollViewModel.PollQuestion = item;
                                var SurveyPollOptionsRes = await _employeeAnnouncementAPIController.SurveyPollOptions(item.SurveyPollQuestionId);
                                if (SurveyPollOptionsRes != null)
                                {
                                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollOptionsRes).StatusCode == 200)
                                    {
                                        surveyPollViewModel.PollOptions = (List<SurveyPollOptionDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollOptionsRes).Value;
                                    }
                                }
                                surveyPollViewModels.Add(surveyPollViewModel);
                            }
                        }
                    }
                }
                employeeAnnouncementViewModel.SurveyPolls = surveyPollViewModels;
                employeeAnnouncementViewModel.SerializedSurveyPolls = JsonConvert.SerializeObject(surveyPollViewModels);
            }
            return View(employeeAnnouncementViewModel);
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
    public async Task<IActionResult> SaveAnnouncement(EmployeeAnnouncementViewModel dataVM)
    {
        try
        {
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            if (dataVM == null || dataVM.employeeAnnouncementDTO == null)
            {
                return BadRequest("Data is not in the valid format");
            }

            int EmployeeAnnouncementId = 0;
            List<int> FileUploadIds = new List<int>();
            string optType = "";

            #region Save EmployeeAnnouncement

            if (dataVM.employeeAnnouncementDTO.Type == "Publish")
            {
                dataVM.employeeAnnouncementDTO.PublishDate = DateTime.Now.Date;
                dataVM.employeeAnnouncementDTO.PublishTime = DateTime.Now.TimeOfDay;
            }

            if (dataVM.employeeAnnouncementDTO.encEmployeeAnnouncementId != null && dataVM.employeeAnnouncementDTO.encEmployeeAnnouncementId != "")
            {
                dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId = Convert.ToInt32(CommonHelper.DecryptURLHTML(dataVM.employeeAnnouncementDTO.encEmployeeAnnouncementId));
                dataVM.employeeAnnouncementDTO.ModifiedDate = DateTime.Now;
                dataVM.employeeAnnouncementDTO.ModifiedBy = empSession.EmployeeId;
                optType = "Update";
            }
            else
            {
                dataVM.employeeAnnouncementDTO.IsActive = true;
                dataVM.employeeAnnouncementDTO.UnitId = empSession.UnitId ?? default(int);
                dataVM.employeeAnnouncementDTO.CreatedDate = DateTime.Now;
                dataVM.employeeAnnouncementDTO.CreatedBy = empSession.EmployeeId;
                optType = "Insert";
            }
            var employeeAnnouncementRes = await _employeeAnnouncementAPIController.SaveEmployeeAnnouncement(dataVM.employeeAnnouncementDTO);
            if (employeeAnnouncementRes != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementRes).StatusCode == 200)
                {
                    EmployeeAnnouncementDTO? ead = (EmployeeAnnouncementDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementRes).Value;
                    if (ead != null)
                    {
                        if (optType == "Update")
                            dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId = ead.EmployeeAnnouncementId;
                        else
                            dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId = EmployeeAnnouncementId = ead.EmployeeAnnouncementId;
                    }
                    else
                    {
                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                    }
                }
                else
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementRes).Value == null)
                    {
                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                    }
                    else
                    {
                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementRes).Value.ToString());
                    }
                }
            }
            #endregion

            #region Polls

            //dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId

            if (dataVM.SerializedSurveyPolls != null)
            {
                List<SurveyPollViewModel>? surveyPolls = JsonConvert.DeserializeObject<List<SurveyPollViewModel>?>(dataVM.SerializedSurveyPolls);

                if (surveyPolls != null && surveyPolls.Count > 0)
                {
                    dataVM.SurveyPolls = surveyPolls;
                    await _employeeAnnouncementAPIController.SaveSurveyPoll(dataVM);
                }

            }


            #endregion


            #region Attachments and Upload

            try
            {
                var attachments = dataVM.Attachment;
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        if (attachment.Length > 0)
                        {
                            EmployeeAnnouncementFileUploadDTO employeeAnnouncementFileUploadDTO = new EmployeeAnnouncementFileUploadDTO();
                            var attachmentName = Path.GetFileName(attachment.FileName);
                            var attachmentExtension = Path.GetExtension(attachmentName);
                            string FilePathWithoutRoot = Path.Combine("EmployeeAnnouncementFileUploads", "Attachment", dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId.ToString());
                            string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                            if (!Directory.Exists(FilePath))
                                Directory.CreateDirectory(FilePath);
                            var filePath = Path.Combine(FilePath, attachmentName);
                            employeeAnnouncementFileUploadDTO.IsActive = true;
                            employeeAnnouncementFileUploadDTO.CreatedDate = DateTime.Now;
                            employeeAnnouncementFileUploadDTO.CreatedBy = empSession.EmployeeId;
                            employeeAnnouncementFileUploadDTO.UploadType = "Attachment";
                            employeeAnnouncementFileUploadDTO.UploadedFileName = attachmentName;
                            employeeAnnouncementFileUploadDTO.UploadedFilePath = FilePathWithoutRoot;
                            employeeAnnouncementFileUploadDTO.UploadedFileExtension = attachmentExtension;
                            employeeAnnouncementFileUploadDTO.EmployeeAnnouncementId = dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId;
                            var employeeAnnouncementFileUploadRes = await _employeeAnnouncementAPIController.SaveEmployeeAnnouncementFile(employeeAnnouncementFileUploadDTO);
                            if (employeeAnnouncementFileUploadRes != null)
                            {
                                if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementFileUploadRes).StatusCode == 200)
                                {
                                    EmployeeAnnouncementFileUploadDTO? eafud = (EmployeeAnnouncementFileUploadDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementFileUploadRes).Value;
                                    if (eafud != null)
                                    {
                                        employeeAnnouncementFileUploadDTO.EmployeeAnnouncementFileUploadsId = eafud.EmployeeAnnouncementFileUploadsId;
                                        FileUploadIds.Add(eafud.EmployeeAnnouncementFileUploadsId);
                                    }
                                    else
                                    {
                                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                                    }
                                }
                                else
                                {
                                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementFileUploadRes).Value == null)
                                    {
                                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                                    }
                                    else
                                    {
                                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementFileUploadRes).Value.ToString());
                                    }
                                }
                            }

                            using (FileStream fs = System.IO.File.Create(filePath))
                            {
                                attachment.CopyTo(fs);
                            }
                        }
                    }
                }

                #region Thumbnail

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
                                EmployeeAnnouncementFileUploadDTO employeeAnnouncementFileUploadDTO = new EmployeeAnnouncementFileUploadDTO();
                                var uploadName = Path.GetFileName(upload.FileName);
                                var uploadExtension = Path.GetExtension(uploadName);
                                string FilePathWithoutRoot = Path.Combine("EmployeeAnnouncementFileUploads", "Upload", dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId.ToString());
                                string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                                if (!Directory.Exists(FilePath))
                                    Directory.CreateDirectory(FilePath);
                                var filePath = Path.Combine(FilePath, uploadName);
                                employeeAnnouncementFileUploadDTO.IsActive = true;
                                employeeAnnouncementFileUploadDTO.CreatedDate = DateTime.Now;
                                employeeAnnouncementFileUploadDTO.CreatedBy = empSession.EmployeeId;
                                employeeAnnouncementFileUploadDTO.UploadType = "Upload";
                                employeeAnnouncementFileUploadDTO.UploadedFileName = uploadName;
                                employeeAnnouncementFileUploadDTO.UploadedFilePath = FilePathWithoutRoot;
                                employeeAnnouncementFileUploadDTO.UploadedFileExtension = uploadExtension;
                                employeeAnnouncementFileUploadDTO.EmployeeAnnouncementId = dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId;
                                var employeeAnnouncementFileUploadRes = await _employeeAnnouncementAPIController.SaveEmployeeAnnouncementFile(employeeAnnouncementFileUploadDTO);
                                if (employeeAnnouncementFileUploadRes != null)
                                {
                                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementFileUploadRes).StatusCode == 200)
                                    {
                                        EmployeeAnnouncementFileUploadDTO? eafud = (EmployeeAnnouncementFileUploadDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementFileUploadRes).Value;
                                        if (eafud != null)
                                        {
                                            employeeAnnouncementFileUploadDTO.EmployeeAnnouncementFileUploadsId = eafud.EmployeeAnnouncementFileUploadsId;
                                            FileUploadIds.Add(eafud.EmployeeAnnouncementFileUploadsId);
                                        }
                                        else
                                        {
                                            throw new Exception("Some error has occurred. Please refresh the page and try again");
                                        }
                                    }
                                    else
                                    {
                                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementFileUploadRes).Value == null)
                                        {
                                            throw new Exception("Some error has occurred. Please refresh the page and try again");
                                        }
                                        else
                                        {
                                            throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)employeeAnnouncementFileUploadRes).Value.ToString());
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
                #endregion
            }
            catch (Exception e)
            {
                await _employeeAnnouncementAPIController.HardDeleteEmployeeAnnouncement(EmployeeAnnouncementId, FileUploadIds);
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
    public async Task<IActionResult> UploadResume(IFormFile file)
    {

        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> DeleteUploadedFile([FromBody] EmployeeAnnouncementFileUploadDTO inputDTO)
    {
        try
        {

            if (inputDTO != null)
            {
                var res = await _employeeAnnouncementAPIController.DeleteEmployeeAnnouncementFilesById(inputDTO.EmployeeAnnouncementFileUploadsId);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> DeleteRecord([FromBody] EmployeeAnnouncementDTO inputDTO)
    {
        if (inputDTO != null)
        {
            if (inputDTO.encEmployeeAnnouncementId != null)
            {
                int EmployeeAnnouncementId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encEmployeeAnnouncementId));
                var res = await _employeeAnnouncementAPIController.DeleteEmployeeAnnouncementById(EmployeeAnnouncementId);
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

    public async Task<IActionResult> DeleteOption([FromBody] SurveyPollOptionDTO inputDTO)
    {
        try
        {

            if (inputDTO != null)
            {
                var res = await _employeeAnnouncementAPIController.DeleteSurveyPollOption(inputDTO.SurveyPollOptionId);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> DeleteQuestion([FromBody] SurveyPollsQuestionDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var res = await _employeeAnnouncementAPIController.DeleteSurveyPollQuestion(inputDTO.SurveyPollQuestionId);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> EmployeeAnnouncementList(string? v, string? e)
    {
        EmployeeAnnouncementDTO employeeAnnouncementDTO = new EmployeeAnnouncementDTO();
        employeeAnnouncementDTO.encEmployeeAnnouncementId = e;
        employeeAnnouncementDTO.Source = v;
        return View(employeeAnnouncementDTO);
    }
    public async Task<IActionResult> AnnouncementListPartialView([FromBody] EmployeeAnnouncementViewModel inputDTO)
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
        EmployeeAnnouncementViewModel viewModel = new EmployeeAnnouncementViewModel();
        viewModel = await getAnnouncementList(empSession, inputDTO);

        return PartialView("_employeeAnnouncementList/_announcementList", viewModel);
    }
    public async Task<IActionResult> AnnouncementCardPartialView([FromBody] EmployeeAnnouncementViewModel inputDTO)
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
        EmployeeAnnouncementViewModel viewModel = new EmployeeAnnouncementViewModel();
        viewModel = await getAnnouncementList(empSession, inputDTO);
        return PartialView("_employeeAnnouncementList/_announcementCard", viewModel);
    }


    [Route("/EmployeeAccouncement/AnnouncementDetails/{eAnnouncementId?}/{source?}")]
    public async Task<IActionResult> AnnouncementDetailsPartialView(string? eAnnouncementId, string source)
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
            EmployeeAnnouncementViewModel employeeAnnouncementViewModel = new EmployeeAnnouncementViewModel();

            if (eAnnouncementId != null)
            {
                int AnnouncementId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eAnnouncementId));
                var EmployementAnnouncementRes = await _employeeAnnouncementAPIController.EmployeeAnnouncementDetailsById(AnnouncementId, empSession.DepartmentId ?? default(int), empSession.UnitId ?? default(int));
                if (EmployementAnnouncementRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementRes).StatusCode == 200)
                    {
                        employeeAnnouncementViewModel.EmployeeAnnouncementWithChild = (EmployeeAnnouncementWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementRes).Value;
                        if (employeeAnnouncementViewModel.EmployeeAnnouncementWithChild != null)
                        {
                            employeeAnnouncementViewModel.EmployeeAnnouncementWithChild.encEmployeeAnnouncementId = eAnnouncementId;
                            employeeAnnouncementViewModel.EmployeeAnnouncementWithChild.Source = source;
                        }

                    }
                }
                var EmployementAnnouncementAttachmentsRes = await _employeeAnnouncementAPIController.EmployeeAnnouncementFilesByAnnouncementId(AnnouncementId);
                if (EmployementAnnouncementAttachmentsRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementAttachmentsRes).StatusCode == 200)
                    {
                        employeeAnnouncementViewModel.EmployeeAnnouncementFileUploadList = (List<EmployeeAnnouncementFileUploadDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementAttachmentsRes).Value;
                        if (employeeAnnouncementViewModel.EmployeeAnnouncementFileUploadList != null)
                        {
                            foreach (var item in employeeAnnouncementViewModel.EmployeeAnnouncementFileUploadList)
                            {
                                item.encEmployeeAnnouncementFileUploadsId = CommonHelper.EncryptURLHTML(item.EmployeeAnnouncementFileUploadsId.ToString());
                            }
                        }
                    }
                }

                #region SurveyPolls

                List<SurveyPollViewModel> surveyPollViewModels = new List<SurveyPollViewModel>();
                var SurveyPollQuestionsRes = await _employeeAnnouncementAPIController.SurveyPollQuestions(AnnouncementId, empSession.EmployeeId);
                if (SurveyPollQuestionsRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollQuestionsRes).StatusCode == 200)
                    {
                        List<SurveyPollsQuestionDTO>? surveyPollsQuestionDTOs = (List<SurveyPollsQuestionDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollQuestionsRes).Value;
                        if (surveyPollsQuestionDTOs != null && surveyPollsQuestionDTOs.Any())
                        {
                            foreach (var item in surveyPollsQuestionDTOs)
                            {
                                SurveyPollViewModel surveyPollViewModel = new SurveyPollViewModel();
                                surveyPollViewModel.PollQuestion = item;
                                var SurveyPollOptionsRes = await _employeeAnnouncementAPIController.SurveyPollOptions(item.SurveyPollQuestionId);
                                if (SurveyPollOptionsRes != null)
                                {
                                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollOptionsRes).StatusCode == 200)
                                    {
                                        surveyPollViewModel.PollOptions = (List<SurveyPollOptionDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)SurveyPollOptionsRes).Value;
                                    }
                                }
                                surveyPollViewModels.Add(surveyPollViewModel);
                            }
                        }
                    }
                }
                employeeAnnouncementViewModel.SurveyPolls = surveyPollViewModels;
                #endregion

            }
            return PartialView("_employeeAnnouncementList/_announcementDetails", employeeAnnouncementViewModel);
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

    public async Task<EmployeeAnnouncementViewModel> getAnnouncementList(EmployeeMasterDTO? empSession, EmployeeAnnouncementViewModel inputDTO)
    {
        EmployeeAnnouncementViewModel viewModel = new EmployeeAnnouncementViewModel();
        #region Announcement Tags
        var resAnnouncementTypes = await _employeeAnnouncementAPIController.AnnouncementTypesForDashboard(empSession.UnitId ?? default(int));
        if (resAnnouncementTypes != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resAnnouncementTypes).StatusCode == 200)
            {
                viewModel.AnnouncementTypeMasterList = (List<AnnouncementTypeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAnnouncementTypes).Value;
            }
        }
        #endregion
        var res = await _employeeAnnouncementAPIController.EmployeeAnnouncementListForViewAll(empSession.UnitId ?? default(int), empSession.DepartmentId ?? default(int), inputDTO);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.EmployeeAnnouncementWithChildList = (List<EmployeeAnnouncementWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                foreach (var item in viewModel.EmployeeAnnouncementWithChildList)
                {
                    item.encEmployeeAnnouncementId = CommonHelper.EncryptURLHTML(item.EmployeeAnnouncementId.ToString());
                    var doc = new HtmlDocument();
                    doc.LoadHtml(item.Description);
                    item.Description = doc.DocumentNode.InnerText;
                }
            }
        }

        EmployeeAnnouncementPageDetails pageDetails = new EmployeeAnnouncementPageDetails();
        pageDetails.TotalRecords = await _employeeAnnouncementAPIController.EmployeeAnnouncementListCountViewAll(empSession.UnitId ?? default(int), empSession.DepartmentId ?? default(int), inputDTO);
        pageDetails.PageSize = (inputDTO != null && inputDTO.PageDetails != null && inputDTO.PageDetails.PageSize != null) ? inputDTO.PageDetails.PageSize : 6;
        pageDetails.PageNumber = (inputDTO != null && inputDTO.PageDetails != null && inputDTO.PageDetails.PageNumber != null) ? inputDTO.PageDetails.PageNumber : 1;
        pageDetails.TotalPages = (int)Math.Ceiling((double)pageDetails.TotalRecords / pageDetails.PageSize ?? default(int));

        viewModel.PageDetails = pageDetails;
        viewModel.AnnouncementTypeId = inputDTO.AnnouncementTypeId;
        viewModel.AnnouncementSearchKeyword = inputDTO.AnnouncementSearchKeyword;
        return viewModel;
    }
    [HttpGet]
    [Route("EmployeeAccouncement/DownloadFile/{eFileId}")]
    public async Task<IActionResult> DownloadFile(string eFileId)
    {
        EmployeeAnnouncementFileUploadDTO? inputDTO = new EmployeeAnnouncementFileUploadDTO();
        if (eFileId != null)
        {
            int FileID = Convert.ToInt32(CommonHelper.DecryptURLHTML(eFileId));
            var res = await _employeeAnnouncementAPIController.EmployeeAnnouncementFilesByFileId(FileID);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    inputDTO = (EmployeeAnnouncementFileUploadDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
        }

        if (inputDTO != null)
        {
            string _filePath = Path.Combine(_hostingEnv.WebRootPath, inputDTO.UploadedFilePath, inputDTO.UploadedFileName);
            if (!System.IO.File.Exists(_filePath))
            {
                return NotFound();
            }
            // Create a FileStream to read the file
            var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            // Determine the content type based on the file extension
            var contentType = CommonHelper.getContentTypeByExtesnion(inputDTO.UploadedFileExtension == null ? "" : inputDTO.UploadedFileExtension);
            var fileName = Path.GetFileName(_filePath);

            // Return FileStreamResult with the file stream and content type
            return File(stream, contentType, fileName);
        }
        else
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> SubmitVote([FromBody] PollResponseDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception("Invalid Data");
            }
            #region Session Check
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                return BadRequest("Session Expired.Login again to continue");
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                return BadRequest("Session Expired.Login again to continue");
            }
            #endregion

            inputDTO.EmployeeId = empSession.EmployeeId;
            inputDTO.CreatedDate = System.DateTime.Now;
            var res = await _employeeAnnouncementAPIController.SavePollResponse(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
