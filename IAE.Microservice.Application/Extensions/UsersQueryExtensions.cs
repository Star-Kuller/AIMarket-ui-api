using IAE.Microservice.Application.Features.Users;
using IAE.Microservice.Domain.Entities.Common;
using IAE.Microservice.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IAE.Microservice.Application.Interfaces;

namespace IAE.Microservice.Application.Extensions
{
    public static class UsersQueryExtensions
    {
        public static IQueryable<User> WithRole(this IQueryable<User> users)
        {
            return users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);
        }

        public static IQueryable<User> WithRole(this IQueryable<User> users, string role)
        {
            return users.WithRole()
                .Where(u => u.UserRoles.Select(ur => ur.Role.Name).Contains(role));
        }

        public static IQueryable<User> WhereForChangePassword(
            this IQueryable<User> users, long userId, ICurrentUser currentUser)
        {
            return users
                .Where(u => u.Status != Status.Deleted)
                .Where(u => u.Id == userId)
                .Where(u => currentUser.IsAdmin || u.Id == currentUser.Id );
        }
        
        public static IQueryable<User> WhereForChangeStatusOrEmailConfirmed(
            this IQueryable<User> users, long userId, ICurrentUser currentUser)
        {
            return users
                .Where(u => u.Status != Status.Deleted)
                .Where(u => u.Id == userId)
                .Where(u => currentUser.IsAdmin);
        }
    }
}