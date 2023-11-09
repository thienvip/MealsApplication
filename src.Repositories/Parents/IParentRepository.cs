using src.Core.Domains;


namespace src.Repositories.Parents
{
    public interface IParentRepository
    {
        Parent GetParentsByEmailAddressFromPhoebus(string email);
        Parent GetParentsByPhoneNumberFromPhoebus(string phoneNumber);
        Parent getParentByEmail(string email);

        Task<Parent> getParentByPhone(string phone);
        Task updateParent(Parent model);
        Task insertParent(Parent model);
    }
}
