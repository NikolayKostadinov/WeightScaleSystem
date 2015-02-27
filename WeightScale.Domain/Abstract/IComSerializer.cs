using System;
namespace WeightScale.Domain.Abstract
{
    public interface IComSerializer
    {
        T Deserialize<T>(byte[] input) where T : class,IComSerializable, new();
        byte[] Setialize<T>(T serObj) where T : class, IComSerializable;
    }
}
