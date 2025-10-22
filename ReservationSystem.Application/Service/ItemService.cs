using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Application.Service
{
    public class ItemService : IItemService
    {
        private readonly IGenericRepository<Item> _Itemrepo;
        private readonly IGenericRepository<ItemType> _ItemTyperepo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;

        public ItemService(IUnitOfWork uow, ICurrentUserService currentUserService, IGenericRepository<ItemType> itemTyperepo)
        {
            _Itemrepo = uow.Repository<Item>();
            _uow = uow;
            _currentUserService = currentUserService;
            _ItemTyperepo = itemTyperepo;
        }

        public async Task<ResponseResult> CreateAsync(CreateItemDto dto)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return new ResponseResult
                {
                    Result = Result.Unauthorized,
                    Alart = new Alart
                    {
                        AlartType = AlartType.Unauthorized,
                        type = AlartShow.popup,
                        MessageAr = "غير مصرح لك بأداء هذا الإجراء.",
                        MessageEn = "You are not authorized to perform this action."
                    }
                };
            }
    
            // Check if user can create more items By SuperAdmin
            if (!await CanUserCreateMoreItemsAsync())
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.popup,
                        MessageAr = "فقط المشرف العام يمكنه إنشاء العناصر.",
                        MessageEn = "Only super admin can create items."
                    }
                };
            }
            //  Validate that all ItemTypeIds exist in DB

            var itemTypes = await _ItemTyperepo
                .FindAllAsync(t => dto.ItemTypeIds.Contains(t.Id));

            if (itemTypes == null || !itemTypes.Any() || itemTypes.Count() != dto.ItemTypeIds.Count)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.popup,
                        MessageAr = "هناك خطأ في بيانات نوع العنصر.",
                        MessageEn = "There is an error in the item type data."
                    }
                };
            }

            if (!await IsAdminAsync(dto.AdminId))
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.popup,
                        MessageAr = "هناك خطأ في تحديد المشرف",
                        MessageEn = "There is an error in specifying the supervisor."
                    }
                };
            }
            var Existitem = await _Itemrepo.FindOneAsync(a=>a.AdminId==dto.AdminId);
            if (Existitem != null)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.popup,
                        MessageAr = "لقد تم تحديد هذا المشرف من قبل",
                        MessageEn = "This Admin has been identified by"
                    }
                };
            }
            var item = new Item
            {
                Name = dto.Name,
                Description = dto.Description,
                PricePerHour = dto.PricePerHour,
                ItemTypes = itemTypes.ToList(),
                CreatedById = currentUserId.Value,
                AdminId = dto.AdminId
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

            // Check if user can delete this item (SuperAdmin can delete all, Admin can only delete their own)
            if (!CanUserAccessItem(item))
            {
                return new ResponseResult
                {
                    Result = Result.Unauthorized,
                    Alart = new Alart
                    {
                        AlartType = AlartType.Unauthorized,
                        type = AlartShow.popup,
                        MessageAr = "غير مصرح لك بحذف هذا العنصر.",
                        MessageEn = "You are not authorized to delete this item."
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

        public async Task<ResponseResult> FilterByTypeAsync(int itemTypeId)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();

            // Apply user filtering for non-SuperAdmin users
            // ✅ Query items that have this ItemTypeId in their collection
            IQueryable<Item> query = _Itemrepo.AsNoTracking()
       .Where(i => i.ItemTypes.Any(t => t.Id == itemTypeId));
            

          
            // 🔐 Restrict to user's own items if not SuperAdmin
            if (!_currentUserService.IsCurrentUserSuperAdmin() && currentUserId != null)
            {
                query = query.Where(i => i.CreatedById == currentUserId || i.AdminId == currentUserId);
            }

            // Include ItemTypes for clarity (optional)
            query = query.Include(i => i.ItemTypes);

            var items = await query.ToListAsync();

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
            var currentUserId = _currentUserService.GetCurrentUserId();
            
            // SuperAdmin sees all items, others see only their own items
            //var items = _currentUserService.IsCurrentUserSuperAdmin()
            //    ? await _Itemrepo.GetAllAsync(
            //        include: query => query.Include(i => i.ItemTypes).Include(i => i.CreatedBy),
            //        asNoTracking: true
            //    )
            //    : await _Itemrepo.GetAllAsync(
            //        predicate: i => i.AdminId == currentUserId,
            //        include: query => query.Include(i => i.ItemTypes).Include(i => i.CreatedBy),
            //        asNoTracking: true
            //    );
            var items = await _currentUserService.IsAdmin(currentUserId)
               ?  await _Itemrepo.GetAllAsync(
                   predicate: i => i.AdminId == currentUserId,
                   include: query => query.Include(i => i.ItemTypes).Include(i => i.CreatedBy),
                   asNoTracking: true
               ): await _Itemrepo.GetAllAsync(
                   include: query => query.Include(i => i.ItemTypes).Include(i => i.CreatedBy),
                   asNoTracking: true
               )
               ;

            var itemDtos = items.Select(i => new ItemDto
            {
                Id = i.Id,
                Name = i.Name,
                ItemTypeNames = string.Join(", ", i.ItemTypes.Select(t => t.Name)),
                CreatedByName = i.CreatedBy?.Name ?? "Unknown",
                CreatedById = i.CreatedById ?? 0,
                AdminId =i.AdminId
            }).ToList();


            if (itemDtos == null || !itemDtos.Any())
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
                DataCount = itemDtos.Count(),
                Data = itemDtos,
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
            var item = await _Itemrepo.GetByIdAsync(id, 
                include: query => query.Include(i => i.CreatedBy), 
                asNoTracking: true);
                
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

            // Check if user can access this item
            if (!CanUserAccessItem(item))
            {
                return new ResponseResult
                {
                    Result = Result.Unauthorized,
                    Alart = new Alart
                    {
                        AlartType = AlartType.Unauthorized,
                        type = AlartShow.popup,
                        MessageAr = "غير مصرح لك بالوصول لهذا العنصر.",
                        MessageEn = "You are not authorized to access this item."
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
        //public async Task<ResponseResult> GetItemByAdminAsync()
        //{
        //    var currentUserId = _currentUserService.GetCurrentUserId();
        //    var item = await _Itemrepo.FindOneAsync(a=>a.AdminId == currentUserId);

        //    if (item == null)
        //    {
        //        return new ResponseResult
        //        {
        //            Result = Result.NoDataFound,
        //            Alart = new Alart
        //            {
        //                AlartType = AlartType.error,
        //                type = AlartShow.note,
        //                MessageAr = "العنصر غير موجود.",
        //                MessageEn = "Item not found."
        //            }
        //        };
        //    }

        //    Check if user can access this item
        //    if (!await IsAdminAsync(currentUserId))
        //    {
        //        return new ResponseResult
        //        {
        //            Result = Result.Unauthorized,
        //            Alart = new Alart
        //            {
        //                AlartType = AlartType.Unauthorized,
        //                type = AlartShow.popup,
        //                MessageAr = "غير مصرح لك بالوصول لهذا العنصر.",
        //                MessageEn = "You are not authorized to access this item."
        //            }
        //        };
        //    }

        //    return new ResponseResult
        //    {
        //        Data = item,
        //        Result = Result.Success,
        //        Alart = new Alart
        //        {
        //            AlartType = AlartType.success,
        //            type = AlartShow.note,
        //            MessageAr = "تم العثور على العنصر.",
        //            MessageEn = "Item found."
        //        }
        //    };
        //}
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
            // Check if user can update this item
            if (!CanUserAccessItem(item))
            {
                return new ResponseResult
                {
                    Result = Result.Unauthorized,
                    Alart = new Alart
                    {
                        AlartType = AlartType.Unauthorized,
                        type = AlartShow.popup,
                        MessageAr = "غير مصرح لك بتحديث هذا العنصر.",
                        MessageEn = "You are not authorized to update this item."
                    }
                };
            }
            //check if notfound this itemTypes in DB
            var itemTypes = await _ItemTyperepo
                    .FindAllAsync(t => dto.ItemTypeIds.Contains(t.Id));

            if (itemTypes == null || !itemTypes.Any() || itemTypes.Count() != dto.ItemTypeIds.Count)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.popup,
                        MessageAr = "هناك خطأ في بيانات نوع العنصر.",
                        MessageEn = "There is an error in the item type data."
                    }
                };
            }

            if (!await IsAdminAsync(dto.AdminId))
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.popup,
                        MessageAr = "هناك خطأ في تحديد المشرف",
                        MessageEn = "There is an error in specifying the supervisor."
                    }
                };
            }
            var Existitem = await _Itemrepo.FindOneAsync(a => a.AdminId == dto.AdminId);
            if (Existitem != null)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.popup,
                        MessageAr = "لقد تم تحديد هذا المشرف من قبل",
                        MessageEn = "This Admin has been identified by"
                    }
                };
            }
            item.Name = dto.Name;
            item.Description = dto.Description;
            item.PricePerHour = dto.PricePerHour;
            item.ItemTypes = itemTypes.ToList();
            item.AdminId = dto.AdminId;
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

        private bool CanUserAccessItem(Item item)
        {
            // SuperAdmin can access all items
            if (_currentUserService.IsCurrentUserSuperAdmin())
                return true;

            // Other users can only access items they created
            var currentUserId = _currentUserService.GetCurrentUserId();
            return currentUserId.HasValue && item.CreatedById.HasValue && item.CreatedById.Value == currentUserId.Value;
        }

        private async Task<bool> CanUserCreateMoreItemsAsync()
        {
            // Only SuperAdmin can create items
            return _currentUserService.IsCurrentUserSuperAdmin();
        }
        private async Task<bool> IsAdminAsync(int? userId)
        {
            // Only SuperAdmin can create items
            return await _currentUserService.IsAdmin(userId);
        }
    }
}
