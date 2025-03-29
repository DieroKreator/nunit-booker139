using Models;
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
        Assert.That(actuaBookingId, Is.GreaterThan(0));
        String actualName = responseBody.booking.firstname;
        Assert.That(actualName, Is.EqualTo("Pedro"));

        int actualTotalPrice = responseBody.booking.totalprice;
        Assert.That(actualTotalPrice, Is.EqualTo(200));

        Environment.SetEnvironmentVariable("bookingId", actuaBookingId.ToString());
    }

    [Test, Order(2)]
    public void GetBookingTest()
    {
        int bookingId = 3694;
        String expFirstName = "Pedro";
        String expLastName = "Perez";
        String expCheckIn = "2025-01-01";

        var client = new RestClient(BASE_URL);
        var request = new RestRequest($"booking/{bookingId}", Method.Get);
        // var request = new RestRequest(Environment.GetEnvironmentVariable("bookingId"), Method.Get);

        request.AddHeader("Accept", "application/json");

        var response = client.Execute(request);

        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        // Assert.That((String)responseBody.firstname, Is.EqualTo(expFirstName));
        string actualName = responseBody.firstname;
        Assert.That(actualName, Is.EqualTo(expFirstName));
        Assert.That((String)responseBody.lastname, Is.EqualTo(expLastName));
        // Assert.That((String)responseBody.bookingdates[0].expCheckIn, Is.EqualTo(expCheckIn));
    }

    [Test, Order(3)]
    public void UpdateBookingTest()
    {
        int bookingId = 3694;
        BookingModel bookingModel = new BookingModel();
        bookingModel.firstname = "Pedro";
        bookingModel.lastname = "Perez";
        bookingModel.totalprice = 500;
        bookingModel.depositpaid = false;
        bookingModel.bookingdates = new BookingDates();
        bookingModel.bookingdates.checkin = "2025-01-01";
        bookingModel.bookingdates.checkout = "2025-01-02";
        bookingModel.additionalneeds = "Breakfast";

        var options = new RestClientOptions("https://restful-booker.herokuapp.com/");
        var client = new RestClient(options);
        var request = new RestRequest($"booking/{bookingId}", Method.Put);

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", "token=" + token);

        String jsonBody = JsonConvert.SerializeObject(bookingModel, Formatting.Indented);
        Console.WriteLine(jsonBody);
        request.AddBody(jsonBody);

        var response = client.Execute(request);

        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That((String)responseBody.firstname, Is.EqualTo("Pedro"));
        Assert.That((String)responseBody.lastname, Is.EqualTo("Perez"));
        Assert.That((int)responseBody.totalprice, Is.EqualTo(500));
        Assert.That((bool)responseBody.depositpaid, Is.EqualTo(false));
        Assert.That((String)responseBody.bookingdates.checkin, Is.EqualTo("2025-01-01"));
        Assert.That((String)responseBody.bookingdates.checkout, Is.EqualTo("2025-01-02"));
        Assert.That((String)responseBody.additionalneeds, Is.EqualTo("Breakfast"));
    }
}