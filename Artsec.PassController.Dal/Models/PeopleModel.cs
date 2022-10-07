using System.ComponentModel.DataAnnotations.Schema;

namespace Artsec.PassController.Dal.Models;

public class PeopleModel
{
    [Column("ID_PEP")] public int PeopleId { get; set; }
    [Column("AUTHMODE")] public int? AuthMode { get; set; } = 5;
}
