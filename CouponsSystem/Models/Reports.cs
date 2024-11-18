using static Azure.Core.HttpHeader;
using ClosedXML.Excel;

namespace CouponsSystem.Models
{
    public class Reports
    {
        private CouponsManager couponsSystem;

        public Reports(CouponsManager couponsSystem)
        {
            this.couponsSystem = couponsSystem;
        }

        public async Task<List<Coupon>> GetCouponsByUserAsync(int userCreatorID)
        {
            var coupons = await couponsSystem.GetAllCouponsAsync(); 
            return coupons.Where(c => c.UserCreatorID == userCreatorID).ToList(); 
        }

        // Get a list of coupons created within a specific date range
        public async Task<List<Coupon>> GetCouponsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var coupons = await couponsSystem.GetAllCouponsAsync(); 
            return coupons.Where(c => c.CreatedDateTime >= startDate && c.CreatedDateTime <= endDate).ToList(); 
        }

        // Export coupons to an Excel file
        public void ExportCouponsToExcel(List<Coupon> coupons, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Coupons");
                worksheet.Cell(1, 1).Value = "Description";
                worksheet.Cell(1, 2).Value = "Code";
                worksheet.Cell(1, 3).Value = "Creator ID";
                worksheet.Cell(1, 4).Value = "Created Date";
                worksheet.Cell(1, 5).Value = "Is Percentage";
                worksheet.Cell(1, 6).Value = "Discount";
                worksheet.Cell(1, 7).Value = "Is Multiple Discounts";
                worksheet.Cell(1, 8).Value = "Max Usage Count";
                worksheet.Cell(1, 9).Value = "Expiration Date";

                int row = 2;
                foreach (var coupon in coupons)
                {
                    worksheet.Cell(row, 1).Value = coupon.Description;
                    worksheet.Cell(row, 2).Value = coupon.Code;
                    worksheet.Cell(row, 3).Value = coupon.UserCreatorID;
                    worksheet.Cell(row, 4).Value = coupon.CreatedDateTime;
                    worksheet.Cell(row, 5).Value = coupon.IsPercentage;
                    worksheet.Cell(row, 6).Value = coupon.Discount;
                    worksheet.Cell(row, 7).Value = coupon.IsMultipleDiscounts;
                    worksheet.Cell(row, 8).Value = coupon.MaxUsageCount;
                    worksheet.Cell(row, 9).Value = coupon.ExpirationDate;
                    row++;
                }

                workbook.SaveAs(filePath);
            }
        }
    }
}
