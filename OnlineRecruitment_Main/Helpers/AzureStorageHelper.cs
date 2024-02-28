using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using log4net;

/// <summary>
/// AzureStorage Helper
/// </summary>
public class AzureStorageHelper
{
    private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public AzureStorageHelper()
    {
        // TODO: Add constructor logic here
    }
    /// <summary>
    /// Get Azure ContainerName
    /// </summary>
    public static string AzureContainerName
    {
        get { return ConfigurationManager.AppSettings["eqhires"]; }
    }
    /// <summary>
    /// Get Deployment Type
    /// </summary>
    //private static string DeploymentType
    //{
    //    get { return ConfigurationManager.AppSettings["DeploymentType"]; }
    //}

    /// <summary>
    /// Get Azure ConnectionString From Configuration Manager
    /// </summary>
    public static string AzureConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString; }
    }

    /// <summary>
    /// Get Blob Container Detail
    /// </summary>
    /// <param name="containerName">ContainerName</param>
    /// <returns>CloudBlobContainer</returns>
    public static CloudBlobContainer GetBlobContainer(string containerName)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AzureConnectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        return blobClient.GetContainerReference(containerName);
    }

    /// <summary>
    /// Check Specific Blob Exists or not
    /// </summary>
    /// <param name="blob">CloudBlockBlob</param>
    /// <returns>bool</returns>
    public static bool IsBlobExists(CloudBlockBlob blob)
    {
        try
        {
            blob.FetchAttributes();
            return true;
        }
        catch (StorageException e)
        {
            if (e.ErrorCode != StorageErrorCode.ResourceNotFound) throw;
            return false;
        }
    }

    /// <summary>
    /// Check Specific Blob Exists or not
    /// </summary>
    /// <param name="ContainerName">ContainerName</param>
    /// <param name="BlobUri">BlobUri</param>
    /// <returns>bool</returns>
    public static bool IsBlobExists(string ContainerName, string BlobUri)
    {
        try
        {
            CloudBlobContainer container = GetBlobContainer(AzureContainerName);
            CloudBlob blockBlob = container.GetBlobReference(BlobUri);
            blockBlob.FetchAttributes();
            return true;
        }
        catch (StorageClientException e)
        {
            if (e.ErrorCode != StorageErrorCode.ResourceNotFound) throw;
            return false;
        }
    }

    /// <summary>
    /// Delete Specific Blob From Storage
    /// </summary>
    /// <param name="ContainerName">ContainerName</param>
    /// <param name="BlobUri">BlobUri</param>
    public static void DeleteBlob(string ContainerName, string BlobUri)
    {
        try
        {
            CloudBlobContainer container = GetBlobContainer(AzureContainerName);
            CloudBlob blockBlob = container.GetBlobReference(BlobUri);
            blockBlob.DeleteIfExists();
        }
        catch (StorageClientException e)
        {
            Log.Error(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Create Client's Azure Container if not Exists
    /// </summary>
    public static void CreateContainer()
    {
        try
        {
            var container = GetBlobContainer(AzureContainerName);
            container.CreateIfNotExist();
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    /// <summary>
    /// Upload Blob item To Storage
    /// </summary>
    /// <param name="DocumentType">v</param>
    /// <param name="upLoadFile">upLoadFile</param>
    /// <param name="fileName">fileName</param>
    /// <returns>string(Uploaded BlobUri)</returns>
    public static string UploadBlob(string DocumentType, HttpPostedFileBase upLoadFile, string fileName)
    {
        DeleteOldFiles(DocumentType);
        string sopFileLocation = string.Empty;
        try
        {          
            sopFileLocation = DocumentType + "/" + fileName;         

            if (!string.IsNullOrEmpty(upLoadFile.FileName))
            {
                if (IsBlobExists(AzureContainerName, sopFileLocation))
                    DeleteBlob(AzureContainerName, sopFileLocation);
                CloudBlobContainer container = GetBlobContainer(AzureContainerName);
                CloudBlob blockBlob = container.GetBlobReference(sopFileLocation);

                string extn, name;
                Match m = Regex.Match(upLoadFile.FileName, @"(?'Name'[^\\]+)\.(?'Ext'.*)");
                extn = m.Groups["Ext"].Value;
                name = m.Groups["Name"].Value;
                string path = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + DocumentType + "/" + name + "." + extn);
                upLoadFile.SaveAs(path);
                blockBlob.Properties.ContentType = upLoadFile.ContentType;
                blockBlob.UploadFile(path);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            sopFileLocation = string.Empty;
        }
        return sopFileLocation;
    }

    /// <summary>
    /// Upload Blob item To Storage
    /// </summary>
    /// <param name="DocumentType">v</param>
    /// <param name="upLoadFile">upLoadFile</param>
    /// <param name="fileName">fileName</param>
    /// <returns>string(Uploaded BlobUri)</returns>
    public static string UploadBlob(string DocumentType, FileUpload upLoadFile, string fileName)
    {
        DeleteOldFiles(DocumentType);
        string sopFileLocation = string.Empty;
        try
        {
            sopFileLocation = DocumentType + "/" + DateTime.Now.ToString("yyyy/MMM") + "/" + fileName;

            if (!string.IsNullOrEmpty(upLoadFile.PostedFile.FileName))
            {
                if (IsBlobExists(AzureContainerName, sopFileLocation))
                    DeleteBlob(AzureContainerName, sopFileLocation);
                CloudBlobContainer container = GetBlobContainer(AzureContainerName);
                CloudBlob blockBlob = container.GetBlobReference(sopFileLocation);

                string extn, name;
                Match m = Regex.Match(upLoadFile.PostedFile.FileName, @"(?'Name'[^\\]+)\.(?'Ext'.*)");
                extn = m.Groups["Ext"].Value;
                name = m.Groups["Name"].Value;
                string path = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + DocumentType + "/" + name + "." + extn);
                upLoadFile.PostedFile.SaveAs(path);
                blockBlob.Properties.ContentType = upLoadFile.PostedFile.ContentType;
                blockBlob.UploadFile(path);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            sopFileLocation = string.Empty;
        }
        return sopFileLocation;
    }

    /// <summary>
    /// Android Image Upload To Storage
    /// </summary>
    /// <param name="DocumentType"></param>
    /// <param name="base64string"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string UploadBlob(string DocumentType, byte[] bytes, string fileName)
    {
        DeleteOldFiles(DocumentType);
        string sopFileLocation = string.Empty;
        try
        {
            sopFileLocation = DocumentType + "/" + DateTime.Now.ToString("yyyy/MMM") + "/" + fileName;
            if (bytes.Length > 0)
            {
                CreateContainer();
                if (IsBlobExists(AzureContainerName, sopFileLocation))
                    DeleteBlob(AzureContainerName, sopFileLocation);

                CloudBlobContainer container = GetBlobContainer(AzureContainerName);
                CloudBlob blockBlob = container.GetBlobReference(sopFileLocation);

                string path = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + DocumentType + "/" + fileName);
                SaveFileLocally(bytes, path, DocumentType);

                //blockBlob.Properties.ContentType = IOHelper.GetContentType(Path.GetExtension(fileName));
                blockBlob.UploadFile(path);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            sopFileLocation = string.Empty;
        }
        return sopFileLocation;
    }

    /// <summary>
    /// View stored blob file (will work only if container is set to "Public Blob")
    /// </summary>
    /// <param name="blobName"></param>
    /// <returns></returns>
    public static string GetViewBlobViewLink(string blobName)
    {
        string blobConString = ConfigurationManager.ConnectionStrings["CloudStorageConnectionString"].ConnectionString;

        Match m = Regex.Match(blobConString, "AccountName=(?'AccountName'[A-Za-z0-9_.]+)");
        string accName = m.Groups["AccountName"].Value;
        string containerName = ConfigurationManager.AppSettings["SOPContainerName"];

        return string.Format("https://{0}.blob.core.windows.net/{1}/{2}", accName, containerName, blobName);
    }

    /// <summary>
    /// Download Specific Blob item From Storage at Specific Path
    /// </summary>
    /// <param name="BlobUri">BlobUri</param>
    /// <param name="FileLocation">FileLocation</param>
    public static void DownloadBlob(string BlobUri, string FileLocation)
    {
        CloudBlobContainer container = GetBlobContainer(AzureContainerName);
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(container.Uri + "/" + BlobUri);
        if (IsBlobExists(blockBlob))
        {
            blockBlob.DownloadToFile(FileLocation);
        }
    }

    /// <summary>
    /// Download Specific Blob item in MemoryStream
    /// </summary>
    /// <param name="BlobUri">BlobUri</param>
    /// <param name="response">response</param>
    public static void DownloadBlob(string BlobUri, HttpResponse response = null)
    {
        CloudBlobContainer container = GetBlobContainer(AzureContainerName);
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(container.Uri + "/" + BlobUri);
        {
            using (var memStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memStream);
                var fileName = GetFileName(blockBlob.Name);


                if (response == null)
                    response = HttpContext.Current.Response;                    
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.Expires = -1;
                response.ContentType = blockBlob.Properties.ContentType;
                response.AddHeader("Content-Disposition", "Attachment; filename=" + fileName);
                response.AddHeader("Content-Length", blockBlob.Properties.Length.ToString());
                response.BinaryWrite(memStream.ToArray());
                response.Flush();
                response.Close();
                response.End();

                //blockBlob.DownloadToStream(memStream);
                //response.ContentType = blockBlob.Properties.ContentType;
                //response.AddHeader("Content-Disposition", "Attachment; filename=" + blockBlob.Name);
                //response.AddHeader("Content-Length", blockBlob.Properties.Length.ToString());
                //response.BinaryWrite(memStream.ToArray());
                //response.Flush();
            }
        }
    }
    private static string GetFileName(string hrefLink)
    {
        string[] parts = hrefLink.Split('/');
        string fileName = "";

        if (parts.Length > 0)
            fileName = parts[parts.Length - 1];
        else
            fileName = hrefLink;

        return fileName;
    }

    /// <summary>
    /// Delete Old Files Less than 1 hour From Download Directory of Specific Document Type
    /// </summary>
    /// <param name="DocumentType">DocumentType</param>
    private static void DeleteOldFiles(string DocumentType)
    {
        try
        {
            string BasePath = HttpContext.Current.Server.MapPath("~/UploadedFiles");
            if (Directory.Exists(BasePath + @"\" + DocumentType))
            {
                DirectoryInfo dir = new DirectoryInfo(BasePath + @"\" + DocumentType);
                FileInfo[] fiList = dir.GetFiles();
                foreach (FileInfo fi in fiList)
                {
                    if (fi.CreationTime <= DateTime.Now.AddHours(-1))
                        fi.Delete();
                }

                DirectoryInfo[] dirList = dir.GetDirectories();
                foreach (DirectoryInfo di in dirList)
                {
                    if (di.CreationTime <= DateTime.Now.AddHours(-1))
                        di.Delete(true);
                }
            }
            else
                Directory.CreateDirectory(BasePath + @"\" + DocumentType);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }
    public static void SaveFileLocally(byte[] bytes, string localPath, string dockType)
    {
        File.WriteAllBytes(localPath, bytes);
    }
}
