using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CaffeMenuBot.Data.Models.Menu;
using Gehtsoft.PDFFlow.Builder;
using Gehtsoft.PDFFlow.Models.Enumerations;

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
            var document = DocumentBuilder.New();
            var section = document.AddSection()
                .SetMargins(0f)
                .SetSize(PaperSize.A4)
                .SetOrientation(PageOrientation.Portrait);
            
            AddHeaderImage(section);

            var memoryStream = new MemoryStream();
            document.Build(memoryStream);
            memoryStream.Position = 0L;
            return new ValueTask<Stream>(memoryStream);
        }

        private static void AddHeaderImage(SectionBuilder section)
        {
            const string headerImageFileName = "header.png";
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPathToHeaderImage = Path.Combine(currentDir!, "Content", "images", headerImageFileName);

            section.AddImage(fullPathToHeaderImage, ScalingMode.Stretch);
        }
    }
}