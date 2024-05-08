using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class EncounterFormRepo : IEncounterForm
    {
        private readonly ApplicationDbContext _context;
        public EncounterFormRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void EncounterFormPost(int requestid, EncounterFormViewModel model)
        {
            var check = _context.Encounterforms.FirstOrDefault(x => x.Requestid == requestid);
            if (check == null)
            {
                Encounterform ef = new()
                {
                    Historyofpresentillnessorinjury = model.IllnessHistory,
                    Medicalhistory = model.MedicalHistory,
                    Medications = model.Medications,
                    Allergies = model.Allergies,
                    Temp = model.Temperature,
                    Hr = model.HR,
                    Rr = model.RR,
                    Bloodpressuresystolic = model.BPLow,
                    Bloodpressurediastolic = model.BPHigh,
                    O2 = model.O2,
                    Pain = model.Pain,
                    Heent = model.heent,
                    Cv = model.cv,
                    Chest = model.chest,
                    Abd = model.abd,
                    Extremities = model.extr,
                    Skin = model.skin,
                    Neuro = model.neuro,
                    TreatmentPlan = model.treatmentPlan,
                    Medicaldispensed = model.MedicationsDispensed,
                    Procedures = model.procedures,
                    Followup = model.followUps,
                    Requestid = model.requestId,
                    Diagnosis = model.diagnosis
                };
                _context.Add(ef);
                _context.SaveChanges();

            }
            if (check != null)
            {
                Encounterform ef = new()
                {
                    Historyofpresentillnessorinjury = model.IllnessHistory,
                    Medicalhistory = model.MedicalHistory,
                    Medications = model.Medications,
                    Allergies = model.Allergies,
                    Temp = model.Temperature,
                    Hr = model.HR,
                    Rr = model.RR,
                    Bloodpressuresystolic = model.BPLow,
                    Bloodpressurediastolic = model.BPHigh,
                    O2 = model.O2,
                    Pain = model.Pain,
                    Heent = model.heent,
                    Cv = model.cv,
                    Chest = model.chest,
                    Abd = model.abd,
                    Extremities = model.extr,
                    Skin = model.skin,
                    Neuro = model.neuro,
                    TreatmentPlan = model.treatmentPlan,
                    Medicaldispensed = model.MedicationsDispensed,
                    Procedures = model.procedures,
                    Followup = model.followUps,
                    Requestid = model.requestId,
                    Diagnosis = model.diagnosis
                };
                _context.Update(ef);
                _context.SaveChanges();
            }

        }
        public EncounterFormViewModel EncounterFormGet(int requestid)
        {
            var check = _context.Encounterforms.FirstOrDefault(x => x.Requestid == requestid);
            var user = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);
            EncounterFormViewModel EncModel = new EncounterFormViewModel()
            {
                FirstName = user.Firstname,
                LastName = user.Lastname,
                requestId = requestid
            };

            if (check != null)
            {
                EncModel.abd = check.Abd;
                EncModel.procedures = check.Procedures;
                EncModel.PhoneNo = user.Phonenumber;
                EncModel.MedicalHistory = check.Medicalhistory;
                EncModel.MedicationsDispensed = check.Medicaldispensed;
                EncModel.Allergies = check.Allergies;
                EncModel.Pain = check.Pain;
                EncModel.RR = check.Rr;
                EncModel.HR = check.Hr;
                EncModel.BPLow = check.Bloodpressuresystolic;
                EncModel.BPHigh = check.Bloodpressurediastolic;
                EncModel.O2 = check.O2;
                EncModel.other = check.Other;
                EncModel.skin = check.Skin;
                EncModel.Temperature = check.Temp;
                EncModel.chest = check.Chest;
                EncModel.IllnessHistory = check.Historyofpresentillnessorinjury;
                EncModel.heent = check.Heent;
                EncModel.cv = check.Cv;
                EncModel.extr = check.Extremities;
                EncModel.neuro = check.Neuro;
                EncModel.followUps = check.Followup;
                EncModel.diagnosis = check.Diagnosis;
                EncModel.Location = user.Address;
                EncModel.Email = user.Email;
                EncModel.Medications = check.Medications;
                EncModel.other = check.Other;
                EncModel.treatmentPlan = check.TreatmentPlan;
                //EncModel.CallType = (short)user.Request.Calltype ;

            }
            return EncModel;
        }
    }
}
