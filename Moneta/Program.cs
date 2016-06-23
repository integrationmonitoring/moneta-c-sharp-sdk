using System;
using System.Collections.Generic;

namespace Moneta
{
    class Program
    {
        static void Main(string[] args)
        {
            MonetaSDK monetaSDK = new MonetaSDK();

            // UNCOMMENT TO EXECUTE:

            // call OperationInfo
            // MonetaSdkResult result1 = monetaSDK.sdkMonetaOperationInfo(375864);
            // Console.WriteLine("Error: " + result1.errorMessage);
            // Console.WriteLine("Result: " + result1.jsonData);

            // call FindAccountById
            // MonetaSdkResult result2 = monetaSDK.sdkMonetaFindAccountById(11493408);
            // Console.WriteLine("Result: " + result2.jsonData);

            // GetProfileInfo
            // MonetaSdkResult result3 = monetaSDK.sdkMonetaGetProfileInfo(44551, 44862);
            // Console.WriteLine("Error: " + result3.errorMessage);
            // Console.WriteLine("Result: " + result3.xmlData);

            // MonetaSdkResult result5 = monetaSDK.sdkMonetaCreateProfile(44551, 44862);
            // Console.WriteLine("Error: " + result5.errorMessage);
            // Console.WriteLine("Result: " + result5.xmlData);

            MonetaSdkResult result4 = monetaSDK.sdkMonetaFindProfileInfo(44281, 44569);

            // MonetaSdkResult result6 = monetaSDK.sdkMonetaCreateProfileDocumentRequest(44569);
            // Console.WriteLine("Error: " + result6.errorMessage);
            // Console.WriteLine("Result: " + result6.xmlData);

            // MonetaSdkResult result7 = monetaSDK.sdkMonetaEditProfileDocumentRequest(2320, 44551, 44956);
            // Console.WriteLine("Error: " + result7.errorMessage);
            // Console.WriteLine("Result: " + result7.xmlData);

            // sdkMonetaEditProfile
            // MonetaSdkResult result8 = monetaSDK.sdkMonetaEditProfile(44551, 44956);
            // Console.WriteLine("Error: " + result8.errorMessage);
            // Console.WriteLine("Result: " + result8.xmlData);

        }

    }

}
