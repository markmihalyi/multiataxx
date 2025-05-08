namespace Backend.DTOs
{
    public record UserBoosterData(int Id, string Name, int Amount);
    public record StoreBoosterData(int Id, string Name, double Price);
    public record TestStripePaymentRequest(int UserId, int BoosterId, int Amount);
}
