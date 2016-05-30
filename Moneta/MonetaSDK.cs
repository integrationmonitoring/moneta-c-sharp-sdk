﻿using System;
using System.Text;
using Moneta.MonetaWSDL;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Collections.Generic;

namespace Moneta
{
    class MonetaSDK
    {
        public MessagesClient client = new MessagesClient();

        public IniParser basicSettings;
        public IniParser paymentSystems;
        public IniParser paymentUrls;

        private Object response;


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

            try {
                FindAccountByIdRequest request = new FindAccountByIdRequest();
                request.Value = accountId;
                response = client.FindAccountById(request);
                result = prepareResult();
            }
            catch (Exception e) {
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
                if (String.Compare(payer, "") != 0) {
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
                    monetaAtribute.key      = "PAYMENTTOKEN";
                    monetaAtribute.value    = "request";
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


        // CreateUser
        public MonetaSdkResult sdkMonetaCreateUser(string firstName, string lastName, string email, string gender)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                if (String.Compare(gender, "MALE") != 0 && String.Compare(gender, "FEMALE") != 0) {
                    gender = "MALE";
                }

                CreateProfileRequest request = new CreateProfileRequest();
                List<KeyValueApprovedAttribute> mntAttributes = new List<KeyValueApprovedAttribute>();
                KeyValueApprovedAttribute monetaAtribute = new KeyValueApprovedAttribute();

                monetaAtribute.key = "first_name";
                monetaAtribute.value = firstName;
                mntAttributes.Add(monetaAtribute);

                monetaAtribute.key = "last_name";
                monetaAtribute.value = lastName;
                mntAttributes.Add(monetaAtribute);

                monetaAtribute.key = "email_for_notifications";
                monetaAtribute.value = email;
                mntAttributes.Add(monetaAtribute);

                monetaAtribute.key = "sex";
                monetaAtribute.value = gender;
                mntAttributes.Add(monetaAtribute);

                request.profile = mntAttributes.ToArray();

                String mntPrototype = basicSettings.GetSetting("BasicSettings", "monetasdk_prototype_user_unit_id");
                if (String.Compare(mntPrototype, "") != 0) {
                    request.unitId = (long) Convert.ToDouble(mntPrototype);
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
        public MonetaSdkResult sdkMonetaEditProfile(long unitId, string firstName, string lastName, string email, string gender)
        {
            MonetaSdkResult result = new MonetaSdkResult();

            try
            {
                if (String.Compare(gender, "MALE") != 0 && String.Compare(gender, "FEMALE") != 0) {
                    gender = "MALE";
                }

                EditProfileRequest request = new EditProfileRequest();
                List<KeyValueApprovedAttribute> mntAttributes = new List<KeyValueApprovedAttribute>();
                KeyValueApprovedAttribute monetaAtribute = new KeyValueApprovedAttribute();

                monetaAtribute.key = "first_name";
                monetaAtribute.value = firstName;
                mntAttributes.Add(monetaAtribute);

                monetaAtribute.key = "last_name";
                monetaAtribute.value = lastName;
                mntAttributes.Add(monetaAtribute);

                monetaAtribute.key = "email_for_notifications";
                monetaAtribute.value = email;
                mntAttributes.Add(monetaAtribute);

                monetaAtribute.key = "sex";
                monetaAtribute.value = gender;
                mntAttributes.Add(monetaAtribute);

                request.profile = mntAttributes.ToArray();
                request.unitId = unitId;

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
                request.payer           = fromAccountId;
                request.paymentPassword = fromAccountPaymentPassword;
                request.payee           = toAccountId;
                request.amount          = amount;
                request.description     = description;
                request.isPayerAmount   = true;

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
                pager.pageSize   = itemsPerPage;

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



        // prepare result
        private MonetaSdkResult prepareResult()
        {
            MonetaSdkResult result = new MonetaSdkResult();

            // prepare result in xml and json
            result.xmlData = ObjectToSOAP(response);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result.xmlData);
            result.jsonData = JsonConvert.SerializeXmlNode(doc);

            return result;
        }

    }

}