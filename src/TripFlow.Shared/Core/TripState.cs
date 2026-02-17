using System;
using System.Collections.Generic;
using System.Text;

namespace TripFlow.Shared.Core
{
    public class TripState
    {
        // ID Saga transction
        public string TripId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        // Where we gonna inject the fail demonstration
        public string ChaosTarget { get; set; } = "None";
        public BookingDetails Flight { get; set; } = new BookingDetails();
        public BookingDetails Hotel { get; set; } = new BookingDetails();
        public BookingDetails Car { get; set; } = new BookingDetails();
    }

    public class BookingDetails
    {
        public string ReservationId { get; set; } = string.Empty;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public decimal Price { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
