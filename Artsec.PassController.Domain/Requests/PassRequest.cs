using Artsec.PassController.Domain.Enums;
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
    public PersonPassMode PassMode { get; set; }
    public DateTime CreationTime { get; } = DateTime.Now;

    public bool IsReadyToProcessing()
    {
        if (DateTime.Now - CreationTime > TimeSpan.FromSeconds(5))
            return true;


        Func<bool> validate = () => false;
        if (PassMode == PersonPassMode.RequaredRfid)
            validate = () => Rfid != string.Empty;
        else if (PassMode == PersonPassMode.RequaredRfidAndPersonFaceId)
            validate = () => Rfid != string.Empty && FaceId != string.Empty;
        else if (PassMode == PersonPassMode.RequaredRfidAndAnyFaceId)
            validate = () => Rfid != string.Empty && FaceId != string.Empty;
        else if (PassMode == PersonPassMode.RequaredFaceId)
            validate = () => FaceId != string.Empty;
        else if (PassMode == PersonPassMode.AnyIdentifier)
            validate = () => Rfid != string.Empty || FaceId != string.Empty;

        return validate.Invoke();
    }
}
