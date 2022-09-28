using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Domain.Requests;

public class PassRequest
{
    public string Rfid { get; set; } = string.Empty;
    public string FaceId { get; set; } = string.Empty;
}
