namespace Models;

public class BookingModel
{
    public String firstname;
    public String lastname;
    public int totalprice;
    public bool depositpaid;
    public BookingDates bookingdates;
    public string additionalneeds;
}

public class BookingDates
{
    public string checkin;
    public string checkout;
}