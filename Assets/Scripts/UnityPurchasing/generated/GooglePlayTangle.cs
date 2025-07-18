// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("sUvd03H7TpeXOJEa66XSim1eofJ6l5jb+5X81ZWijOsvGY/SMPEkaRJ9cKUAsvm3kHr+ZfAEFQHHyYCMvMOSibVELL5VhBBKYZO+bElI4akcVyyo7rQGSMYvaUvOXzMAkpCg1NmrQEYG9Efkm9ubWQgCFLzW46/m1gGGPTI27NTWdrGsdh3j1heJQg38TjiahCrL5WAp756bM69HwHeC/xSmJQYUKSItDqJsotMpJSUlISQncAieT99zpWbRu2eP1Kef3gwmwiWRNEnMZychLyVxSYtOB8S2ssoTlaYlKyQUpiUuJqYlJSTqME1lV/aZRSiBFzoB2+mbEKaxtI6XP+U+pL+aCIsRMKQ4aFgjnt8b/TUSKxnrcn/9277Wscp7oSYnWYQl");
        private static int[] order = new int[] { 6,2,8,11,5,7,12,9,12,13,10,12,13,13,14 };
        private static int key = 36;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
