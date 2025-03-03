using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.StatutoryComponent;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.MenuMaster;
using SimpliHR.Infrastructure.Models.StatutoryComponent;

namespace SimpliHR.WebUI.Controllers;

public class MenuMasterController : Controller
{
    private readonly MenuMasterAPIController _menuMasterAPIController;
    private readonly ModuleMasterController _ModuleAPIController;
    public MenuMasterController(MenuMasterAPIController menuMasterAPIController, ModuleMasterController moduleAPIController)
    {
        _menuMasterAPIController = menuMasterAPIController;
        _ModuleAPIController = moduleAPIController;
    }
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> List()
    {
        List<MenuMasterDTO> outputData = new List<MenuMasterDTO>();
        var res = await _menuMasterAPIController.GetMenuMaster(-1);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                outputData = (List<MenuMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return View(outputData);
    }
    public async Task<IActionResult> ListMenusPartialView([FromBody] MenuMasterDTO menuMasterDTO)
    {
        MenuMasterListView menuMasterListView = new MenuMasterListView();
        var res = await _menuMasterAPIController.GetMenuMaster();
        var resModules = await _ModuleAPIController.GetAllModules();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                if (menuMasterDTO.ParentMenuId == -1)
                {
                    menuMasterListView.MenuMasterList = ((List<MenuMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value).OrderBy(x => x.Sn).ToList();
                }
                else
                {
                    menuMasterListView.MenuMasterList = ((List<MenuMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value).Where(x => x.ParentMenuId == menuMasterDTO.ParentMenuId).OrderBy(x => x.Sn).ToList();
                }
                menuMasterListView.MenuMasterListAll = ((List<MenuMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value).OrderBy(x => x.Sn).ToList();

            }
        }
        if (resModules != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resModules).StatusCode == 200)
            {
                menuMasterListView.ModuleMasterList = ((List<ModuleMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resModules).Value);
            }
        }
        return PartialView("_menuMaster/_list", menuMasterListView);
    }


    public async Task<IActionResult> DeleteMenuAndReturnPartialView([FromBody] MenuMasterDTO menuMasterDTO)
    {

        var delres = await _menuMasterAPIController.DeleteMenuMaster(menuMasterDTO.MenuId);

        MenuMasterListView menuMasterListView = new MenuMasterListView();
        var res = await _menuMasterAPIController.GetMenuMaster();
        var resModules = await _ModuleAPIController.GetAllModules();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                menuMasterListView.MenuMasterList = ((List<MenuMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value).Where(x => x.ParentMenuId == menuMasterDTO.ParentMenuId).OrderBy(x => x.Sn).ToList();
                menuMasterListView.MenuMasterListAll = ((List<MenuMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value).OrderBy(x => x.Sn).ToList();

            }
        }
        if (resModules != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resModules).StatusCode == 200)
            {
                menuMasterListView.ModuleMasterList = ((List<ModuleMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resModules).Value);
            }
        }
        return PartialView("_menuMaster/_list", menuMasterListView);
    }


    public async Task<IActionResult> AddMenusPartialView(int? MenuId = null)
    {
        MenuMasterListView menuMasterListView = new MenuMasterListView();

        if (MenuId != null)
        {
            var resMenu = await _menuMasterAPIController.GetMenuById(MenuId ?? default(int));
            if (resMenu != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resMenu).StatusCode == 200)
                {
                    menuMasterListView.MenuMaster = (MenuMasterDTO)((Microsoft.AspNetCore.Mvc.ObjectResult)resMenu).Value;
                }
            }
        }

        var res = await _menuMasterAPIController.GetMenuMaster(-1);
        var resModules = await _ModuleAPIController.GetAllModules();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                menuMasterListView.MenuMasterListAll = ((List<MenuMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value).OrderBy(x => x.Sn).ToList();
            }
        }
        if (resModules != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resModules).StatusCode == 200)
            {
                menuMasterListView.ModuleMasterList = ((List<ModuleMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resModules).Value);
            }
        }
        return View(menuMasterListView);
    }
    public async Task<IActionResult> SaveMenuMaster([FromBody] MenuMasterDTO inputDTO)
    {
        inputDTO.IsActive = 1;

        var res = await _menuMasterAPIController.SaveMenuMaster(inputDTO);

        return res;

    }

}
