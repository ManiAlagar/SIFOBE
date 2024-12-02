namespace SIFO.Model.Response
{
    public class PageResponse
    {
        public long Id { get; set; }
        public string PageName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public long? ParentPageId { get; set; }
        public string MenuIcon { get; set; }
        public string PageUrl { get; set; }
    }
}

