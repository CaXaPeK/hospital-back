using Hospital.Database.Icd;
using Hospital.Database.TableModels;
using System;

namespace Hospital.Database
{
    public class DictionaryDataFiller : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DictionaryDataFiller(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!dbContext.Diagnoses.Any())
                {
                    IcdCsvParser parser = new IcdCsvParser();
                    parser.LoadAndReformatCsv("Database/Icd/Icd.csv");
                    List<Diagnosis> diagnoses = parser.GetDiagnosesList();

                    foreach (Diagnosis diagnosis in diagnoses)
                    {
                        dbContext.Diagnoses.Add(diagnosis);
                    }

                    await dbContext.SaveChangesAsync();
                }

                if (!dbContext.Specialities.Any())
                {
                    dbContext.Specialities.Add(new Speciality("Акушер-гинеколог"));
                    dbContext.Specialities.Add(new Speciality("Анестезиолог-реаниматолог"));
                    dbContext.Specialities.Add(new Speciality("Дерматовенеролог"));
                    dbContext.Specialities.Add(new Speciality("Инфекционист"));
                    dbContext.Specialities.Add(new Speciality("Кардиолог"));
                    dbContext.Specialities.Add(new Speciality("Невролог"));
                    dbContext.Specialities.Add(new Speciality("Онколог"));
                    dbContext.Specialities.Add(new Speciality("Отоларинголог"));
                    dbContext.Specialities.Add(new Speciality("Офтальмолог"));
                    dbContext.Specialities.Add(new Speciality("Психиатр"));
                    dbContext.Specialities.Add(new Speciality("Психолог"));
                    dbContext.Specialities.Add(new Speciality("Рентгенолог"));
                    dbContext.Specialities.Add(new Speciality("Стоматолог"));
                    dbContext.Specialities.Add(new Speciality("Терапевт"));
                    dbContext.Specialities.Add(new Speciality("УЗИ-специалист"));
                    dbContext.Specialities.Add(new Speciality("Уролог"));
                    dbContext.Specialities.Add(new Speciality("Хирург"));
                    dbContext.Specialities.Add(new Speciality("Эндокринолог"));

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
