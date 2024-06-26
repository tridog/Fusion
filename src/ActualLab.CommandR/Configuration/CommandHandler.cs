using System.Diagnostics.CodeAnalysis;

namespace ActualLab.CommandR.Configuration;

public interface ICommandHandler
{
    Symbol Id { get; }
    bool IsFilter { get; }
    double Priority { get; }

    Type GetHandlerServiceType();
    object GetHandlerService(ICommand command, CommandContext context);
    Task Invoke(ICommand command, CommandContext context, CancellationToken cancellationToken);
}

public abstract record CommandHandler(
    Symbol Id,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type CommandType,
    bool IsFilter = false,
    double Priority = 0
    ) : ICommandHandler
{
    public abstract Type GetHandlerServiceType();
    public abstract object GetHandlerService(ICommand command, CommandContext context);

    public abstract Task Invoke(
        ICommand command, CommandContext context,
        CancellationToken cancellationToken);

    public override string ToString()
        => $"{Id.Value}[Priority = {Priority}{(IsFilter ? ", IsFilter = true" : "")}]";

    // This record relies on reference-based equality
    public virtual bool Equals(CommandHandler? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}

public abstract record CommandHandler<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TCommand>
    (Symbol Id, bool IsFilter = false, double Priority = 0)
    : CommandHandler(Id, typeof(TCommand), IsFilter, Priority)
    where TCommand : class, ICommand
{
    public override string ToString() => base.ToString();

    // This record relies on reference-based equality
    public virtual bool Equals(CommandHandler<TCommand>? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
