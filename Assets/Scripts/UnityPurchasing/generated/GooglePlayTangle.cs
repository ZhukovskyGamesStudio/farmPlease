// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("mnkMB+DpBfBpynkXinc37gZCceAuF+TJqk9kzRXYFK5bZizGO5EnaKEVJO7hSCp/d3YvDBjY0MYdEj673W/sz93g6+THa6VrGuDs7Ozo7e5bOO5yAjhJuGc6VNgHZWKCGcx6uHpkxuzcosdL/vE6iJ/iVUw0ALsTC+tLg+C/vbEMt3dwLyj9iA/rD7JBt/z6AE9PH+3nh2O6MyqsMwjhVDXtUx5rgoSyjGDCOSMx8GZnJBGvfuLdMAGp5bylasOTTdiTylNE12GDFaXQuovMcBftCJ0sMyBdKBQEbuC54ZAzELEMhMFlguJ3VCQjp96pb+zi7d1v7Ofvb+zs7X9drsuxKDuuZKc48JSoH0bu8OeaCpVpOCVgM6Gp9RtGNekNfu/u7O3s");
        private static int[] order = new int[] { 7,3,4,7,6,13,12,12,10,13,11,13,12,13,14 };
        private static int key = 237;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
