using System.Collections.Concurrent;

namespace Converter.ApiServer.Services
{
    /// <summary>
    /// In-memory implementation of IStorageService for storing converted result
    /// </summary>
    public class MemoryStorageService : IStorageService
    {
        private ConcurrentDictionary<StorageKey, AttachmentDTO> _store;

        public MemoryStorageService()
        {
            _store = new ConcurrentDictionary<StorageKey, AttachmentDTO>();
        }

        public async Task<AttachmentDTO> GetFileAsync(string userToken, int fileHash)
        {
            var storageKey = new StorageKey(userToken, fileHash);

            _store.TryGetValue(storageKey, out var attachmentDTO);
            _store.TryRemove(storageKey, out _);

            return await Task.FromResult(attachmentDTO);
        }

        public void SaveFile(byte[] bytes, string filename, int fileHash, string userToken) => 
            _store.TryAdd(
                new StorageKey(userToken, fileHash),
                new AttachmentDTO { Blob = bytes, FileName = filename });
    }

    internal record StorageKey(string UserToken, int FileHash);
}
