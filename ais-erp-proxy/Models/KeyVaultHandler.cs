using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;

namespace ais_erp_proxy.Models
{
    public class KeyVaultHandler
    {
        private static string secret_word = "mklds"; // leave this as it is

        private static string client_id = ConfigurationManager.AppSettings["client_id"];
        private static string client_secret = ConfigurationManager.AppSettings["client_secret"];
        private static string tenant_id = ConfigurationManager.AppSettings["tenant_id"];
        private static string scope = ConfigurationManager.AppSettings["scope"];
        private static string grant_type = ConfigurationManager.AppSettings["grant_type"];
        private static string token_url = ConfigurationManager.AppSettings["token_url"];
        private static string vault_uri = ConfigurationManager.AppSettings["vault_uri"];
        private static string bearer_token = ConfigurationManager.AppSettings["bearer_token"];
        private static string api_version = ConfigurationManager.AppSettings["api_version"];

        public static credential_info GetCredentials(string type)
        {
            string new_token;

            if (!TryGetSecret("bearer-token", bearer_token))
            {
                new_token = GetAndUpdateToken();
            }
            else
            {
                return CollectCredentials(type, bearer_token);
            }

            if (new_token != null)
            {
                return CollectCredentials(type, new_token);
            }
            else
            {
                return null;
            }
        }

        private static credential_info CollectCredentials(string type, string current_token)
        {
            credential_info credential_info = new credential_info();
            credential_info.username = GetSecret(type + "-username", current_token).value;
            credential_info.password = GetSecret(type + "-password", current_token).value;
            return credential_info;
        }

        private static string GetAndUpdateToken()
        {
            azure_token_json azure_token_json = new azure_token_json();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string tmp_token_url = token_url;
                    string url = tmp_token_url.Replace("{tenant_id}", tenant_id);

                    var data = new[]
                    {
                        new KeyValuePair<string, string>("grant_type", grant_type),
                        new KeyValuePair<string, string>("client_id", client_id),
                        new KeyValuePair<string, string>("client_secret", client_secret),
                        new KeyValuePair<string, string>("scope", scope),
                    };

                    var response = httpClient.PostAsync(url, new FormUrlEncodedContent(data)).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        azure_token_json = JsonConvert.DeserializeObject<azure_token_json>(response.Content.ReadAsStringAsync().Result);
                        UpdateWebConfigValue("bearer_token", azure_token_json.access_token);
                        return azure_token_json.access_token;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static azure_secret_json GetSecret(string secret_name, string bearer_token)
        {
            azure_secret_json azure_Secret_Json = new azure_secret_json();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string url = vault_uri + "/secrets/" + secret_name + "?api-version=" + api_version;
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearer_token);
                    var response = httpClient.GetAsync(url).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        azure_Secret_Json = JsonConvert.DeserializeObject<azure_secret_json>(response.Content.ReadAsStringAsync().Result);
                        return azure_Secret_Json;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool TryGetSecret(string secret_name, string bearer_token)
        {
            azure_secret_json azure_Secret_Json = new azure_secret_json();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string url = vault_uri + "/secrets/" + secret_name + "?api-version=" + api_version;
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearer_token);
                    var response = httpClient.GetAsync(url).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) { return true; }
                    else { return false; }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void UpdateWebConfigValue(string key_name, string new_value)
        {
            try
            {
                var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                configuration.AppSettings.Settings.Remove(key_name);
                configuration.AppSettings.Settings.Add(key_name, new_value);
                configuration.Save();
            }
            catch(Exception)
            {  }
        }

        private static string Decrypt(string text_criptat_tb)
        {
            try
            {
                byte[] initial_text_bytes = Convert.FromBase64String(text_criptat_tb);
                byte[] secret_word_bytes = Encoding.UTF8.GetBytes(secret_word);

                byte[] encrypted_bytes = new byte[initial_text_bytes.Length];

                int secret_word_index = 0;
                for (int i = 0; i < initial_text_bytes.Length; i++)
                {
                    if (secret_word_index == secret_word_bytes.Length)
                    {
                        secret_word_index = 0;
                    }
                    encrypted_bytes[i] = (byte)(initial_text_bytes[i] - secret_word_bytes[secret_word_index]);
                    secret_word_index++;
                }

                string initial_text_tb = Encoding.UTF8.GetString(encrypted_bytes);
                return initial_text_tb;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class azure_token_json
    {
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get; set; }
    }

    public class azure_secret_json
    {
        public string value { get; set; }
        public string id { get; set; }
        public attributes attributes { get; set; }
        public object tags { get; set; }
    }

    public class attributes
    {
        public string enabled { get; set; }
        public int created { get; set; }
        public int updated { get; set; }
        public string recoveryLevel { get; set; }
    }

    public class secret_update_json
    {
        public string value { get; set; }
    }

    public class credential_info
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}