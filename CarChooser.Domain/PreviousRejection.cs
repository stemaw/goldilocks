namespace CarChooser.Domain
{
    public class PreviousRejection
    {
        public long CarId { get; set; }
        public RejectionReasons Reason { get; set; }
    }
}