using ReservationSystem.Application.IService;

namespace ReservationSystem.Application.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IGenericRepository<Permission> _permissionRepo;
        private readonly IUnitOfWork _uow;

        public PermissionService(IUnitOfWork uow)
        {
            _uow = uow;
            _permissionRepo = uow.Repository<Permission>();
        }
        public async Task<ResponseResult> CreateAsync(CreatePermissionDto dto)
        {
            var permission = new Permission
            {
                isShow = dto.IsShow,
                isAdd = dto.IsAdd,
                isEdit = dto.IsEdit,
                isDelete = dto.IsDelete
            };

            await _permissionRepo.AddAsync(permission);
            var saveResult = await _uow.SaveAsync();

            return new ResponseResult
            {
                Data = permission.Id,
                Result = saveResult ? Result.Success : Result.Failed,
                Alart = new Alart
                {
                    AlartType = saveResult ? AlartType.success : AlartType.error,
                    type = AlartShow.note,
                    MessageAr = saveResult ? "تم إنشاء الصلاحية بنجاح." : "فشل في إنشاء الصلاحية.",
                    MessageEn = saveResult ? "Permission created successfully." : "Failed to create permission.",
                }
            };
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            var permission = await _permissionRepo.GetByIdAsync(id);
            if (permission == null)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "الصلاحية غير موجودة.",
                        MessageEn = "Permission not found."
                    }
                };
            }

            _permissionRepo.Delete(permission);
            var saveResult = await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = saveResult ? Result.Success : Result.Failed,
                Alart = new Alart
                {
                    AlartType = saveResult ? AlartType.success : AlartType.error,
                    type = AlartShow.note,
                    MessageAr = saveResult ? "تم حذف الصلاحية بنجاح." : "فشل في حذف الصلاحية.",
                    MessageEn = saveResult ? "Permission deleted successfully." : "Failed to delete permission.",
                }
            };
        }

        public async Task<ResponseResult> GetAllAsync()
        {
            var permissions = await _permissionRepo.GetAllAsync();
            if (permissions == null || !permissions.Any())
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "لا توجد صلاحيات متاحة.",
                        MessageEn = "No permissions available."
                    }
                };
            }

            return new ResponseResult
            {
                Data = permissions,
                DataCount = permissions.Count(),
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب الصلاحيات بنجاح.",
                    MessageEn = "Permissions retrieved successfully."
                }
            };
        }

        public async Task<ResponseResult> GetByIdAsync(int id)
        {
            var permission = await _permissionRepo.GetByIdAsync(id);
            if (permission == null)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "الصلاحية غير موجودة.",
                        MessageEn = "Permission not found."
                    }
                };
            }
            return new ResponseResult
            {
                Data = permission,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم جلب الصلاحية بنجاح.",
                    MessageEn = "Permission retrieved successfully."
                }
            };
        }

        public async Task<ResponseResult> UpdateAsync(UpdatePermissionDto dto)
        {
            var permission = await _permissionRepo.GetByIdAsync(dto.Id);
            if (permission == null)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "الصلاحية غير موجودة.",
                        MessageEn = "Permission not found."
                    }
                };
            }
            permission.isShow = dto.IsShow;
            permission.isAdd = dto.IsAdd;
            permission.isEdit = dto.IsEdit;
            permission.isDelete = dto.IsDelete;

            _permissionRepo.Update(permission);
            var saveResult = await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = saveResult ? Result.Success : Result.Failed,
                Alart = new Alart
                {
                    AlartType = saveResult ? AlartType.success : AlartType.error,
                    type = AlartShow.note,
                    MessageAr = saveResult ? "تم تحديث الصلاحية بنجاح." : "فشل في تحديث الصلاحية.",
                    MessageEn = saveResult ? "Permission updated successfully." : "Failed to update permission.",
                }
            };
        }



    }

}
