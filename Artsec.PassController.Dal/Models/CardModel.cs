using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Artsec.PassController.Dal.Models;

public class CardModel
{
    [StringLength(32)]
    [Column("ID_CARD")] public string CardId { get; set; } = string.Empty;

    [Column("ID_CARDTYPE")] public int CardTypeId { get; set; } = 1;
    [Column("ID_PEP")] public int PeopleId { get; set; }
    public PeopleModel? People { get; set; }
}
