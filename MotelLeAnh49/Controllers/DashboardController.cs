using BusinessLogic.Interfaces;
using BusinessLogic.Service;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MotelLeAnh49.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;

        public DashboardController(
            IBookingService bookingService,
            IRoomService roomService)
        {
            _bookingService = bookingService;
            _roomService = roomService;

            // License QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public IActionResult Index()
        {
            var bookings = _bookingService.GetAll();
            var rooms = _roomService.GetAllRooms();

            ViewBag.TotalRooms = rooms.Count();

            var bookedRooms = bookings
                .Where(b => b.Status == "Confirmed")
                .Select(b => b.RoomId)
                .Distinct()
                .Count();

            ViewBag.BookedRooms = bookedRooms;
            ViewBag.EmptyRooms = rooms.Count() - bookedRooms;

            // Doanh thu hôm nay
            ViewBag.TodayRevenue = bookings
                .Where(b => b.Status == "Confirmed"
                         && b.CheckIn.Date == DateTime.Today)
                .Sum(b => b.Room.OvernightPrice);

            // Top 5 phòng được thuê nhiều nhất
            ViewBag.TopRooms = bookings
                .Where(b => b.Status == "Confirmed")
                .GroupBy(b => b.Room.RoomNumber)
                .Select(g => new
                {
                    Room = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            // Doanh thu theo tháng
            ViewBag.RevenueByMonth = bookings
                .Where(b => b.Status == "Confirmed")
                .GroupBy(b => b.CheckIn.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(x => x.Room.OvernightPrice)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return View();
        }

        // =============================
        // EXPORT EXCEL
        // =============================

        public IActionResult ExportExcel()
        {
            var bookings = _bookingService.GetAll()
                .Where(b => b.Status == "Confirmed")
                .ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Revenue");

            ws.Cell(1, 1).Value = "Customer";
            ws.Cell(1, 2).Value = "Room";
            ws.Cell(1, 3).Value = "CheckIn";
            ws.Cell(1, 4).Value = "Revenue";

            int row = 2;

            foreach (var b in bookings)
            {
                ws.Cell(row, 1).Value = b.FullName;
                ws.Cell(row, 2).Value = b.Room.RoomNumber;
                ws.Cell(row, 3).Value = b.CheckIn.ToString("dd/MM/yyyy");
                ws.Cell(row, 4).Value = b.Room.OvernightPrice;

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RevenueReport.xlsx");
        }

        // =============================
        // EXPORT PDF
        // =============================

        public IActionResult ExportPdf()
        {
            var bookings = _bookingService.GetAll()
                .Where(b => b.Status == "Confirmed")
                .ToList();

            var pdf = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Header()
                        .Text("Revenue Report")
                        .FontSize(20)
                        .Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Customer").Bold();
                            header.Cell().Text("Room").Bold();
                            header.Cell().Text("Revenue").Bold();
                        });

                        foreach (var b in bookings)
                        {
                            table.Cell().Text(b.FullName);
                            table.Cell().Text(b.Room.RoomNumber);
                            table.Cell().Text(b.Room.OvernightPrice.ToString("N0") + " đ");
                        }
                    });
                });
            });

            using var stream = new MemoryStream();
            pdf.GeneratePdf(stream);

            return File(stream.ToArray(),
                "application/pdf",
                "RevenueReport.pdf");
        }
    }
}