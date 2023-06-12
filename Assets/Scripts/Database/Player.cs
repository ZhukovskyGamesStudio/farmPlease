using System;

namespace Database
{
    [Serializable]
    public class Player {
        public int Id;
        public string Email;
        public string Password;

        public bool IsConfirmed;

        public string Name;
        public int FarmId;
    }
}