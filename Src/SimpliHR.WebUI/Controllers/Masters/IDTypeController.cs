using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using SimpliHR.Infrastructure.Helper;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class IDTypeController : Controller
    {
        private readonly IdtypeMasterController _idtypeAPIController;
        public IDTypeController(IdtypeMasterController idtypeAPIController)
        {
            _idtypeAPIController = idtypeAPIController;
        }

        public async Task<IActionResult> IDType()
        {
            IdtypeMasterDTO outputData = new IdtypeMasterDTO();
            outputData.IdtypeMasterList = await GetIDTypeList();
            if (outputData != null)
            {
                //foreach (var item in outputData.IdtypeMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.IdentityId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<IdtypeMasterDTO>?> GetIDTypeList()
        {

            IActionResult actionResult = await _idtypeAPIController.GetIdtypes(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<IdtypeMasterDTO> objResultData = (List<IdtypeMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.IdentityId.ToString());
            }

            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveIDType(IdtypeMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("IDType", inputData);
            //}
            inputData.IsActive = true;
            IActionResult actionResult;
            IdtypeMasterDTO viewModel = new IdtypeMasterDTO();

            if (inputData.IdentityId == 0)
                actionResult = _idtypeAPIController.SaveIdtype(inputData);
            else
                actionResult = _idtypeAPIController.UpdateIdtype(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.IdentityId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.IdentityId = 0;
                inputData.IdtypeMasterList = await GetIDTypeList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            viewModel = inputData;
            return View("IDType", viewModel);

        }


        [HttpGet]
        [Route("IDType/GetIDTypeInfo/{eidentityId}")]
        public async Task<IActionResult> GetIDTypeInfo(string eidentityId)
        {

            int identityId = 0;
            try
            {
                identityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eidentityId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (identityId != 0)
            {
                IdtypeMasterDTO outputData = new IdtypeMasterDTO();
                outputData.IdentityId = identityId;

                IActionResult actionResult;

                actionResult = await _idtypeAPIController.GetIdtype(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (IdtypeMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("IDType", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.IdentityId = 0;
                    objResultData.IdtypeMasterList = await GetIDTypeList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("IDType", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("IDType", "IDType");

        }

        [HttpGet]
        [Route("IDType/DeleteIDType/{eidentityId}")]
        public async Task<IActionResult> DeleteIDType(string eidentityId)
        {
            int identityId = 0;
            try
            {
                identityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eidentityId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (identityId != 0)
            {
                IdtypeMasterDTO outputData = new IdtypeMasterDTO();
                outputData.IdentityId = identityId;

                IActionResult actionResult;

                actionResult = await _idtypeAPIController.DeleteIdtype(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.IdentityId = 0;
                outputData.IdtypeMasterList = await GetIDTypeList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("IDType", outputData);
                //}
            }
            return RedirectToAction("IDType", "IDType");

        }
    }
}
