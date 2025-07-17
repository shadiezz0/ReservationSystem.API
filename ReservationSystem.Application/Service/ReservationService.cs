using ReservationSystem.Application.IService;

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

        public async Task<ResponseResult> CreateAsync(CreateReservationDto dto)
        {
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
            var reservation = new Reservation
            {
                ReservationDate = dto.ReservationDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                ItemId = dto.ItemId,
                UserId = dto.UserId,
                Status = "Pending",
                TotalPrice = item.PricePerHour * (dto.EndTime - dto.StartTime).TotalHours // Assuming TotalPrice is calculated based on hours
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

        public async Task<ResponseResult> GetAllAsync()
        {
            var data = await _reservation.GetAllAsync();
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
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب جميع الحجوزات بنجاح.",
                    MessageEn = "All reservations retrieved successfully.",
                }
            };
        }

        public async Task<ResponseResult> GetByIdAsync(int id)
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

        public async Task<ResponseResult> UpdateAsync(UpdateReservationDto dto)
        {
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
            res.ReservationDate = dto.ReservationDate;
            res.StartTime = dto.StartTime;
            res.EndTime = dto.EndTime;
            res.Status = dto.Status;

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

        public async Task<ResponseResult> GetByUserIdAsync(int userId)
        {
            var reservations = await _reservation.FindAllAsync(r => r.UserId == userId);
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

            if (res.Status != "Pending")
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

            res.Status = "Confirmed";
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

            if (res.Status == "Cancelled")
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

            res.Status = "Cancelled";
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

        public async Task<ResponseResult> FilterByDateAsync(FilterReservationDto dto)
        {
            var reservations = await _reservation.FindAllAsync(r => r.ReservationDate >= dto.FromDate && r.ReservationDate <= dto.ToDate);
            if (reservations == null || !reservations.Any())
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "لا توجد حجوزات في هذا النطاق الزمني.",
                        MessageEn = "No reservations found in this date range.",
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
                TotalCount = result.Count(),
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب الحجوزات بنجاح.",
                    MessageEn = "Reservations retrieved successfully.",
                }
            };

        }
    }
}
