using Entities;
using Factory.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using Notification.Interfaces;
using Notification.Models;
using PersonalTracking.DAL.DataStorage;
using PersonalTracking.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTracking.DAL.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly Context _context;

        private INotificationService _notificationService;
        private IModelFactory<Department, DepartmentDto> _factory;

        public DepartmentRepository(Context context, INotificationService notificationService, IModelFactory<Department, DepartmentDto> factory)
        {
            _context = context;
            _notificationService = notificationService;
            _factory = factory;
        }

        public async Task<IEnumerable<Department>> GetDepartments()
        {
            try
            {
                var models = await _context.Departments.AsNoTracking().ToListAsync(); // Obtém todas as entidades de departamento
                return models;
            }
            catch (Exception ex)
            {
                _notificationService.AddError(ex.Message);
                return null;
            }
        }

        public async Task<Department> CreateDepartment(Department department)
        {
            try
            {
                var entity = _factory.SetModelWhithoutValidation(department);
                _context.Departments.Add(entity); // Insere a entidade no contexto
                await _context.SaveChangesAsync(); // Salva as alterações no banco de dados
                return entity;
            }
            catch (Exception ex)
            {
                _notificationService.AddError(ex.Message);
                return department;
            }
        }

        public async Task<Department> GetDepartmentById(int? id)
        {
            try
            {
                var entity = await _context.Departments.FindAsync(id);
                return entity;
            }
            catch (Exception ex)
            {
                _notificationService.AddError(ex.Message);
                return null;
            }
        }

        public async Task<Department> UpdateDepartment(Department model)
        {
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return model; // Retorna o modelo de departamento atualizado
            }
            catch (Exception ex)
            {
                _notificationService.AddError(ex.Message);
                return model;
            }
        }

        public async Task<Department> RemoveDepartment(Department model)
        {
            try
            {
                _context.Remove(model);
                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                _notificationService.AddError(ex.Message);
                return model;
            }
        }

        public List<NotificationMessage> GetNotifications()
        {
            return _notificationService.Messages;
        }
    }
}