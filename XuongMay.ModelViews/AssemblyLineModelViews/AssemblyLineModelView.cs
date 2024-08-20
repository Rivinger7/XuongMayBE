using System.Text.Json.Serialization;

namespace XuongMay.ModelViews.AssemblyLineModelView
{
    public class AssemblyLineModelView
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int ManagerID { get; set; }
        public required string ManagerFullName { get; set; }
        public required int NumberOfStaffs { get; set; }
        public required string CreatedBy { get; set; }
        public required string CreatedTime { get; set; }
        public string? LastUpdatedTime { get; set; }
        public string? DeletedTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
