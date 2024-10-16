using System.Text.RegularExpressions;
using Content.Server.Administration;
using Content.Server.Chat.Managers;
using Content.Shared.Preferences;
using Robust.Shared.Player;

namespace Content.Server.Custom.RenameOnStartComponent;

public sealed class RenameOnStartSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaSystem = default!;
    [Dependency] private readonly QuickDialogSystem _quickDialog = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    private static readonly Regex RestrictedNameRegex = new("[^А-Яа-яёЁ0-9' -]"); // Corvax-Localization
    // private static readonly Regex ICNameCaseRegex = new(@"^(?<word>\w)|\b(?<word>\w)(?=\w*$)");

    public override void Initialize()
    {
        SubscribeLocalEvent<RenameOnStartComponent, PlayerAttachedEvent>(OnPlayerAttached);
    }

    private void OnPlayerAttached(EntityUid ent, RenameOnStartComponent component, PlayerAttachedEvent message)
    {
        OpenNameChangeMenu(ent, component, message);
        RemComp<RenameOnStartComponent>(ent);
    }

    private void OpenNameChangeMenu(EntityUid ent, RenameOnStartComponent component, PlayerAttachedEvent message)
    {
        _quickDialog.OpenDialog(message.Player,
            Loc.GetString("rename-onstart-dialog-title"),
            Loc.GetString("rename-onstart-dialog-newname-text"),
            (LongString newName) =>
            {
                if (newName.String.Length < 2)
                {
                    _chatManager.DispatchServerMessage(
                        message.Player,
                        Loc.GetString("rename-onstart-too-short"),
                        true);
                    OpenNameChangeMenu(ent, component, message);
                    return;
                }

                if (newName.String.Length > HumanoidCharacterProfile.MaxNameLength)
                {
                    _chatManager.DispatchServerMessage(
                        message.Player,
                        Loc.GetString("cmd-rename-too-long"),
                        true);
                    OpenNameChangeMenu(ent, component, message);
                    return;
                }

                if (RestrictedNameRegex.IsMatch(newName.String))
                {
                    _chatManager.DispatchServerMessage(
                        message.Player,
                        Loc.GetString("rename-onstart-regex-error"),
                        true);
                    OpenNameChangeMenu(ent, component, message);
                    return;
                }

                _metaSystem.SetEntityName(ent, newName.String);
            });
    }
}
