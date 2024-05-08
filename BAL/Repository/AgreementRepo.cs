using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class AgreementRepo : IAgreement
    {
        private readonly ApplicationDbContext _context;
        public AgreementRepo(ApplicationDbContext context) 
        { 
            _context = context;
        }
        public void AgreementAccepted(int Requestid)
        {
            Request req = _context.Requests.FirstOrDefault(x => x.Requestid == Requestid);

            req.Status = 4;
            req.Modifieddate = DateTime.Now;

            _context.Update(req);
            _context.SaveChanges();

            Requeststatuslog requeststatuslog = new Requeststatuslog();

            requeststatuslog.Requestid = Requestid;
            requeststatuslog.Notes = "Agreement Accepted";
            requeststatuslog.Createddate = DateTime.Now;
            requeststatuslog.Status = 4;

            _context.Add(requeststatuslog);
            _context.SaveChanges();
        }

        public void AgreementRejected(int Requestid , string notes)
        {
            Request req = _context.Requests.FirstOrDefault(x => x.Requestid == Requestid);

            req.Status = 7;
            req.Modifieddate = DateTime.Now;

            _context.Update(req);
            _context.SaveChanges();

            Requeststatuslog requeststatuslog = new Requeststatuslog();

            requeststatuslog.Requestid = Requestid;
            requeststatuslog.Notes = notes;
            requeststatuslog.Createddate = DateTime.Now;
            requeststatuslog.Status = 7;

            _context.Add(requeststatuslog);
            _context.SaveChanges();

        }
    }
}
