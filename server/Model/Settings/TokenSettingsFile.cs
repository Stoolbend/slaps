using System;
using System.Text.Json.Serialization;

namespace BCI.SLAPS.Server.Model.Settings
{
    public class TokenSettingsFile
    {
        [JsonIgnoreAttribute]
        public byte[] Secret
        {
            get => Convert.FromBase64String(EncSecret);
            set => EncSecret = Convert.ToBase64String(value, Base64FormattingOptions.None);
        }
        public string EncSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
