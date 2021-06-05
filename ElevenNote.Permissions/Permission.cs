using System.ComponentModel.DataAnnotations;

namespace ElevenNote.Permissions
{
    public enum Action { Get, Create, Edit, Remove }
    public enum Role { Student, LA, Instructor, Admin, SuperAdmin }
    public enum AccessLevel { None, Own, Class, All }
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public string Controller { get; set; }
        public Action Action { get; set; }
        public Role Role { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }

}