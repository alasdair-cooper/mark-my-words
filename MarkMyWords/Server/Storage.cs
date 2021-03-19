using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Threading.Tasks;

using System.IO;
using System.IO.Compression;

using System.Text.Json;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using MarkMyWords.Shared;

namespace MarkMyWords.Server
{
    public class Storage
    {
        public static async Task<AssignmentModel> DownloadFromStorage(string fileName)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using Stream downloadStream = download.Content;

            Stream decompressedStream = await Decompress(downloadStream);

            AssignmentModel assignment = await JsonSerializer.DeserializeAsync<AssignmentModel>(decompressedStream, Utils.DefaultOptions());

            return assignment;
        }

        public static async Task UploadToStorage(string fileName, AssignmentModel assignment)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobClient blobClient = new BlobClient(connectionString, "assignments", fileName);

            MemoryStream stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, assignment, Utils.DefaultOptions());
            stream.Position = 0;
            Stream compressedStream = await Compress(stream);
            compressedStream.Position = 0;
            await blobClient.UploadAsync(compressedStream, overwrite: true);
            await blobClient.SetMetadataAsync(assignment.GetAssignmentInfo());
        }

        private static async Task<Stream> Compress(Stream streamToCompress)
        {
            MemoryStream memoryStream = new MemoryStream();
            using GZipStream compressionStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
            await streamToCompress.CopyToAsync(compressionStream);
            return memoryStream;
        }

        private static async Task<Stream> Decompress(Stream streamToDecompress)
        {
            MemoryStream stream = new MemoryStream();

            using GZipStream decompressionStream = new GZipStream(streamToDecompress, CompressionMode.Decompress, true);
            await decompressionStream.CopyToAsync(stream);

            stream.Position = 0;
            return stream;
        }

        public static AssignmentModel Encrypt(AssignmentModel assignment, string password)
        {
            foreach (AttemptModel attempt in assignment.Attempts)
            {
                if (!string.IsNullOrWhiteSpace(attempt.AttemptName))
                {
                    attempt.AttemptName = StringCipher.Encrypt(attempt.AttemptName, password);
                }
            }

            assignment.DataEncrypted = true;
            return assignment;
        }

        public static AssignmentModel Decrypt(AssignmentModel assignment, string password)
        {
            foreach (AttemptModel attempt in assignment.Attempts)
            {
                if (!string.IsNullOrWhiteSpace(attempt.AttemptName))
                {
                    attempt.AttemptName = StringCipher.Decrypt(attempt.AttemptName, password);
                }
                attempt.ReevaluateLock();
            }

            assignment.DataEncrypted = false;
            return assignment;
        }
    }
}
