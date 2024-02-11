﻿using Microsoft.AspNetCore.Identity;

namespace TaskBoard.Data.Configuration
{
    public static class ConfigurationHelper
    {
        public static IdentityUser TestUser = GetUser();
        public static Board OpenBoard = new Board()
        {
            Id = 1,
            Name = "Open"
        };
        public static Board InProgressBoard = new Board()
        {
            Id = 2,
            Name = "In Progress"
        };
        public static Board DoneBoard = new Board()
        {
            Id = 3,
            Name = "Done"
        };

        private static IdentityUser GetUser()
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var user = new IdentityUser()
            {
                UserName = "Test",
                NormalizedUserName = "TEST@SOFTUNI.BG"
            };

            user.PasswordHash = hasher.HashPassword(user, "softuni");

            return user;
        }
    }
}
