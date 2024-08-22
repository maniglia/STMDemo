using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace STMDemo
{
    public interface IFirmwareService
    {
        public Task<bool> UploadFirmware();
    }
}
