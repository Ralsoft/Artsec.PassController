using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.Carddex.DAL.FireBird.Dtos;

public class CardInDevRow
{
    [Column("ID_CARDINDEV")]
    public int Id { get; set; }

    [Column("ID_DB")]
    public int DbId { get; set; }

    [Column("ID_CARD")]
    [StringLength(12)]
    public string? CardId { get; set; }

    [Column("DEVIDX")]
    public int? DeviceIdx { get; set; }

    [Column("ID_DEV")]
    public int? Timezones { get; set; }

    [Column("OPERATION")]
    public int? Operation { get; set; }

    [Column("ATTEMPTS")]
    public int Attempts { get; set; } = 0;

    [Column("ID_PEP")]
    public int PeopleId { get; set; }
}
