using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BCI.SLAPS.Server.Model.Settings
{
    public class SettingsEntry
    {
        [Key]
        public string Key { get; set; }
        public SettingsEntryType Type { get; set; }
        public string StringContent { get; set; }
        public int? IntegerContent { get; set; }
        public byte[] ByteArrayContent { get; set; }

        [NotMapped]
        public object Value
        {
            get
            {
                switch (Type)
                {
                    case SettingsEntryType.String:
                        return StringContent;
                    case SettingsEntryType.Integer:
                        return IntegerContent;
                    case SettingsEntryType.ByteArray:
                        return ByteArrayContent;
                    default:
                        return StringContent;
                }
            }
        }
    }
}
