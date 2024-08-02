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
                pageSize.SetHeight(625);
            }
            else
            {
                var addsize = (numberOfElement - 1) * 25;
                pageSize.SetHeight(addsize + 625);
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

        private void AddInvoiceContent(Document document, int id, Receipt receipt)
        {

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
            document.Add(paragrahElement("CUSTOMER ID TYPE: ", receipt.CustIdType));
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
            document.Add(paragrahElement("TOTAL EXCL OF TAX: ", receipt.TotalTaxExcl.ToString()));
            document.Add(paragrahElement("TOTAL TAX: ", (receipt.TotalTaxIncl - receipt.TotalTaxExcl).ToString()));
            document.Add(paragrahElement("TOTAL INCL OF TAX: ", receipt.TotalTaxIncl.ToString()));
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


        // GET: Receipts/PrintReceipt/5
       /* public async Task<IActionResult> PrintReceipt(int id)
        {
            var receipt = await _context.Receipts
                .Include(r => r.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            var dt = new DataTable();
            dt = GetItemList(id);
            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("BName", receipt.Account.BusinessName));
            parameters.Add(new ReportParameter("BPhone", receipt.Account.Phone.ToString()));
            parameters.Add(new ReportParameter("Tin", receipt.Account.Tin.ToString()));
            parameters.Add(new ReportParameter("Vrn", receipt.Account.Vrn));
            parameters.Add(new ReportParameter("Serial", receipt.Account.Serial));
            parameters.Add(new ReportParameter("Uin", receipt.Account.Uin));
            parameters.Add(new ReportParameter("TaxOffice", receipt.Account.TaxOffice));
            parameters.Add(new ReportParameter("CustName", receipt.CustName));
            parameters.Add(new ReportParameter("CustIdType", receipt.CustIdType));
            parameters.Add(new ReportParameter("CustId", receipt.CustId));
            parameters.Add(new ReportParameter("CustNum", receipt.CustNum.ToString()));
            parameters.Add(new ReportParameter("RctNum", receipt.RctNum));
            parameters.Add(new ReportParameter("Znum", receipt.Znum));
            parameters.Add(new ReportParameter("RctDate", receipt.RctDate.ToString()));
            //parameters.Add(new ReportParameter("RctTime", receipt.RctTime.ToString()));
            parameters.Add(new ReportParameter("TotalExclTax", receipt.TotalTaxExcl.ToString()));
            parameters.Add(new ReportParameter("TotalTax", (receipt.TotalTaxIncl - receipt.TotalTaxExcl).ToString()));
            parameters.Add(new ReportParameter("TotalInclTax",receipt.TotalTaxIncl.ToString()));

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\Receipts.rdlc";
            //ReportDataSource reportDataSource = new ReportDataSource();
            //reportDataSource.Name = "receiptDataSet";
            //reportDataSource.Value = dt;
            localReport.DataSources.Add(new ReportDataSource("Receipts",dt));
            localReport.SetParameters(parameters);

            var reportBytes = localReport.Render("PDF");
            return File(reportBytes, "application/pdf", "Receipt.pdf");
        }*/



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

        // POST: Receipts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RctNum,Znum,TotalTaxExcl,TotalTaxIncl,CustName,CustIdType,CustId,CustNum")] Receipt receipt)
        {
            //if (ModelState.IsValid)
            //{
               
                var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                var operatorMatch =  _context.OperatorMatchs.FirstOrDefault(r => r.OperatorID == CurrentUser.Id);
                var Items = _context.ReceiptItems.Where(r => r.IsNew == true).ToList();
                receipt.RctDate = DateTime.Now.Date;
                receipt.RctTime = DateTime.Now.TimeOfDay;
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
                return RedirectToAction(nameof(Index));
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

            var receipt = await _context.Receipts.FindAsync(id);
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

        private bool ReceiptExists(int id)
        {
            return _context.Receipts.Any(e => e.Id == id);
        }
    }
}
