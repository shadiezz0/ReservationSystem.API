namespace ReservationSystem.Application.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IGenericRepository<Reservation> _reservation;
        private readonly IGenericRepository<Item> _itemRepo;
        private readonly IUnitOfWork _uow;
        
        public ReservationService(IUnitOfWork uow)
        {
            _reservation = uow.Repository<Reservation>();
            _itemRepo = uow.Repository<Item>();
            _uow = uow;
        }

        // Create Reservation
        public async Task<ResponseResult> CreateAsync(CreateReservationDto dto)
        {
            var validationResult = await ReservationHelper.ValidateReservationDto(dto, _reservation);
            if (validationResult != null)
                return validationResult;


            var item = await _itemRepo.GetByIdAsync(dto.ItemId);
            if (item == null || item.ItemTypeId == 0)
               return ResponseHelper.Warning("العنصر غير موجود.", "Item not found.");

            var userId = ResponseHelper.GetCurrentUserId();

            var reservation = new Reservation
            {
                ReservationDate = dto.ReservationDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                ItemId = dto.ItemId,
                UserId = userId,
                IsAvailable = false,
                Status = Status.Pending,
                TotalPrice = ReservationHelper.CalculateTotalPrice(item.PricePerHour, dto.StartTime, dto.EndTime),
                ItemTypeId = item.ItemTypeId
            };
            await _reservation.AddAsync(reservation);
            var save = await _uow.SaveAsync();
            if (save)
            {
                 return ResponseHelper.Success("تم إنشاء الحجز بنجاح.", "Reservation created successfully.");
            }
            return ResponseHelper.Failed("لم يتم إنشاء الحجز بنجاح.", "Reservation Not created successfully.");
        }


        //  Delete Reservation
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            var res = await _reservation.GetByIdAsync(id);
            if (res == null)
               return ResponseHelper.Warning("الحجز غير موجود.", "Reservation not found.");
            _reservation.Delete(res);
            await _uow.SaveAsync();

            return ResponseHelper.Success("تم حذف الحجز بنجاح.", "Reservation deleted successfully.");
        }


        //  View All Reservations
        public async Task<ResponseResult> GetAllAsync()
        {
            var data = await _reservation.GetAllAsync(
                        include: q => q.Include(r => r.Item).Include(r => r.User),
                        asNoTracking: true
                    );
            
            if (!data.Any())
            {
                return ResponseHelper.Warning("الحجز غير موجود.", "Reservation not found.");
            }

            var result = ReservationHelper.MapToReservationDtoList(data);

            return ResponseHelper.Success("تم جلب جميع الحجوزات بنجاح.", "All reservations retrieved successfully.");
        }


        // View Reservation Details
        public async Task<ResponseResult> GetByIdAsync(int id)
        {
            var res = await _reservation.GetByIdAsync(
                            id,
                            include: q => q.Include(r => r.Item).Include(r => r.User),
                            asNoTracking: true
                        );

            if (res == null)
              return ResponseHelper.Warning("الحجز غير موجود.", "Reservation not found.");

            var result = ReservationHelper.MapToReservationDto(res);
                return ResponseHelper.Success("تم جلب تفاصيل الحجز بنجاح.", "Reservation details retrieved successfully.");
        }


        // Update Reservation
        public async Task<ResponseResult> UpdateAsync(UpdateReservationDto dto)
        {
            // Validate input data
            var validationResult = await ReservationHelper.ValidateReservationDto(dto , _reservation);
            if (validationResult != null)
                return validationResult;

            var item = await _itemRepo.GetByIdAsync(dto.ItemId);
            if (item == null)
                return ResponseHelper.Warning("العنصر غير موجود.", "Item not found.");

            var res = await _reservation.GetByIdAsync(dto.Id);
            if (res == null)
               return ResponseHelper.Warning("الحجز غير موجود.", "Reservation not found.");


            res.ReservationDate = dto.ReservationDate;
            res.StartTime = dto.StartTime;
            res.EndTime = dto.EndTime;
            res.ItemTypeId = dto.ItemTypeId;
            res.ItemId = dto.ItemId;
            res.TotalPrice = ReservationHelper.CalculateTotalPrice(item.PricePerHour, dto.StartTime, dto.EndTime);
            
            _reservation.Update(res);
            var save = await _uow.SaveAsync();
            if (save)
                return ResponseHelper.Success("تم تحديث الحجز بنجاح." , "Reservation updated successfully.");
            
            return ResponseHelper.Failed("لم يتم تحديث الحجز بنجاح.", "Reservation not updated successfully.");

        }
        

        // View Personal Reservations
        public async Task<ResponseResult> GetByUserIdAsync(int userId)
        {
            var reservations = await _reservation.FindAllAsync(
                        predicate: r => r.UserId == userId,
                        include: q => q.Include(r => r.Item).Include(r => r.User),
                        asNoTracking: true
                    );

            if (reservations == null || !reservations.Any())
                return ResponseHelper.Failed("لا توجد حجوزات لهذا المستخدم.", "No reservations found for this user.");

            var result = ReservationHelper.MapToReservationDtoList(reservations);
                return ResponseHelper.Success("تم جلب حجوزات المستخدم بنجاح.", "User reservations retrieved successfully.");
        }


        // Confirm Pending Reservation
        public async Task<ResponseResult> ConfirmReservationAsync(int id)
        {
            var res = await _reservation.GetByIdAsync(id);
            if (res == null)
              return ResponseHelper.Warning("الحجز غير موجود.", "Reservation not found.");

            if (res.Status != Status.Pending)
                return ResponseHelper.Failed("لا يمكن تأكيد الحجز إلا إذا كان في حالة انتظار.",
                "Reservation can only be confirmed if it is in pending status.");

            res.Status = Status.Confirmed;
            _reservation.Update(res);
            await _uow.SaveAsync();

            return ResponseHelper.Success("تم تأكيد الحجز بنجاح.", "Reservation confirmed successfully.");
        }


        // Cancel Reservation
        public async Task<ResponseResult> CancelReservationAsync(int id)
        {
            var res = await _reservation.GetByIdAsync(id);
            if (res == null)
            return ResponseHelper.Warning("الحجز غير موجود.", "Reservation not found.");

            if (res.Status == Status.Cancelled)
            return ResponseHelper.Warning("لا يمكن إلغاء الحجز بعد تأكيده.",
                "Cannot cancel reservation after it has been confirmed.");

            res.Status = Status.Cancelled;
            res.IsAvailable = true;

            _reservation.Update(res);
            await _uow.SaveAsync();

            return ResponseHelper.Success("تم إلغاء الحجز بنجاح.", "Reservation cancelled successfully.");
        }


        //Filter By Date
        public async Task<ResponseResult> FilterByDateAsync(FilterReservationDto dto)
        {
            //var userId = ResponseHelper.GetCurrentUserId();

            // Start with all reservations
            var filteredReservations =  _reservation.AsNoTracking()/*.Where(a=>a.UserId == userId)*/.Include(a=>a.Item)
               .AsQueryable();
            
            // Date filtering
            if (dto.FromDate != default && dto.ToDate != default)
            {
                filteredReservations = filteredReservations.Where(r => r.ReservationDate >= dto.FromDate && r.ReservationDate <= dto.ToDate);
            }
            else if (dto.FromDate != default)
            {
                filteredReservations = filteredReservations.Where(r => r.ReservationDate >= dto.FromDate);
            }
            else if (dto.ToDate != default)
            {
                filteredReservations = filteredReservations.Where(r => r.ReservationDate <= dto.ToDate);
            }

            // Item filters
            if (dto.ItemId > 0)
                filteredReservations = filteredReservations.Where(r => r.ItemId == dto.ItemId);

            if (dto.ItemTypeId > 0)
                filteredReservations = filteredReservations.Where(r => r.ItemTypeId == dto.ItemTypeId);

            // Time filters
            if (dto.StartTime.HasValue)
                filteredReservations = filteredReservations.Where(r => r.StartTime >= dto.StartTime.Value);

            if (dto.EndTime.HasValue)
                filteredReservations = filteredReservations.Where(r => r.EndTime <= dto.EndTime.Value);

            // Status
            if (dto.Status > 0)
                filteredReservations = filteredReservations.Where(r => r.Status == dto.Status);
            
            //// User
            //if (dto.UserId.HasValue && dto.UserId.Value > 0)
            //    filteredReservations = filteredReservations.Where(r => r.UserId == dto.UserId.Value);
           
            // Availability
            filteredReservations = filteredReservations.Where(r => r.IsAvailable == dto.IsAvailable);

            var finalResults = filteredReservations.ToList();

            if (!finalResults.Any())
                    return ResponseHelper.Warning("لا توجد حجوزات تطابق معايير البحث.", "No reservations found matching the search criteria.");

            var reslut = ReservationHelper.MapToReservationDtoList(finalResults);

            return ResponseHelper.Success("تم جلب الحجوزات بنجاح.", "Reservations retrieved successfully.");
        }
    }
}
