using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace bc_handball_be.Core.Services
{
    public class ClubRegistrationService : IClubRegistrationService
    {
        private readonly IClubRegistrationRepository _clubRegistrationRepository;
        private readonly IClubService _clubService;
        private readonly IAuthService _authService;
        private readonly ILogger<ClubRegistrationService> _logger;

        public ClubRegistrationService(
            IClubRegistrationRepository clubRegistrationRepository,
            IClubService clubService,
            IAuthService authService,
            ILogger<ClubRegistrationService> logger
        )
        {
            _clubRegistrationRepository = clubRegistrationRepository;
            _clubService = clubService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<ClubRegistration> CreateFullRegistrationAsync(
            Club club,
            Person person,
            string username,
            string password,
            ClubRegistration registrationData
        )
        {
            _logger.LogInformation("Creating full club registration for club: {ClubName}", club.Name);

            try
            {
                // Step 1: Create the Club with Pending status
                club.Status = ClubStatus.Pending;
                await _clubService.AddClubAsync(club);
                _logger.LogInformation("Club created with ID: {ClubId}", club.Id);

                // Step 2: Create Login entity with hashed password
                var login = new Login
                {
                    Username = username,
                    Person = person
                };
                login.SetPassword(password); // Hash the password

                // Step 3: Create ClubAdmin entity linking Person to Club
                var clubAdmin = new ClubAdmin
                {
                    Club = club,
                    Person = person
                };

                // Step 4: Register the user (creates Person, Login, and ClubAdmin in one transaction)
                var registrationSuccess = await _authService.RegisterAsync(person, login, clubAdmin);
                if (!registrationSuccess)
                {
                    _logger.LogError("Failed to register club admin - username {Username} may already exist", username);
                    throw new InvalidOperationException($"Username {username} is already taken.");
                }
                _logger.LogInformation("ClubAdmin registered successfully for club: {ClubName}", club.Name);

                // Step 5: Create the ClubRegistration
                registrationData.Club = club;
                registrationData.ClubId = club.Id;
                registrationData.Status = RegistrationStatus.Pending;
                registrationData.SubmittedDate = DateTime.UtcNow;
                registrationData.CalculatedFee = await CalculateRegistrationFeeAsync(registrationData);

                var createdRegistration = await _clubRegistrationRepository.AddAsync(registrationData);
                _logger.LogInformation(
                    "Club registration created successfully with ID: {RegistrationId}",
                    createdRegistration.Id
                );

                return createdRegistration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating full club registration for club: {ClubName}", club.Name);
                throw;
            }
        }

        public async Task<ClubRegistration> CreateRegistrationAsync(
            ClubRegistration clubRegistration
        )
        {
            _logger.LogInformation(
                "Creating new club registration for ClubId: {ClubId}",
                clubRegistration.ClubId
            );

            // Calculate registration fee
            clubRegistration.CalculatedFee = await CalculateRegistrationFeeAsync(clubRegistration);
            clubRegistration.Status = RegistrationStatus.Pending;
            clubRegistration.SubmittedDate = DateTime.UtcNow;

            return await _clubRegistrationRepository.AddAsync(clubRegistration);
        }

        public async Task<ClubRegistration> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching club registration by ID: {Id}", id);
            return await _clubRegistrationRepository.GetByIdAsync(id);
        }

        public async Task<List<ClubRegistration>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all club registrations");
            return await _clubRegistrationRepository.GetAllAsync();
        }

        public async Task<ClubRegistration> UpdateRegistrationAsync(
            ClubRegistration clubRegistration
        )
        {
            _logger.LogInformation("Updating club registration ID: {Id}", clubRegistration.Id);

            var existingRegistration = await GetByIdAsync(clubRegistration.Id);
            if (existingRegistration == null)
            {
                _logger.LogWarning("Club registration with ID {Id} not found", clubRegistration.Id);
                return null;
            }

            // Update fields
            existingRegistration.PackageACount = clubRegistration.PackageACount;
            existingRegistration.PackageBCount = clubRegistration.PackageBCount;

            // Recalculate fee
            existingRegistration.CalculatedFee = await CalculateRegistrationFeeAsync(
                clubRegistration
            );

            // Update category team counts
            existingRegistration.CategoryTeamCounts = clubRegistration.CategoryTeamCounts;

            return await _clubRegistrationRepository.UpdateAsync(existingRegistration);
        }

        public async Task<bool> DeleteRegistrationAsync(int id)
        {
            _logger.LogInformation("Deleting club registration ID: {Id}", id);
            return await _clubRegistrationRepository.DeleteAsync(id);
        }

        public async Task<List<ClubRegistration>> GetByTournamentInstanceIdAsync(
            int tournamentInstanceId
        )
        {
            _logger.LogInformation(
                "Fetching club registrations for tournament instance ID: {TournamentInstanceId}",
                tournamentInstanceId
            );
            return await _clubRegistrationRepository.GetByTournamentInstanceIdAsync(
                tournamentInstanceId
            );
        }

        public async Task<ClubRegistration?> GetByClubIdAsync(int clubId)
        {
            _logger.LogInformation("Fetching club registration for club ID: {ClubId}", clubId);
            return await _clubRegistrationRepository.GetByClubIdAsync(clubId);
        }

        public async Task<List<ClubRegistration>> GetByStatusAsync(RegistrationStatus status)
        {
            _logger.LogInformation("Fetching club registrations with status: {Status}", status);
            return await _clubRegistrationRepository.GetByStatusAsync(status);
        }

        public async Task<ClubRegistration> ApproveRegistrationAsync(int id)
        {
            _logger.LogInformation("Approving club registration ID: {Id}", id);

            var registration = await GetByIdAsync(id);
            if (registration == null)
            {
                _logger.LogWarning("Club registration with ID {Id} not found", id);
                return null;
            }

            registration.Status = RegistrationStatus.Approved;
            registration.ApprovedDate = DateTime.UtcNow;
            registration.DeniedDate = null;
            registration.DenialReason = null;

            return await _clubRegistrationRepository.UpdateAsync(registration);
        }

        public async Task<ClubRegistration> DenyRegistrationAsync(int id, string denialReason)
        {
            _logger.LogInformation(
                "Denying club registration ID: {Id} with reason: {Reason}",
                id,
                denialReason
            );

            var registration = await GetByIdAsync(id);
            if (registration == null)
            {
                _logger.LogWarning("Club registration with ID {Id} not found", id);
                return null;
            }

            registration.Status = RegistrationStatus.Denied;
            registration.DeniedDate = DateTime.UtcNow;
            registration.DenialReason = denialReason;
            registration.ApprovedDate = null;

            return await _clubRegistrationRepository.UpdateAsync(registration);
        }

        public async Task<float> CalculateRegistrationFeeAsync(ClubRegistration clubRegistration)
        {
            _logger.LogInformation("Calculating registration fee");

            // TODO: Implement actual fee calculation logic based on your business rules
            // This is a placeholder implementation
            float baseFee = 0;

            // Calculate fee based on packages
            float packageAFee = clubRegistration.PackageACount * 100; // Example: 100 per Package A
            float packageBFee = clubRegistration.PackageBCount * 75; // Example: 75 per Package B

            // Calculate fee based on team counts
            float teamFee = 0;
            if (clubRegistration.CategoryTeamCounts != null)
            {
                foreach (var categoryTeamCount in clubRegistration.CategoryTeamCounts)
                {
                    teamFee += categoryTeamCount.TeamCount * 50; // Example: 50 per team
                }
            }

            baseFee = packageAFee + packageBFee + teamFee;

            _logger.LogInformation("Calculated fee: {Fee}", baseFee);
            return await Task.FromResult(baseFee);
        }
    }
}
