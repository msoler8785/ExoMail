using System;

namespace ExoMail.Smtp.Common.Services
{
    public interface IResolver
    {
        bool Exists(Type type);
    }
}