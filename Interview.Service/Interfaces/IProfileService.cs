using Interview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Service.Interfaces
{
    public interface IProfileService
    {
        List<ProfileModel> GetAll();
        bool InsertProfile(ProfileModel model);
        ProfileModel getProfileByID(int ProfileId);
        bool UpdateProfile(ProfileModel model);
        void DeleteProfile(int ProfileId);
    }
}
