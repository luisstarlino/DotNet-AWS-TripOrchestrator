using System;
using System.Collections.Generic;
using System.Text;

namespace TripFlow.Shared
{
    public enum BookingStatus
    {
        Pending,    // --- not proccess
        Confirmed,  // --- successSucesso
        Failed,     // --- Failed trying to reservFalhou na tentativa de reservar
        Cancelled   // --- Rollback done
    }

    public enum ChaosTarget
    {
        None,
        Flight,
        Hotel,
        Car
    }
}
