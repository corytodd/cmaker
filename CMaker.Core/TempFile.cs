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

        public TempFile() : this(System.IO.Path.GetTempFileName())
        {
        }

        public TempFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            _path = path;
        }

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

        ~TempFile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

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