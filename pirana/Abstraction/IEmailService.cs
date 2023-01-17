namespace pirana.Abstraction
{
    public interface IEmailService
    {
        void Send(string mailto, string subject, string body, bool isbodyhtml = false);
    }
}
