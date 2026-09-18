using AuthentikUserImporter.Exceptions;
using AuthentikUserImporter.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuthentikUserImporter.Helper
{
    internal class AuthentikHelper
    {
        static readonly ConcurrentDictionary<string, string> _emailStageCache = new();

        public static async Task TriggerRecoveryEmail(HttpClient http, string userId, string emailStageName)
        {
            var tokenResp = await http.PostAsync(
                $"/api/v3/core/users/{userId}/recovery/",
                new StringContent("{}", Encoding.UTF8, "application/json"));

            if (!tokenResp.IsSuccessStatusCode)
            {
                var body = await tokenResp.Content.ReadAsStringAsync();
                throw new AuthentikException($"Recovery-Token fehlgeschlagen: {body}");
            }

            var tokenJson = await tokenResp.Content.ReadAsStringAsync();
            var tokenDoc = JsonDocument.Parse(tokenJson);

            var emailStageId = await ResolveEmailStageId(http, emailStageName);

            var mailResp = await http.PostAsync(
                $"/api/v3/core/users/{userId}/recovery_email/",
                new StringContent(
                    JsonSerializer.Serialize(new { email_stage = emailStageId }),
                    Encoding.UTF8, "application/json"));

            if (!mailResp.IsSuccessStatusCode)
            {
                var body = await mailResp.Content.ReadAsStringAsync();
                Console.Write($"[WARNUNG: Recovery-Mail nicht gesendet: {(int)mailResp.StatusCode}] ");
            }
        }
        static async Task<string> ResolveEmailStageId(HttpClient http, string name)
        {
            if (_emailStageCache.TryGetValue(name, out var cachedId))
                return cachedId;

            var response = await http.GetAsync(
                $"/api/v3/stages/email/?name={Uri.EscapeDataString(name)}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("results");

            if (results.GetArrayLength() == 0)
                throw new AuthentikException($"Email-Stage mit Namen '{name}' wurde nicht gefunden.");

            var id = results[0].GetProperty("pk").GetString()
                ?? throw new AuthentikException($"Email-Stage '{name}' hat keine gültige ID.");

            _emailStageCache[name] = id;
            return id;
        }
        public static async Task AddUserToGroup(HttpClient http, string userId, string groupId)
        {
            var payload = new { pk = userId };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"/api/v3/core/groups/{groupId}/add_user/";
            var resp = await http.PostAsync(url, content);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                // Nicht-fatal: nur warnen
                Console.Write($"[WARNUNG: Gruppe nicht zugewiesen: {body}] ");
            }
        }
        public static async Task<string> CreateUser(HttpClient http, User user)
        {
            var payload = new Dictionary<string, object?>
            {
                ["username"] = user.Username,
                ["email"] = user.Email,
                ["name"] = $"{user.LastName}, {user.FirstName}, MBG Harsewinkel".Trim(),
                ["is_active"] = true,
                ["path"] = user.Path,
                ["password"] = null,
                ["type"] = "internal",
            };

            // Custom Attributes zusammenbauen (nur setzen, wenn Werte vorhanden sind)
            var attributes = new Dictionary<string, object?>();
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                attributes["phone_number"] = user.PhoneNumber;
            }
            attributes["sms"] = user.Sms;

            payload["attributes"] = attributes;

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await http.PostAsync("/api/v3/core/users/", content);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var body = await response.Content.ReadAsStringAsync();
                // Authentik meldet doppelte Benutzer als 400 mit "username" im Body
                if (body.Contains("username") || body.Contains("already exists"))
                    throw new AuthentikException("Bereits vorhanden", isConflict: true);
                throw new AuthentikException($"400 Bad Request: {body}");
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(result);
            var pk = doc.RootElement.GetProperty("pk");
            // Authentik liefert pk je nach Version als Integer oder String
            return pk.ValueKind == JsonValueKind.Number
                ? pk.GetInt64().ToString()
                : pk.GetString() ?? throw new AuthentikException("Keine ID in API-Antwort.");
        }
        public static async Task<bool> UsernameExists(HttpClient http, string username)
        {
            var response = await http.GetAsync($"/api/v3/core/users/?username={Uri.EscapeDataString(username)}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(result);
            var count = doc.RootElement
                .GetProperty("pagination")
                .GetProperty("count")
                .GetInt32();
            return count > 0;
        }
    }
}
