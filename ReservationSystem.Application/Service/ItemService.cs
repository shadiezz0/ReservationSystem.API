
using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Application.Service
{
    public class ItemService : IItemService
    {
        private readonly IGenericRepository<Item> _Itemrepo;
        private readonly IUnitOfWork _uow;

        public ItemService(IUnitOfWork uow)
        {
            _Itemrepo = uow.Repository<Item>();
            _uow = uow;
        }
        public async Task<ResponseResult> CreateAsync(CreateItemDto dto)
        {
            var item = new Item
            {
                Name = dto.Name,
                Description = dto.Description,
                PricePerHour = dto.PricePerHour,
                IsAvailable = dto.IsAvailable,
                ItemTypeId = dto.ItemTypeId
            };

            await _Itemrepo.AddAsync(item);
            var saveResult = await _uow.SaveAsync();

            return new ResponseResult
            {
                Data = item.Id,
                Result = saveResult ? Result.Success : Result.Failed,
                Alart = new Alart
                {
                    AlartType = saveResult ? AlartType.success : AlartType.error,
                    type = AlartShow.note,
                    MessageAr = saveResult ? "تم إنشاء العنصر بنجاح." : "فشل في إنشاء العنصر.",
                    MessageEn = saveResult ? "Item created successfully." : "Failed to create item.",
                }
            };
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            var item = await _Itemrepo.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "العنصر غير موجود.",
                        MessageEn = "Item not found."
                    }
                };
            }

            _Itemrepo.Delete(item);
            var saveResult = await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = saveResult ? Result.Success : Result.Failed,
                Alart = new Alart
                {
                    AlartType = saveResult ? AlartType.success : AlartType.error,
                    type = AlartShow.note,
                    MessageAr = saveResult ? "تم حذف العنصر بنجاح." : "فشل في حذف العنصر.",
                    MessageEn = saveResult ? "Item deleted successfully." : "Failed to delete item.",
                }
            };

        }

        public async Task<ResponseResult> FilterAvailableAsync()
        {
            var items = await _Itemrepo.FindAllAsync(item => item.IsAvailable, asNoTracking: true);
            if (items == null || !items.Any())
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "لا توجد عناصر متاحة.",
                        MessageEn = "No available items found."
                    }
                };
            }
            return new ResponseResult
            {
                DataCount = items.Count(),
                Data = items,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم العثور على العناصر المتاحة.",
                    MessageEn = "Available items found."
                }
            };
        }

        public async Task<ResponseResult> FilterByTypeAsync(int itemTypeId)
        {
            var items = await _Itemrepo.FindAllAsync(item => item.ItemTypeId == itemTypeId, asNoTracking: true);
            if (items == null || !items.Any())
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "لا توجد عناصر من هذا النوع.",
                        MessageEn = "No items of this type found."
                    }
                };
            }
            return new ResponseResult
            {
                DataCount = items.Count(),
                Data = items,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم العثور على العناصر من هذا النوع.",
                    MessageEn = "Items of this type found."
                }
            };

        }

        public async Task<ResponseResult> GetAllAsync()
        {
            var items = await _Itemrepo.GetAllAsync(
                include: query => query.Include(i => i.ItemType),
                asNoTracking: true
            ); if (items == null || !items.Any())
            {
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "لا توجد عناصر.",
                        MessageEn = "No items found."
                    }
                };
            }
            return new ResponseResult
            {
                DataCount = items.Count(),
                Data = items,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم العثور على العناصر.",
                    MessageEn = "Items found."
                }
            };
        }

        public async Task<ResponseResult> GetByIdAsync(int id)
        {
            var item = await _Itemrepo.GetByIdAsync(id, asNoTracking: true);
            if (item == null)
            {
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "العنصر غير موجود.",
                        MessageEn = "Item not found."
                    }
                };
            }
            return new ResponseResult
            {
                Data = item,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم العثور على العنصر.",
                    MessageEn = "Item found."
                }
            };
        }

        public async Task<ResponseResult> UpdateAsync(UpdateItemDto dto)
        {
            var item = await _Itemrepo.GetByIdAsync(dto.Id);
            if (item == null)
            {
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "العنصر غير موجود.",
                        MessageEn = "Item not found."
                    }
                };
            }

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.PricePerHour = dto.PricePerHour;
            item.IsAvailable = dto.IsAvailable;
            item.ItemTypeId = dto.ItemTypeId;

            _Itemrepo.Update(item);
            var saveResult = await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = saveResult ? Result.Success : Result.Failed,
                Alart = new Alart
                {
                    AlartType = saveResult ? AlartType.success : AlartType.error,
                    type = AlartShow.note,
                    MessageAr = saveResult ? "تم تحديث العنصر بنجاح." : "فشل في تحديث العنصر.",
                    MessageEn = saveResult ? "Item updated successfully." : "Failed to update item.",
                }
            };
        }



    }
}
