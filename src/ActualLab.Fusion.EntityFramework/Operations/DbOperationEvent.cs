using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ActualLab.CommandR.Operations;
using ActualLab.Fusion.EntityFramework.LogProcessing;
using ActualLab.Versioning;
using Microsoft.EntityFrameworkCore;

namespace ActualLab.Fusion.EntityFramework.Operations;

#pragma warning disable IL2026

[Table("_OperationEvents")]
[Index(nameof(Uuid), IsUnique = true, Name = "IX_Uuid")] // "Uuid -> Index" queries
[Index(nameof(State), nameof(LoggedAt), Name = "IX_State")] // "!IsProcessed -> min(Index)" queries
[Index(nameof(LoggedAt), Name = "IX_LoggedAt")] // "LoggedAt > minLoggedAt -> min(Index)" queries
public sealed class DbOperationEvent : ILogEntry, IHasId<string>, IHasId<long>
{
    public static ITextSerializer Serializer { get; set; } = NewtonsoftJsonSerializer.Default;

    private long? _index;
    private Symbol _uuid;
    private DateTime _loggedAt;

    Symbol IHasUuid.Uuid => _uuid;
    string IHasId<string>.Id => _uuid.Value;
    long IHasId<long>.Id => Index;
    DateTime ILogEntry.FiresAt => default;

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Index {
        get => _index ?? 0;
        set => _index = value;
    }
    [NotMapped] public bool HasIndex => _index.HasValue;

    public string Uuid { get => _uuid; set => _uuid = value; }
    [ConcurrencyCheck] public long Version { get; set; }

    public DateTime LoggedAt {
        get => _loggedAt.DefaultKind(DateTimeKind.Utc);
        set => _loggedAt = value.DefaultKind(DateTimeKind.Utc);
    }

    public string ValueJson { get; set; } = "";
    public LogEntryState State { get; set; }

    public DbOperationEvent() { }
    public DbOperationEvent(OperationEvent model, VersionGenerator<long> versionGenerator)
        => UpdateFrom(model, versionGenerator);

    public OperationEvent ToModel()
    {
        var value = ValueJson.IsNullOrEmpty()
            ? null
            : Serializer.Read(ValueJson, typeof(object));
        return new OperationEvent(Uuid, LoggedAt, default, value);
    }

    public DbOperationEvent UpdateFrom(OperationEvent model, VersionGenerator<long> versionGenerator)
    {
        Uuid = model.Uuid;
        LoggedAt = model.LoggedAt;
        ValueJson = Serializer.Write(model.Value, typeof(object));
        Version = versionGenerator.NextVersion(Version);
        return this;
    }
}