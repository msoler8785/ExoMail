using Microsoft.IO;
using System;

namespace ExoMail.Smtp.Common.Services
{
    /// <summary>
    /// A singleton class for storing a recyclable memory stream manager.
    /// </summary>
    public sealed class BufferManager
    {
        public RecyclableMemoryStreamManager MemoryStreamManager { get; private set; }

        /// <summary>
        /// A lazy singleton initializer.
        /// </summary>
        private static readonly Lazy<BufferManager> _instance =
            new Lazy<BufferManager>(() => new BufferManager());

        /// <summary>
        /// Retrieve the singleton instance.
        /// </summary>
        public static BufferManager Instance { get { return _instance.Value; } }

        public static RecyclableMemoryStream GetStream()
        {
            return BufferManager.Instance.MemoryStreamManager.GetStream() as RecyclableMemoryStream;
        }

        /// <summary>
        /// Private constructor for this singleton.
        /// </summary>
        private BufferManager()
        {
            //int blockSize = 1024 * 8;
            //int largeBufferMultiple = 1024 * 1024;
            //int maxBufferSize = 1024 * largeBufferMultiple;

            //MemoryStreamManager = new RecyclableMemoryStreamManager(blockSize, largeBufferMultiple, maxBufferSize)
            //{
            //    AggressiveBufferReturn = true,
            //    MaximumFreeLargePoolBytes = maxBufferSize * 4,
            //    MaximumFreeSmallPoolBytes = 200 * blockSize
            //};

            MemoryStreamManager = new RecyclableMemoryStreamManager();
        }
    }
}