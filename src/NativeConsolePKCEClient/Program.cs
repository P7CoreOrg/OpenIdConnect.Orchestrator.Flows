﻿using IdentityModel.OidcClient;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NativeConsolePKCEClient
{
    class Program
    {
        static string _clientId = "1096301616546-edbl612881t7rkpljp3qa3juminskulo.apps.googleusercontent.com";
        static string _authority = "https://localhost:5001";

        static OidcClient _oidcClient;
       

        public static void Main(string[] args) => RunAsync().GetAwaiter().GetResult();

        public static async Task RunAsync()
        {
            await Login();
        }
        private static async Task Login()
        {
            var browser = new SystemBrowser(45656);
            string redirectUri = "http://127.0.0.1:45656";

            var options = new OidcClientOptions
            {
                Authority = _authority,
                ClientId = _clientId,
                RedirectUri = redirectUri,
                Scope = "openid profile",
                FilterClaims = false,
                Browser = browser,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                LoadProfile = true
                
            };
           
            options.Policy.Discovery.ValidateIssuerName = false;
            options.Policy.Discovery.ValidateEndpoints = false;
            var serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                .CreateLogger();

            options.LoggerFactory.AddSerilog(serilog);

            _oidcClient = new OidcClient(options);
            var result = await _oidcClient.LoginAsync(new LoginRequest());

            ShowResult(result);
            
        }

        private static void ShowResult(LoginResult result)
        {
            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
                return;
            }

            Console.WriteLine("\n\nClaims:");
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            Console.WriteLine($"\nidentity token: {result.IdentityToken}");
            Console.WriteLine($"access token:   {result.AccessToken}");
            Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
        }

    }
}
