using System.IO;

namespace Leafing.Core.Text {
    public static class BOMChecker {
        public static bool IsSignatured(string fileName) {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                return IsSignatured(stream);
            }
        }

        public static bool IsSignatured(Stream stream) {
            var bom = new byte[4];
            if (stream.Read(bom, 0, 4) >= 2) {
                UtfEncoding utfEncoding = GetUtfEncoding(bom);
                if (utfEncoding != UtfEncoding.Unknown) {
                    return true;
                }
            }
            return false;
        }

        public static UtfEncoding GetUtfEncoding(byte[] bom) {
            if (bom.Length == 4) {
                if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) {
                    return UtfEncoding.BigEndianUTF32;
                }
                if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) {
                    return UtfEncoding.LittleEndianUTF32;
                }
            }

            if (bom.Length == 3 && bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) {
                return UtfEncoding.UTF8;
            }

            if (bom.Length == 2) {
                if (bom[0] == 0xfe && bom[1] == 0xff) {
                    return UtfEncoding.BigEndianUTF16;
                }

                if (bom[0] == 0xff && bom[1] == 0xfe) {
                    return UtfEncoding.LittleEndianUTF16;
                }
            }

            return UtfEncoding.Unknown;
        }
    }
}