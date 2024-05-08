using BAL.Interfaces.IExcelMethods;
using ClosedXML.Excel;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BAL.Repository.ExcelMethodsRepository
{
    public class ExcelExportRepo : IExcelExport
    {

        private readonly ApplicationDbContext _context;
        public ExcelExportRepo(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
