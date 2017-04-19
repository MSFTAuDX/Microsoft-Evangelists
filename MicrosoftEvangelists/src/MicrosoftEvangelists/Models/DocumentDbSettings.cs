using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace MicrosoftEvangelists.Models
{
    public class DocumentDbSettings

    {

        public DocumentDbSettings(IConfiguration configuration)

        {

            try
            {

                DatabaseName = configuration.GetSection("database").Value;

                CollectionName = configuration.GetSection("collection").Value;

                DatabaseUri = new Uri(configuration.GetSection("endpoint").Value);

                DatabaseKey = configuration.GetSection("authKey").Value;

            }

            catch

            {

                throw new MissingFieldException("IConfiguration missing a valid Azure DocumentDB fields on DocumentDB > [DatabaseName,CollectionName,EndpointUri,Key]");

            }

        }

        public string DatabaseName { get; private set; }

        public string CollectionName { get; private set; }

        public Uri DatabaseUri { get; private set; }

        public string DatabaseKey { get; private set; }

    }
}
