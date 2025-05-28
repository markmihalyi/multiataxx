using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Net.Mime;

namespace Backend.Controllers;

[ApiController]
[Route("api/store")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class StoreController(GameService gameService) : ControllerBase
{
    private readonly GameService _gameService = gameService;

    /// <summary>
    /// Get all purchasable boosters
    /// </summary>
    /// <response code="200">If the request was successful</response>
    [HttpGet("boosters")]
    [Authorize]
    [ProducesResponseType(typeof(UserStatistics), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBoosters()
    {
        var boosters = await _gameService.GetAllBoosters();
        return Ok(boosters);
    }

    /// <summary>
    /// Create payment intent to purchase boosters
    /// </summary>
    /// <response code="200">If the request was successful</response>
    [HttpPost("create-booster-invoice")]
    [Authorize]
    [ProducesResponseType(typeof(UserStatistics), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> BuyBooster([FromBody] BuyBoosterRequestBody body)
    {
        var booster = await _gameService.GetBoosterById(body.BoosterId);
        if (booster == null)
        {
            return BadRequest(new ErrorResponse("There is no booster with the specified ID."));
        }

        int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        double priceInUsd = booster.Price * body.Amount;

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(priceInUsd * 100),
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() },
                { "boosterId", body.BoosterId.ToString() },
                { "amount", body.Amount.ToString() }
            }
        };
        var service = new PaymentIntentService();
        var intent = service.Create(options);

        return Ok(new { clientSecret = intent.ClientSecret });
    }

    /// <summary>
    /// Webhook for Stripe
    /// </summary>
    /// <response code="200">If the request was successful</response>
    /// <response code="401">If there is no booster with the specified ID</response>
    [HttpPost("stripe-webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        const string endpointSecret = "whsec_21ad31aabe1915a39adc5fdfca6806d5ffce6d08a953a3105bd9c30861d051c2";

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                endpointSecret
            );

            if (stripeEvent.Type == "payment_intent.succeeded" && stripeEvent.Data.Object is PaymentIntent paymentIntent)
            {
                var userId = int.Parse(paymentIntent.Metadata["userId"]);
                var boosterId = int.Parse(paymentIntent.Metadata["boosterId"]);
                var amount = int.Parse(paymentIntent.Metadata["amount"]);

                Console.WriteLine("Payment successful!");
                Console.WriteLine("-> User ID: " + userId);
                Console.WriteLine("-> Booster ID: " + boosterId);
                Console.WriteLine("-> Amount: " + amount);

                bool success = await _gameService.AddBoosterToUser(userId, boosterId, amount);
                if (!success)
                {
                    return BadRequest(new ErrorResponse("There is no booster with the specified ID."));
                }
            }

            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine("Webhook error: " + e.Message);
            return BadRequest();
        }
    }

    /// <summary>
    /// Test endpoint for Stripe webhook testing
    /// </summary>
    /// <response code="200">If the request was successful</response>
    [HttpPost("test-stripe-payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult TestStripePayment([FromBody] TestStripePaymentRequest body)
    {
        var paymentIntentService = new PaymentIntentService();

        var options = new PaymentIntentCreateOptions
        {
            Amount = 100, // $1
            Currency = "usd",
            PaymentMethodTypes = ["card"],
            Metadata = new Dictionary<string, string>
            {
                { "userId", body.UserId.ToString() },
                { "boosterId", body.BoosterId.ToString() },
                { "amount", body.Amount.ToString() }
            }
        };

        var intent = paymentIntentService.Create(options);

        var confirmOptions = new PaymentIntentConfirmOptions
        {
            PaymentMethod = "pm_card_visa"
        };

        var confirmedIntent = paymentIntentService.Confirm(intent.Id, confirmOptions);

        return Ok(new { confirmedIntent.Id, confirmedIntent.Status });
    }

    /// <summary>
    /// Test endpoint to add boosters to users
    /// </summary>
    /// <response code="200">If the request was successful</response>
    [HttpPost("test-add-booster")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddBooster([FromBody] TestStripePaymentRequest body)
    {
        var booster = await _gameService.GetBoosterById(body.BoosterId);
        if (booster == null)
        {
            return BadRequest(new ErrorResponse("There is no booster with the specified ID."));
        }

        await _gameService.AddBoosterToUser(body.UserId, body.BoosterId, body.Amount);

        return Ok();
    }
}
