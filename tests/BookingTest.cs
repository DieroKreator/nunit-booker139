using Newtonsoft.Json;
using RestSharp;

namespace Booking;

public class BookingTest
{
    private const string BASE_URL = "https://restful-booker.herokuapp.com/";
    string token = "";

    [Test, Order(0)]
    public void GetToken()
    {
        var client = new RestClient(BASE_URL);
        var request = new RestRequest("auth", Method.Post);

        request.AddHeader("Content-Type", "application/json");

        var authBody = new
        {
            username = "admin", // Default credentials from API docs
            password = "password123"
        };

        request.AddJsonBody(authBody);

        var response = client.Execute(request);
        Console.WriteLine($"Auth Response: {response.Content}");

        var tokenResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
        token = tokenResponse.token;

        Console.WriteLine($"Token: {token}");
    }

    [Test, Order(1)]
    public void PostBookingTest()
    {
        var options = new RestClientOptions("https://restful-booker.herokuapp.com/");
        var client = new RestClient(options);
        var request = new RestRequest("booking", Method.Post);

        // ✅ Manually add the Authorization header
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {token}");

        string jsonBody = File.ReadAllText("/Users/dierokreator/Programming/Interasys/nunit-booker139/fixtures/booking1.json");

        request.AddBody(jsonBody);

        var response = client.Execute(request);
        Console.WriteLine($"Response Content: {response.Content}");
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        Console.WriteLine(responseBody);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        int actuaBookingId = responseBody.bookingid;
        // Assert.That(actuaBookingId, Is.EqualTo(1748));

        String actualName = responseBody.firstname;
        Assert.That(actualName, Is.EqualTo("Pedro"));

        int actualTotalPrice = responseBody.bookingid;
        Assert.That(actualTotalPrice, Is.EqualTo(200));

        Environment.SetEnvironmentVariable("bookingId", actuaBookingId.ToString());
    }
}