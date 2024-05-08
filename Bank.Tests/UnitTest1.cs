namespace Bank.Tests;
using Bank_Application_Extended;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {   
        //Arrange
        Account account = new Account("John Doe", 1000);
        //Act
        account.Deposit(500);
        //Assert
        Assert.Equal(1500, account.Balance);
        
    }
}