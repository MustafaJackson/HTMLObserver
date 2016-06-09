using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace HTMLObserver
{
    public class HTMLObserver
    {
        private Uri urlName { get; set; }
        private string oldHTML { get; set; }
        private string newHTML { get; set; }
        private StorageFile htmlFile { get; set; }
        public bool HTMLChanged { get; set; }

        public async Task<bool> CheckForChanges(string urlString)
        {
            urlName = new Uri(urlString.Trim());
            BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
            htmlFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("htmlFile",CreationCollisionOption.ReplaceExisting);
            DownloadOperation download = backgroundDownloader.CreateDownload(urlName, htmlFile);
        }
    }
}
