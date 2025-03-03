using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.ITDeclaration;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.WebUI.Modals.ITDeclarations;
using System.Collections.Generic;

namespace SimpliHR.WebUI.Controllers.Masters;

public class Investments80CController : Controller
{
    private readonly Investments80CAPIController _investments80CAPIController;
    public Investments80CController(Investments80CAPIController investments80CAPIController)
    {
        _investments80CAPIController = investments80CAPIController;
    }

   
}
