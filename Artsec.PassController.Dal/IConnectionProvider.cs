﻿using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Dal;

public interface IConnectionProvider
{
    FbConnection CreateConnection();
}
