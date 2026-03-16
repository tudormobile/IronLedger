namespace IronLedgerLib.Services.Tests;

[TestClass]
public class IronLedgerResponseTests
{
    // --- Success ---

    [TestMethod]
    public void Success_IsSuccess_IsTrue()
    {
        var response = IronLedgerResponse<string>.Success("data");

        Assert.IsTrue(response.IsSuccess);
    }

    [TestMethod]
    public void Success_Data_IsSet()
    {
        var data = "some data";

        var response = IronLedgerResponse<string>.Success(data);

        Assert.AreEqual(data, response.Data);
    }

    [TestMethod]
    public void Success_ErrorMessage_IsNull()
    {
        var response = IronLedgerResponse<string>.Success("data");

        Assert.IsNull(response.ErrorMessage);
    }

    [TestMethod]
    public void Success_WithValueType_IsSuccess_IsTrue()
    {
        var response = IronLedgerResponse<int>.Success(0);

        Assert.IsTrue(response.IsSuccess);
    }

    // --- Failure ---

    [TestMethod]
    public void Failure_IsSuccess_IsFalse()
    {
        var response = IronLedgerResponse<string>.Failure("something went wrong");

        Assert.IsFalse(response.IsSuccess);
    }

    [TestMethod]
    public void Failure_ErrorMessage_IsSet()
    {
        var errorMessage = "something went wrong";

        var response = IronLedgerResponse<string>.Failure(errorMessage);

        Assert.AreEqual(errorMessage, response.ErrorMessage);
    }

    [TestMethod]
    public void Failure_Data_IsNull()
    {
        var response = IronLedgerResponse<string>.Failure("error");

        Assert.IsNull(response.Data);
    }

    // --- string T edge case ---

    [TestMethod]
    public void Success_WhenTIsString_IsDistinctFromFailure()
    {
        var success = IronLedgerResponse<string>.Success("value");
        var failure = IronLedgerResponse<string>.Failure("value");

        Assert.IsTrue(success.IsSuccess);
        Assert.IsFalse(failure.IsSuccess);
    }
}
