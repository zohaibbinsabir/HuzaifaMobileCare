namespace Huzaifa_Mobile_Care.BL
{
    class User
    {
        public enum UserRole
        {
            owner,
            attendant
        }

        public enum UserState
        {
            active,
            inactive,
            deleted
        }

        public string Name { get; set; }
        public string Pin { get; set; }
        public UserRole Role { get; set; }
        public UserState State { get; set; }

        public User(string name, string pin, UserRole role, UserState state)
        {
            Name = name;
            Pin = pin;
            Role = role;
            State = state;
        }
    }
}
