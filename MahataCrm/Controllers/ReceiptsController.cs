using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MahataCrm.Data;
using MahataCrm.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.Reporting.NETCore;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.StyledXmlParser.Jsoup.Nodes;
using Document = iText.Layout.Document;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas.Draw;
using System.Reflection;
using MahataCrm.Service;
using Newtonsoft.Json.Linq;
using Microsoft.ReportingServices.Interfaces;
using CrmMahata.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using iText.IO.Image;
using System.Drawing;
using Path = System.IO.Path;
using System.Drawing.Imaging;
using Images = iText.Layout.Element.Image;
using Image = System.Drawing.Image;
using QRCoder;
using Org.BouncyCastle.Utilities;

namespace MahataCrm.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Operator> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReceiptsController(ApplicationDbContext context, UserManager<Operator> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public void GenerateInvoice(int id, Receipt receipt, int numberOfElement)
        {
            string filePath = $"{_webHostEnvironment.WebRootPath}\\Reports\\facture.pdf";
            PdfWriter writer = new PdfWriter(filePath, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new PdfDocument(writer);
            PageSize pageSize = PageSize.A6;
            if(numberOfElement == 1)
            {
                pageSize.SetHeight(850);
            }
            else
            {
                var addsize = (numberOfElement - 1) * 25;
                pageSize.SetHeight(addsize + 850);
            }
            
            Document document = new Document(pdf, pageSize);

            // Ajoutez le contenu de la facture au document
            AddInvoiceContent(document, id, receipt);

            // Fermez le document
            document.Close();
        }

        private Paragraph paragrahElement(string title, string value)
        {
            Paragraph Paragraph = new Paragraph();
            Paragraph.Add(new Text(title).SetBold().SetFontSize(10));
            Paragraph.Add(new Text(value));
            return Paragraph;
        }

        private string ResizeImage(string imagePath, int targetWidth, int targetHeight)
        {
            using (var image = Image.FromFile(imagePath))
            {
                var resizedImage = new Bitmap(targetWidth, targetHeight);
                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.DrawImage(image, 0, 0, targetWidth, targetHeight);
                }

                string resizedImagePath = Path.Combine(Path.GetDirectoryName(imagePath), $"resized_{Path.GetFileName(imagePath)}");
                resizedImage.Save(resizedImagePath, ImageFormat.Jpeg);

                return resizedImagePath;
            }
        }
        static byte[] GenerateQrCode(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);  // Ajustez la taille du QR code ici
        }

        private void AddInvoiceContent(Document document, int id, Receipt receipt)
        {
            string imagePath = $"{_webHostEnvironment.WebRootPath}\\Image\\tra.jpg";
            string resizedImagePath = ResizeImage(imagePath, 80, 30);
            Images logo = new Images(ImageDataFactory.Create(resizedImagePath));
            logo.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            logo.SetMargins(10, 50, 10, 80);
            logo.SetBackgroundColor(iText.Kernel.Colors.ColorConstants.WHITE);
            document.Add(new Paragraph().Add(logo));
            // Entête de la facture
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            Paragraph header = new Paragraph(receipt.Account.BusinessName)
                    .SetTextAlignment(TextAlignment.CENTER).SetFont(boldFont).SetFontSize(20);
            document.Add(header);
            document.Add(new Paragraph(" "));

            // Informations de l'entreprise
            document.Add(paragrahElement("MOBILE: ", receipt.Account.Phone.ToString()));
            document.Add(paragrahElement("TIN: ", receipt.Account.Tin.ToString()));
            document.Add(paragrahElement("VRN: ", receipt.Account.Vrn));
            document.Add(paragrahElement("SERIAL: ", receipt.Account.Serial));
            document.Add(paragrahElement("UIN: ", receipt.Account.Uin));
            document.Add(paragrahElement("TAX OFFICE: ", receipt.Account.TaxOffice));
            document.Add(new Paragraph(" "));

            LineSeparator separator = new LineSeparator(new SolidLine());
            document.Add(separator);
            document.Add(new Paragraph(" "));

            document.Add(paragrahElement("CUSTOMER NAME: ", receipt.CustName));
            document.Add(paragrahElement("CUSTOMER ID TYPE: ", receipt.CustIdType.ToString()));
            document.Add(paragrahElement("CUSTOMER ID: ", receipt.CustId));
            document.Add(paragrahElement("CUSTOMER MOBILE: ", receipt.CustNum.ToString()));
            document.Add(paragrahElement("RECEIPT NO: ", receipt.RctNum));
            document.Add(paragrahElement("ZNUM: ", receipt.Znum));
            document.Add(paragrahElement("DATE: ", receipt.RctDate.ToString()));
            document.Add(new Paragraph(" "));

            document.Add(separator);

            // Tableau des produits
            Table table = GetItemList(id);
            document.Add(table);

            document.Add(separator);

            // Montant total
            document.Add(paragrahElement("TOTAL EXCL OF TAX: ", receipt.TotalTaxExcl?.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture)));
            //document.Add(paragrahElement("TOTAL TAX: ", (receipt.TotalTaxIncl - receipt.TotalTaxExcl)?.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture)));
            document.Add(paragrahElement("TOTAL INCL OF TAX: ", receipt.TotalTaxIncl?.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture)));
            Images qrImage = new Images(ImageDataFactory.Create(GenerateQrCode(receipt.LinkVerification)));
            qrImage.SetWidth(100);
            qrImage.SetHeight(100);
            qrImage.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            qrImage.SetMargins(10, 70, 10, 60);
            document.Add(qrImage);
        }

        public async Task<ActionResult> DownloadInvoice(int id)
        {
            var receipt = await _context.Receipts
                .Include(r => r.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            var Items = _context.ReceiptItems.Where(r => r.ReceiptID == id).ToList();
            // Générez la facture au format PDF
            GenerateInvoice(id, receipt, Items.Count);

            // Obtenez le chemin complet du fichier PDF généré
            string filePath = $"{_webHostEnvironment.WebRootPath}\\Reports\\facture.pdf";

            // Renvoyez le fichier PDF en tant que réponse
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = "receipt.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }

        public Table GetItemList(int id)
        {
            var Items = _context.ReceiptItems.Where(r => r.ReceiptID == id).ToList();
            Table table = new Table(3);
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.AddCell("Description");
            table.AddCell("Price");
            table.AddCell("Quantity");
            foreach(var item in Items)
            {
                table.AddCell(item.Description);
                table.AddCell(item.Price.ToString());
                table.AddCell(item.Quantity.ToString());
            }
            return table;
        }



        // GET: Receipts
        public async Task<IActionResult> Index()
        {
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            var operatorMatch = _context.OperatorMatchs.FirstOrDefault(r => r.OperatorID == CurrentUser.Id);
            var applicationDbContext = _context.Receipts.Where(r => r.AccountID == operatorMatch.BussinessId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Receipts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipts
                .Include(r => r.ReceiptItems)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // POST: Receipts/CreateItem
        [HttpPost]
        public async Task<bool> CreateItem(ICollection<ReceiptItem> ReceiptItems)
        {
            foreach (var item in ReceiptItems)
            {
                _context.ReceiptItems.Add(item);
            }

            await _context.SaveChangesAsync();
            return true;
            /*try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }*/
        }


        // GET: Receipts/Create
        public IActionResult Create()
        {
            ViewData["AccountID"] = new SelectList(_context.Accounts, "Id", "Id");
            return View();
        }

        // POST: Receipts/CreateReceipt
        [HttpPost]
        public async Task<IActionResult> CreateReceipt([Bind("Id,RctNum,Znum,TotalTaxExcl,TotalTaxIncl,CustName,CustIdType,CustId,CustNum,PaymentType")] Receipt receipt, ICollection<ReceiptItem> ReceiptItems)
        {
            //if (ModelState.IsValid)
            //{
                foreach (var item in ReceiptItems)
                {
                    if (item.Tax == 1) { item.Tax = 0.18; }  
                    _context.ReceiptItems.Add(item);
                }
                await _context.SaveChangesAsync();
                var rcts = _context.Receipts.ToList();
                int num = rcts.Count() + 1;
                var customer = _context.Customers.FirstOrDefault(c => c.Id == int.Parse(receipt.CustName));
                var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                var operatorMatch =  _context.OperatorMatchs.FirstOrDefault(r => r.OperatorID == CurrentUser.Id);
                var Items = _context.ReceiptItems.Where(r => r.IsNew == true).ToList();
                receipt.RctDate = DateTime.Now.Date;
                receipt.RctTime = DateTime.Now.TimeOfDay;
                receipt.CustName = customer.FirstName + ' ' + customer.LastName;
                receipt.CustId = customer.CustId;
                receipt.CustIdType = customer.CustIdType.ToString();
                receipt.RctNum = num.ToString("D6");
                receipt.isPost = false;
                receipt.AccountID = operatorMatch.BussinessId;
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Add(receipt);
                        await _context.SaveChangesAsync();

                        foreach (var item in Items)
                        {
                            item.ReceiptID = receipt.Id;
                            item.IsNew = false;
                            _context.ReceiptItems.Update(item);
                        }

                        await _context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                string urlDetail = Url.Action("Details", "Receipts", new { id = receipt.Id });
                return Json(new { url = urlDetail });
            //}
            //ViewData["AccountID"] = new SelectList(_context.Accounts, "Id", "Id", receipt.AccountID);
            //return View(receipt);
        }

        // GET: Receipts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipts.Include(r => r.ReceiptItems).FirstOrDefaultAsync(r => r.Id == id);
            if (receipt == null)
            {
                return NotFound();
            }
            ViewData["AccountID"] = new SelectList(_context.Accounts, "Id", "Id", receipt.AccountID);
            return View(receipt);
        }

        // POST: Receipts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RctNum,Znum,RctDate,RctTime,TotalTaxExcl,TotalTaxIncl,CustName,CustIdType,CustId,CustNum,AccountID")] Receipt receipt)
        {
            if (id != receipt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receipt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceiptExists(receipt.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountID"] = new SelectList(_context.Accounts, "Id", "Id", receipt.AccountID);
            return View(receipt);
        }

        // GET: Receipts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipts
                .Include(r => r.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // POST: Receipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receipt = await _context.Receipts.FindAsync(id);
            if (receipt != null)
            {
                _context.Receipts.Remove(receipt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public List<Customer> GetCustomers()
        {
            var customers = new List<Customer>();
            customers = _context.Customers.ToList();
            return customers;
        }

        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            products = _context.Products.ToList();
            return products;
        }

        public async Task<ActionResult> PostReceipt(int id)
        {

            var tokenResponse = await APIService.GetAccessToken("demo", "rehema");
            if (bool.Parse(tokenResponse["isError"]?.ToString()) == false)
            {
                var token = tokenResponse["accessToken"]?.ToString();
                var receipt = await _context.Receipts
                .Include(r => r.ReceiptItems)
                .FirstOrDefaultAsync(m => m.Id == id);
                var jsonResponse = await APIService.PostGenerateReceipt(token, receipt);
                var zNum = jsonResponse["znum"]?.ToString();
                var totalExclOfTax = jsonResponse["total_excl_of_tax"]?.ToString();
                var totalTax = jsonResponse["total_tax"]?.ToString();
                var totalInclOfTax = jsonResponse["total_incl_of_tax"]?.ToString();
                var link = jsonResponse["link"]?.ToString();
                receipt.Znum = zNum;
                receipt.TotalTaxExcl = double.Parse(totalExclOfTax);
                receipt.TotalTaxIncl = double.Parse(totalInclOfTax);
                receipt.TotalTax = double.Parse(totalTax);
                receipt.LinkVerification = link;
                receipt.isPost = true;
                _context.Update(receipt);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Receipts", new { id = id });
            }
            return RedirectToAction("Details", "Receipts", new { id = id });
        }

        public async Task<ActionResult> Zreport()
        {
            try
            {
                var tokenResponse = await APIService.GetAccessToken("demo", "rehema");
                if (bool.Parse(tokenResponse["isError"]?.ToString()) == false)
                {
                    var token = tokenResponse["accessToken"]?.ToString();
                    var jsonResponse = await APIService.GetZReports(token);
                    List<ZreportViewModel> zreports = new List<ZreportViewModel>();
                    if (jsonResponse["isError"] == null)
                    {
                        var zreportsResponse = jsonResponse["zreport"]?.ToArray();
                        foreach (var report in zreportsResponse)
                        {
                            string subtotalstr = report["subtotal"]?.ToString().Substring(0, (int)report["subtotal"]?.ToString().IndexOf("."));
                            string totalstr = report["total"]?.ToString().Substring(0, (int)report["total"]?.ToString().IndexOf("."));
                            string totalGrossStr = report["total_gross"]?.ToString().Substring(0, (int)report["total_gross"]?.ToString().IndexOf("."));
                            var subtotal = double.Parse(subtotalstr);
                            var total = double.Parse(totalstr);
                            var totalGross = double.Parse(totalGrossStr);
                            var Zreport = new ZreportViewModel();
                            Zreport.ReportNumber = report["report_number"]?.ToString();
                            Zreport.ReportDate = report["report_date"]?.ToString();
                            Zreport.ReportTime = report["report_time"]?.ToString();
                            Zreport.SubTotal = subtotal.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture); //report["subtotal"]?.ToString(); 
                            Zreport.Discount = report["discount"]?.ToString();
                            Zreport.Total = total.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture); // report["total"]?.ToString();
                            Zreport.Vat = report["vat"]?.ToString();
                            Zreport.TotalGross = totalGross.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture);//  report["total_gross"]?.ToString();
                            zreports.Add(Zreport);
                        }
                        ViewBag.isError = false;
                        return View(zreports);
                    }
                    else
                    {
                        ViewBag.isError = true;
                        ViewBag.ErrorStatus = jsonResponse["ErrorStatus"];
                        ViewBag.Error = jsonResponse["Error"];
                        return View();
                    }
                }
                else
                {
                    ViewBag.isError = true;
                    ViewBag.ErrorStatus = tokenResponse["ErrorStatus"];
                    ViewBag.Error = tokenResponse["Error"];
                    return View();
                }
            }catch(HttpRequestException ex)
            {
                ViewBag.isError = true;
                ViewBag.Error = "Connection Error, we were unable to connect";
                return View();
            }
        }

        public IActionResult search()
        {
            return View();
        }

        public async Task<IActionResult> searchReceipt(SearchAccountModel search)
        {
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            var operatorMatch = _context.OperatorMatchs.FirstOrDefault(r => r.OperatorID == CurrentUser.Id);
            List<Receipt> receipts = new List<Receipt>();
            if (search.FromC != null && search.ToC != null)
            {
                receipts = _context.Receipts
                        .Where(t => t.RctDate >= search.FromC && t.RctDate <= search.ToC && t.AccountID == operatorMatch.BussinessId)
                        .ToList();
                return View("Index", receipts);
            }

            return View("Index", receipts);
        }

        private bool ReceiptExists(int id)
        {
            return _context.Receipts.Any(e => e.Id == id);
        }
    }
}
