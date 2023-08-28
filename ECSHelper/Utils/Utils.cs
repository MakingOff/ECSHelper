using System;

namespace ECSHelper.Utils; 

public static class Utils {
    public static bool HasValue(this object obj) {
        return obj != null;
    }

    public static bool HasValue(this string str) {
        return !string.IsNullOrEmpty(str);
    }
    
    public static byte[] ToByteArray(this string hex) {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2) {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        
        return bytes;
    }
}