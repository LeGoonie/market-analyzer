using MarketAnalyzer.Data;
using MarketAnalyzer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Business
{
    public class TickerDataBO
    {
        public void SaveToggleBuyTicker(Guid companyId, string buyId, bool isChecked)
        {
            using (var context = new MarketAnalyzerDB2Context())
            {
                var company = context.Companies.SingleOrDefault(c => c.Id == companyId);
                if (buyId == "buyR")
                    company.BuyR = isChecked;

                if (buyId == "buyL")
                    company.BuyL = isChecked;
                context.SaveChanges();
            }
        }

        public void SaveTickerNotes(Guid companyId, string notes)
        {
            using (var context = new MarketAnalyzerDB2Context())
            {
                var company = context.Companies.SingleOrDefault(c => c.Id == companyId);
                company.Notes = notes;
                context.SaveChanges();
            }
        }

        public string GetNoteFromUserCompany(Guid companyId, string userId)
        {
            var genericDao = new GenericDao<UserNote>();
            var note = genericDao.GetListBySync(x => x.CompanyId == companyId && x.UserId == userId).OrderByDescending(x=> x.DateCreated).FirstOrDefault();
            if (note == null) return "";
            return note.Note;
        }

        public void AddNote(Guid companyId, string userId, string note)
        {
            var genericDao = new GenericDao<UserNote>();
            var newUserNote = new UserNote
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                UserId = userId,
                Note = note,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };
            genericDao.AddSync(newUserNote);
        }

        public List<UserNote> GetAllNotesFromUserCompany(Guid companyId, string userId)
        {
            var genericDao = new GenericDao<UserNote>();
            var notes = genericDao.GetListBySync(x => x.CompanyId == companyId && x.UserId == userId).OrderByDescending(x => x.DateCreated).ToList();
            /*var notesList = new List<string>();
            foreach(var item in notes)
            {
                if (item.Note != null)
                {
                    notesList.Add(item.Note);
                }

            }*/
            if (notes.Count() == 0)
            {
                var errorUserNote = new UserNote();
                errorUserNote.Note = "No previous notes";
                errorUserNote.DateCreated = DateTime.UtcNow;
                notes.Add(errorUserNote);
            }
            return notes;
        }
    }
}