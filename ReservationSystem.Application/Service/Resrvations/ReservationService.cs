using ReservationSystem.Application.IService.IResrvations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservationSystem.Application.Service.Resrvations
{
    public class ReservationService : IReservationService
    {
        private readonly IRepository<Reservation> _reservation;
        private readonly IRepository<Item> _itemRepo;
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
            if (item == null || !item.IsAvailable)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "العنصر غير متاح.",
                        MessageEn = "Item is not available.",
                    }
                };

            var duration = (dto.EndTime - dto.StartTime).TotalHours;
            var totalPrice = item.PricePerHour * (decimal)duration;

            var reservation = new Reservation
            {
                ReservationDate = dto.ReservationDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = "Pending",
                ItemId = dto.ItemId,
                UserId = dto.UserId,
                TotalPrice = totalPrice
            };

            await _reservation.AddAsync(reservation);
            await _uow.SaveAsync();

            return new ResponseResult
            {
                Data =  reservation.Id ,
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
            var r = await _reservation.GetByIdAsync(id);
            if (r == null)
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

            var dto = new ReservationDto
            {
                Id = r.Id,
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                ItemName = r.Item.Name,
                UserName = r.User.Name,
                Status = r.Status,
                TotalPrice = r.TotalPrice
            };

            return new ResponseResult
            {
                Data = dto,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب الحجز بنجاح.",
                    MessageEn = "Reservation retrieved successfully.",
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
            await _uow.SaveAsync();

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


    }
}
