using System;
using System.Text;
using Moneta.MonetaWSDL;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Moneta
{
    class MonetaSDK
    {
        public MessagesClient client = new MessagesClient();

        public IniParser basicSettings;
        public IniParser paymentSystems;
        public IniParser paymentUrls;

        private Object response;
        private Dictionary<string, string> attributes = new Dictionary<string, string>();


        public MonetaSDK()
        {
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            basicSettings = new IniParser(dir + @"\config\basic_settings.ini");
            paymentSystems = new IniParser(dir + @"\config\payment_systems.ini");
            paymentUrls = new IniParser(dir + @"\config\payment_urls.ini");

            client.ClientCredentials.UserName.UserName = basicSettings.GetSetting("BasicSettings", "monetasdk_account_username");
            client.ClientCredentials.UserName.Password = basicSettings.GetSetting("BasicSettings", "monetasdk_account_password");

        }


        public static string ObjectToSOAP(object Object)
        {
            try
            {
                using (MemoryStream Stream = new MemoryStream())
                {
                    System.Runtime.Serialization.Formatters.Soap.SoapFormatter Serializer =
                        new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();

                    Serializer.Serialize(Stream, Object);
                    Stream.Flush();
                    return UTF8Encoding.UTF8.GetString(Stream.GetBuffer(), 0, (int)Stream.Position);
                }
            }
            catch
            {
                throw;
            }
        }


        // FindAccountById
        public MonetaSdkResult sdkMonetaFindAccountById(long accountId)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                FindAccountByIdRequest request = new FindAccountByIdRequest();
                request.Value = accountId;
                response = client.FindAccountById(request);
                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }

        public MonetaSdkResult sdkMonetaFindProfileInfo(long unitId, long profileId = 0)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                FindProfileInfoRequestFilter findRequest = new FindProfileInfoRequestFilter();
                findRequest.unitId = unitId;
                findRequest.unitIdSpecified = true;
                if (profileId > 0)
                {
                    findRequest.profileId = profileId;
                    findRequest.profileIdSpecified = true;
                }

                FindProfileInfoRequest request = new FindProfileInfoRequest();
                request.filter = findRequest;
                FindProfileInfoResponse response = client.FindProfileInfo(request);

                Array attrList = response.profile.ToArray();
                foreach (KeyValueApprovedAttribute item in attrList)
                {
                    attributes.Add(item.key, item.value);
                    Console.WriteLine(item.key + ": " + item.value);
                }

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        // GetProfileInfo
        public MonetaSdkResult sdkMonetaGetProfileInfo(long unitId, long profileId = 0)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                GetProfileInfoRequest request = new GetProfileInfoRequest();
                request.unitId = unitId;
                request.unitIdSpecified = true;
                if (profileId > 0)
                {
                    request.profileId = profileId;
                    request.profileIdSpecified = true;
                }

                response = client.GetProfileInfo(request);
                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }



        // OperationInfo
        public MonetaSdkResult sdkMonetaOperationInfo(long operationId)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                response = client.GetOperationDetailsById(operationId);
                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        // create Invoice
        public MonetaSdkResult sdkMonetaCreateInvoice(string payer, long payee, decimal amount, string clientTransaction, bool isRegular)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                InvoiceRequest invoiceRequest = new InvoiceRequest();
                if (String.Compare(payer, "") != 0)
                {
                    invoiceRequest.payer = payer;
                }

                invoiceRequest.payee = payee;
                invoiceRequest.amount = amount;
                invoiceRequest.clientTransaction = clientTransaction;

                OperationInfo operationInfo = new OperationInfo();
                List<KeyValueAttribute> mntAttributes = new List<KeyValueAttribute>();

                if (isRegular)
                {
                    KeyValueAttribute monetaAtribute = new KeyValueAttribute();
                    monetaAtribute.key = "PAYMENTTOKEN";
                    monetaAtribute.value = "request";
                    mntAttributes.Add(monetaAtribute);
                }

                operationInfo.attribute = mntAttributes.ToArray();
                invoiceRequest.operationInfo = operationInfo;

                response = client.Invoice(invoiceRequest);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        public MonetaSdkResult sdkMonetaEditProfileDocumentRequest(long docId, long unitId, long profileId = 0)
        {
            MonetaSdkResult result = new MonetaSdkResult();
            try
            {
                EditProfileDocumentRequest request = new EditProfileDocumentRequest();

                List<KeyValueApprovedAttribute> mntAttributes = new List<KeyValueApprovedAttribute>();

                KeyValueApprovedAttribute monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "SERIES";
                monetaAtribute.value = "1111";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "NUMBER";
                monetaAtribute.value = "111111";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "ISSUER";
                monetaAtribute.value = "test";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "ISSUED";
                monetaAtribute.value = "2002-02-04";
                mntAttributes.Add(monetaAtribute);

                request.id = docId;
                request.idSpecified = true;

                request.attribute = mntAttributes.ToArray();
                request.unitId = unitId;
                request.unitIdSpecified = true;
                if (profileId > 0)
                {
                    request.profileId = profileId;
                    request.profileIdSpecified = true;
                }

                request.type = DocumentType.PASSPORT;
                request.typeSpecified = true;

                response = client.EditProfileDocument(request);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        public MonetaSdkResult sdkMonetaCreateProfileDocumentRequest(long unitId, long profileId = 0)
        {
            MonetaSdkResult result = new MonetaSdkResult();
            try
            {
                CreateProfileDocumentRequest request = new CreateProfileDocumentRequest();

                List<KeyValueApprovedAttribute> mntAttributes = new List<KeyValueApprovedAttribute>();

                KeyValueApprovedAttribute monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "SERIES";
                monetaAtribute.value = "1111";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "NUMBER";
                monetaAtribute.value = "111111";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "ISSUER";
                monetaAtribute.value = "test";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "ISSUED";
                monetaAtribute.value = "2002-02-04";
                mntAttributes.Add(monetaAtribute);

                request.attribute = mntAttributes.ToArray();
                request.unitId = unitId;
                request.unitIdSpecified = true;
                if (profileId > 0)
                {
                    request.profileId = profileId;
                    request.profileIdSpecified = true;
                }

                request.type = DocumentType.PASSPORT;
                request.typeSpecified = true;

                response = client.CreateProfileDocument(request);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        public MonetaSdkResult sdkMonetaCreateProfile(long unitId, long profileId)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                CreateProfileRequest request = new CreateProfileRequest();


                request.profileId = profileId;
                request.profileIdSpecified = true;
                request.unitId = unitId;
                request.unitIdSpecified = true;

                request.profileType = ProfileType.client;

                List<KeyValueApprovedAttribute> mntAttributes = new List<KeyValueApprovedAttribute>();

                KeyValueApprovedAttribute monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "first_name";
                monetaAtribute.value = "first_name";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "last_name";
                monetaAtribute.value = "last_name";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "email_for_notifications";
                monetaAtribute.value = "email_for_notifications";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "sex";
                monetaAtribute.value = "MALE";
                mntAttributes.Add(monetaAtribute);

                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "childprofiletypeid";
                monetaAtribute.value = "DIRECTOR";
                mntAttributes.Add(monetaAtribute);

                request.profile = mntAttributes.ToArray();

                response = client.CreateProfile(request);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;

        }

        // CreateUser
        public MonetaSdkResult sdkMonetaCreateUser(string firstName, string lastName, string email, string gender)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                if (String.Compare(gender, "MALE") != 0 && String.Compare(gender, "FEMALE") != 0)
                {
                    gender = "MALE";
                }

                CreateProfileRequest request = new CreateProfileRequest();
                List<KeyValueApprovedAttribute> mntAttributes = new List<KeyValueApprovedAttribute>();

                KeyValueApprovedAttribute monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "first_name";
                monetaAtribute.value = firstName;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "last_name";
                monetaAtribute.value = lastName;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "email_for_notifications";
                monetaAtribute.value = email;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "sex";
                monetaAtribute.value = gender;
                mntAttributes.Add(monetaAtribute);

                request.profile = mntAttributes.ToArray();

                String mntPrototype = basicSettings.GetSetting("BasicSettings", "monetasdk_prototype_user_unit_id");
                if (String.Compare(mntPrototype, "") != 0)
                {
                    request.unitId = (long)Convert.ToDouble(mntPrototype);
                    request.unitIdSpecified = true;
                }

                request.profileType = ProfileType.client;

                response = client.CreateProfile(request);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        // EditProfile
        public MonetaSdkResult sdkMonetaEditProfile(long unitId, long profileId)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                EditProfileRequest request = new EditProfileRequest();
                List<KeyValueApprovedAttribute> mntAttributes = new List<KeyValueApprovedAttribute>();

                KeyValueApprovedAttribute monetaAtribute = new KeyValueApprovedAttribute();

                monetaAtribute.key = "kpp";
                monetaAtribute.value = "1111";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "ogrn";
                monetaAtribute.value = "2222";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "ogrnip";
                monetaAtribute.value = "3333";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "okpo";
                monetaAtribute.value = "4444";
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "okved";
                monetaAtribute.value = "5555";
                mntAttributes.Add(monetaAtribute);

                request.profile = mntAttributes.ToArray();
                request.unitId = unitId;
                request.profileId = profileId;
                request.profileIdSpecified = true;

                response = client.EditProfile(request);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        // MonetaTransfer
        public MonetaSdkResult sdkMonetaTransfer(string fromAccountId, string fromAccountPaymentPassword, string toAccountId, decimal amount, string description)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                TransferRequest request = new TransferRequest();
                request.payer = fromAccountId;
                request.paymentPassword = fromAccountPaymentPassword;
                request.payee = toAccountId;
                request.amount = amount;
                request.description = description;
                request.isPayerAmount = true;

                response = client.Transfer(request);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        // MonetaHistory
        public MonetaSdkResult sdkMonetaHistory(long accountId, DateTime dateFrom, DateTime dateTo, int itemsPerPage = 20, int pageNumber = 1)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                FindOperationsListRequestFilter filter = new FindOperationsListRequestFilter();
                filter.accountId = accountId;
                filter.dateFrom = dateFrom;
                filter.dateTo = dateTo;

                Pager pager = new Pager();
                pager.pageNumber = pageNumber;
                pager.pageSize = itemsPerPage;

                FindOperationsListRequest request = new FindOperationsListRequest();
                request.filter = filter;
                request.pager = pager;

                response = client.FindOperationsList(request);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        // CreateOrganizationProfile
        public MonetaSdkResult sdkMonetaCreateOrganizationProfile(string inn, string url, string rf_resident, string alias, string organization_name,
            string organization_name_short, string contact_email)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                CreateProfileRequest request = new CreateProfileRequest();
                List<KeyValueApprovedAttribute> mntAttributes = new List<KeyValueApprovedAttribute>();

                KeyValueApprovedAttribute monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "inn";
                monetaAtribute.value = inn;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "url";
                monetaAtribute.value = url;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "rf_resident";
                monetaAtribute.value = rf_resident;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "alias";
                monetaAtribute.value = alias;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "organization_name";
                monetaAtribute.value = organization_name;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "organization_name_short";
                monetaAtribute.value = organization_name_short;
                mntAttributes.Add(monetaAtribute);
                monetaAtribute = new KeyValueApprovedAttribute();
                monetaAtribute.key = "contact_email";
                monetaAtribute.value = contact_email;
                mntAttributes.Add(monetaAtribute);

                request.profile = mntAttributes.ToArray();

                String mntPrototype = basicSettings.GetSetting("BasicSettings", "monetasdk_prototype_user_unit_id");
                if (String.Compare(mntPrototype, "") != 0)
                {
                    request.unitId = (long)Convert.ToDouble(mntPrototype);
                    request.unitIdSpecified = true;
                }

                request.profileType = ProfileType.organization;

                response = client.CreateProfile(request);

                result = prepareResult();
            }
            catch (Exception e)
            {
                result.error = true;
                result.errorMessage = e.Message;
            }

            return result;
        }


        // prepare result
        private MonetaSdkResult prepareResult()
        {
            MonetaSdkResult result = new MonetaSdkResult();

            // json
            result.jsonData = JsonConvert.SerializeObject(response);
            // xml
            result.xmlData = ObjectToSOAP(response);
            // attributes
            result.attributes = attributes;
            // pure data
            result.response = response;

            return result;
        }

    }

}
