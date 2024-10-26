using System;
using Xunit;
using Sempi5.Domain.TokenEntity;

namespace Sempi5.DomainTests.TokenTest;

public class CryptographyTest
{
    [Fact]
    public void CanEncryptAndDecryptString()
    {
        var cryptography = new Cryptography();
        var plainText = "Hello, World!";

        var encryptedText = cryptography.EncryptString(plainText);
        
        Assert.False(string.IsNullOrWhiteSpace(encryptedText));

        var decryptedText = cryptography.DecryptString(encryptedText);

        Assert.Equal(plainText, decryptedText);
    }

    [Fact]
    public void EncryptString_ReturnsDifferentValuesForSameInput()
    {
        var cryptography = new Cryptography();
        var plainText = "Hello, World!";

        var encryptedText1 = cryptography.EncryptString(plainText);
        var encryptedText2 = cryptography.EncryptString(plainText);

        Assert.NotEqual(encryptedText1, encryptedText2);
    }

    [Fact]
    public void DecryptString_ThrowsExceptionOnInvalidInput()
    {
        var cryptography = new Cryptography();
        var invalidCipherText = "InvalidCipherText";

        Assert.Throws<FormatException>(() => cryptography.DecryptString(invalidCipherText));
    }
}
