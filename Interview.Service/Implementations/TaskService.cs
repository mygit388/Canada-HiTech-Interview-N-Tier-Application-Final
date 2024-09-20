using Interview.Model;
using Interview.Repository.Interfaces;
using Interview.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Service.Implementations
{
    public class TaskService:ITaskService
    {
        ILoggerService _iLog;
        private ITaskRepository _taskRepository;
        public TaskService(ITaskRepository taskRepository, ILoggerService iLog)
        {
            _iLog = iLog;
            _taskRepository = taskRepository;
        }

        public void DeleteTask(int Id)
        {
            try
            {
                _taskRepository.Delete(Id);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
        }

        public TaskModel getTaskByID(int Id)
        {
            try
            {
                return _taskRepository.TaskByID(Id);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
        }

        public List<TaskModel> GetTasks(int ProfileId)
        {
            try
            {
                return _taskRepository.GetTasks(ProfileId);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
        }

        public bool TaskAdding(TaskModel model)
        {
            try
            {
                return _taskRepository.Insert(model);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
        }

        public bool UpdatingTask(TaskModel model)
        {
            try
            {
                return _taskRepository.Update(model);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
        }
    }
}
