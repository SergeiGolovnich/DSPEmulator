using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DSPEmulatorUI
{
    public class AudioFileInfo
    {
        public string OriginFolder { get; set; }
        public string FullPath { get; set; }
        [JsonIgnore]
        public string RelativePath => Path.GetRelativePath(OriginFolder, FullPath);
    }
}
