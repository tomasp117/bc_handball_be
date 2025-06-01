using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;


namespace bc_handball_be.Core.Services
{
    public class ClubAdminService : IClubAdminService
    {
        private readonly IClubAdminRepository _clubAdminRepository;
        private readonly IUserRepository _userRepository;

        public ClubAdminService(IClubAdminRepository clubRepository, IUserRepository userRepository)
        {
            _clubAdminRepository = clubRepository;
            _userRepository = userRepository;

        }

        public async Task<Club> GetClubByPersonIdAsync(int personId)
        {
            var clubAdmin = await _clubAdminRepository.GetByPersonIdAsync(personId);
            if (clubAdmin == null)
            {
                throw new Exception($"ClubAdmin with PersonId {personId} not found");
            }
            return clubAdmin.Club;
        }

        public async Task<ClubAdmin> GetByClubIdAsync(int clubId)
        {
            var clubAdmin = await _clubAdminRepository.GetByClubIdAsync(clubId);
            if (clubAdmin == null)
            {
                //throw new Exception($"ClubAdmin with ClubId {clubId} not found");
                return null;
            }
            return clubAdmin;
        }

        public async Task CreateAsync(ClubAdmin clubAdmin)
        {
            if (clubAdmin == null) throw new ArgumentNullException(nameof(clubAdmin));
            try
            {
                if (await _userRepository.UsernameExistsAsync(clubAdmin.Person.Login.Username))
                {
                    throw new InvalidOperationException($"Username {clubAdmin.Person.Login.Username} already exists.");
                }
                await _userRepository.AddUserWithRoleAsync(clubAdmin.Person, clubAdmin.Person.Login, clubAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating ClubAdmin: {ex.Message}", ex);
            }
        }

    }
}
