using Interview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Repository.Interfaces
{
    public interface IProfileRepository
    {
        List<ProfileModel> GetAll();
        bool Insert(ProfileModel model);
        ProfileModel GetById(int ProfileId);
        bool Update(ProfileModel model);
        void Delete(int ProfileId);
    }
}
