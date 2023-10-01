namespace Shared.DTO.ScheduledJobs
{
    public record ReminderCheckoutOrderDto(
        string Email, string Subject, string EmailContent, DateTimeOffset EnqueueAt);
   
}
