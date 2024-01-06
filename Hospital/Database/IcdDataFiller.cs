using Hospital.Database.TableModels;
using System;

namespace Hospital.Database
{
    public class IcdDataFiller : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public IcdDataFiller(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IcdDbContext>();
                if (!dbContext.Diagnoses.Any())
                {
                    IcdCsvParser parser = new IcdCsvParser();
                    parser.LoadAndReformatCsv("Database/Icd.csv");
                    List<Diagnosis> diagnoses = parser.GetDiagnosesList();

                    foreach(Diagnosis diagnosis in diagnoses)
                    {
                        dbContext.Diagnoses.Add(diagnosis);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
