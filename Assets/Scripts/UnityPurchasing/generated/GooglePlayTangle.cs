// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("rc4YhPTOv06RzKIu8ZOUdO86jE7Y4RI/XLmSO+Mu4litkNowzWfRnsMbpeiddHJEepY0z9XHBpCR0udZmRoUGyuZGhEZmRoaG4mrWD1H3s1X49IYF77ciYGA2fruLiYw6+TITf0dvXUWSUtH+kGBhtneC375HflEFk8XZsXmR/pyN5N0FIGi0tVRKF9YklHOBmJe6bAYBhFs/GOfztOWxYySMBoqVDG9CAfMfmkUo7rC9k3lK5kaOSsWHRIxnVOd7BYaGhoeGxiIFCvG918TSlOcNWW7LmU8pbIhl3XjUyZMfTqG4Rv+a9rF1qve4vKYbI/68RYf8wafPI/hfIHBGPC0hxa3QQoM9rm56RsRcZVMxdxaxf4XoldfA+2wwx/7iBkYGhsa");
        private static int[] order = new int[] { 6,3,10,3,4,12,9,8,13,9,12,11,13,13,14 };
        private static int key = 27;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
