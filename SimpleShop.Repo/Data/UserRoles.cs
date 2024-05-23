﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Repo.Data
{
    public static class UserRoles
    {
        public const string Guest = nameof(Guest);
        public const string RegisteredUser = nameof(RegisteredUser);
        public const string ProductManager = nameof(ProductManager);
        public const string OrderAdministrator = nameof(OrderAdministrator);
        public const string ContentManager = nameof(ContentManager);
        public const string SystemAdministrator = nameof(SystemAdministrator);
        public const string SuperAdministrator = nameof(SuperAdministrator);
        public static readonly string[] AllRoles = { Guest, RegisteredUser, ProductManager, OrderAdministrator, ContentManager, SystemAdministrator, SuperAdministrator };
    }
}
