using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly ILogger<ClubService> _logger;
        public ClubService(IClubRepository clubRepository, ILogger<ClubService> logger)
        {
            _clubRepository = clubRepository;
            _logger = logger;
        }

        public async Task AddClubAsync(Club club)
        {
            await _clubRepository.AddClubAsync(club);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var club = await GetByIdAsync(id);
            if (club == null)
            {
                return false;
            }
            return await _clubRepository.DeleteAsync(club);
        }

        public async Task<List<Club>> GetAllAsync()
        {
            return await _clubRepository.GetAllAsync();
        }

        public async Task<Club> GetByIdAsync(int id)
        {
            return await _clubRepository.GetByIdAsync(id);
        }

        public async Task<Club> UpdateAsync(Club club)
        {
            var existingClub = await GetByIdAsync(club.Id);
            if (existingClub == null)
            {
                return null;
            }
            existingClub.Name = club.Name;
            existingClub.Logo = club.Logo;
            existingClub.Email = club.Email;
            existingClub.Address = club.Address;
            existingClub.State = club.State;
            existingClub.Website = club.Website;

            return await _clubRepository.UpdateAsync(existingClub);

        }

        public async Task<Club> GetPlaceholderClubAsync()
        {
            var clubs = await _clubRepository.GetAllAsync();
            var placeholderClub = clubs.FirstOrDefault(c => c.IsPlaceholder == true);
            if (placeholderClub == null)
            {
                _logger.LogWarning("No placeholder club found.");
                return null;
            }
            return placeholderClub;
        }

        public async Task UpdateLogoAsync(int clubId, string logoFileName)
        {
            var club = await GetByIdAsync(clubId);
            if (club == null)
            {
                _logger.LogError($"Club with ID {clubId} not found.");
                return;
            }
            club.Logo = logoFileName;
            
            await _clubRepository.UpdateAsync(club);

            _logger.LogInformation($"Logo for club {club.Name} updated successfully.");
        }

        public async Task<Club?> GetBySlugAsync(string clubName)
        {
            var clubs = await _clubRepository.GetAllAsync();
            var club = clubs.FirstOrDefault(c => ToSlug(c.Name) == clubName.ToLower());
            if (club == null)
            {
                _logger.LogWarning($"Club with name {clubName} not found.");
                return null;
            }
            return club;
        }

        private static string ToSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Odstranění diakritiky
            string normalized = input.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (char c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            // Malá písmena, mezery na pomlčky, odstranění speciálních znaků
            string slug = sb.ToString()
                .ToLower()
                .Replace(' ', '-')
                .Replace("’", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(",", "")
                .Replace(".", "")
                .Replace("–", "-")
                .Replace("--", "-")
                .Trim('-');

            // Odstraní všechny znaky kromě a-z, 0-9 a '-'
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-"); // více pomlček na jednu

            return slug;
        }
    }
}
