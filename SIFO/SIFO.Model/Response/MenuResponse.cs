

namespace SIFO.Model.Response
{
    public class MenuResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public long? ParentId { get; set; }
        public string Icon { get; set; }
        public string MenuUrl { get; set; }
        public int MenuOrder { get; set; }
        public bool IsTitleMenu { get; set; }
        public long? TotalCount { get; set; }
    }
}
