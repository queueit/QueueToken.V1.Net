using System;
using QueueIT.QueueToken;

public interface IEnqueueToken
{
    TokenVersion TokenVersion { get; }
    EncryptionType Encryption { get; }
    DateTime Issued { get; }
    DateTime Expires { get; }
    string TokenIdentifier { get; }
    string CustomerId { get; }
    string EventId { get; }
    IEnqueueTokenPayload Payload { get; }
    string Token { get; }
    string SignedToken { get; }
    string Signature { get; }
}