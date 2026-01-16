using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace WorkShopManagement.Storage;

public static class SignedUrlFactory
{
    private static UrlSigner? _signer;
    private static readonly Lock _lock = new();

    public static string CreateGetSignedUrl(
        string bucketName,
        string objectName,
        string clientEmail,
        string privateKeyPem,
        TimeSpan lifetime)
    {
        var signer = GetOrCreateSigner(clientEmail, privateKeyPem);
        return signer.Sign(bucketName, objectName, lifetime, HttpMethod.Get);
    }

    private static UrlSigner GetOrCreateSigner(string clientEmail, string privateKeyPem)
    {
        if (_signer != null) return _signer;

        lock (_lock)
        {
            if (_signer != null) return _signer;

            var values = new Dictionary<string, string>
            {
                { "type", "service_account" },
                { "client_email", clientEmail },
                { "private_key", privateKeyPem }
            };

            var credential = GoogleCredential.FromJson(JsonSerializer.Serialize(values));

            if (credential.UnderlyingCredential is not ServiceAccountCredential saCred)
                throw new InvalidOperationException("Invalid service account credentials. Check ClientEmail/PrivateKey.");

            _signer = UrlSigner.FromCredential(saCred);
            return _signer;
        }
    }
}