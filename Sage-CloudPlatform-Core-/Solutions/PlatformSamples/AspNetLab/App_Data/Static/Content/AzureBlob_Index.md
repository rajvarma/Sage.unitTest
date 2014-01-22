##Sage BlobStore usage and Sample

BlobStore service is an extension to Windows Azure Blob, with transient fault handling. The implementation provides APIs for upload, download, delete. Following are the steps involved to use the blob service.

1. Install Sage.Core.Blob NuGet package from Sage NuGet gallery.
2. Add a new configuration in azure configuration with the name SystemStorageConnectionString and set the storage connection string.
3. You can DI BlobStore or manually instantiate the object and use it.
4. Currently uploading of Stream, Byte array and string is allowed.

####Usage

To instantiate BlobStore

    IBlobStore blob = new BlobStore(new ConfigurationManager());

To upload a file

        blob.Put(context, blobData);

    Where,
    context: The context should have application's current context id and its children.

    blobData: Uploading of Stream, Byte array and string is possible.

    Creating StreamBlob
        StreamBlob blobData = new StreamBlob();
            
    blobData.Content will have the stream content to be uploaded.
    blobData.Name will have the unique name of the file. Name can only contain letters in lowercase, numbers, or the following special characters:  . + - ! * ` ( ) $.
    blobData.Path will have the container name (place) where the content will be uploaded. Path must start with a letter or number, and can contain only letters in lowercase, numbers, and the dash (-) character. Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted.

    Creating ByteBlob
        ByteBlob blobData = new ByteBlob();
        
    blobData.Content will have the byte array to be uploaded.
    blobData.Name will have the unique name of the file. Name can only contain letters in lowercase, numbers, or the following special characters:  . + - ! * ` ( ) $.
    blobData.Path will have the container name (place) where the content will be uploaded. Path must start with a letter or number, and can contain only letters in lowercase, numbers, and the dash (-) character. Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted.

    Creating TextBlob
        TextBlob blobData = new TextBlob();
        
    blobData.Content will have the string (text) to be uploaded.
    blobData.Name will have the unique name of the file. Name can only contain letters in lowercase, numbers, or the following special characters:  . + - ! * ` ( ) $.
    blobData.Path will have the container name (place) where the content will be uploaded. Path must start with a letter or number, and can contain only letters in lowercase, numbers, and the dash (-) character. Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted.


To download a file
    
        blob.Get(context, blobData);

    Blob content can be downloaded as Stream, Byte array or Text (string). The type of the content need to be passed as input to the Get.
    
    
To delete a file
    
    blob.Delete(context, blobData);
