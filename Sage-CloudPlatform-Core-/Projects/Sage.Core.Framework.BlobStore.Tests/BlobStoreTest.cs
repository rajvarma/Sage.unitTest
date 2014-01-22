using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sage.Core.Framework.Common;
using Sage.Core.Framework.Configuration;
using Sage.Core.Framework.Storage;
using System.IO;


namespace Sage.Core.Framework.BlobStore.Tests
{
    /// <summary>
    /// Summary description for BlobStoreTest
    /// </summary>
    [TestClass]
    public class BlobStoreTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>

        #region Test methods

        [TestMethod]
        public void CanPut()
        {
            var mockConfiguration = new Mock<IConfigurationManager>();

            var systemConfiguration = new Mock<IConfiguration>();
            var applicationConfiguration = new Mock<IConfiguration>();
        
            mockConfiguration.Setup(ctx => ctx.ApplicationConfiguration).Returns(applicationConfiguration.Object);
            mockConfiguration.Setup(ctx => ctx.SystemConfiguration).Returns(systemConfiguration.Object);

            
            _container = new UnityContainer();
            var mock = new Mock<IBlobStore>();
           
            _container.RegisterInstance(mock.Object);

            _container.RegisterInstance(mockConfiguration.Object);
            var test = new MockBlobData();
          
           

            mock.Setup(m => new BaseBlobStore(Storage.BlobStore.GetBlobClient(mockConfiguration.Object)).Put(test));

            mock.Setup(m => new Storage.BlobStore(mockConfiguration.Object).Put(_context, streamBlobData));
            var mock2 = new Mock<BaseBlobStore>();
            mock2.Setup(m =>  new BaseBlobStore(Storage.BlobStore.GetBlobClient(mockConfiguration.Object)).Put(test));

            mock2.Verify();
            
            mock.Verify();

        }

       

        [TestMethod]
        public void CanGet()
        {
            _container = new UnityContainer();
            var mock = new Mock<IBlobStore>();
            _container.RegisterInstance(mock.Object);
            mock.Setup(m => m.Get(_context, streamBlobData));
            mock.Verify();
        }

        [TestMethod]
        public void CanDelete()
        {
            _container = new UnityContainer();
            var mock = new Mock<IBlobStore>();
            _container.RegisterInstance(mock.Object);
            mock.Setup(m => m.Delete(_context, streamBlobData));
            mock.Verify();
        }

        #endregion

        #region Additional test attributes

        /// <summary>
        /// Run windows azure emulator to do azure operation.
        /// </summary>
        [AssemblyInitialize]
        public static void BeforeAssembly(TestContext testContext)
        {
        }

        //{
        //    var process = Process.Start(@"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun", "/devstore");
        //    if (process != null)
        //    {
        //        process.WaitForExit();
        //    }
        //    else
        //    {
        //        throw new ApplicationException("Unable to start storage emulator.");
        //    }
        //}

        /// <summary>
        /// Delete table from table storage.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {

        }

        /// <summary>
        ///  
        /// You can use the following additional attributes as you write your tests:
        ///
        /// Use ClassInitialize to run code before running the first test in the class
        /// </summary>
        [ClassInitialize]
        public static void BeforeTest(TestContext testContext)
        {
            _context = new Context {ContextChain = new ContextChainLink()};

        }
      

        #endregion

        #region Properties

        private static Context _context;

        private  IUnityContainer _container;

        private StreamBlob streamBlobData
        {
            get { return new StreamBlob {Name = Guid.NewGuid().ToString(), Path = "sagelabs", Content = new MemoryStream()}; }
        }

        #endregion
    }
}
