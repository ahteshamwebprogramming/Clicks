using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using ProjectTracker.Controllers;
using SimpliHR.Endpoints.EmployeeSocialActivity;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.ProjectTracker.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Page;
using SimpliHR.Infrastructure.Models.ProjectTracker;
using System.Globalization;
using System.IO.Compression;

namespace SimpliHR.WebUI.Controllers.ProjectTracker;

public class ProjectTrackerController : Controller
{

    private readonly ProjectCategoryMasterAPIController _projectCategoryMasterAPIController;
    private readonly ProjectAPIController _projectAPIController;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public ProjectTrackerController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnv, ProjectCategoryMasterAPIController projectCategoryMasterAPIController, ProjectAPIController projectAPIController)
    {
        this._hostingEnv = hostingEnv;
        _projectCategoryMasterAPIController = projectCategoryMasterAPIController;
        _projectAPIController = projectAPIController;
    }
    public async Task<IActionResult> ProjectList()
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
        ProjectViewModel viewModel = new ProjectViewModel();
        var res = await _projectAPIController.ProjectList(empSession.UnitId ?? default(int), empSession.EmployeeId);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.ProjectWithChildList = (List<ProjectWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (viewModel.ProjectWithChildList != null)
                {
                    foreach (var item in viewModel.ProjectWithChildList)
                    {
                        if (item != null)
                        {
                            item.encProjectID = CommonHelper.EncryptURLHTML(item.ProjectID.ToString());
                        }
                    }
                }

            }
        }
        return View(viewModel);
    }


    [Route("/ProjectTracker/ProjectList1/{ss?}")]
    public async Task<IActionResult> ProjectList1(string? ss)
    {
        ProjectViewModel viewModel = new ProjectViewModel();

        if (!String.IsNullOrEmpty(ss))
        {
            var parts = ss.Split('-');
            if (parts.Length == 2)
            {
                viewModel.Source = CommonHelper.DecryptURLHTML(parts[0]);
                viewModel.encProjectId = parts[1];
            }
            else
            {
                viewModel.Source = CommonHelper.DecryptURLHTML(ss);
            }
        }
        return View(viewModel);
    }

    public async Task<IActionResult> ProjectListPartialView([FromBody] ProjectPageDetails inputDTO)
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

        //if (inputDTO != null && String.IsNullOrEmpty(inputDTO.Source) == false)
        //{
        //    inputDTO.Source = CommonHelper.DecryptURLHTML(inputDTO.Source);
        //    if (inputDTO.Source == "md" || inputDTO.Source == "ed")
        //    {
        //        inputDTO.ProjectStatusType = "All";
        //    }
        //}

        ProjectViewModel viewModel = new ProjectViewModel();
        //var res1 = await _projectAPIController.ProjectListProjectStatusWise(empSession.UnitId ?? default(int), empSession.EmployeeId, inputDTO.ProjectStatusType);
        var res = await _projectAPIController.ProjectListProjectStatusWiseAndPageWise(empSession.UnitId ?? default(int), empSession.EmployeeId, inputDTO, "Data");
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.ProjectWithChildList = (List<ProjectWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (viewModel.ProjectWithChildList != null)
                {

                    foreach (var item in viewModel.ProjectWithChildList)
                    {
                        if (item != null)
                        {
                            item.encProjectID = CommonHelper.EncryptURLHTML(item.ProjectID.ToString());
                        }
                    }
                    int[] ProjectIDs = viewModel.ProjectWithChildList.Select(x => x.ProjectID).ToArray();
                    if (ProjectIDs.Length > 0)
                    {
                        var teamMembersRes = await _projectAPIController.GetTeamMembersByProjectIdArray(ProjectIDs);
                        if (teamMembersRes != null)
                        {
                            if (((Microsoft.AspNetCore.Mvc.ObjectResult)teamMembersRes).StatusCode == 200)
                            {
                                viewModel.ProjectMembersWithChild = (List<PTProjectMemberWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)teamMembersRes).Value;

                                if (viewModel.ProjectMembersWithChild != null && viewModel.ProjectMembersWithChild.Any())
                                {
                                    foreach (var item in viewModel.ProjectMembersWithChild)
                                    {
                                        if (item.ProfileImage != null)
                                            item.UserProfileImagePath = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
                                    }

                                }
                            }
                        }
                    }

                }
            }
        }

        var resCount = await _projectAPIController.ProjectListProjectStatusWiseAndPageWise(empSession.UnitId ?? default(int), empSession.EmployeeId, inputDTO, "Count");
        if (resCount != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCount).StatusCode == 200)
        {
            ProjectPageDetails pageDetails = new ProjectPageDetails();
            var totalRecords = (List<int>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCount).Value;
            pageDetails.TotalRecords = totalRecords == null ? 0 : totalRecords.Count == 0 ? 0 : totalRecords[0];

            pageDetails.PageSize = (inputDTO != null && inputDTO.PageSize != null) ? inputDTO.PageSize : 6;
            pageDetails.PageNumber = (inputDTO != null && inputDTO.PageNumber != null) ? inputDTO.PageNumber : 1;
            pageDetails.TotalPages = (int)Math.Ceiling((double?)pageDetails.TotalRecords / pageDetails.PageSize ?? default(int));
            pageDetails.SearchKeyword = inputDTO == null ? "" : String.IsNullOrEmpty(inputDTO.SearchKeyword) ? "" : inputDTO.SearchKeyword;
            viewModel.PageDetails = pageDetails;
        }

        return PartialView("_projectList1/_projects", viewModel);
    }


    public async Task<IActionResult> GetProjectWithChildById([FromBody] PTProjectDTO inputDTO)
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
        ProjectViewModel viewModel = new ProjectViewModel();

        if (inputDTO == null)
        {
            throw new Exception("Invalid Data");
        }
        if (inputDTO.encProjectID == null)
        {
            throw new Exception("Invalid Data");
        }
        if (String.IsNullOrEmpty(inputDTO.encProjectID))
        {
            throw new Exception("Project Unvailable at the moment");
        }
        inputDTO.ProjectID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectID));
        var res = await _projectAPIController.ProjectWithChildById(inputDTO.ProjectID, empSession.EmployeeId);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.ProjectWithChild = (ProjectWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (viewModel.ProjectWithChild != null)
                {
                    viewModel.ProjectWithChild.encProjectID = CommonHelper.EncryptURLHTML(viewModel.ProjectWithChild.ProjectID.ToString());
                    if (viewModel.ProjectWithChild.ProfileImage != null)
                        viewModel.ProjectWithChild.InitiatorProfileImagePath = "data:image/png;base64," + Convert.ToBase64String(viewModel.ProjectWithChild.ProfileImage, 0, viewModel.ProjectWithChild.ProfileImage.Length);
                }
            }
        }
        List<PTMilestonesDTO>? milestonesList = new List<PTMilestonesDTO>();
        var MilestonesRes = await _projectAPIController.GetMilestonesByProjectId(inputDTO.ProjectID);
        if (MilestonesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)MilestonesRes).StatusCode == 200)
        {
            milestonesList = (List<PTMilestonesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)MilestonesRes).Value;
            if (milestonesList != null && milestonesList.Any())
            {
                foreach (var itemMilestone in milestonesList)
                {
                    itemMilestone.encMilestoneId = CommonHelper.EncryptURLHTML(itemMilestone.MilestoneId.ToString());
                }
                viewModel.MilestonesList = milestonesList;
                viewModel.SerializedMilestones = JsonConvert.SerializeObject(milestonesList);
            }
        }
        List<PTProjectMemberWithChild>? projectMembers = new List<PTProjectMemberWithChild>();
        var projectMembersRes = await _projectAPIController.GetTeamMembersByProjectId(inputDTO.ProjectID);
        if (projectMembersRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)projectMembersRes).StatusCode == 200)
        {
            projectMembers = (List<PTProjectMemberWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)projectMembersRes).Value;
            if (projectMembers != null && projectMembers.Any())
            {
                viewModel.ProjectMembersWithChild = projectMembers;
                foreach (var item in viewModel.ProjectMembersWithChild)
                {
                    if (item.ProfileImage != null)
                        item.UserProfileImagePath = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
                }
            }
        }

        List<PTCommentWithChild>? comments = new List<PTCommentWithChild>();
        var commentsRes = await _projectAPIController.GetCommentsByProjectId(inputDTO.ProjectID, empSession.EmployeeId);
        if (commentsRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)commentsRes).StatusCode == 200)
        {
            comments = (List<PTCommentWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)commentsRes).Value;
            if (comments != null && comments.Any())
            {
                viewModel.Comments = comments;
                foreach (var item in viewModel.Comments)
                {
                    item.encCommentID = CommonHelper.EncryptURLHTML(item.CommentID.ToString());
                    if (item.ProfileImage != null)
                        item.ProfileImagePath = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
                }
            }
        }

        var projectStatusList = await _projectCategoryMasterAPIController.GetProjectStatusList(empSession.UnitId ?? default(int));
        if (projectStatusList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)projectStatusList).StatusCode == 200)
        {
            viewModel.StatusList = (List<PTStatusDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)projectStatusList).Value;
        }

        return PartialView("_projectList1/_projectView", viewModel);
        //return View(viewModel);
    }

    [Route("/ProjectTracker/AddProject/{eProjectId?}")]
    public async Task<IActionResult> AddProject(string? eProjectId)
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

            ProjectViewModel projectViewModel = new ProjectViewModel();
            //employeeAnnouncementViewModel.Departments = await _mastersKeyValueController.DepartmentKeyValueUnitWise(empSession.UnitId ?? default(int));
            var projectCategoryList = await _projectCategoryMasterAPIController.GetProjectCategoryList(empSession.UnitId ?? default(int));
            var projectStatusList = await _projectCategoryMasterAPIController.GetProjectStatusList(empSession.UnitId ?? default(int));
            var priorityList = await _projectCategoryMasterAPIController.GetPriorities(empSession.UnitId ?? default(int));
            if (projectCategoryList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)projectCategoryList).StatusCode == 200)
            {
                projectViewModel.ProjectCategoryList = (List<PTProjectCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)projectCategoryList).Value;
            }
            if (projectStatusList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)projectStatusList).StatusCode == 200)
            {
                projectViewModel.StatusList = (List<PTStatusDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)projectStatusList).Value;
            }
            if (priorityList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)priorityList).StatusCode == 200)
            {
                projectViewModel.PriorityList = (List<PTProjectPriorityDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)priorityList).Value;
            }
            var EmployeeRes = await _projectAPIController.GetEmployeeListByUnitId(empSession.UnitId ?? default(int));
            if (EmployeeRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)EmployeeRes).StatusCode == 200)
            {
                projectViewModel.EmployeeList = (List<PTEmployees>?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployeeRes).Value;
            }

            if (!(projectViewModel.ProjectCategoryList != null && projectViewModel.ProjectCategoryList.Count() != 0))
            {
                Error error = new Error();
                error.Heading = "Project Categories Not Found";
                error.Message = "Please contact your administrator before accessing this screen";
                error.ButtonMessage = "Go To Back To Previous Page";
                error.ButtonURL = "javascript:history.go(-1)";
                return View("../Page/Error", error);
            }
            if (!(projectViewModel.StatusList != null && projectViewModel.StatusList.Count() != 0))
            {
                Error error = new Error();
                error.Heading = "Project Status Not Found";
                error.Message = "Please contact your administrator before accessing this screen";
                error.ButtonMessage = "Go To Back To Previous Page";
                error.ButtonURL = "javascript:history.go(-1)";
                return View("../Page/Error", error);
            }
            if (!(projectViewModel.PriorityList != null && projectViewModel.PriorityList.Count() != 0))
            {
                Error error = new Error();
                error.Heading = "Priority List Not Found";
                error.Message = "Please contact your administrator before accessing this screen";
                error.ButtonMessage = "Go To Back To Previous Page";
                error.ButtonURL = "javascript:history.go(-1)";
                return View("../Page/Error", error);
            }

            if (eProjectId != null)
            {
                int ProjectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eProjectId));
                var ProjectRes = await _projectAPIController.ProjectById(ProjectId);
                if (ProjectRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)ProjectRes).StatusCode == 200)
                {
                    projectViewModel.Project = (PTProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)ProjectRes).Value;
                    if (projectViewModel.Project != null)
                        projectViewModel.Project.encProjectID = eProjectId;
                }
                //var EmployementAnnouncementFileUploadRes = await _employeeAnnouncementAPIController.EmployeeAnnouncementFilesByAnnouncementId(AnnouncementId);
                //if (EmployementAnnouncementFileUploadRes != null)
                //{
                //    if (((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementFileUploadRes).StatusCode == 200)
                //    {
                //        employeeAnnouncementViewModel.EmployeeAnnouncementFileUploadList = (List<EmployeeAnnouncementFileUploadDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)EmployementAnnouncementFileUploadRes).Value;
                //    }
                //}
                List<PTMilestonesDTO>? milestonesList = new List<PTMilestonesDTO>();
                List<PTDeliverablesDTO>? deliverablesList = new List<PTDeliverablesDTO>();
                var MilestonesRes = await _projectAPIController.GetMilestonesByProjectId(ProjectId);
                if (MilestonesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)MilestonesRes).StatusCode == 200)
                {
                    milestonesList = (List<PTMilestonesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)MilestonesRes).Value;
                    if (milestonesList != null && milestonesList.Any())
                    {
                        projectViewModel.MilestonesList = milestonesList;
                        projectViewModel.SerializedMilestones = JsonConvert.SerializeObject(milestonesList);
                    }
                }
                var DeliverablesRes = await _projectAPIController.GetDeliverablesByProjectId(ProjectId);
                if (DeliverablesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)DeliverablesRes).StatusCode == 200)
                {
                    deliverablesList = (List<PTDeliverablesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)DeliverablesRes).Value;
                    if (deliverablesList != null && deliverablesList.Any())
                    {
                        projectViewModel.DeliverablesList = deliverablesList;
                        projectViewModel.SerializedDeliverables = JsonConvert.SerializeObject(deliverablesList);
                    }
                }
                List<PTProjectMemberDTO>? projectMemberList = new List<PTProjectMemberDTO>();
                var ProjectMembersRes = await _projectAPIController.GetProjectMembersByProjectId(ProjectId);
                if (ProjectMembersRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)ProjectMembersRes).StatusCode == 200)
                {
                    projectMemberList = (List<PTProjectMemberDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)ProjectMembersRes).Value;
                    if (projectMemberList != null && projectMemberList.Any())
                    {
                        var approverList = projectMemberList.Where(x => x.RoleType == "Approver");
                        if (approverList != null && approverList.Any())
                        {
                            var approver = approverList.FirstOrDefault();
                            if (approver != null)
                            {
                                projectViewModel.ApproverId = approver.UserID;
                            }

                        }
                        var collaboratorList = projectMemberList.Where(x => x.RoleType == "Collaborator");
                        if (collaboratorList != null && collaboratorList.Any())
                        {
                            projectViewModel.CollaboratorIds = string.Join(",", collaboratorList.Select(x => x.UserID));
                        }
                    }
                }
            }
            return View(projectViewModel);
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
    public async Task<IActionResult> SaveProject(ProjectViewModel dataVM)
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
            if (dataVM == null || dataVM.Project == null)
            {
                return BadRequest("Data is not in the valid format");
            }

            int ProjectID = 0;
            List<int> FileUploadIds = new List<int>();
            string optType = "";

            #region Save Project

            TimeZoneInfo indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            if (dataVM.Project.encProjectID != null && dataVM.Project.encProjectID != "")
            {
                dataVM.Project.ProjectID = Convert.ToInt32(CommonHelper.DecryptURLHTML(dataVM.Project.encProjectID));
                dataVM.Project.ModifiedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);
                dataVM.Project.ModifiedBy = empSession.EmployeeId;
                optType = "Update";
            }
            else
            {
                dataVM.Project.IsApproved = 0;
                dataVM.Project.IsActive = true;
                dataVM.Project.UnitId = empSession.UnitId ?? default(int);
                dataVM.Project.CreatedDate = DateTime.Now;
                dataVM.Project.CreatedBy = empSession.EmployeeId;
                optType = "Insert";
            }
            var ProjectRes = await _projectAPIController.SaveProject(dataVM.Project);
            if (ProjectRes != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)ProjectRes).StatusCode == 200)
                {
                    PTProjectDTO? ead = (PTProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)ProjectRes).Value;
                    if (ead != null)
                    {
                        if (optType == "Update")
                            dataVM.Project.ProjectID = ead.ProjectID;
                        else
                            dataVM.Project.ProjectID = ProjectID = ead.ProjectID;
                    }
                    else
                    {
                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                    }
                }
                else
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)ProjectRes).Value == null)
                    {
                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                    }
                    else
                    {
                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)ProjectRes).Value.ToString());
                    }
                }
            }
            #endregion

            #region TeamMembers

            await _projectAPIController.SaveProjectMembers(dataVM, empSession.EmployeeId);

            #endregion

            #region Polls

            //dataVM.employeeAnnouncementDTO.EmployeeAnnouncementId

            if (dataVM.SerializedMilestones != null)
            {
                List<PTMilestonesDTO>? pTMilestonesDTOs = JsonConvert.DeserializeObject<List<PTMilestonesDTO>?>(dataVM.SerializedMilestones);

                if (pTMilestonesDTOs != null && pTMilestonesDTOs.Count > 0)
                {
                    dataVM.MilestonesList = pTMilestonesDTOs;
                    await _projectAPIController.SaveMilestones(dataVM);
                }
            }
            if (dataVM.SerializedDeliverables != null)
            {
                List<PTDeliverablesDTO>? pTDeliverablesDTOs = JsonConvert.DeserializeObject<List<PTDeliverablesDTO>?>(dataVM.SerializedDeliverables);

                if (pTDeliverablesDTOs != null && pTDeliverablesDTOs.Count > 0)
                {
                    dataVM.DeliverablesList = pTDeliverablesDTOs;
                    await _projectAPIController.SaveDeliverables(dataVM);
                }
            }


            #endregion




            return Ok("Success");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> DeleteMilestone([FromBody] PTMilestonesDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var res = await _projectAPIController.DeleteMilestones(inputDTO.MilestoneId);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> MarkMilestoneCompleted([FromBody] PTMilestonesDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var res = await _projectAPIController.MarkMilestoneCompleted(inputDTO.MilestoneId);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> ActionOnMilestone([FromBody] PTMilestonesDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.encMilestoneId != null)
                {
                    inputDTO.MilestoneId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encMilestoneId));
                    var res = await _projectAPIController.ActionOnMilestone(inputDTO.MilestoneId, inputDTO.Action == null ? "" : inputDTO.Action);
                    return res;
                }
                else
                {
                    throw new Exception("Milestone record not found");
                }

            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveComments(PTCommentViewModel dataVM)
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
            if (dataVM == null || dataVM.Comments == null)
            {
                return BadRequest("Data is not in the valid format");
            }
            if (dataVM.Comments.encProjectID == null)
            {
                return BadRequest("Data is not in the valid format");
            }
            else
            {
                dataVM.Comments.TaskID = Convert.ToInt32(CommonHelper.DecryptURLHTML(dataVM.Comments.encProjectID));
            }
            int CommentId = 0;
            List<int> FileUploadIds = new List<int>();
            string optType = "";

            TimeZoneInfo indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            #region Save Comment

            dataVM.Comments.IsActive = true;
            dataVM.Comments.UserID = empSession.EmployeeId;
            dataVM.Comments.CommentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);
            dataVM.Comments.IsVisible = true;
            dataVM.Comments.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);
            dataVM.Comments.CreatedBy = empSession.EmployeeId;


            var commentsRes = await _projectAPIController.SaveComments(dataVM.Comments);
            if (commentsRes != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)commentsRes).StatusCode == 200)
                {
                    PTCommentDTO? ead = (PTCommentDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)commentsRes).Value;
                    if (ead != null)
                    {
                        dataVM.Comments.CommentID = CommentId = ead.CommentID;
                    }
                    else
                    {
                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                    }
                }
                else
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)commentsRes).Value == null)
                    {
                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                    }
                    else
                    {
                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)commentsRes).Value.ToString());
                    }
                }
            }
            #endregion

            #region Attachments

            try
            {
                var attachments = dataVM.Attachment;
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        if (attachment.Length > 0)
                        {
                            PTAttachmentDTO pTAttachmentDTO = new PTAttachmentDTO();
                            var attachmentName = Path.GetFileName(attachment.FileName);
                            var attachmentExtension = Path.GetExtension(attachmentName);
                            string FilePathWithoutRoot = Path.Combine("Uploads", "ProjectTracker", "Attachment", dataVM.Comments.CommentID.ToString());
                            string FilePath = Path.Combine(_hostingEnv.WebRootPath, FilePathWithoutRoot);
                            if (!Directory.Exists(FilePath))
                                Directory.CreateDirectory(FilePath);
                            var filePath = Path.Combine(FilePath, attachmentName);
                            pTAttachmentDTO.CommentID = dataVM.Comments.CommentID;
                            pTAttachmentDTO.FileName = attachmentName;
                            pTAttachmentDTO.FilePath = FilePathWithoutRoot;
                            pTAttachmentDTO.FileType = attachmentExtension;
                            pTAttachmentDTO.IsActive = true;
                            pTAttachmentDTO.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);
                            pTAttachmentDTO.CreatedBy = empSession.EmployeeId;

                            var attachmentFileUploadRes = await _projectAPIController.SaveCommentAttachments(pTAttachmentDTO);
                            if (attachmentFileUploadRes != null)
                            {
                                if (((Microsoft.AspNetCore.Mvc.ObjectResult)attachmentFileUploadRes).StatusCode == 200)
                                {
                                    PTAttachmentDTO? eafud = (PTAttachmentDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)attachmentFileUploadRes).Value;
                                    if (eafud != null)
                                    {
                                        pTAttachmentDTO.AttachmentID = eafud.AttachmentID;
                                        FileUploadIds.Add(eafud.AttachmentID);
                                    }
                                    else
                                    {
                                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                                    }
                                }
                                else
                                {
                                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)attachmentFileUploadRes).Value == null)
                                    {
                                        throw new Exception("Some error has occurred. Please refresh the page and try again");
                                    }
                                    else
                                    {
                                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)attachmentFileUploadRes).Value.ToString());
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

            }
            catch (Exception e)
            {
                await _projectAPIController.HardDeleteAttachments(CommentId, FileUploadIds);
                return BadRequest(e.Message);
            }

            #endregion

            if (dataVM != null && dataVM.Comments != null)
            {
                dataVM.Comments.encCommentID = CommonHelper.EncryptURLHTML(dataVM.Comments.CommentID.ToString());
            }

            return Ok(dataVM);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> ActionOnProject([FromBody] PTProjectDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (String.IsNullOrEmpty(inputDTO.encProjectID))
                {
                    return BadRequest("No Project Found");
                }
                inputDTO.ProjectID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectID));
                var res = await _projectAPIController.ActionOnProject(inputDTO);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> ActionOnStatus([FromBody] PTProjectDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (String.IsNullOrEmpty(inputDTO.encProjectID))
                {
                    return BadRequest("No Project Found");
                }
                inputDTO.ProjectID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectID));
                var res = await _projectAPIController.ActionOnStatus(inputDTO);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> ProjectStatusChange([FromBody] PTProjectDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (String.IsNullOrEmpty(inputDTO.encProjectID))
                {
                    return BadRequest("No Project Found");
                }
                inputDTO.ProjectID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectID));
                var res = await _projectAPIController.ProjectStatusChange(inputDTO);
                return res;
            }
            throw new Exception("Data is not valid");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet]
    [Route("ProjectTracker/DownloadAttachments/{encCommentID}")]
    public async Task<IActionResult> DownloadAttachments(string encCommentID)
    {
        try
        {
            if (!String.IsNullOrEmpty(encCommentID))
            {
                int CommentID = Convert.ToInt32(CommonHelper.DecryptURLHTML(encCommentID));

                List<PTAttachmentDTO>? attachments = new List<PTAttachmentDTO>();
                var resAttachments = await _projectAPIController.GetAttachmentsByCommentId(CommentID);
                if (resAttachments != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAttachments).StatusCode == 200)
                {
                    attachments = (List<PTAttachmentDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAttachments).Value;
                    if (attachments != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                            {
                                foreach (var attachment in attachments)
                                {
                                    var filePath = attachment.FilePath;
                                    var fileName = attachment.FileName;
                                    var fullPath = Path.Combine(_hostingEnv.WebRootPath, filePath, fileName);
                                    var fileBytes = System.IO.File.ReadAllBytes(fullPath);
                                    var entry = archive.CreateEntry(fileName);

                                    using (var entryStream = entry.Open())
                                    {
                                        entryStream.Write(fileBytes, 0, fileBytes.Length);
                                    }
                                }
                            }
                            return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
                        }
                    }
                }
            }
            else
            {
                return NotFound();
            }
            throw new Exception("No Attachment Found");
        }
        catch (Exception ex)
        {
            return NotFound("No Attachment Found");
        }
    }
}
