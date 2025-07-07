using ReservationSystem.Application.IService.IItemType;

public class ItemTypeService : IItemTypeService
{
    private readonly IGenericRepository<ItemType> _ItemTyperepo;
    private readonly IUnitOfWork _uow;

    public ItemTypeService(IUnitOfWork uow)
    {
        _ItemTyperepo = uow.Repository<ItemType>();
        _uow = uow;
    }

    public async Task<ResponseResult> CreateAsync(CreateItemTypeDto dto)
    {
        var itemType = new ItemType
        {
            Name = dto.Name,
        };

        await _ItemTyperepo.AddAsync(itemType);
        var saveResult = await _uow.SaveAsync();

        return new ResponseResult
        {
            Data = itemType.Id,
            Result = saveResult ? Result.Success : Result.Failed,
            Alart = new Alart
            {
                AlartType = saveResult ? AlartType.success : AlartType.error,
                type = AlartShow.note,
                MessageAr = saveResult ? "تم إنشاء نوع العنصر بنجاح." : "فشل في إنشاء نوع العنصر.",
                MessageEn = saveResult ? "Item type created successfully." : "Failed to create item type.",
            }
        };
    }

    public async Task<ResponseResult> DeleteAsync(int id)
    {
        var itemType = await _ItemTyperepo.GetByIdAsync(id);
        if (itemType == null)
        {
            return new ResponseResult
            {
                Result = Result.Failed,
                Alart = new Alart
                {
                    AlartType = AlartType.error,
                    type = AlartShow.note,
                    MessageAr = "نوع العنصر غير موجود.",
                    MessageEn = "Item type not found."
                }
            };
        }
        _ItemTyperepo.Delete(itemType);
        var saveResult = await _uow.SaveAsync();
        return new ResponseResult
        {
            Result = saveResult ? Result.Success : Result.Failed,
            Alart = new Alart
            {
                AlartType = saveResult ? AlartType.success : AlartType.error,
                type = AlartShow.note,
                MessageAr = saveResult ? "تم حذف نوع العنصر بنجاح." : "فشل في حذف نوع العنصر.",
                MessageEn = saveResult ? "Item type deleted successfully." : "Failed to delete item type.",
            }
        };
    }

    public async Task<ResponseResult> GetAllAsync()
    {
        var itemTypes = await _ItemTyperepo.GetAllAsync();
        if (itemTypes == null || !itemTypes.Any())
        {
            return new ResponseResult
            {
                Result = Result.Failed,
                Alart = new Alart
                {
                    AlartType = AlartType.error,
                    type = AlartShow.note,
                    MessageAr = "لا توجد أنواع عناصر متاحة.",
                    MessageEn = "No item types available."
                }
            };
        }
        return new ResponseResult
        {
            Data = itemTypes,
            Result = Result.Success,
            Alart = new Alart
            {
                AlartType = AlartType.success,
                type = AlartShow.note,
                MessageAr = "تم استرجاع أنواع العناصر بنجاح.",
                MessageEn = "Item types retrieved successfully."
            }
        };
    }
}
