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
using System.Security.Authentication;

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
                .OrderBy(s => s.Name)
                .AsQueryable();

            if (name != "" && name != null)
            {
                specialities = specialities.Where(s => s.Name.ToLower().Contains(name.ToLower()));
            }

            var pagedSpecialities = PaginateSpecialities(specialities, page, size);

            var pageCount = (int)Math.Ceiling((double)specialities.Count() / size);

            //вроде в вашем API такая же логика
            if (page > pageCount && pageCount != 0)
            {
                throw new InvalidCredentialException("Invalid page");
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
                .Where(d => d.ParentId == null)
                .OrderBy(d => d.MkbCode)
                .Select(diagnosis => new Icd10RecordModel
                {
                    Id = diagnosis.Id,
                    CreateTime = diagnosis.CreateDate,
                    Name = diagnosis.MkbName,
                    Code = diagnosis.MkbCode
                }).ToListAsync();

            return diagnoses;
        }

        public async Task<Icd10SearchModel> GetDiagnosesList(string? request, int page, int size)
        {
            var diagnoses = _dbContext.Diagnoses
                .OrderBy(x => x.MkbCode)
                .AsQueryable();

            if (request != "" && request != null)
            {
                diagnoses = diagnoses.Where(d => d.MkbName.ToLower().Contains(request.ToLower()) || d.MkbCode.ToLower().Contains(request.ToLower()));
            }

            var pagedDiagnoses = PaginateDiagnoses(diagnoses, page, size);

            var pageCount = (int)Math.Ceiling((double)diagnoses.Count() / size);

            if (page > pageCount && pageCount != 0)
            {
                throw new InvalidCredentialException("Invalid page");
            }

            var dto = new Icd10SearchModel
            {
                Records = await pagedDiagnoses
                    .Select(diagnosis => new Icd10RecordModel
                    {
                        Id = diagnosis.Id,
                        CreateTime = diagnosis.CreateDate,
                        Name = diagnosis.MkbName,
                        Code = diagnosis.MkbCode
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

        public bool SpecialityExists(Guid id)
        {
            return _dbContext.Specialities.FirstOrDefault(s => s.Id == id) != null;
        }

        public bool DiagnosisExists(Guid id)
        {
            return _dbContext.Diagnoses.FirstOrDefault(d => d.Id == id) != null;
        }

        private IQueryable<Speciality> PaginateSpecialities(IQueryable<Speciality> specialities, int page, int size)
        {
            return specialities.Skip((page - 1) * size).Take(size);
        }

        private IQueryable<Diagnosis> PaginateDiagnoses(IQueryable<Diagnosis> diagnoses, int page, int size)
        {
            return diagnoses.Skip((page - 1) * size).Take(size);
        }
    }
}
