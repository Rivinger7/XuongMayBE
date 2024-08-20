namespace XuongMay.ModelViews.AssemblyLineModelViews
{
    public class AssemblyLineUpdateModel
    {
        public required int ManagerID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int NumberOfStaffs { get; set; }
    }
}
