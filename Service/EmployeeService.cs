﻿using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IEmployeeLinks employeeLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _employeeLinks = employeeLinks;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(employee);

            _repository.Employee.CreateEmployee(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeForCompany = await GetEmployeeForCompanyIfExists(companyId, id, trackChanges);

            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeDb = await GetEmployeeForCompanyIfExists(companyId, id, trackChanges);

            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return employee;
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool companyTrackChanges, bool employeeTrackChanges)
        {
            await CheckIfCompanyExists(companyId, companyTrackChanges);

            var employeeEntity = await GetEmployeeForCompanyIfExists(companyId, id, employeeTrackChanges);

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

            return (employeeToPatch, employeeEntity);
        }

        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid companyid, LinkParameters linkParameters, bool trackChanges)
        {
            if (!linkParameters.EmployeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();

            await CheckIfCompanyExists(companyid, trackChanges);

            var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyid, linkParameters.EmployeeParameters, trackChanges);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);

            var links = _employeeLinks.TryGenerateLinks(employeesDto, linkParameters.EmployeeParameters.Fields, companyid, linkParameters.HttpContext);

            return (linkResponse: links, metaData: employeesWithMetaData.MetaData);
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool companyTrackChanges, bool employeeTrackChanges)
        {
            await CheckIfCompanyExists(companyId, companyTrackChanges);

            var employeeEntity = await GetEmployeeForCompanyIfExists(companyId, id, employeeTrackChanges);

            _mapper.Map(employeeForUpdate, employeeEntity);
            await _repository.SaveAsync();
        }

        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }

        private async Task<Employee> GetEmployeeForCompanyIfExists(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeEntity is null)
                throw new EmployeeNotFoundException(id);

            return employeeEntity;
        }
    }
}
