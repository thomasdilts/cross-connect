// <copyright file="WebClient.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

namespace CrossConnect
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.Networking.BackgroundTransfer;
    using Windows.Storage;
    using Windows.Storage.Streams;

    internal class WebClient
    {
        public const string destination = "Atemporaryfilenamehere";

        #region Fields

        public StorageFile downloadedFile;

        private CancellationTokenSource cts = new CancellationTokenSource();

        private DownloadOperation download;

        #endregion

        #region Delegates

        public delegate void OpenReadCompletedDelegate(string error);

        public delegate void OpenReadDownloadingDelegate(byte percent);

        #endregion

        #region Events

        private event OpenReadCompletedDelegate OpenReadCompleted;

        private event OpenReadDownloadingDelegate OpenReadDownloading;

        #endregion

        #region Public Methods and Operators

        public static async Task<byte[]> ReadStreamAsyncBytes(IRandomAccessStreamWithContentType stream)
        {
            using (var dataReader = new DataReader(stream))
            {
                var bytes = new byte[stream.Size];
                await dataReader.LoadAsync((uint)stream.Size);
                dataReader.ReadBytes(bytes);
                return bytes;
            }
        }

        public void CancelDownload()
        {
            this.cts.Cancel();
            this.cts.Dispose();

            // Re-create the CancellationTokenSource and activeDownloads for future downloads.
            this.cts = new CancellationTokenSource();
            this.download = null;
        }

        public async Task<byte[]> ReadStorageFileAsyncBytes()
        {
            if (this.downloadedFile == null)
            {
                return new byte[0];
            }

            IRandomAccessStreamWithContentType stream = await this.downloadedFile.OpenReadAsync();
            byte[] bytes = await ReadStreamAsyncBytes(stream);
            return bytes;
        }

        // Note that this event is invoked on a background thread, so we cannot access the UI directly.

        public async Task<string> ReadStorageFileAsyncString()
        {
            byte[] buffer = await this.ReadStorageFileAsyncBytes();
            return Encoding.UTF8.GetString(buffer, 0, buffer.Count());
        }

        public async void RemoveTempFile()
        {
            if (this.downloadedFile != null)
            {
                try
                {
                    await this.downloadedFile.DeleteAsync();
                }
                catch (Exception ee)
                {
                    
                }
                this.downloadedFile = null;
            }
        }

        public async void StartDownload(
            string url, OpenReadCompletedDelegate OpenReadCompletedIn, OpenReadDownloadingDelegate OpenReadDownloadingIn)
        {
            this.OpenReadCompleted += OpenReadCompletedIn;
            this.OpenReadDownloading += OpenReadDownloadingIn;

            // By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
            // box validating the URI is required since it was received from an untrusted source (user input).
            // The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.
            Uri source;
            if (!Uri.TryCreate(url, UriKind.Absolute, out source))
            {
                if (this.OpenReadCompleted != null)
                {
                    this.OpenReadCompleted("Invalid URI.");
                }
                return;
            }

            try
            {
                this.downloadedFile =
                    await
                    ApplicationData.Current.TemporaryFolder.CreateFileAsync(
                        destination, CreationCollisionOption.GenerateUniqueName);
            }
            catch (FileNotFoundException ex)
            {
                this.OpenReadCompleted("Error while creating file: " + ex.Message);
                //rootPage.NotifyUser("Error while creating file: " + ex.Message, NotifyType.ErrorMessage);
                return;
            }

            var downloader = new BackgroundDownloader();
            try
            {
                this.download = downloader.CreateDownload(source, this.downloadedFile);
            }
            catch (Exception ex)
            {
                this.OpenReadCompleted("Error while downloading file: " + ex.Message);
                return;
            }
            //Log(String.Format("Downloading {0} to {1}, {2}", source.AbsoluteUri, destinationFile.Name, download.Guid));

            // Attach progress and completion handlers.
            await this.HandleDownloadAsync();
        }

        #endregion

        #region Methods

        private void DownloadProgress(DownloadOperation download)
        {
            //MarshalLog(String.Format("Progress: {0}, Status: {1}", download.Guid, download.Progress.Status));

            double percent = 100;
            if (download.Progress.TotalBytesToReceive > 0)
            {
                percent = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive;
            }

            if (this.OpenReadDownloading != null)
            {
                this.OpenReadDownloading((byte)percent);
            }

            //MarshalLog(String.Format(" - Transfered bytes: {0} of {1}, {2}%",
            //    download.Progress.BytesReceived, download.Progress.TotalBytesToReceive, percent));

            if (download.Progress.HasRestarted)
            {
                //MarshalLog(" - Download restarted");
            }

            if (download.Progress.HasResponseChanged)
            {
                // We've received new response headers from the server.
                //MarshalLog(" - Response updated; Header count: " + download.GetResponseInformation().Headers.Count);

                // If you want to stream the response data this is a good time to start.
                // download.GetResultStreamAt(0);
            }
        }

        private async Task HandleDownloadAsync()
        {
            try
            {
                //LogStatus("Running: " + download.Guid, NotifyType.StatusMessage);

                // Store the download so we can pause/resume.
                //activeDownloads.Add(download);

                var progressCallback = new Progress<DownloadOperation>(this.DownloadProgress);

                var xx = new CancellationToken(false);
                //cts.Token.CanBeCanceled = false;
                // Start the download and attach a progress handler.
                DownloadOperation x =
                    await this.download.StartAsync().AsTask(new CancellationToken(false), progressCallback);

                ResponseInformation response = this.download.GetResponseInformation();

                if (this.OpenReadCompleted != null)
                {
                    this.OpenReadCompleted(string.Empty);
                }

                //LogStatus(String.Format("Completed: {0}, Status Code: {1}", download.Guid, response.StatusCode),
                //    NotifyType.StatusMessage);
            }
            catch (TaskCanceledException)
            {
                if (this.OpenReadCompleted != null)
                {
                    this.OpenReadCompleted("Canceled");
                }
            }
            catch (Exception ex)
            {
                if (this.OpenReadCompleted != null)
                {
                    this.OpenReadCompleted(ex.Message);
                }
            }
            finally
            {
                //download = null;
            }
        }

        #endregion
    }
}