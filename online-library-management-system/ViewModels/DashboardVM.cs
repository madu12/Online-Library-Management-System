namespace online_library_management_system.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalItems { get; set; }
        public int PendingReservations { get; set; }
        public int ExpiredReservationsCount { get; set; }
        public int IssuedCount { get; set; }
        public int ReturnedCount { get; set; }
        public List<OverdueItemVM>? OverdueItemsList { get; set; }
    }
}
