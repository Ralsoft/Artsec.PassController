using System.ComponentModel.DataAnnotations.Schema;

namespace Artsec.PassController.Dal.Models;

public class DeviceModel
{
    [Column("ID_DEV")] public int Id { get; set; }
    [Column("ID_CTRL")] public int ControllerId { get; set; }
}
