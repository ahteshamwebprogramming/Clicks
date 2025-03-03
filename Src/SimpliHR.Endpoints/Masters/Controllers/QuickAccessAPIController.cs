using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Collections.Generic;
using System.Data;

namespace Masters.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuickAccessAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuickAccessAPIController> _logger;
        private readonly IMapper _mapper;

        public QuickAccessAPIController(IUnitOfWork unitOfWork, ILogger<QuickAccessAPIController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<QuickAccessUnitListDTO> GetQuickAccessDetails(int? unitId)
        {
            QuickAccessUnitListDTO QuickAccessList = new QuickAccessUnitListDTO();
            var parms = new DynamicParameters();
            parms.Add(@"UnitId", unitId, DbType.Int32);

            try
            {

                QuickAccessList.QuickAccessUnitLists = _mapper.Map<List<QuickAccessUnitListDTO>>(await _unitOfWork.QuickAccessUnitList.GetSPData("usp_GetQuickAccessList", parms));
                return QuickAccessList;
            }
            catch (SystemException ex)
            {
                return null;
            }

        }


        public async Task<QuickAccessUnitListDTO> GetQuickAccessUnitDetails(int? unitId)
        {
            QuickAccessUnitListDTO QuickAccessList = new QuickAccessUnitListDTO();
            var parms = new DynamicParameters();
            parms.Add(@"UnitId", unitId, DbType.Int32);

            try
            {
                 QuickAccessList.QuickAccessUnitLists = _mapper.Map<List<QuickAccessUnitListDTO>>(await _unitOfWork.QuickAccessUnitList.GetSPData("usp_GetUnitQuickAccessList", parms));
                return QuickAccessList;
            }
            catch (SystemException ex)
            {
                return null;
            }

        }

        [HttpPost]
        public async Task<string> SaveQuickAccessUnitLink(QuickAccessAction userAction)
        {
            try
            {
                var parms = new DynamicParameters();
                parms.Add(@"@QuickAccessIds", userAction.QuickAccessIds, DbType.String);
                parms.Add(@"@PostionIds", userAction.PositionIds, DbType.String);
                parms.Add(@"@Links", userAction.Links, DbType.String);
                parms.Add(@"@IsActives", userAction.IsActives, DbType.String);
                parms.Add(@"@UnitId", userAction.UnitId, DbType.Int32);

                try
                {
                    await _unitOfWork.QuickAccessUnitList.GetStoredProcedure("usp_SaveQuickAccessLinks", parms);
                }
                catch (Exception ex) { return "Error while saving links."; }
                try
                {
                    string returnMessage = "SUCCESS"; // await _unitOfWork.ManualPunches.SendApprovalMail(userAction);

                    return returnMessage;

                }
                catch (Exception ex) { return "Error while sending mail to user."; }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in while saving links {nameof(SaveQuickAccessUnitLink)}");
                throw;
            }
        }
    }
}
