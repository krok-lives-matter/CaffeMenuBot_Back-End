using System;
using QRCoder;
using Microsoft.AspNetCore.Hosting;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace CaffeMenuBot.AppHost.Configuration

{
    public static class QRCodeConfigurator
    {
        private static string LOGO_NAME = "logo.png";
        private static string QR_NAME = "qr.png";
        private static string SAVE_FOLDER = "media";

        /// <summary>
        /// Generates QR Code image with bot telegram bot link and logo
        /// </summary>
        /// <param name="webHostEnvironment">to get app root folder</param>
        /// <param name="botLink">link to open telegram bot</param>
        public static void GenerateQRCodeForBot(IWebHostEnvironment webHostEnvironment, string botLink)
        {
            string savePath = Path.Combine(webHostEnvironment.WebRootPath, SAVE_FOLDER);
            Directory.CreateDirectory(savePath);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(botLink, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile($"{savePath}/{LOGO_NAME}"), 30);

            qrCodeImage.Save($"{savePath}/{QR_NAME}", ImageFormat.Png);

            Console.WriteLine("Successfully generated QR code");
        }
    }
}