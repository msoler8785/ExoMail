namespace ExoMail.Smtp.Common.Collections
{
    public interface IExpirable
    {
        bool Expired { get; }
    }
}