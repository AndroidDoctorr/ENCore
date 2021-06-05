using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ElevenNote.Permissions
{
    public class PermissionService : IPermissionService
    {
        private IPermissionDbContext _context;
        public PermissionService(IPermissionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionListItem>> ListPermissions()
        {
            List<Permission> permissions = await _context.Permissions.ToListAsync();
            List<PermissionListItem> permissionListItems = new List<PermissionListItem>();
            // Sort by role first
            permissions.Sort((a, b) => a.Role.CompareTo(b.Role));
            // Convert to list items
            permissionListItems = permissions.Select(p => new PermissionListItem()
            {
                Controller = p.Controller,
                Action = p.Action,
                AccessLevel = p.AccessLevel,
                Role = p.Role,
            }).ToList();
            return permissionListItems;
        }

        public async Task<IEnumerable<string>> ListPermissionsAsStrings()
        {
            IEnumerable<PermissionListItem> permissions = await ListPermissions();
            return permissions.Select(p => p.ToString());
        }

        public async Task<bool> CheckPermission(string controller, Action action, Role role, AccessLevel accessLevel)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => (
                p.Controller == controller &&
                p.Action == action &&
                p.Role == role &&
                p.AccessLevel >= accessLevel
            ));
            if (permission != default)
            {
                return true;
            }
            return false;
        }

        public async Task<int> GetAccessLevel(string controller, Action action, Role role)
        {
            // Find matching permission
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => (
                p.Controller == controller &&
                p.Action == action &&
                p.Role == role
            ));
            // Return access level
            if (permission != default)
            {
                return (int)permission.AccessLevel;
            }
            // If permission item does not exist, access level is 0
            return 0;

        }

        public async Task GrantPermission(string controller, Action action, Role role, AccessLevel accessLevel)
        {
            // Check for existing permission
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => (
                p.Controller == controller &&
                p.Action == action &&
                p.Role == role
            ));
            // If it exists, update the access level
            if (permission != default)
            {
                permission.AccessLevel = accessLevel;
                await _context.SaveChangesAsync();
            }
            // Otherwise, create it
            else
            {
                var newPermission = new Permission()
                {
                    Controller = controller,
                    Action = action,
                    Role = role,
                    AccessLevel = accessLevel,
                };
                _context.Permissions.Add(newPermission);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RemovePermission(string controller, Action action, Role role)
        {
            // Find any permissions with a nonzero access level
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => (
                p.Controller == controller &&
                p.Action == action &&
                p.Role == role &&
                p.AccessLevel > 0
            ));

            // If one exists, reduce it and save
            int changes = 0;
            if (permission != default)
            {
                permission.AccessLevel = 0;
                changes = await _context.SaveChangesAsync();
            }

            // Make sure permission was altered or not found
            if (changes == 1 || permission == default)
            {
                return true;
            }
            // Something went wrong
            return false;
        }
    }
}