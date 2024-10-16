using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared.Custom;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpeedImplantComponent : Component
{
    [AutoNetworkedField, ViewVariables]
    public bool SpeedImplantEnabled = false;

}
public sealed partial class UseSpeedImplantEvent : InstantActionEvent
{

}
