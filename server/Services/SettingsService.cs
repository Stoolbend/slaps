using BCI.SLAPS.Server.Helpers;
using BCI.SLAPS.Server.Model.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BCI.SLAPS.Server.Services
{
    public interface ISettingsService
    {
        SettingsEntry GetEntry(string key);
        Task<SettingsEntry> GetEntryAsync(string key, CancellationToken ct = default);

        byte[] GetJwtSecret();
        string GetJwtIssuer();
        string GetJwtAudience();
    }

    public class SettingsService : ISettingsService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _db;

        public SettingsService(IConfiguration config, DataContext db)
        {
            _config = config;
            _db = db;
        }

        #region Generic settings
        public async Task<SettingsEntry> GetEntryAsync(string key, CancellationToken ct = default)
        {
            return await _db.Settings.Where(s => s.Key == key)
                                     .FirstAsync(ct);
        }

        public SettingsEntry GetEntry(string key)
        {
            return _db.Settings.Where(s => s.Key == key)
                               .First();
        }
        #endregion

        #region Specific settings
        private const string JWT_CONFIG_FILENAME = "tokensettings.json";
        private TokenSettingsFile _tokenSettings;

        // Read existing stored values for token secrets & issuers
        private TokenSettingsFile ReadTokenSettingsFile()
        {
            try
            {
                // Attempt reading the file
                using (var reader = new StreamReader(Path.Combine(_config["ConfigFilePath"], JWT_CONFIG_FILENAME)))
                {
                    return JsonSerializer.Deserialize<TokenSettingsFile>(reader.ReadToEnd());
                }
            }
            catch (IOException)
            {
                // If any errors, then regenerate the local secrets file
                // Generate values and serialize object
                var tokenSettings = new TokenSettingsFile()
                {
                    Secret = PasswordUtils.GenerateSalt(128),
                    Issuer = Convert.ToBase64String(PasswordUtils.GenerateSalt(64), Base64FormattingOptions.None),
                    Audience = Convert.ToBase64String(PasswordUtils.GenerateSalt(64), Base64FormattingOptions.None)
                };

                // Write new values to file
                if (!Directory.Exists(_config["ConfigFilePath"]))
                    Directory.CreateDirectory(_config["ConfigFilePath"]);
                var file = File.Create(Path.Combine(_config["ConfigFilePath"], JWT_CONFIG_FILENAME));
                using (var writer = new StreamWriter(file, Encoding.UTF8))
                {
                    writer.WriteLine(JsonSerializer.Serialize(tokenSettings));
                    return tokenSettings;
                }
            }
        }

        public byte[] GetJwtSecret()
        {
            return ReadTokenSettingsFile().Secret;
        }
        public string GetJwtIssuer()
        {
            return ReadTokenSettingsFile().Issuer;
        }
        public string GetJwtAudience()
        {
            return ReadTokenSettingsFile().Audience;
        }
        #endregion
    }
}
