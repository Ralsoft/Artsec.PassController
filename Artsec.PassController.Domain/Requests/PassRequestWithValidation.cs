﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Domain.Requests;

public class PassRequestWithValidation : PassRequestWithMode
{
    public bool IsValid { get; set; }
}