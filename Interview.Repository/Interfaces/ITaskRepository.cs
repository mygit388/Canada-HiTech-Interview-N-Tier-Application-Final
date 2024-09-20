using Interview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Repository.Interfaces
{
    public interface ITaskRepository
    {
        List<TaskModel> GetTasks(int ProfileId);
        bool Insert(TaskModel model);
        TaskModel TaskByID(int Id);
        bool Update(TaskModel model);
        void Delete(int Id);
    }
}
