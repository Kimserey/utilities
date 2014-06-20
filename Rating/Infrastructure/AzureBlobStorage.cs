using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rating.Infrastructure
{
    public class AzureBlobStorage : IFileStorage
    {
        private CloudBlobClient _blobClient;
        public AzureBlobStorage()
        {
            var setting = CloudConfigurationManager.GetSetting("ratingstorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(setting);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public string UploadFile(string containerName, string fileName, System.IO.Stream fileStream)
        {
            fileName = string.Format("{0}_{1}", Guid.NewGuid().ToString(), fileName); 
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.UploadFromStream(fileStream);
            return blockBlob.Uri.ToString();
        }

        public void DeleteFile(string containerName, string fileName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}