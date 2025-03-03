using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Webui.Modals.Account;
using System.ComponentModel.Design;
using X.PagedList;
using System.Net;
using rolesandpermissions = SimpliHR.Endpoints.RolesAndPermissions;
using SimpliHR.Core.Entities;
using System.Linq;

namespace SimpliHR.WebUI.Controllers.RolesAndPermissions
{
    public class RolesController : Controller
    {
        private readonly rolesandpermissions.RolesController _rolesAPIController;
        private readonly ClientController _clientAPIController;
        public RolesController(rolesandpermissions.RolesController rolesAPIController, ClientController clientAPIController)
        {
            _rolesAPIController = rolesAPIController;
            _clientAPIController = clientAPIController;
        }


        public async Task<List<MenuMasterDTO>> fetchAllMenuItems(int JobTitleId, int RoleId, int DepartmentId)
        {
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) == true && unitId != null)
            {
                string Modules = HttpContext.Session.GetString("Modules");
                var ModuleIds = Modules?.Split(',')?.Select(Int32.Parse)?.ToList();
                Core.Helper.RequestParams requestParams = new Core.Helper.RequestParams();

                IActionResult actionResultMenu = await _rolesAPIController.GetMenus();
                ObjectResult objResultMenu = (ObjectResult)actionResultMenu;
                List<MenuMasterDTO> menuMasterDTOs1 = (List<MenuMasterDTO>)objResultMenu.Value;
                RoleMenuMappingDTO roleMenuMappingDTO = new RoleMenuMappingDTO();
                roleMenuMappingDTO.RoleId = RoleId;
                roleMenuMappingDTO.JobTitleId = JobTitleId;
                roleMenuMappingDTO.DepartmentId = DepartmentId;

                IActionResult actionResultMapping = await _rolesAPIController.GetRoleMenuMappings(roleMenuMappingDTO, false, clientId, unitId);

                ObjectResult objResultMapping = (ObjectResult)actionResultMapping;
                List<RoleMenuMappingDTO> Mapping = (List<RoleMenuMappingDTO>)objResultMapping.Value;
                List<MenuMasterDTO> menuMasterDTOs;
                if (ModuleIds != null)
                {
                    menuMasterDTOs = menuMasterDTOs1.Where(x => ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
                }
                else
                {
                    menuMasterDTOs = menuMasterDTOs1.ToList();
                }
                foreach (var item in menuMasterDTOs)
                {
                    if (Mapping.Where(x => x.MenuId == item.MenuId && x.IsActive == 1).Count() > 0)
                    {
                        item.Checked = true;
                    }
                }
                return menuMasterDTOs;
            }
            return null;
        }

        [HttpPost]
        public ActionResult MapRoles(ListRoleMenuMappingDTOForSave roleMenuMappingDTO)
        {
            try
            {

                int clientId;
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
                {
                    roleMenuMappingDTO.ClientId = clientId;
                    roleMenuMappingDTO.UnitId = unitId ?? default(int);
                    var res = _rolesAPIController.DeleteMappingByJobTitleIdRoleId(roleMenuMappingDTO);
                    if (res != null)
                    {
                        if (((Microsoft.AspNetCore.Mvc.StatusCodeResult)res).StatusCode == 200)
                        {
                            foreach (var item in roleMenuMappingDTO.Menus)
                            {
                                RoleMenuMappingDTO roleMenuMappingDTO1 = new RoleMenuMappingDTO();
                                roleMenuMappingDTO1.MenuId = item;
                                roleMenuMappingDTO1.JobTitleId = roleMenuMappingDTO.JobTitleId;
                                roleMenuMappingDTO1.RoleId = roleMenuMappingDTO.RoleId;
                                roleMenuMappingDTO1.DepartmentId = roleMenuMappingDTO.DepartmentId;
                                roleMenuMappingDTO1.ClientId = roleMenuMappingDTO.ClientId;
                                roleMenuMappingDTO1.UnitId = roleMenuMappingDTO.UnitId;
                                roleMenuMappingDTO1.IsActive = 1;
                                _rolesAPIController.SaveMenuMapping(roleMenuMappingDTO1);
                            }
                            return Ok(StatusCodes.Status200OK);
                        }
                        else
                        {
                            throw new Exception("Unable to process the mapping right now");
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to process the mapping right now");
                    }
                }
                else
                {
                    throw new Exception("Unable to process the mapping right now");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        public async Task<IActionResult> RoleMenuMapping()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            ListRoleMenuMappingDTO listRoleMenuMappingDTO = new ListRoleMenuMappingDTO();

            IActionResult actionResultJobTitle = await _rolesAPIController.GetJobTitlesUnitWise(unitId ?? default(int), null);
            ObjectResult objResultJobTitle = (ObjectResult)actionResultJobTitle;
            List<JobTitleMasterDTO> JobTitle = (List<JobTitleMasterDTO>)objResultJobTitle.Value;

            IActionResult actionResultRoles = await _rolesAPIController.GetRolesUnitWise(null, unitId ?? default(int));
            ObjectResult objResultRoles = (ObjectResult)actionResultRoles;
            List<RoleMasterDTO> Roles = (List<RoleMasterDTO>)objResultRoles.Value;

            IActionResult actionResultDepartment = await _rolesAPIController.GetDepartmentUnitWise(null, unitId ?? default(int));
            ObjectResult objResultDepartment = (ObjectResult)actionResultDepartment;
            List<DepartmentMasterDTO> Departments = (List<DepartmentMasterDTO>)objResultDepartment.Value;

            listRoleMenuMappingDTO.jobTitles = JobTitle;
            listRoleMenuMappingDTO.Roles = Roles;
            listRoleMenuMappingDTO.Departments = Departments;

            return View(listRoleMenuMappingDTO);
        }
        public async Task<IActionResult> RoleMenuMapping_New()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            ListRoleMenuMappingDTO listRoleMenuMappingDTO = new ListRoleMenuMappingDTO();

            IActionResult actionResultJobTitle = await _rolesAPIController.GetJobTitlesUnitWise(unitId ?? default(int), null);
            ObjectResult objResultJobTitle = (ObjectResult)actionResultJobTitle;
            List<JobTitleMasterDTO> JobTitle = (List<JobTitleMasterDTO>)objResultJobTitle.Value;

            IActionResult actionResultRoles = await _rolesAPIController.GetRolesUnitWise(null, unitId ?? default(int));
            ObjectResult objResultRoles = (ObjectResult)actionResultRoles;
            List<RoleMasterDTO> Roles = (List<RoleMasterDTO>)objResultRoles.Value;

            IActionResult actionResultDepartment = await _rolesAPIController.GetDepartmentUnitWise(null, unitId ?? default(int));
            ObjectResult objResultDepartment = (ObjectResult)actionResultDepartment;
            List<DepartmentMasterDTO> Departments = (List<DepartmentMasterDTO>)objResultDepartment.Value;

            listRoleMenuMappingDTO.jobTitles = JobTitle;
            listRoleMenuMappingDTO.Roles = Roles;
            listRoleMenuMappingDTO.Departments = Departments;

            return View(listRoleMenuMappingDTO);
        }
        public async Task<IActionResult> Mappings()
        {

            //    IActionResult actionResultMenu = await _rolesAPIController.GetMenus(null);
            //    ObjectResult objResultMenu = (ObjectResult)actionResultMenu;

            //    List<MenuMasterDTO> Menu = (List<MenuMasterDTO>)objResultMenu.Value;

            //    IActionResult actionResultJobTitle = await _rolesAPIController.GetJobTitles(null);
            //    ObjectResult objResultJobTitle = (ObjectResult)actionResultJobTitle;
            //    List<JobTitleMasterDTO> JobTitle = (List<JobTitleMasterDTO>)objResultJobTitle.Value;

            //    IActionResult actionResultRoles = await _rolesAPIController.GetRoles(null);
            //    ObjectResult objResultRoles = (ObjectResult)actionResultRoles;
            //    List<RoleMasterDTO> Roles = (List<RoleMasterDTO>)objResultRoles.Value;

            //    IActionResult actionResultMapping = await _rolesAPIController.GetRoleMenuMappings(null,null);
            //    ObjectResult objResultMapping = (ObjectResult)actionResultMapping;
            //    List<RoleMenuMappingDTO> Mapping = (List<RoleMenuMappingDTO>)objResultMapping.Value;

            //    ClientSettingDTO clientSettingDTO = await _clientAPIController.GetClientSettingDetails(Convert.ToInt32(HttpContext.Session.GetString("ClientId")));

            //    string[] modules = clientSettingDTO.ModuleList.ToString().Split(',').ToArray();

            //    ListRoleMenuMappingDTO listRoleMenuMappingDTO = new ListRoleMenuMappingDTO();

            //    foreach (string item in modules)
            //    {
            //        int mid = Convert.ToInt32(item);
            //        var menus = Menu.Where(x => x.ModuleId == mid).ToList();
            //        foreach (var item1 in menus)
            //        {
            //            listRoleMenuMappingDTO.Menus.Add(item1);
            //        }
            //    }
            return null;
        }
    }
}
