namespace CarChooser.Domain
{
    public class PreviousRejection
    {
        public int CarId { get; set; }
        public RejectionReasons Reason { get; set; }
    }
}