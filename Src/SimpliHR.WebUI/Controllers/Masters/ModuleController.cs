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
using SimpliHR.Infrastructure.Helper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class ModuleController : Controller
    {

        private readonly ModuleMasterController _ModuleAPIController;
        public ModuleController(ModuleMasterController ModuleAPIController)
        {
            _ModuleAPIController = ModuleAPIController;
        }

        public async Task<IActionResult> Module()
        {
            ModuleMasterDTO outputData = new ModuleMasterDTO();
            outputData.ModuleMasterList = await GetModuleList();

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Module1()
        {
            ModuleMasterDTO outputData = new ModuleMasterDTO();
            outputData.ModuleMasterList = await GetModuleList();


            if (outputData != null)
            {
                foreach (var item in outputData.ModuleMasterList)
                {
                    item.EncryptedId = CommonHelper.EncryptURLHTML(item.ModuleId.ToString());
                }
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        public async Task<List<ModuleMasterDTO>?> GetModuleList()
        {

            IActionResult actionResult = await _ModuleAPIController.GetModules(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<ModuleMasterDTO> objResultData = (List<ModuleMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.ModuleId.ToString());
            }
            return objResultData;
        }

        [HttpGet]
        [Route("Module/GetModuleInfo/{eModuleId}")]
        public async Task<IActionResult> GetModuleInfo(string eModuleId)
        {
            int ModuleId = 0;
            try
            {
                ModuleId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eModuleId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModuleId != 0)
            {
                ModuleMasterDTO outputData = new ModuleMasterDTO();
                outputData.ModuleId = ModuleId;

                IActionResult actionResult;

                actionResult = await _ModuleAPIController.GetModule(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (ModuleMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("Module", objResultData);
                    //return RedirectToAction("Module","Module", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.ModuleId = 0;
                    objResultData.ModuleMasterList = await GetModuleList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    //return View("Module",objResultData);
                    //return RedirectToAction("Module", objResultData);
                }
            }
            return RedirectToAction("Module", "Module");
        }

        [HttpGet]
        [Route("Module/DeleteModule/{eModuleId}")]
        public async Task<IActionResult> DeleteModule(string eModuleId)
        {
            int ModuleId = 0;
            try
            {
                ModuleId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eModuleId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModuleId != 0)
            {
                ModuleMasterDTO outputData = new ModuleMasterDTO();
                outputData.ModuleId = ModuleId;

                IActionResult actionResult;

                actionResult = await _ModuleAPIController.DeleteModule(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.ModuleId = 0;
                outputData.ModuleMasterList = await GetModuleList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("Module", outputData);
                //}
            }
            return RedirectToAction("Module", "Module");
        }

        [HttpPost]
        public async Task<IActionResult> SaveModule(ModuleMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Module", inputData);
            //}

            inputData.IsActive = true;
            IActionResult actionResult;
            ModuleMasterDTO viewModel = new ModuleMasterDTO();
            if (inputData.ModuleId == 0)
                actionResult = _ModuleAPIController.SaveModule(inputData);
            else
                actionResult = _ModuleAPIController.UpdateModule(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.ModuleId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    viewModel.DisplayMessage = "Transaction Successful!";
                inputData.ModuleId = 0;
                inputData.ModuleMasterList = await GetModuleList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("Module", viewModel);

        }
    }
}
