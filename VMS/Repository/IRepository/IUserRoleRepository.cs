﻿using VMS.Models;

namespace VMS.Repository.IRepository
{
    public interface IUserRoleRepository
    {
        Task<UserRole> GetUserRoleByUserIdAsync(int userId);
        Task AddUserRoleAsync(UserRole userRole);
        Task<List<UserRole>> GetAllUserRolesAsync();
        Task UpdateUserRoleAsync(UserRole userRole);

    }
}
