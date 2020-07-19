namespace CMaker.Core
{
    using System;
    using System.IO;

    /// <summary>
    /// Self-deleting temporary file
    /// https://stackoverflow.com/a/400391/1755158
    /// </summary>
    internal sealed class TempFile : IDisposable
    {
        private string _path;

        /// <summary>
        /// Create a new temp file with a random name in the standard temp directory
        /// </summary>
        public TempFile() : this(System.IO.Path.GetTempFileName())
        {
        }

        /// <summary>
        /// Create a new temp file at the specified location
        /// </summary>
        /// <param name="path">Path to file to use as temp file</param>
        public TempFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            _path = path;
        }

        /// <summary>
        /// Full path to temp file
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public string Path
        {
            get
            {
                if (_path == null)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _path;
            }
        }

        /// <inheritdoc />
        ~TempFile()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }
        
        /// <summary>
        /// Make a best effort attempt to delete this temp file
        /// </summary>
        /// <param name="disposing">true to apply finalization</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            if (_path == null)
            {
                return;
            }

            try
            {
                File.Delete(_path);
            }
            catch
            {
                // best effort
            }

            _path = null;
        }
    }
}