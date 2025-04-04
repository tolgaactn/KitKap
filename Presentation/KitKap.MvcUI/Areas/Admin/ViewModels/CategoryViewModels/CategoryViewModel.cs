namespace KitKap.MvcUI.Areas.Admin.ViewModels.CategoryViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
