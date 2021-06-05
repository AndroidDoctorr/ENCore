using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevenNote.Permissions
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionListItem>> ListPermissions();
        Task<IEnumerable<string>> ListPermissionsAsStrings();
        Task<bool> CheckPermission(string controller, Action action, Role role, AccessLevel accessLevel);
        Task<int> GetAccessLevel(string controller, Action action, Role role);
        Task GrantPermission(string controller, Action action, Role role, AccessLevel accessLevel);
        Task<bool> RemovePermission(string controller, Action action, Role role);
    }
}