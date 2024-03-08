using HeartLog.SimpleDbTool.Attribute;
using HeartLog.SimpleDbTool.Enum;
using HeartLog.SimpleDbTool.Interface;

namespace Log2Html.Dao.Model
{
    [Table("t_convert_entry")]
    class ConvertEntryEntity : ISimpleOrm
    {
        [Column("id", false, true, ColumnType.TEXT)]
        public string Id { get; set; }

        [Column("original_file_path", ColumnType.TEXT)]
        public string OriginalFilePath { get; set; }

        [Column("converted_file_path", ColumnType.TEXT)]
        public string ConvertedFilePath { get; set; }

        [Column("convert_date", ColumnType.TEXT)]
        public string ConvertDate { get; set; }
    }
}
