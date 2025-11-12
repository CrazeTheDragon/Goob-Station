using Content.Shared.Preferences;
using Robust.Shared.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Localization;
using Content.Shared._Pirate.Contractors.Prototypes;

namespace Content.Shared.Preferences.Loadouts.Effects;

[DataDefinition]
public sealed partial class CharacterEmployerRequirement : LoadoutEffect
{
    [DataField(required: true)]
    public List<string> Employers = new();

    public override bool Validate(
        HumanoidCharacterProfile profile,
        RoleLoadout loadout,
        ICommonSession? session,
        IDependencyCollection collection,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = null;

        if (session == null || profile.Employer == string.Empty || !Employers.Contains(profile.Employer))
        {
            var protoManager = collection.Resolve<IPrototypeManager>();

            var employerNames = Employers.Select(id =>
                protoManager.TryIndex<EmployerPrototype>(id, out var proto)
                    ? Loc.GetString(proto.NameKey) 
                    : id)
                .Select(name => $"[color=#ff0000]{name}[/color]");

            reason = FormattedMessage.FromMarkup($"Вимагається один із роботодавців: {string.Join(", ", employerNames)}.");
            return false;
        }

        return true;
    }

    public override void Apply(RoleLoadout loadout) {}
}