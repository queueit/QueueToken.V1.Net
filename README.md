>Download latest version from here: [![NuGet](https://img.shields.io/nuget/v/QueueIT.QueueToken.svg)](https://www.nuget.org/packages/QueueIT.QueueToken/)
# Queue-it Queue Token SDK for .NET
The Queue-it Queue Token SDK is used to ensure that end users cannot enter the queue without a valid token and to be a container which can car-ry sensitive user information from integrating system into the queue. The token can be issued by any application that supports .NET Standard 2.0+.
## The Token
The token consists of two parts. Firstly, a header containing non-sensitive metadata. Secondly the payload of the token. Both header and payload are in JSON format.
### Token Header
```
{ 
  "typ": "QT1",
  "enc": "AES256",
  "iss": 1526464517,
  "exp": 1526524517,
  "ti": "159aba3e-55e1-4f54-b6ee-e5b943d7e885�,
  "c": "ticketania", 
  "e": "demoevent�
}
```
- `typ`: The type of the token. Value must be �QFT1�. Required.
- `enc`: Payload encryption algorithm. Value must be �AES256�. Required.
- `iss`: NumericDate of when token was issued. Required.
- `exp`: NumericDate of when token expires. Optional.
- `ti`: Unique Token ID (e.g. uuid). Used to uniquely identify tokens and restrict replay attacks. Required.
- `c`: The Customer ID of the issuer. Token will only be valid on events on this account. Required.
- `e`: The Event ID. If provided, token will only be valid on this event. Optional.

### Token Payload
```
{ 
  "r": 0.4578,
  "k": "XKDI42W",
  "cd": { "size": "medium" }
}
```
- `r`: The relative quality of the key. Must be a decimal value. Used for determining the quality of the token. Optional
- `k`: A unique key that holds value to the integrating system (e.g. email or user id). Used to restrict users from issuing multiple queue ids. Optional.
- `cd`: Any custom data of the user. This is a set of key-value pairs. Optional

## Usage
```
var secretKey = ...;
var token = Token
    .Enqueue("ticketania")
    .WithPayload(Payload
		.Enqueue()
		.WithKey("XKDI42W")
		.WithRelativeQuality(0.4578)
		.WithCustomData("size", "medium")
		.Generate())
    .WithEventId("demoevent")
    .WithValidity(60000)
    .Generate(secretKey);

string token = token.Token;
```

## Serialized Token
> eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsImV4cCI6MTUzOTEyOTYwMDAwMCwidGkiOiJhMjFkNDIzYS00M2ZkLTQ4MjEtODRmYS00MzkwZjZhMmZkM2UiLCJjIjoidGlja2V0YW5pYSIsImUiOiJteWV2ZW50In0.0rDlI69F1Dx4Twps5qD4cQrbXbCRiezBd6fH1PVm6CnVY456FALkAhN3rgVrh_PGCJHcEXN5zoqFg65MH8WZc_CQdD63hJre3Sedu0-9zIs.aZgzkJm57etFaXjjME_-9LjOgPNTTqkp1aJ057HuEiU

The format of the token is [header].[payload].[hash] where each part is Base64Url encoded. The payload is AES 256 encrypted with the secret key supplied in the `.Generate(secretKey)` method. If the �e� key is provided in the header, the secret key on the event must be used. If no �e� key is provided the default key on the customer account must be used.
The token is signed with SHA 256 using the same secret key.
