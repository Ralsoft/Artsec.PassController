using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artsec.Carddex.DAL.FireBird.Dtos;

public class CardInDevGetListRow
{
    [Column("ID_DEV")]
    public int? DeviceId { get; set; }

    [Column("ID_CTRL")]
    public int? CtlrId { get; set; } = 1;

    [Column("ID_READER")]
    public int? ReaderId { get; set; }

    [Column("ID_CARD")]
    [StringLength(12)]
    public string? CardId { get; set; }

    [Column("DEVIDX")]
    public int? DeviceIdx { get; set; }

    [Column("OPERATION")]
    public int? Operation { get; set; }

    [Column("TIMEZONES")]
    public int? Timezones { get; set; }

    [Column("STATUS")]
    public int? Status { get; set; }

    [Column("ID_CARDINDEV")]
    public int? CardInDevId { get; set; }

    [Column("ATTEMPTS")]
    public int? Attempts { get; set; }

    [Column("ID_PEP")]
    public int? PeopleId { get; set; }
}
