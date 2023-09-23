using Microsoft.EntityFrameworkCore;
using PatientInformationsAPI.Model;

namespace PatientInformationsAPI.Repository.PatientDetails
{
    public class PatientDetailsRepository: IPatientDetailsRepository
    {
        private readonly PatientInformationContext _patientInformationContext;

        public PatientDetailsRepository(PatientInformationContext patientInformationContext)
        {
            _patientInformationContext = patientInformationContext;
        }

        public async Task<List<PatientInformation>> PatientInformationDetails()
        {
            var details= _patientInformationContext.PatientInformations.Include(x=>x.Disease)
                .Include(x=>x.NcdDetails).Include(x=>x.AllergiesDetails).ToList();
            return details;
        }

        public async Task<string> PostPatientInformationDetails(PatientInformationView patientInformation)
        {
            PatientInformation information = new PatientInformation();
            information.Epliepsy = patientInformation.Epilepsy.HasFlag(Model.Enums.Epilepsy.Yes);
            information.DiseaseId = patientInformation.DiseaseInformation.Id;
            information.Name = patientInformation.Name;
            information.ModifierDate = DateTime.Now;
            information.CreationDate = DateTime.Now;
            information.IsActve = true;
            information.Id = Guid.NewGuid().ToString();
            _patientInformationContext.PatientInformations.Add(information);

            foreach (var item in patientInformation.SelectedNCDs)
            {
                _patientInformationContext.NcdDetails.Add(new NcdDetail()
                {
                    Id = Guid.NewGuid().ToString(),
                    Ncdid = item.Id,
                    PatientId = information.Id,
                    CreationDate = DateTime.Now,
                    ModifierDate = DateTime.Now
                });
            }
            foreach (var item in patientInformation.SelectedAllergies)
            {
                _patientInformationContext.AllergiesDetails.Add(new AllergiesDetail()
                {
                    Id = Guid.NewGuid().ToString(),
                    AllergieId = item.Id,
                    PatientId = information.Id,
                    CreationDate = DateTime.Now,
                    ModifierDate = DateTime.Now
                });
            }
            _patientInformationContext.SaveChanges();

            return "Object created";
        }
    }
}
