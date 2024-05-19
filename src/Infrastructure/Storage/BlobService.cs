using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text.Json;

namespace AdventureArray.Infrastructure.Storage;

/// <summary>
/// Abstractielaag bovenop Azure Blob Storage.
/// </summary>
public interface IBlobService
{
	Task<string?> GetMostRecentBlobOrDefaultAsync(string containerName, string directoryName);
	Task<Stream> DownloadBlobAsync(string containerName, string blobName);
	Task UploadBlobAsync<T>(string containerName, string directoryName, string blobName, IEnumerable<T> content);
}

public class BlobStorageSettings
{
	public required string ConnectionString { get; set; }
}

public class BlobService : IBlobService
{
	private readonly BlobStorageSettings _settings;
	private const string _dateTimePrefixFormat = "yyyyMMddHHmmss";

	public BlobService(IOptions<BlobStorageSettings> settings)
	{
		_settings = settings.Value;
	}

	public async Task<string?> GetMostRecentBlobOrDefaultAsync(string containerName, string directoryName)
	{
		var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

		await blobContainerClient.CreateIfNotExistsAsync();
		var blobs = blobContainerClient.GetBlobsAsync(prefix: directoryName);

		BlobItem? mostRecentBlob = null;
		await foreach (var blob in blobs)
		{
			if (mostRecentBlob == null || string.CompareOrdinal(blob.Name, mostRecentBlob.Name) > 0)
			{
				mostRecentBlob = blob;
			}
		}

		return mostRecentBlob?.Name;
	}

	public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
	{
		var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		var blobClient = blobContainerClient.GetBlobClient(blobName);

		var response = await blobClient.DownloadAsync();
		return response.Value.Content;
	}

	public async Task UploadBlobAsync<T>(string containerName, string directoryName, string blobName, IEnumerable<T> content)
	{
		await using var stream = new MemoryStream();
		var writer = new StreamWriter(stream);

		foreach (var @object in content)
		{
			var json = JsonSerializer.Serialize(@object);
			await writer.WriteLineAsync(json);
		}

		// Important: flush the writer to ensure all data is written to the MemoryStream
		await writer.FlushAsync();

		// Reset the position of the stream to ensure all data is read during upload
		stream.Position = 0;

		var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		await blobContainerClient.CreateIfNotExistsAsync();

		var fileName = $"{DateTime.UtcNow.ToString(_dateTimePrefixFormat, CultureInfo.InvariantCulture)}-{blobName}";
		var blobClient = blobContainerClient.GetBlobClient($"{directoryName}/{fileName}");

		await blobClient.UploadAsync(stream, overwrite: true);
	}
}
