using Artsec.PassController.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Domain.Requests;

public class PassRequestWithMode : PassRequestWithPersonId
{
    public PersonPassMode PassMode { get; set; }
}
