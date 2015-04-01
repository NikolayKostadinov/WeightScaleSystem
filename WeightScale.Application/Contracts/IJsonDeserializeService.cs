using System;

namespace WeightScale.Application.Contracts
{
    public interface IJsonDeserializeService
    {
        object GetResultFromJson(string jsonAnswer, object inputObject);
    }
}
