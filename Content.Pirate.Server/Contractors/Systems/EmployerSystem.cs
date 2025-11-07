using System.Linq;
using Content.Server.Players.PlayTimeTracking;
using Content.Pirate.Shared.Contractors.Prototypes;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Players;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Utility;

namespace Content.Pirate.Server.Contractors.Systems;

public sealed class EmployerSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly ISerializationManager _serialization = default!;
    [Dependency] private readonly PlayTimeTrackingManager _playTimeTracking = default!;
    [Dependency] private readonly IConfigurationManager _configuration = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete);
    }

    // When the player is spawned in, add the employer components selected during character creation
    private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent args) =>
        ApplyEmployer(args.Mob, args.JobId, args.Profile,
            _playTimeTracking.GetTrackerTimes(args.Player));

    /// <summary>
    ///     Adds the employer selected by a player to an entity.
    /// </summary>
    public void ApplyEmployer(EntityUid uid, ProtoId<JobPrototype>? jobId, HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes)
    {
        if (jobId == null || !_prototype.TryIndex(jobId, out _))
            return;

        var jobPrototypeToUse = _prototype.Index(jobId.Value);

        ProtoId<EmployerPrototype> employer = profile.Employer != string.Empty ? profile.Employer : "NanoTrasen";

        if(!_prototype.TryIndex<EmployerPrototype>(employer, out var employerPrototype))
        {
            DebugTools.Assert($"Employer '{employer}' not found!");
            return;
        }

        // Requirements checks are disabled for now; apply employer without gating.
        AddEmployer(uid, employerPrototype);
    }

    /// <summary>
    ///     Adds a single Employer Prototype to an Entity.
    /// </summary>
    public void AddEmployer(EntityUid uid, EmployerPrototype employerPrototype)
    {
        // No-op: trait functions not implemented in this fork.
    }
}
