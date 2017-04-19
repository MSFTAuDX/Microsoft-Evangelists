using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MicrosoftEvangelists.Models;
using Newtonsoft.Json;
using MicrosoftEvangelists.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Globalization;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;

namespace MicrosoftEvangelists.Controllers
{
    public class HomeController : Controller
    {
        private DocumentClient client;
        DocumentDbSettings settings;
        public HomeController(DocumentDbSettings _settings)
        {
            //dependency injection of Azure DocDB connection settings.
            settings = _settings;
        }
        private List<Profile> ExecuteSimpleQuery(string database, string collection)
        { 
            List<Profile> profiles = new List<Profile>();
            
            //create Query on Profiles DB,
            IQueryable<Profile> profileQuery = this.client.CreateDocumentQuery<Profile>(
                    UriFactory.CreateDocumentCollectionUri(database, collection));
            
            //add each profile to List of profiles.
            foreach (Profile profile in profileQuery)
            {
                profiles.Add(profile);
            }

            return profiles;
            
        }
        public async Task<IActionResult> Index(string SelectedCountry = "ALL", string SelectedState = "ALL", string SelectedCity = "ALL", string SelectedTag = "ALL")
        {
           //create DB client
            client = new DocumentClient(settings.DatabaseUri, settings.DatabaseKey);
            
            //get profiles
            List<Profile> allProfiles = this.ExecuteSimpleQuery(settings.DatabaseName, settings.CollectionName);

            
            //get filtered profiles and randomise order
            var rnd = new Random();
            var filteredProfiles = allProfiles
                .Where(p => p.country == SelectedCountry || SelectedCountry == "ALL" || p.country.Contains("ALL"))
                .Where(p => p.states.Contains(SelectedState) || SelectedState == "ALL" || p.states.Contains("ALL"))
                .Where(p => p.cities.Contains(SelectedCity) || SelectedCity == "ALL" || p.cities.Contains("ALL"))
                .Where(p => p.tags.Contains(SelectedTag) || SelectedTag == "ALL" || p.tags.Contains("ALL"))
                .OrderBy(p => rnd.Next())
                .ToList();

            //mine data for drop-down values and sort them
            var avaliableCities = allProfiles.SelectMany(p => p.cities).Distinct().ToList();
            var avaliableStates = allProfiles.SelectMany(p => p.states).Distinct().ToList();
            var avaliableTags = allProfiles.SelectMany(p => p.tags).Distinct().ToList();
            var avaliableCountries = allProfiles.Select(p => p.country).Distinct().ToList();
            avaliableCities.Sort();
            avaliableCities.Remove("ALL");
            avaliableStates.Sort();
            avaliableStates.Remove("ALL");
            avaliableTags.Sort();
            avaliableTags.Remove("ALL");
            avaliableCountries.Sort();
            avaliableCountries.Remove("ALL");

            //create select lists for view
            var avaliableCitiesSelect = new SelectList(avaliableCities);
            var avaliableStatesSelect = new SelectList(avaliableStates);
            var avaliableTagsSelect = new SelectList(avaliableTags);
            var avaliableCountriesSelect = new SelectList(avaliableCountries);

            //construct view model
            var vm = new HomeIndexViewModel() {
                Profiles = filteredProfiles,
                AvaliableCities = avaliableCitiesSelect,
                AvaliableStates = avaliableStatesSelect,
                AvaliableTags = avaliableTagsSelect,
                AvaliableCountries = avaliableCountriesSelect,
                SelectedCountry = SelectedCountry,
                SelectedState = SelectedState,
                SelectedCity = SelectedCity,
                SelectedTag = SelectedTag
            };

            //render view
            return View(vm);
        }

        public IActionResult Refresh()
        {
            return RedirectToAction("Index");
        }

        private string ReadSessionData(string key)
        {
            byte[] bytes;
            HttpContext.Session.TryGetValue(key, out bytes);
            if (bytes == null)
            {
                return string.Empty;
            }
            else
            {
                char[] chars = new char[bytes.Length / sizeof(char)];
                Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
                return new string(chars);
            }

        }

        private void SetSessionData(string key, string value)
        {
            byte[] valueAsBytes = new byte[value.Length * sizeof(char)];
            System.Buffer.BlockCopy(value.ToCharArray(), 0, valueAsBytes, 0, valueAsBytes.Length);
            HttpContext.Session.Set(key, valueAsBytes);
        }

    }
}
