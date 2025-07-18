using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using LoanCalculatorApp.Models;
using System;
using System.Collections.Generic;



namespace LoanCalculatorApp.Controllers
{
    public class LoanController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Foreclosure() => View();

        [HttpPost]
        public IActionResult Generate(double principal, double rate, int months)
        {
            double r = rate / 12;
            double emi = principal * r * Math.Pow(1 + r, months) / (Math.Pow(1 + r, months) - 1);

            var schedule = new List<AmortizationEntry>();
            double remaining = principal;

            for (int i = 1; i <= months; i++)
            {
                double interest = remaining * r;
                double principalPaid = emi - interest;
                remaining -= principalPaid;
                if (remaining < 0) remaining = 0;

                schedule.Add(new AmortizationEntry
                {
                    Month = i,
                    EMI = Math.Round(emi, 2),
                    InterestPaid = Math.Round(interest, 2),
                    PrincipalPaid = Math.Round(principalPaid, 2),
                    RemainingBalance = Math.Round(remaining, 2)
                });
            }

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Schedule");

            ws.Cells[1, 1].Value = "Month";
            ws.Cells[1, 2].Value = "EMI";
            ws.Cells[1, 3].Value = "Interest Paid";
            ws.Cells[1, 4].Value = "Principal Paid";
            ws.Cells[1, 5].Value = "Remaining Balance";

            for (int i = 0; i < schedule.Count; i++)
            {
                var row = i + 2;
                ws.Cells[row, 1].Value = schedule[i].Month;
                ws.Cells[row, 2].Value = schedule[i].EMI;
                ws.Cells[row, 3].Value = schedule[i].InterestPaid;
                ws.Cells[row, 4].Value = schedule[i].PrincipalPaid;
                ws.Cells[row, 5].Value = schedule[i].RemainingBalance;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "loan_schedule.xlsx");
        }

        [HttpPost]
        public IActionResult CalculateForeclosure(double principal, double rate, int months, int forecloseMonth)
        {
            double r = rate / 12;
            double emi = principal * r * Math.Pow(1 + r, months) / (Math.Pow(1 + r, months) - 1);

            double remaining = principal;
            for (int i = 1; i <= forecloseMonth; i++)
            {
                double interest = remaining * r;
                double principalPaid = emi - interest;
                remaining -= principalPaid;
                if (remaining < 0)
                {
                    remaining = 0;
                    break;
                }
            }

            double charge = Math.Round(0.04 * remaining, 2);
            double gst = Math.Round(0.18 * charge, 2);
            double total = Math.Round(remaining + charge + gst, 2);

            ViewBag.Month = forecloseMonth;
            ViewBag.Remaining = Math.Round(remaining, 2);
            ViewBag.Charge = charge;
            ViewBag.GST = gst;
            ViewBag.Total = total;

            return View("Foreclosure");
        }
    }
}
