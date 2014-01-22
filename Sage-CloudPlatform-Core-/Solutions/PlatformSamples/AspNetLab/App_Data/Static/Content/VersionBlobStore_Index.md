##Sage VersionBlobStore usage and Sample

Sage Blob service is an extension to Windows Azure Blob, with transient fault handling. The basic implementation provides APIs for upload, download, delete. Following are the steps involved to use the blob service.

1. Install Sage.Core.Blob NuGet package from Sage NuGet gallery.
2. Add a new configuration in azure configuration with the name SystemStorageConnectionString and set the storage connection string.
3. You can DI VersionBlobStore or manually instantiate the object and use it.
4. Currently uploading of Stream, Byte array and string is allowed.

####Usage

To instantiate VersionBlobStore

    IVersionBlobStore versionBlob = new VersionBlobStore(new ConfigurationManager());

To upload a file

If the blob already exists a new version will be created. Otherwise, 1st version of the file will be uploaded.

        versionBlob.Put(context, blobData);
    Where, 
    context: The context should have application's current context id and its children.

    blobData: Uploading of Stream (StreamBlob), Byte array (ByteBlob) and string (TextBlob) is possible.

        StreamBlob blobData = new StreamBlob();            
    blobData.Content will have the stream content to be uploaded.
    blobData.Name will have the unique name of the file. Name can only contain letters in lowercase, numbers, or the following special characters:  . + - ! * ` ( ) $.
    blobData.Path will have the container name (place) where the content will be uploaded. Path must start with a letter or number, and can contain only letters in lowercase, numbers, and the dash (-) character. Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted.

        ByteBlob blobData = new ByteBlob();        
    blobData.Content will have the byte array to be uploaded.
    blobData.Name will have the unique name of the file. Name can only contain letters in lowercase, numbers, or the following special characters:  . + - ! * ` ( ) $.
    blobData.Path will have the container name (place) where the content will be uploaded. Path must start with a letter or number, and can contain only letters in lowercase, numbers, and the dash (-) character. Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted.

        TextBlob blobData = new TextBlob();        
    blobData.Content will have the string (text) to be uploaded.
    blobData.Name will have the unique name of the file. Name can only contain letters in lowercase, numbers, or the following special characters:  . + - ! * ` ( ) $.
    blobData.Path will have the container name (place) where the content will be uploaded. Path must start with a letter or number, and can contain only letters in lowercase, numbers, and the dash (-) character. Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted.

To download a version
    
        versionBlob.Get(context, versionBlobData);

    Blob content can be downloaded as Stream, Byte array or Text (string). The type of the content need to be passed as input to the Get.
    
    Where, 
    context: The context should have application's current context id and its children.
    
    versionBlobData: 
    Creating versionBlobData   
        var blobData = new VersionBlobData<Stream>();

    Downloading Stream,
        version.BaseBlobData = new new StreamBlob();
                       
To get all version

        versionBlob.GetVersionInfo(context, blobData);

    Where, 
    context: The context should have application's current context id and its children.
    
    blobData: The information such as name, path of the file.
   
To delete a version
    
        versionBlob.Delete(context, versionBlobData);

    Where, 
    context: The context should have application's current context id and its children.

    Creating versionBlobData   
        var blobData = new VersionBlobData<Stream>();

    blobData.SnapshotDateTime will have the snapshot datetime of that version.