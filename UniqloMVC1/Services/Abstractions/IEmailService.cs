namespace UniqloMVC1.Services.Abstractions
{
    public interface IEmailService
    {
       //  Task SendAsync();
         public void SendEmailConfirmation(string receiver, string name , string token);
    }
}
