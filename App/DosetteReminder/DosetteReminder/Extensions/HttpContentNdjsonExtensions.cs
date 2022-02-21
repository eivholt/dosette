using System.Diagnostics;
using System.Text.Json;

namespace DosetteReminder.Extensions
{
    internal static class HttpContentNdjsonExtensions
    {
        private static readonly JsonSerializerOptions m_serializerOptions
            = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

        public static async IAsyncEnumerable<TValue> ReadFromNdjsonAsync<TValue>(this HttpContent content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string? mediaType = content.Headers.ContentType?.MediaType;

            if (mediaType is null || 
                (!mediaType.Equals("application/x-ndjson", StringComparison.OrdinalIgnoreCase) && 
                !mediaType.Equals("text/event-stream", StringComparison.OrdinalIgnoreCase)))
            {
                throw new NotSupportedException();
            }

            Stream contentStream = await content.ReadAsStreamAsync().ConfigureAwait(false);

            using (contentStream)
            {
                using (StreamReader contentStreamReader = new StreamReader(contentStream))
                {
                    while (!contentStreamReader.EndOfStream)
                    {
                        TValue message = default(TValue);
                        try
                        {
                            message = JsonSerializer.Deserialize<TValue>(await contentStreamReader.ReadLineAsync()
                              .ConfigureAwait(false), m_serializerOptions);
                        }
                        catch (JsonException jex)
                        {
                            Debug.WriteLine($"Error parsing ndjson: {jex.Message}");
                            continue;
                        }

                        yield return message;
                    }
                }
            }
        }
    }
}
