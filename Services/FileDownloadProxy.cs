using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Downloader;

namespace LLC_MOD_Toolbox.Services
{
    internal class FileDownloadProxy(
        GrayFileDownloadService grayFileDownloadService,
        RegularFileDownloadService regularFileDownloadService
    )
    {
        private IFileDownloadService _fileDownloadService = regularFileDownloadService;

        void ChangeService(bool isGray)
        {
            if (isGray)
            {
                _fileDownloadService = grayFileDownloadService;
            }
            else
            {
                _fileDownloadService = regularFileDownloadService;
            }
        }
    }
}
