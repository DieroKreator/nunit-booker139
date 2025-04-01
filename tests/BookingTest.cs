using Models;
using Newtonsoft.Json;
using RestSharp;

namespace Booking;

public class BookingTest
{
    private const string BASE_URL = "https://restful-booker.herokuapp.com/";

    // string token = "";
    string token = GetAuthToken();

    public static IEnumerable<TestCaseData> getTestData()
    {
        String caminhoMassa = @"fixtures/pets.csv";

        using var reader = new StreamReader(caminhoMassa);

        // Pula a primeira linha com os cabeçalhos
        reader.ReadLine();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(",");

            yield return new TestCaseData(int.Parse(values[0]), int.Parse(values[1]), values[2], values[3], values[4], values[5], values[6], values[7]);
        }

    }

    public static string GetAuthToken()
    {
        var options = new RestClientOptions(BASE_URL);
        var client = new RestClient(options);
        var request = new RestRequest("auth", Method.Post);

        var credentials = new { username = "admin", password = "password123" };

        request.AddJsonBody(credentials);

        var response = client.Execute(request);
        Console.WriteLine($"Auth Response: {response.Content}");

        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        // token = tokenResponse.token;

        // Console.WriteLine($"Token: {token}");
        return responseBody.token;
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

        string jsonBody = File.ReadAllText(
            "/Users/dierokreator/Programming/Interasys/nunit-booker139/fixtures/booking1.json"
        );

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
        int bookingId = 282;
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
        int bookingId = 3613;
        BookingModel bookingModel = new BookingModel
        {
            firstname = "Pedro",
            lastname = "Perez",
            totalprice = 500,
            depositpaid = false,
            bookingdates = new BookingDates
            {
                checkin = "2025-01-01",
                checkout = "2025-01-02"
            },
            additionalneeds = "Breakfast"
        };

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

    [Test, Order(4)]
    public void DeleteBookingTest()
    {
        int bookingId = 1716;

        var options = new RestClientOptions("https://restful-booker.herokuapp.com/");
        var client = new RestClient(options);
        var request = new RestRequest($"booking/{bookingId}", Method.Delete);

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", "token=" + token);

        var response = client.Execute(request);
        Console.WriteLine($"Response Content: {response.Content}");

        Assert.That((int)response.StatusCode, Is.EqualTo(201));
    }

    [TestCaseSource("getTestData", new object[] {}), Order(5)]
    public void GetBookingTestWithData(string firstName, 
                                       string lastName, 
                                       int totalprice, 
                                       bool depositpaid, 
                                       object bookingdates, 
                                       string additionalneeds)
    {
        var options = new RestClientOptions("https://restful-booker.herokuapp.com/");
        var client = new RestClient(options);
        var request = new RestRequest($"booking", Method.Get);

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");

        var response = client.Execute(request);
        Console.WriteLine($"Response Content: {response.Content}");

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
    }
}
