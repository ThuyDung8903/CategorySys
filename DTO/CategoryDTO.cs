namespace CategorySys.DTO
{
    public class CategoryDTO
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
