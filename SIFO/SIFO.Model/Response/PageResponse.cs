using SIFO.Model.Entity;

namespace SIFO.Model.Response
{
    public class PageResponse
    {
        public long Id { get; set; }  
        public string PageName { get; set; } 
        public bool IsActive { get; set; }  
        public long? ParentPageId { get; set; }  
        public string MenuIcon { get; set; }  
        public string PageUrl { get; set; }  
        public string EventName { get; set; }
        public long? userRoleId { get; set; }
        public List<PageResponse> SubPages { get; set; }
    }
}

