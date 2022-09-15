internal class PaymentResponse
{
    const string CARD_METHOD = "CARD";
    public enum PaymentStatus
    {
        UNKNOWN,
        SUCCESS,
        FAIL,
        TIMEOUT
    }

    public PaymentResponse(string report)
    {
        this.report = report;
    }
    public PaymentResponse(PaymentStatus status, float totalPaid, string sourceOfFunds, string reference)
    {
        this.method = CARD_METHOD;
        this.status = status.ToString();
        this.totalPaid = totalPaid;
        this.sourceOfFunds = sourceOfFunds;
        this.reference = reference;
    }

    public string report;
    public string status;
    public float totalPaid;
    public string reference;
    public string method;
    public string sourceOfFunds;
}