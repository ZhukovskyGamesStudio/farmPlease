// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("UeNgQ1FsZ2hL5ynnlmxgYGBkYWI/0t2evtC5kNDnya5qXMqXdbRhLPQOmJY0vgvS0n3UX67gl88oG+S3k0TDeHdzqZGTM/TpM1imk1LMB0jfTc5UdeF9LR1m25peuHBXblyuN+NgbmFR42BrY+NgYGGvdQggErPcuQt938FvjqAlbKrb3nbqAoUyx7pZEmntq/FDDYNqLA6LGnZF19XlkfmG18zwAWn7EMFVDyTW+ykMDaTsVzg14EX3vPLVP7sgtUFQRIKMxcmc7gUDQ7ECod6e3hxNR1H5k6bqo9RxDIkiYmRqYDQMzgtCgfP3j1bQAG3EUn9EnqzeVeP08cvSeqB74fo1TdsKmjbgI5T+IsqR4tqbSWOHYDq4nvuT9I8+5GNiHMFg");
        private static int[] order = new int[] { 0,2,6,12,12,6,9,9,11,11,11,11,12,13,14 };
        private static int key = 97;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
