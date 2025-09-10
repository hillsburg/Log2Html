using SqlSugar;
namespace Log2Html.Dao.Model
{
    [SugarTable("t_convert_entry")]
    class ConvertEntryEntity
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
        public string Id { get; set; }

        [SugarColumn(ColumnName = "file_name_alias")]
        public string FileNameAlias { get; set; }


        [SugarColumn(ColumnName = "original_file_path")]
        public string OriginalFilePath { get; set; }


        [SugarColumn(ColumnName = "converted_file_path")]
        public string ConvertedFilePath { get; set; }


        [SugarColumn(ColumnName = "convert_date")]
        public string ConvertDate { get; set; }
    }
}
