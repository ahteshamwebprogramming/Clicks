﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Repository;

public interface IDapperDbStrategy
{
    IDbConnection GetConnection(string connectionString);
}
