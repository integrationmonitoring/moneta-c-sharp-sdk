В сгенерированном файле Reference.cs удостоверьтесь что следующие фрагменты Вашего кода выглядят именно так:

1) public partial class FindProfileInfoResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private long pageSizeField;
        
        private long pageNumberField;
        
        private long pagesCountField;
        
        private long sizeField;
        
        private long totalSizeField;
        
        private KeyValueApprovedAttribute[] profileField;


2)      [System.Xml.Serialization.XmlArrayAttribute(Order=5)]
        [System.Xml.Serialization.XmlArrayItemAttribute("attribute", typeof(KeyValueApprovedAttribute), IsNullable=false)]
        public KeyValueApprovedAttribute[] profile {
            get {
                return this.profileField;
            }
            set {
                this.profileField = value;
                this.RaisePropertyChanged("profile");
            }
        }

3) конфигурационный файл приложения, в части настроек транспорта WSDL, должен быть таким:

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
    </startup>
    <system.serviceModel>
        <bindings>
            <customBinding>
              <binding name="MonetaCustom">
                <security authenticationMode="UserNameOverTransport" enableUnsecuredResponse="true" 
		includeTimestamp="false" />
                <textMessageEncoding messageVersion="Soap11" />
                <httpsTransport />
              </binding>
            </customBinding>
        </bindings>      
        <client>
            <endpoint address="https://demo.moneta.ru:443/services" binding="customBinding"
                bindingConfiguration="MonetaCustom" contract="MonetaWSDL.Messages"
                name="MessagesSoap11" />
        </client>
    </system.serviceModel>
</configuration>
