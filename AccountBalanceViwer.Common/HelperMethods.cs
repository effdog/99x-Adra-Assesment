using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountBalanceViwer.Common
{
    public static class HelperMethods
    {

       
        /// <summary>
        /// To get month name from month number
        /// </summary>
        /// <param name="MonthNumber"></param>
        /// <returns></returns>
        public static string GetMonthName(int MonthNumber)
        {
            string month = string.Empty;

            switch (MonthNumber)
            {
                case 1:
                    month = "January";
                    break;
                case 2:
                    month = "February";
                    break;
                case 3:
                    month = "March";
                    break;
                case 4:
                    month = "April";
                    break;
                case 5:
                    month = "May";
                    break;
                case 6:
                    month = "June";
                    break;
                case 7:
                    month = "July";
                    break;
                case 8:
                    month = "August";
                    break;
                case 9:
                    month = "September";
                    break;
                case 10:
                    month = "October";
                    break;
                case 11:
                    month = "November";
                    break;
                case 12:
                    month = "December";
                    break;
            }

            return month;
        }

        /// <summary>
        /// Upload the excel file to blob storage
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="log"></param>
        public static void UploadToBlob(byte[] file, string fileName, string fileLocation, string ContainerConnection)
        {

            string blobContainerName = fileLocation;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ContainerConnection);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(blobContainerName);

            try
            {
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName);
                string[] stringArray = fileName.Split('.');
                string fileExtension = stringArray[stringArray.Count() - 1];

                blob.Properties.ContentType = GetMimeType(fileExtension);
                blob.UploadFromByteArrayAsync(file, 0, file.Length);

            }
            catch (Exception ex)
            {
                //log.LogError("Error occured in ExtractGripAgencyFiles - " + ex.Message);
            }
        }

        public static string GetMimeType(string extension)
        {
            //Check extension.
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            //Mime type.
            string mime = "";
            //Get mime type for extension.
            return mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        private static IDictionary<string, string> mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
           #region  mime types      
        {".csv", "text/csv"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".pdf", "application/pdf"},       
        #endregion
        };
    }

}