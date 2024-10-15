using Content.Server.Administration;
using Robust.Shared.Player;

namespace Content.Server.Custom.RenameOnStartComponent;

public sealed class RenameOnStartSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaSystem = default!;
    [Dependency] private readonly QuickDialogSystem _quickDialog = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<RenameOnStartComponent, PlayerAttachedEvent>(OnPlayerAttached);
    }

    private void OnPlayerAttached(EntityUid ent, RenameOnStartComponent component, PlayerAttachedEvent message)
    {
        _quickDialog.OpenDialog(message.Player,
            "Character Rename",
            "New name:",
            (LongString newDescription) =>
            {
                _metaSystem.SetEntityName(ent, newDescription.String);
            });

        RemComp<RenameOnStartComponent>(ent);
    }
}
