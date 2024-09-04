using OfficeOpenXml;
using System.Drawing;

namespace hb29.API.Helpers
{
    public abstract class BaseExcelManager
    {
        public static void SetHeaderStyle(ExcelRange range)
        {
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#26b4ff"));
            range.Style.Font.Color.SetColor(Color.White);
        }
    }
}
