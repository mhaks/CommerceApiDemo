namespace CommerceApiDemo.AdminDto
{
    public class InventorySummaryResponse
    {
        public int Active { get; set; }
        public int Inactive { get; set; }
        public int AboveThreshold { get; set; }
        public int BelowThreshold { get; set; }
        public int OutOfStock { get; set; }
    }
}
