namespace Stl.Fusion.Client.Internal;

public static class Errors
{
    public static Exception InterfaceTypeExpected(Type type, bool mustBePublic, string? argumentName = null)
    {
        var message = $"'{type}' must be {(mustBePublic ? "a public": "an")} interface type.";
        return argumentName.IsNullOrEmpty()
            ? new InvalidOperationException(message)
            : new ArgumentOutOfRangeException(argumentName, message);
    }

    public static Exception UnknownServerSideError()
        => new RemoteException("Unknown server-side error.");

    public static Exception BackendIsUnreachable(Exception innerException)
        => new HttpRequestException("Backend is unreachable.", innerException);
}
