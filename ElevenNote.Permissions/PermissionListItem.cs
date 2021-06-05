namespace ElevenNote.Permissions
{
    public class PermissionListItem
    {
        public string Controller { get; set; }
        public Action Action { get; set; }
        public Role Role { get; set; }
        public AccessLevel AccessLevel { get; set; }

        public override string ToString()
        {
            return $"{Role.ToString()}s may {Action.ToString()} {AccessLevel.ToString()} {Controller}";
        }
    }
}