namespace online_library_management_system.ViewModels
{
    public class ReservationAdminVM
    {
        public List<ReservationVM> Reservations { get; set; } = new List<ReservationVM>();
        public string? Status{ get; set; }

    }
}
