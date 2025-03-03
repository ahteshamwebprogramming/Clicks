using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using SimpliHR.WebUI.Modals.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.WebUI.Masters;
using System.Data;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace SimpliHR.WebUI.Controllers.Masters
{
    [Authorize(Roles = "Clientadmin")]
    public class RoleController : Controller
    {

        private readonly RoleMasterController _roleAPIController;
        public RoleController(RoleMasterController roleAPIController)
        {
            _roleAPIController = roleAPIController;
        }

        public async Task<IActionResult> Role()
        {
            RoleMasterDTO outputData = new RoleMasterDTO();
            outputData.RoleMasterList = await GetRoleList();


            if (outputData != null)
            {
                //foreach (var item in outputData.RoleMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.RoleId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<RoleMasterDTO>?> GetRoleList()
        {

            IActionResult actionResult = await _roleAPIController.GetRoles(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<RoleMasterDTO> objResultData = (List<RoleMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.RoleId.ToString());
            }
            return objResultData;
        }

        [HttpGet]
        [Route("Role/GetRoleInfo/{eroleId}")]
        public async Task<IActionResult> GetRoleInfo(string eroleId)
        {


            int roleId = 0;
            try
            {
                roleId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eroleId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (roleId != 0)
            {
                RoleMasterDTO outputData = new RoleMasterDTO();
                outputData.RoleId = roleId;

                IActionResult actionResult;

                actionResult = await _roleAPIController.GetRole(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (RoleMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    if (objResultData != null && objResultData.RoleType != null)
                    {
                        objResultData.RoleType = objResultData.RoleType.Trim();
                    }

                    return View("Role", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.RoleId = 0;
                    objResultData.RoleMasterList = await GetRoleList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    //return View("Role",objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("Role", "Role");

        }




        [HttpGet]
        [Route("Role/DeleteRole/{eroleId}")]
        public async Task<IActionResult> DeleteRole(string eroleId)
        {
            int roleId = 0;
            try
            {
                roleId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eroleId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (roleId != 0)
            {
                RoleMasterDTO outputData = new RoleMasterDTO();
                outputData.RoleId = roleId;

                IActionResult actionResult;

                actionResult = await _roleAPIController.DeleteRole(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.RoleId = 0;
                outputData.RoleMasterList = await GetRoleList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("Role", outputData);
                //}
            }
            return RedirectToAction("Role", "Role");

        }

        [HttpPost]
        public async Task<IActionResult> SaveRole(RoleMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Role", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;

            IActionResult actionResult;
            RoleMasterDTO viewModel = new RoleMasterDTO();
            if (inputData.RoleId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
                actionResult = _roleAPIController.SaveRole(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifedBy = employeeId;
                actionResult = _roleAPIController.UpdateRole(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.RoleId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.RoleId = 0;
                inputData.RoleMasterList = await GetRoleList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("Role", viewModel);

        }
    }
}
