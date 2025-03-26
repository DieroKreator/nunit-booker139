using Newtonsoft.Json;
using RestSharp;

namespace Booking;

public class BookingTest
{
    private const string BASE_URL = "https://restful-booker.herokuapp.com/";

    [Test, Order(1)]
    public void PostBookingTest()
    {
        var options = new RestClientOptions(BASE_URL)
        {
            Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator("admin", "password123")
        };

        var client = new RestClient(BASE_URL);
        var request = new RestRequest("booking", Method.Post);

        string jsonBody = File.ReadAllText("");

        request.AddBody(jsonBody);

        var response = client.Execute(request);
        Console.WriteLine($"Response Content: {response.Content}");
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        Console.WriteLine(responseBody);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        int bookingid = responseBody.id;
        Assert.That(bookingid, Is.EqualTo(602740501));

        String actualName = responseBody.firstname;
        Assert.That(actualName, Is.EqualTo("Pedro"));

        int actualTotalPrice = responseBody.bookingid;
        Assert.That(actualTotalPrice, Is.EqualTo(200));
    }
}