using Amazon.Lambda.Core;
using TripFlow.Shared;
using TripFlow.Shared.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TripFlow.HotelService;

public class Function
{
    public TripState ReserveHotel(TripState state, ILambdaContext context)
    {
        context.Logger.LogInformation($"[Hotel] Processing reservation for Trip: {state.TripId}");

        // 1. CHAOS
        if (state.ChaosTarget != null && state.ChaosTarget.Equals("Hotel", StringComparison.OrdinalIgnoreCase))
        {
            context.Logger.LogError("[Hotel] 🔥 CHAOS INJECTED! Failing hotel reservation.");
            throw new Exception("Simulated Failure in Hotel Service");
        }

        // 2. Business Logic
        state.Hotel.ReservationId = "HTL-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        state.Hotel.Status = BookingStatus.Confirmed;
        state.Hotel.Price = 200.00m;

        context.Logger.LogInformation($"[Hotel] Reserved successfully. ID: {state.Hotel.ReservationId}");

        return state;
    }

    public TripState CancelHotel(TripState state, ILambdaContext context)
    {
        context.Logger.LogInformation($"[Hotel] ↩️ Compensating transaction for Trip: {state.TripId}");

        if (state.Hotel.Status == BookingStatus.Confirmed)
        {
            state.Hotel.Status = BookingStatus.Cancelled;
            state.Hotel.ErrorMessage = "Booking cancelled by Saga Orchestrator.";
            context.Logger.LogInformation($"[Hotel] Reservation {state.Hotel.ReservationId} cancelled.");
        }

        return state;
    }

}
