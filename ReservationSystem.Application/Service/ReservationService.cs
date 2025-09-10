using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            // Validate end time is after start time
            if (dto.EndTime <= dto.StartTime)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "وقت انتهاء الحجز يجب أن يكون بعد وقت البداية.",
                        MessageEn = "End time must be after start time.",
                    }
                };
            }

            // Validate reservation is not in the past
            var reservationDateTime = dto.ReservationDate.Add(dto.StartTime);
            if (reservationDateTime <= DateTime.Now)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "لا يمكن إنشاء حجز في الماضي.",
                        MessageEn = "Cannot create reservation in the past.",
                    }
                };
            }

            var item = await _itemRepo.GetByIdAsync(dto.ItemId);
            if (item == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "العنصر غير موجود.",
                        MessageEn = "Item not found.",
                    }
                };

            // Check availability (Double Booking Prevention)
            var CheckIsAvailable = await FilterByIsAvilableAsync(dto);
            if (!CheckIsAvailable)
            {
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "الحجز غير متاح حالياً",
                        MessageEn = "Reservation is Currently not available.",
                    }
                };
            }
            
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
                return new ResponseResult
                {
                    Result = Result.Success,
                    Alart = new Alart
                    {
                        AlartType = AlartType.success,
                        type = AlartShow.note,
                        MessageAr = "تم إنشاء الحجز بنجاح.",
                        MessageEn = "Reservation created successfully.",
                    }
                };
            }
            return new ResponseResult
            {
                Result = Result.Failed,
                Alart = new Alart
                {
                    AlartType = AlartType.error,
                    type = AlartShow.note,
                    MessageAr = "لم يتم إنشاء الحجز بنجاح.",
                    MessageEn = "Reservation Not created successfully.",
                }
            };
        }

        //  Delete Reservation
        public async Task<ResponseResult> DeleteAsync(int id)
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
            _reservation.Delete(res);
            await _uow.SaveAsync();
            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم حذف الحجز بنجاح.",
                    MessageEn = "Reservation deleted successfully.",
                }
            };
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
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "لا توجد حجوزات.",
                        MessageEn = "No reservations found.",
                    }
                };
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
            
            return new ResponseResult
            {
                Data = result,
                Result = Result.Success,
                TotalCount = result.Count,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب جميع الحجوزات بنجاح.",
                    MessageEn = "All reservations retrieved successfully.",
                }
            };
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
            return new ResponseResult
            {
                Data = result,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب تفاصيل الحجز بنجاح.",
                    MessageEn = "Reservation details retrieved successfully.",
                }
            };
        }

        // Update Reservation
        public async Task<ResponseResult> UpdateAsync(UpdateReservationDto dto)
        {
            // Validate input data
            var validationResult = ValidateReservationDto(dto);
            if (validationResult != null)
                return validationResult;

            var item = await _itemRepo.GetByIdAsync(dto.ItemId);
            if (item == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "العنصر غير موجود.",
                        MessageEn = "Item not found.",
                    }
                };

            var res = await _reservation.GetByIdAsync(dto.Id);
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

            // Check if date/time changed and validate availability
            bool timeChanged = res.ReservationDate != dto.ReservationDate || 
                              res.StartTime != dto.StartTime || 
                              res.EndTime != dto.EndTime ||
                              res.ItemId != dto.ItemId;
            
            if (timeChanged)
            {
                var createDto = new CreateReservationDto
                {
                    ReservationDate = dto.ReservationDate,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    ItemId = dto.ItemId,
                    ItemTypeId = dto.ItemTypeId
                };
                
                var isAvailable = await FilterByIsAvilableAsync(createDto);
                if (!isAvailable)
                {
                    return new ResponseResult
                    {
                        Result = Result.NoDataFound,
                        Alart = new Alart
                        {
                            AlartType = AlartType.warning,
                            type = AlartShow.note,
                            MessageAr = "الوقت المحدد غير متاح للحجز.",
                            MessageEn = "The selected time slot is not available.",
                        }
                    };
                }
            }

            res.ReservationDate = dto.ReservationDate;
            res.StartTime = dto.StartTime;
            res.EndTime = dto.EndTime;
            res.Status = dto.Status;
            res.ItemTypeId = dto.ItemTypeId;
            res.ItemId = dto.ItemId;
            res.TotalPrice = item.PricePerHour * (dto.EndTime - dto.StartTime).TotalHours;
            
            _reservation.Update(res);
            var save = await _uow.SaveAsync();
            if (save)
            {
                return new ResponseResult
                {
                    Result = Result.Success,
                    Alart = new Alart
                    {
                        AlartType = AlartType.success,
                        type = AlartShow.note,
                        MessageAr = "تم تحديث الحجز بنجاح.",
                        MessageEn = "Reservation updated successfully.",
                    }
                };
            }
            return new ResponseResult
            {
                Result = Result.Failed,
                Alart = new Alart
                {
                    AlartType = AlartType.error,
                    type = AlartShow.note,
                    MessageAr = "لم يتم تحديث الحجز بنجاح.",
                    MessageEn = "Reservation not updated successfully.",
                }
            };
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
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "لا توجد حجوزات لهذا المستخدم.",
                        MessageEn = "No reservations found for this user.",
                    }
                };
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
            return new ResponseResult
            {
                Data = result,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب حجوزات المستخدم بنجاح.",
                    MessageEn = "User reservations retrieved successfully.",
                }
            };
        }

        // Confirm Pending Reservation
        public async Task<ResponseResult> ConfirmReservationAsync(int id)
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

            if (res.Status != Status.Pending)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "لا يمكن تأكيد الحجز إلا إذا كان في حالة انتظار.",
                        MessageEn = "Reservation can only be confirmed if it is in pending status.",
                    }
                };

            res.Status = Status.Confirmed;
            _reservation.Update(res);
            await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم تأكيد الحجز بنجاح.",
                    MessageEn = "Reservation confirmed successfully.",
                }
            };
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

        // Check Item Availability  & Double Booking Prevention
        public async Task<bool> FilterByIsAvilableAsync(CreateReservationDto dto)
        {
            var reservations = await _reservation.FindAllAsync(
                r => r.ReservationDate == dto.ReservationDate 
                    && ((r.StartTime == dto.StartTime && r.EndTime == dto.EndTime) || 
                        (dto.StartTime < r.EndTime && dto.EndTime > r.StartTime))
                    && r.IsAvailable == false 
                    && r.ItemId == dto.ItemId 
                    && r.Status != Status.Cancelled,
                asNoTracking: true
            );
            
            return !reservations.Any();
        }

        // Filter Reservations by Date Range
        //public async Task<ResponseResult> FilterByDateAsync(FilterReservationDto dto)
        //{
        //    // Start with all reservations and apply filters progressively
        //    var filteredReservations = await _reservation.GetAllAsync(
        //        include: q => q.Include(r => r.Item).Include(r => r.User),
        //        asNoTracking: true
        //    );

          
        //    // Date filtering - support both range and exact date
        //    if (dto.FromDate != default && dto.ToDate != default)
        //    {
        //        filteredReservations = filteredReservations.Where(r => 
        //            r.ReservationDate <= dto.FromDate && r.ReservationDate >= dto.ToDate);
        //    }

        //    // Item filtering
        //    if (dto.ItemId > 0)
        //    {
        //        filteredReservations = filteredReservations.Where(r => r.ItemId == dto.ItemId);
        //    }

        //    // Additional filters can be added here based on enhanced FilterReservationDto
        //    // Apply availability filter (only show unavailable reservations)
        //    filteredReservations = filteredReservations.Where(r => r.IsAvailable == false);

        //    var finalResults = filteredReservations.ToList();

        //    if (!finalResults.Any())
        //        return new ResponseResult
        //        {
        //            Result = Result.NoDataFound,
        //            Alart = new Alart
        //            {
        //                AlartType = AlartType.warning,
        //                type = AlartShow.note,
        //                MessageAr = "لا توجد حجوزات في هذا النطاق الزمني.",
        //                MessageEn = "No reservations found in this date range.",
        //            }
        //        };

        //    var result = finalResults.Select(r => new ReservationDto
        //    {
        //        Id = r.Id,
        //        ReservationDate = r.ReservationDate,
        //        StartTime = r.StartTime,
        //        EndTime = r.EndTime,
        //        ItemName = r.Item.Name,
        //        UserName = r.User.Name,
        //        Status = r.Status,
        //        TotalPrice = r.TotalPrice
        //    }).ToList();
            
        //    return new ResponseResult
        //    {
        //        Data = result,
        //        Result = Result.Success,
        //        TotalCount = result.Count,
        //        Alart = new Alart
        //        {
        //            AlartType = AlartType.success,
        //            type = AlartShow.note,
        //            MessageAr = "تم جلب الحجوزات بنجاح.",
        //            MessageEn = "Reservations retrieved successfully.",
        //        }
        //    };
        //}




        // Enhanced Filter Reservations with comprehensive filtering options
        public async Task<ResponseResult> FilterByDateAsync(FilterReservationDto dto)
        {
            // Start with all reservations
            var Query = await _reservation.GetAllAsync(
                include: q => q.Include(r => r.Item).Include(r => r.User),
                asNoTracking: true
            );
            var filteredReservations = Query.AsQueryable();

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

            // Availability
            if (dto.IsAvailable.HasValue)
                filteredReservations = filteredReservations.Where(r => r.IsAvailable == dto.IsAvailable.Value);

            // Status
            if (dto.Status.HasValue)
                filteredReservations = filteredReservations.Where(r => r.Status == dto.Status.Value);

            // User
            if (dto.UserId.HasValue && dto.UserId.Value > 0)
                filteredReservations = filteredReservations.Where(r => r.UserId == dto.UserId.Value);

            var finalResults = filteredReservations.ToList();

            if (!finalResults.Any())
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "لا توجد حجوزات تطابق معايير البحث.",
                        MessageEn = "No reservations found matching the search criteria.",
                    }
                };

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

            return new ResponseResult
            {
                Data = result,
                Result = Result.Success,
                TotalCount = result.Count,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب الحجوزات بنجاح.",
                    MessageEn = "Reservations retrieved successfully.",
                }
            };
        }



        // Helper method for validation
        private ResponseResult ValidateReservationDto(CreateReservationDto dto)
        {
            // Validate end time is after start time
            if (dto.EndTime <= dto.StartTime)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "وقت انتهاء الحجز يجب أن يكون بعد وقت البداية.",
                        MessageEn = "End time must be after start time.",
                    }
                };
            }

            // Validate reservation is not in the past
            var reservationDateTime = dto.ReservationDate.Add(dto.StartTime);
            if (reservationDateTime <= DateTime.Now)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "لا يمكن إنشاء حجز في الماضي.",
                        MessageEn = "Cannot create reservation in the past.",
                    }
                };
            }

            return null; 
        }
    }
}
