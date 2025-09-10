using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ReservationSystem.Application.Comman.Helpers;

namespace ReservationSystem.Application.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IGenericRepository<Reservation> _reservation;
        private readonly IGenericRepository<Item> _itemRepo;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public ReservationService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
        {
            _reservation = uow.Repository<Reservation>();
            _itemRepo = uow.Repository<Item>();
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
        }

        // Create Reservation
        public async Task<ResponseResult> CreateAsync(CreateReservationDto dto)
        {
            var validationResult = await ValidateReservationDto(dto);
            if (validationResult != null)
                return validationResult;


            var item = await _itemRepo.GetByIdAsync(dto.ItemId);
            if (item == null || item.ItemTypeId != dto.ItemTypeId)
               return ResponseHelper.Warning("العنصر غير موجود.", "Item not found.");

            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var reservation = new Reservation
            {
                ReservationDate = dto.ReservationDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                ItemId = dto.ItemId,
                UserId = int.Parse(userIdString),
                IsAvailable = false,
                Status = Status.Pending,
                TotalPrice = item.PricePerHour * (dto.EndTime - dto.StartTime).TotalHours, // Assuming TotalPrice is calculated based on hours
                ItemTypeId =dto.ItemTypeId
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

            var result = data.Select(r => new ReservationDto
            {
                Id = r.Id,
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                ItemName = r.Item.Name,
                UserName = r.User.Name,
                Status = r.Status,
                TotalPrice = r.TotalPrice
            }).ToList();
            
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
            var result = new ReservationDto
            {
                Id = res.Id,
                ReservationDate = res.ReservationDate,
                StartTime = res.StartTime,
                EndTime = res.EndTime,
                ItemName = res.Item.Name,
                UserName = res.User.Name,
                Status = res.Status,
                TotalPrice = res.TotalPrice
            };
;
            return ResponseHelper.Success("تم جلب تفاصيل الحجز بنجاح.", "Reservation details retrieved successfully.");
        }

        // Update Reservation
        public async Task<ResponseResult> UpdateAsync(UpdateReservationDto dto)
        {
            // Validate input data
            var validationResult = await ValidateReservationDto(dto);
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
            res.TotalPrice = item.PricePerHour * (dto.EndTime - dto.StartTime).TotalHours;
            
            _reservation.Update(res);
            var save = await _uow.SaveAsync();
            if (save)
            {
                return ResponseHelper.Success("تم تحديث الحجز بنجاح." , "Reservation updated successfully.");

            }

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

            var result = reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                ItemName = r.Item.Name,
                UserName = r.User.Name,
                Status = r.Status,
                TotalPrice = r.TotalPrice
            }).ToList();

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
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "الحجز غير موجود.",
                        MessageEn = "Reservation not found.",
                    }
                };

            if (res.Status == Status.Cancelled)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "هذا الحجز تم إلغاءه بالفعل.",
                        MessageEn = "This reservation has already been cancelled.",
                    }
                };

            res.Status = Status.Cancelled;
            // Make time slot available again when cancelled
            res.IsAvailable = true;
            _reservation.Update(res);
            await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم إلغاء الحجز بنجاح.",
                    MessageEn = "Reservation cancelled successfully.",
                }
            };
        }

        //Filter By Date
        public async Task<ResponseResult> FilterByDateAsync(FilterReservationDto dto)
        {
            // Start with all reservations
            
            var filteredReservations =  _reservation.AsNoTracking().Include(a=>a.Item)
                .Include(r => r.User).AsQueryable();
            

            // Apply date filtering
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

        

            var result = finalResults.Select(r => new ReservationDto
            {
                Id = r.Id,
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                ItemName = r.Item.Name,
                UserName = r.User.Name,
                Status = r.Status,
                IsAvailable = r.IsAvailable,
                TotalPrice = r.TotalPrice
            }).ToList();

            return ResponseHelper.Success("تم جلب الحجوزات بنجاح.", "Reservations retrieved successfully.");

        }

        // Helper method for validation
        private async Task<ResponseResult> ValidateReservationDto(CreateReservationDto dto)
        {
            // Validate range time
            var reservations = await _reservation.AsNoTracking()
                .Where(r => r.ReservationDate == dto.ReservationDate
                && ((r.StartTime == dto.StartTime && r.EndTime == dto.EndTime) ||
                    (dto.StartTime < r.EndTime && dto.EndTime > r.StartTime))
                && r.IsAvailable == false
                && r.ItemId == dto.ItemId
                && r.Status != Status.Cancelled).ToListAsync();

             if (reservations.Any())
                return ResponseHelper.Warning("الحجز غير متاح حالياً", "Reservation is Currently not available.");
            // Validate end time is after start time
            if (dto.EndTime <= dto.StartTime)
            {
                return ResponseHelper.Failed("وقت انتهاء الحجز يجب أن يكون بعد وقت البداية.",
                    "End time must be after start time.");
            }

            // Validate reservation is not in the past
            var reservationDateTime = dto.ReservationDate.Add(dto.StartTime);
            if (reservationDateTime.Date < DateTime.Now.Date)
            {
                return ResponseHelper.Failed("لا يمكن إنشاء حجز في الماضي.",
                  "Cannot create reservation in the past.");
            }

            return null; 
        }
    }
}
