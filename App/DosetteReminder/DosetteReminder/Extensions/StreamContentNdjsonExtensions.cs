using System.Diagnostics;
using System.Text.Json;

namespace DosetteReminder.Extensions
{
    internal static class StreamContentNdjsonExtensions
    {
        private static readonly JsonSerializerOptions m_serializerOptions
            = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

        public static async IAsyncEnumerable<TValue> ReadFromNdjsonAsync<TValue>(this Stream contentStream)
        {
            if (contentStream is null)
            {
                throw new ArgumentNullException(nameof(contentStream));
            }

            using (contentStream)
            {
                using (StreamReader contentStreamReader = new StreamReader(contentStream))
                {
                    while (!contentStreamReader.EndOfStream)
                    {
                        TValue message = default(TValue);
                        try {
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
