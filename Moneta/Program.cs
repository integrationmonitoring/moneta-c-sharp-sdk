using System;

/*
 * In Visual Studio 2015 after generating a Service Reference we have an exception:
 * 
 * error CS0030: Cannot convert type 'Moneta.MonetaWSDL.KeyValueApprovedAttribute[]'
 * to 'Moneta.MonetaWSDL.KeyValueApprovedAttribute'
 * 
 * To fix it replace typeof(KeyValueApprovedAttribute) to typeof(KeyValueApprovedAttribute[]) in
 * 
 * public partial class FindProfileInfoResponse : object, System.ComponentModel.INotifyPropertyChanged {
 * ...
 *      /// <remarks/>
 *      [System.Xml.Serialization.XmlArrayAttribute(Order=5)]
 *      [System.Xml.Serialization.XmlArrayItemAttribute("attribute", typeof(KeyValueApprovedAttribute[]), IsNullable=false)]
 *      public KeyValueApprovedAttribute[][] profile {
 *      }
 * ...
 * }
 * 
 */

namespace Moneta
{
    class Program
    {
        static void Main(string[] args)
        {
            MonetaSDK monetaSDK = new MonetaSDK();

            // call OperationInfo
            MonetaSdkResult result1 = monetaSDK.sdkMonetaOperationInfo(375864);
            Console.WriteLine("Error: " + result1.errorMessage);
            Console.WriteLine("Result: " + result1.xmlData);

            // call FindAccountById
            MonetaSdkResult result2 = monetaSDK.sdkMonetaFindAccountById(11493408);
            Console.WriteLine("Result: " + result2.jsonData);

        }

    }

}
