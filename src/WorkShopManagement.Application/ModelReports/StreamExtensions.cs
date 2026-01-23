using System.IO;

namespace WorkShopManagement.ModelReports;

//Converting bytes into stream => ABP accept RemoteStreamContent , but pdf generator return the bytes
public static class StreamExtensions
{
    public static Stream ToStream(this byte[] bytes) => new MemoryStream(bytes);
}
