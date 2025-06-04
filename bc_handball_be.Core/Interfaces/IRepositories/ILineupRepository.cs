using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface ILineupRepository
    {
        Task CreateLineupsForMatchAsync(int matchId, int homeTeamId, List<int> homePlayerIds, int awayTeamId, List<int> awayPlayerIds);
    }
}
