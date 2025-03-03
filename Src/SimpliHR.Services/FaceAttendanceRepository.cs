using SimpliHR.Services.DBContext;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Helper;
using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using SimpliHR.Infrastructure.Models.Employee;
using Microsoft.AspNetCore.Mvc;
using static System.Collections.Specialized.BitVector32;
using SimpliHR.Infrastructure.Models.KeyValueModels;

namespace SimpliHR.Services;

public class FaceAttendanceRepository : DapperGenericRepository<FaceAttendance>, IFaceAttendanceRepository
{
    public FaceAttendanceRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
       
    }
}


