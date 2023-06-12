using System;

namespace Database
{
    [Serializable]
    public class Message {
        public int Id;
        public int FarmId;

        public string PlayerName;
        public string Text;

        public string PostDate;
    }
}