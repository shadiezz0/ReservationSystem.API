namespace ReservationSystem.Application.IService
{
    public interface IRoleService
    {
        Task<List<RoleDetailsDto>> GetAllAsync();
        Task<RoleDetailsDto> GetByIdAsync(int id);
        Task<RoleDetailsDto> CreateAsync(CreateRoleDto dto);
        Task<RoleDetailsDto> UpdateAsync(int id, UpdateRoleDto dto);
        Task DeleteAsync(int id);
    }

}
