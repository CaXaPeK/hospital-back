﻿using Hospital.Database;
using Hospital.Database.TableModels;
using Hospital.Models.General;
using Hospital.Models.Icd;
using Hospital.Models.Speciality;
using Hospital.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Drawing;

namespace Hospital.Services.Logic
{
    public class DictionaryService : IDictionaryService
    {
        private readonly AppDbContext _dbContext;

        public DictionaryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SpecialtiesPagedListModel> GetSpecialitiesList(string? name, int page, int size)
        {
            var specialities = _dbContext.Specialities
                .OrderBy(x => x.Name)
                .AsQueryable();

            if (name != "" && name != null)
            {
                specialities = specialities.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }

            var pagedSpecialities = Paginate(specialities, page, size);

            var pageCount = (int)Math.Ceiling((double)specialities.Count() / size);

            if (page > pageCount)
            {
                throw new InvalidOperationException("Invalid page");
            }

            var dto = new SpecialtiesPagedListModel
            {
                Specialities = await pagedSpecialities
                    .Select(speciality => new SpecialityModel
                    {
                        Name = speciality.Name,
                        Id = speciality.Id,
                        CreateTime = speciality.CreateDate
                    })
                    .ToListAsync(),
                Pagination = new PageInfoModel
                {
                    Size = size,
                    Count = pageCount,
                    Current = page
                }
            };

            return dto;
        }

        public async Task<List<Icd10RecordModel>> GetRootDiagnoses()
        {
            var diagnoses = await _dbContext.Diagnoses
                .Where(x => x.ParentId == null)
                .OrderBy(x => x.MkbCode)
                .Select(diagnosis => new Icd10RecordModel
                {
                    Id = diagnosis.Id,
                    CreateTime = diagnosis.CreateDate,
                    Name = diagnosis.MkbName,
                    Code = diagnosis.MkbCode
                }).ToListAsync();

            return diagnoses;
        }

        private IQueryable<Speciality> Paginate(IQueryable<Speciality> specialities, int page, int size)
        {
            return specialities.Skip((page - 1) * size).Take(size);
        }
    }
}
