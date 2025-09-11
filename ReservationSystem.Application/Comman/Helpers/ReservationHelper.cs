namespace ReservationSystem.Application.Comman.Helpers
{
    public static class ReservationHelper
    {
        public static double CalculateTotalPrice(double pricePerHour, TimeSpan startTime, TimeSpan endTime)
        {
            TimeSpan duration = endTime - startTime;
            double totalHours = duration.TotalHours;
            return totalHours * pricePerHour;
        }

        public static ReservationDto MapToReservationDto(Reservation reservation)
        {
            return new ReservationDto
            {
                Id = reservation.Id,
                ReservationDate = reservation.ReservationDate,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                ItemName = reservation.Item?.Name ?? string.Empty,
                UserName = reservation.User?.Name ?? string.Empty,
                Status = reservation.Status,
                IsAvailable = reservation.IsAvailable,
                TotalPrice = reservation.TotalPrice
            };
        }

        public static List<ReservationDto> MapToReservationDtoList(IEnumerable<Reservation> reservations)
        {
            return reservations.Select(MapToReservationDto).ToList();
        }

    }
}
