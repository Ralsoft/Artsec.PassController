using System.Text;

namespace Artsec.PassController.Dal.Base;

public class DapperDbContextBase
{
    protected DapperDbContextBase()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ConfigureModel();
    }
    protected virtual void ConfigureModel()
    {
    }
    protected void AddModel<TModel>()
    {
        //SqlMapper.SetTypeMap(
        //    typeof(TModel),
        //    new CustomPropertyTypeMap(
        //        typeof(TModel),
        //        (type, columnName) =>
        //            type.GetProperties().FirstOrDefault(prop =>
        //                                                prop.GetCustomAttributes(false).OfType<ColumnAttribute>()
        //                                                .Any(attr => attr.Name == columnName))));
    }
}
