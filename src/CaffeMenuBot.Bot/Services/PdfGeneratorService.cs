using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data.Models.Menu;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
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
            
            document.SetMargins(0f, 0f, 40f, 0f);
            
            AddHeaderImage(document);
            AddMenuData(document, category);
            
            document.Close();
            return new ValueTask<Stream>(new MemoryStream(memoryStream.ToArray()));
        }

        private static void AddHeaderImage(Document document)
        {
            const string headerImageFileName = "header.png";
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPathToHeaderImage = Path.Combine(currentDir!, "Content", "images", headerImageFileName);
            
            document.Add(new Image(ImageDataFactory.Create(fullPathToHeaderImage)));
        }

        private static void AddMenuData(Document document, Category category)
        {
            document.Add(new Paragraph(category.CategoryName)
                .SetMargins(40f, 40f, 0f, 40f)
                //.SetFontFamily("Segoe UI")
                .SetFontSize(30f)); // 40px

            foreach (var dish in category.Dishes)
            {
                var dishTable = new Table(new[]
                {
                    UnitValue.CreatePercentValue(54f),
                    UnitValue.CreatePercentValue(23f),
                    UnitValue.CreatePercentValue(23f)
                });
                //dishTable.SetFontFamily("Roboto");
                dishTable.SetMargins(40f, 40f, 0f, 40f)
                    .UseAllAvailableWidth()
                    .SetKeepWithNext(true)
                    .SetFontColor(new DeviceRgb(17, 17, 17));
                var dishNameCell = new Cell().Add(new Paragraph(dish.DishName)).SetBorder(Border.NO_BORDER)
                    .SetFontSize(13.5f); // 18px
                var dishServingCell = new Cell().Add(new Paragraph(dish.Serving)).SetBorder(Border.NO_BORDER)
                    .SetFontSize(12f); // 16px
                var dishPriceCell = new Cell().Add(new Paragraph($"{dish.Price} UAH")).SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT).SetFontSize(12f); // 16px
                dishTable.AddCell(dishNameCell);
                dishTable.AddCell(dishServingCell);
                dishTable.AddCell(dishPriceCell);
                document.Add(dishTable);

                var dishDescription = new Paragraph(dish.Description)
                    .SetMargins(10f, 40f, 0f, 40f)
                    //.SetFontFamily("Roboto")
                    .SetFontColor(new DeviceRgb(125, 125, 125))
                    .SetFontSize(12f); // 16f
                document.Add(dishDescription);
            }
        }
    }
}