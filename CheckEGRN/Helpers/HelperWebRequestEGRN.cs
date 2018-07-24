using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Configuration;

namespace CheckEGRN.Helpers.HelperWebRequestEGRN
{
    /// <summary>
    /// Универсальные ошибки
    /// </summary>
    [DataContract]
    public class Error
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }
        [DataMember(Name = "mess")]
        public string Mess { get; set; }
    }

    /// <summary>
    /// Возвращает список найденных объектов по ключевому запросу
    /// </summary>
    [DataContract]
    public class EGRNResponseCadasterSearsh
    {
        [DataMember(Name = "objects")]
        public List<ListObjects> Objects { get; set; }
        [DataMember(Name = "found")]
        public int Found { get; set; }
        [DataMember(Name = "region")]
        public string Region { get; set; }
        [DataMember(Name = "error")]
        public Error Error { get; set; }
    }
    [DataContract]
    public class ListObjects
    {
        [DataMember(Name = "CADNOMER")]
        public string Cadnomer { get; set; }
        [DataMember(Name = "ADDRESS")]
        public string Address { get; set; }
        [DataMember(Name = "TYPE")]
        public string Type { get; set; }
        [DataMember(Name = "AREA")]
        public string Area { get; set; }
        [DataMember(Name = "CATEGORY")]
        public string Category { get; set; }
    }

    /// <summary>
    /// Возвращает сведения об объекте из ЕГРН, также список доступных документов для заказа
    /// </summary>
    [DataContract]
    public class EGRNResponseCadasterObjectInfoFull
    {
        [DataMember(Name = "EGRN")]
        public EGRNObject EGRN { get; set; }
        [DataMember(Name = "region")]
        public string Region { get; set; }
        [DataMember(Name = "documents")]
        public DocumentType Documents { get; set; }
        [DataMember(Name = "encoded_object")]
        public string Encoded_object { get; set; }
        [DataMember(Name = "error")]
        public Error Error { get; set; }
    }
    [DataContract]
    public class EGRNObject
    {
        [DataMember(Name = "object")]
        public ListObjects Object { get; set; }
        [DataMember(Name = "details")]
        public ListDetails Details { get; set; }
    }
    [DataContract]
    public class ListDetails
    {
        [DataMember(Name = "Тип объекта")]
        public string ObjectType { get; set; }
        [DataMember(Name = "Кадастровый номер")]
        public string CadasterNumber { get; set; }
        [DataMember(Name = "Статус объекта")]
        public string ObjectState { get; set; }
        [DataMember(Name = "Дата постановки на кадастровый учет")]
        public string DateCadasterState { get; set; }
        [DataMember(Name = "Категория земель")]
        public string LandCategory { get; set; }
        [DataMember(Name = "Разрешенное использование")]
        public string PermittedUse { get; set; }
        [DataMember(Name = "Площадь")]
        public string Square { get; set; }
        [DataMember(Name = "Единица измерения (код)")]
        public string MeasureUnit { get; set; }
        [DataMember(Name = "Кадастровая стоимость")]
        public string CadasterValue { get; set; }
        [DataMember(Name = "Дата определения стоимости")]
        public string DateValue { get; set; }
        [DataMember(Name = "Дата внесения стоимости")]
        public string DateDeposit { get; set; }
        [DataMember(Name = "Дата утверждения стоимости")]
        public string DateApprovalValue { get; set; }
        [DataMember(Name = "Адрес (местоположение)")]
        public string Address { get; set; }
        [DataMember(Name = "Дата обновления информации")]
        public string DateInfoUpdate { get; set; }
        [DataMember(Name = "Форма собственности")]
        public string OwnerType { get; set; }
        [DataMember(Name = "Количество правообладателей")]
        public string RightholdersCount { get; set; }
        [DataMember(Name = "Кадастровый инженер")]
        public string CadasterEngineer { get; set; }
    }
    [DataContract]
    public class DocumentType
    {
        [DataMember(Name = "XZP")]
        public DocumentDetails XZP { get; set; }
        [DataMember(Name = "SOPP")]
        public DocumentDetails SOPP { get; set; }
        [DataMember(Name = "KPT")]
        public DocumentDetails KPT { get; set; }
    }
    [DataContract]
    public class DocumentDetails
    {
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "available")]
        public bool Available { get; set; }
        [DataMember(Name = "price")]
        public float Price { get; set; }
    }

    /// <summary>
    /// Сканирует и возвращает количество помещений входящих в состав многоквартирного дома
    /// </summary>
    [DataContract]
    public class EGRNResponseReestrScan
    {
        [DataMember(Name ="scan")]
        public string Scan { get; set; }
        [DataMember(Name ="note")]
        public string Note { get; set; }
        [DataMember(Name ="reestr")]
        public Reestr Reestr { get; set; }
        [DataMember(Name ="error")]
        public Error Error { get; set; }
    }
    [DataContract]
    public class Reestr
    {
        [DataMember(Name ="id")]
        public int Id { get; set; }
        [DataMember(Name = "categories")]
        public ApartmentType Categories { get; set; }
        [DataMember(Name ="count")]
        public int Count { get; set; }
    }
    [DataContract]
    public class ApartmentType
    {
        [DataMember(Name = "apartments_residential")]
        public ApartmentCount Apartments_residential { get; set; }
        [DataMember(Name = "apartments_nonresidential")]
        public ApartmentCount Apartments_nonresidential { get; set; }
        [DataMember(Name = "others")]
        public ApartmentCount Others { get; set; }
    }
    [DataContract]
    public class ApartmentCount
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
    }

    class HelperWebRequestEGRN
    {
        /// <summary>
        /// Возвращает список найденных объектов по ключевому запросу
        /// </summary>
        /// <param name="_query">Kадастровый номер или адрес</param>
        /// <returns></returns>
        public EGRNResponseCadasterSearsh WRCadasterSearch(string _query)
        {
            string _parQuery = @"query=" + _query;
            byte[] _arrQuery = Encoding.UTF8.GetBytes(_parQuery);

            WebRequest _webRequest = (HttpWebRequest)WebRequest.Create("https://apirosreestr.ru/api/cadaster/search");
            _webRequest.ContentType = "application/x-www-form-urlencoded";
            _webRequest.Method = "POST";
            _webRequest.Headers.Add("Token", ConfigurationManager.AppSettings["_token"]);
            _webRequest.ContentLength = _arrQuery.Length;

            Stream dataStream = _webRequest.GetRequestStream();
            dataStream.Write(_arrQuery, 0, _arrQuery.Length);
            dataStream.Close();

            WebResponse _webResponse = _webRequest.GetResponse();
            Stream streamResponse = _webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            string Response = streamReader.ReadToEnd();

            DataContractJsonSerializer _dataContractJsonSerializer = new DataContractJsonSerializer(typeof(EGRNResponseCadasterSearsh));
            EGRNResponseCadasterSearsh _result = (EGRNResponseCadasterSearsh)_dataContractJsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(Response)));

            return _result;
        }

        /// <summary>
        /// Возвращает сведения об объекте из ЕГРН, также список доступных документов для заказа
        /// </summary>
        /// <param name="_query">Только кадастровый номер</param>
        /// <returns></returns>
        public EGRNResponseCadasterObjectInfoFull WRCadasterObjectInfoFull(string _query)
        {
            string _parQuery = @"query=" + _query;
            byte[] _arrQuery = Encoding.UTF8.GetBytes(_parQuery);

            WebRequest _webRequest = (HttpWebRequest)WebRequest.Create("https://apirosreestr.ru/api/cadaster/objectInfoFull");
            _webRequest.ContentType = "application/x-www-form-urlencoded";
            _webRequest.Method = "POST";
            _webRequest.Headers.Add("Token", ConfigurationManager.AppSettings["_token"]);
            _webRequest.ContentLength = _arrQuery.Length;

            Stream dataStream = _webRequest.GetRequestStream();
            dataStream.Write(_arrQuery, 0, _arrQuery.Length);
            dataStream.Close();

            WebResponse _webResponse = _webRequest.GetResponse();
            Stream streamResponse = _webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            string Response = streamReader.ReadToEnd();

            DataContractJsonSerializer _dataContractJsonSerializer = new DataContractJsonSerializer(typeof(EGRNResponseCadasterObjectInfoFull));
            EGRNResponseCadasterObjectInfoFull _result = (EGRNResponseCadasterObjectInfoFull)_dataContractJsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(Response)));

            return _result;
        }

        /// <summary>
        /// Сканирует и возвращает количество помещений входящих в состав многоквартирного дома
        /// </summary>
        /// <param name="_query">Кадастровый номер МКД</param>
        /// <returns></returns>
        public EGRNResponseReestrScan WRReestrScan(string _query)
        {
            string _parQuery = @"query=" + _query; // + "&scan_if_no_cache=1";
            byte[] _arrQuery = Encoding.UTF8.GetBytes(_parQuery);

            WebRequest _webRequest = (HttpWebRequest)WebRequest.Create("https://apirosreestr.ru/api/reestr/scan");
            _webRequest.ContentType = "application/x-www-form-urlencoded";
            _webRequest.Method = "POST";
            _webRequest.Headers.Add("Token", ConfigurationManager.AppSettings["_token"]);
            _webRequest.ContentLength = _arrQuery.Length;

            Stream dataStream = _webRequest.GetRequestStream();
            dataStream.Write(_arrQuery, 0, _arrQuery.Length);
            dataStream.Close();

            WebResponse _webResponse = _webRequest.GetResponse();
            Stream streamResponse = _webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            string Response = streamReader.ReadToEnd();

            DataContractJsonSerializer _dataContractJsonSerializer = new DataContractJsonSerializer(typeof(EGRNResponseReestrScan));
            EGRNResponseReestrScan _result = (EGRNResponseReestrScan)_dataContractJsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(Response)));

            return _result;
        }
    }
}
