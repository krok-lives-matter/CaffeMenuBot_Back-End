using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data.Models.Menu;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Path = System.IO.Path;

namespace CaffeMenuBot.Bot.Services
{
    public interface IPdfGeneratorService
    {
        ValueTask<Stream> GenerateMenuForCategoryAsync(Category category, CancellationToken cancellationToken);
    }
    
    public sealed class PdfGeneratorService : IPdfGeneratorService
    {
        public ValueTask<Stream> GenerateMenuForCategoryAsync(Category category, CancellationToken cancellationToken)
        {
            var memoryStream = new MemoryStream();
            var pdfWriter = new PdfWriter(memoryStream);
            var pdfDocument = new PdfDocument(pdfWriter);
            pdfDocument.SetDefaultPageSize(PageSize.A4);
            var document = new Document(pdfDocument);
            
            document.SetMargins(0f, 0f, 0f, 0f);
            
            AddHeaderImage(document);
            
            memoryStream.Position = 0L;
            return new ValueTask<Stream>(memoryStream);
        }

        private static void AddHeaderImage(Document document)
        {
            const string headerImageFileName = "header.png";
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPathToHeaderImage = Path.Combine(currentDir!, "Content", "images", headerImageFileName);
            
            document.Add(new Image(ImageDataFactory.Create(fullPathToHeaderImage)));
        }
    }
}