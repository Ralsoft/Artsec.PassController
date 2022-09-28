using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artsec.Carddex.DAL.FireBird.Dtos;
public class DeviceRow
{
    [Column("ID_DEV")]
    public int Id { get; set; }

    [Column("ID_DB")]
    public int DbId { get; set; } = 1;

    [Column("ID_SERVER")]
    public int? ServerId { get; set; }

    [Column("ID_DEVTYPE")]
    public int DeviceTypeId { get; set; } = 1;

    [Column("ID_CTRL")]
    public int CtlrId { get; set; } = 1;

    [Column("ID_READER")]
    public int? ReaderId { get; set; }

    [Column("NETADDR")]
    [StringLength(23)]
    public string? NetAddress { get; set; }

    [Column("NAME")]
    [StringLength(50)]
    public string Name { get; set; }

    [Column("VERSION")]
    public int Version { get; set; }

    [Column("INTERVAL")]
    public int? Interval { get; set; } = 1000;

    [Column("DSS1")]
    public int? Dss1 { get; set; } = 1;

    [Column("DSS2")]
    public int? Dss2 { get; set; } = 1;

    [Column("FLAG")]
    public int Flag { get; set; }

    [Column("ID_PLAN")]
    public int? PlanId { get; set; }

    [Column("POS_X")]
    public int PositionX { get; set; }

    [Column("POS_Y")]
    public int PositionY { get; set; }

    [Column("PSW")]
    [StringLength(12)]
    public string? Password { get; set; }

    [Column("ACTIVE")]
    public int Active { get; set; } = 1;

    [Column("CONFIG")]
    [StringLength(1024)]
    public string? Config { get; set; }

    [Column("PARAM")]
    [StringLength(1024)]
    public string? Param { get; set; }

    [Column("TAGNAME")]
    [StringLength(1024)]
    public string? TagName { get; set; }

    [Column("ID_GUIDE")]
    public int? GuideId { get; set; }

    [Column("ID_OBJECT")]
    public int? ObjectId { get; set; }

    [Column("ID_PARENT")]
    public int? ParentId { get; set; } = 1;
}
