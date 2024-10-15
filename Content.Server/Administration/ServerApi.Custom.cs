using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Robust.Server.ServerStatus;

namespace Content.Server.Administration;

/// <summary>
/// Custom API
/// </summary>
public sealed partial class ServerApi
{
    [Dependency] private readonly IAdminManager _serverAdminManager = default!;

    /// <summary>
    /// Get players and active admins list
    /// </summary>
    private async Task GetPlayers(IStatusHandlerContext context)
    {
        var playersList = new JsonArray();
        foreach (var player in _playerManager.GetAllPlayerData())
        {
            playersList.Add(player.UserName);
        }

        var adminsDict = new JsonObject();
        foreach (var admin in _serverAdminManager.ActiveAdmins)
        {
            var adminData = _serverAdminManager.GetAdminData(admin)!;
            if (!adminData.Stealth)
                adminsDict[admin.Name] = adminData.Title;
        }

        var jObject = new JsonObject
        {
            ["players"] = playersList,
            ["admins"] = adminsDict
        };

        await context.RespondJsonAsync(jObject);
    }
}
