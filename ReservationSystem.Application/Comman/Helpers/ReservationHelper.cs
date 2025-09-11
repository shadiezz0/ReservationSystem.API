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
                //UserName = reservation.User?.Name ?? string.Empty,
                Status = reservation.Status,
                IsAvailable = reservation.IsAvailable,
                TotalPrice = reservation.TotalPrice
            };
        }

        public static List<ReservationDto> MapToReservationDtoList(IEnumerable<Reservation> reservations)
        {
            return reservations.Select(MapToReservationDto).ToList();
        }

        public static async Task<ResponseResult?> ValidateReservationDto(CreateReservationDto dto, IGenericRepository<Reservation> reservationRepository)
        {
            var userId = await ResponseHelper.GetCurrentUserId();

            // Validate range time
            var reservations = await reservationRepository.AsNoTracking().Where(a => a.UserId == userId)
            .Where(r => r.ReservationDate.Date == dto.ReservationDate.Date
                && ((r.StartTime == dto.StartTime && r.EndTime == dto.EndTime) ||
                    (dto.StartTime < r.EndTime && dto.EndTime > r.StartTime))
                && r.IsAvailable == false
                && r.ItemId == dto.ItemId
                && r.Status != Status.Cancelled)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
        
                // add only what you need (no navigation properties)
            })
            .ToListAsync();

            if (reservations.Any())
                return ResponseHelper.Warning("الحجز غير متاح حالياً", "Reservation is Currently not available.");


            // Validate end time is after start time
            if (dto.EndTime <= dto.StartTime)
                return ResponseHelper.Failed("وقت انتهاء الحجز يجب أن يكون بعد وقت البداية.", "End time must be after start time.");


            // Validate reservation is not in the past
            var reservationDateTime = dto.ReservationDate.Add(dto.StartTime);
            if (reservationDateTime.Date < DateTime.Now.Date)
                return ResponseHelper.Failed("لا يمكن إنشاء حجز في الماضي.", "Cannot create reservation in the past.");

            return null;
        }

    }
}
