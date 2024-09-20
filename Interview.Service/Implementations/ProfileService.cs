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
    public class ProfileService : IProfileService
    {
        private IProfileRepository _profileRepository;
        ILoggerService _iLog;
        public ProfileService(IProfileRepository profileRepository, ILoggerService iLog)
        {
            _profileRepository = profileRepository;
            _iLog = iLog;
        }
        public void DeleteProfile(int ProfileId)
        {
            try
            {
                _profileRepository.Delete(ProfileId);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }

        }

        public List<ProfileModel> GetAll()
        {
            try
            {
                return _profileRepository.GetAll();
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
            
        }

        public ProfileModel getProfileByID(int ProfileId)
        {
            try
            {
                return _profileRepository.GetById(ProfileId);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
            
        }

        public bool InsertProfile(ProfileModel model)
        {
            try
            {
                return _profileRepository.Insert(model);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
            
        }

        public bool UpdateProfile(ProfileModel model)
        {
            try
            {
                return _profileRepository.Update(model);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
            
        }
    }
}
