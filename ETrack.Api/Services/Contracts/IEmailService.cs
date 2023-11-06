using Microsoft.AspNetCore.Mvc;

namespace ETrack.Api.Services.Contracts
{
   public interface IEmailService
   {
        void sendEmail(EmailDto emailDto);
   } 
}