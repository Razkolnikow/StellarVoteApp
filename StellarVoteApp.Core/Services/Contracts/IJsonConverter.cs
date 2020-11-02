using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Services.Contracts
{
    public interface IJsonConverter
    {
        T DeserializeJson<T>(string json);

        string SerializeObject<T>(T obj);
    }
}
