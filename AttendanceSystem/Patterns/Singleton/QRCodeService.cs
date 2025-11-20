using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace AttendanceSystem.Patterns.Singleton
{
    // Singleton Pattern for QR Code Service
    public sealed class QRCodeService
    {
        private static QRCodeService? _instance;
        private static readonly object _lock = new object();

        private QRCodeService()
        {
            // Private constructor to prevent instantiation
        }

        public static QRCodeService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new QRCodeService();
                        }
                    }
                }
                return _instance;
            }
        }

        public string GenerateQRCodeBase64(string data, int pixelsPerModule = 20)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(pixelsPerModule);
                    return Convert.ToBase64String(qrCodeBytes);
                }
            }
        }

        public byte[] GenerateQRCodeBytes(string data, int pixelsPerModule = 20)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(pixelsPerModule);
                }
            }
        }

        public string GenerateAttendanceQRData(int sessionId, string sessionCode)
        {
            // Create a JSON-like structure for the QR code
            return $"{{\"type\":\"attendance\",\"sessionId\":{sessionId},\"code\":\"{sessionCode}\"}}";
        }
    }
}
