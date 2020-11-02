using Newtonsoft.Json;
using StellarVoteApp.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Services
{
    public class JsonConverter : IJsonConverter
    {
        public T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
