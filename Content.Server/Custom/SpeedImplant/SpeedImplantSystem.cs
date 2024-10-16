using System.Threading.Tasks;
using Content.Shared.Custom;
using Content.Shared.Implants.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;

namespace Content.Server.Custom.SpeedImplant;

public sealed class SpeedImplantSystem : EntitySystem
{
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifier = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<SubdermalImplantComponent, UseSpeedImplantEvent>(UseSpeedImplant);
        SubscribeLocalEvent<SpeedImplantComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovespeed);
    }

    private void UseSpeedImplant(EntityUid uid, SubdermalImplantComponent component, UseSpeedImplantEvent args)
    {
        if (component.ImplantedEntity is not { } ent)
            return;

        if (!TryComp<SpeedImplantComponent>(ent, out var comp))
            return;

        if (comp.SpeedImplantEnabled)
            return;

        Task.Run(async () =>
        {
            EnableSpeedImplant(ent, comp);
            await Task.Delay(10000);
            DisableSpeedImplant(ent, comp);
        });

        args.Handled = true;
    }

    private void OnRefreshMovespeed(EntityUid uid, SpeedImplantComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        if (!TryComp<SpeedImplantComponent>(uid, out var comp))
            return;

        if (!comp.SpeedImplantEnabled)
            return;

        args.ModifySpeed(1.1f, 1.1f);
    }

    private void EnableSpeedImplant(EntityUid ent, SpeedImplantComponent comp)
    {
        EnsureComp<MovementSpeedModifierComponent>(ent);
        comp.SpeedImplantEnabled = true;
        _movementSpeedModifier.RefreshMovementSpeedModifiers(ent);
    }

    private void DisableSpeedImplant(EntityUid ent, SpeedImplantComponent comp)
    {
        comp.SpeedImplantEnabled = false;
        _movementSpeedModifier.RefreshMovementSpeedModifiers(ent);
    }
}
