using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

namespace dotnet_graphql.Helpers
{
    public static class PermissionExtenstion
    {
        private const string PermissionKey = "permission";

        public static bool RequiresPermission(this IProvideMetadata type)
        {
            var permissions = type.GetMetadata<IEnumerable<string>>(PermissionKey, new List<string> { });
            return permissions.Any();
        }

        public static bool CanAccess(this IProvideMetadata type, IEnumerable<string> claimes)
        {
            var permissions = type.GetMetadata<IEnumerable<string>>(PermissionKey, new List<string> { });
            return permissions.Any(claimes.Contains);
        }

        public static void AddPermissions(this IProvideMetadata type, string permission)
        {
            var permissions = type.GetMetadata<List<string>>(PermissionKey) ?? new List<string>();
            permissions.Add(permission);
            type.Metadata[PermissionKey] = permissions;
        }
    }
}
