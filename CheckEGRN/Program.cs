using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using CheckEGRN.Helpers.HelperWebRequestEGRN;
using System.Data.SqlClient;

namespace CheckEGRN
{
    class Program
    {
        static string _address;
        static void Main(string[] args)
        {
            //string _address = "Екатеринбург, Уральская 46";
            //string _cadnomer = "";

            List<string> _listCityAddress = new List<string>();
            string connectionString = @"Data Source=storage;Initial Catalog=FiasSQL;User Id=sa;Password=SQLsa2012";
            string sqlExpression = @"SELECT
                                        CONCAT
                                        (REPLACE(lev1.SHORTNAME, ' ', '')
                                        ,' '
                                        ,REPLACE(lev1.OFFNAME, ' ', '')
                                        ,' '
                                        ,REPLACE(lev2.SHORTNAME, ' ', '')
                                        ,' '
                                        ,REPLACE(lev2.OFFNAME, ' ', '')
                                        ,' '
                                        ,REPLACE(lev4.HOUSENUM, ' ', '')
                                        ,' '
                                        ,REPLACE(lev4.BUILDNUM, ' ', '')
                                        ,' '
                                        ,REPLACE(lev4.STRUCNUM, ' ', '')) AS CityAddress
                                    FROM
                                        FiasSQL.dbo.addrObj lev0
                                        LEFT JOIN addrObj lev1 ON lev0.AOGUID = lev1.PARENTGUID
                                        LEFT JOIN addrObj lev2 ON lev1.AOGUID = lev2.PARENTGUID
                                        LEFT JOIN house lev4 ON lev2.AOGUID = lev4.AOGUID
                                    WHERE
                                        lev0.STATUS = 1
                                        AND lev1.STATUS = 1
                                        AND lev2.STATUS = 1
                                        AND lev1.OFFNAME = 'Екатеринбург'
                                        AND lev4.ENDDATE = '2079-06-06'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        _address = reader["CityAddress"].ToString();
                        _listCityAddress.Add(_address);
                    }
                }
                connection.Close();
            }

            HelperWebRequestEGRN helperWebRequestEGRN = new HelperWebRequestEGRN();

            List<string[]> _listCadasterNumber = new List<string[]>();
            foreach(var _itemCityAddress in _listCityAddress)
            {
                EGRNResponseCadasterSearsh cadasterSearsh = helperWebRequestEGRN.WRCadasterSearch(_itemCityAddress);
                if (cadasterSearsh.Error.Code == null)
                {
                    Console.WriteLine("Found: {0}", cadasterSearsh.Found);
                    Console.WriteLine("Region: {0}", cadasterSearsh.Region);
                    foreach (var itemCadasterSearch in cadasterSearsh.Objects)
                    {
                        Console.WriteLine("----------");
                        Console.WriteLine("=> Address: {0}", itemCadasterSearch.Address);
                        Console.WriteLine("=> Area: {0}", itemCadasterSearch.Area);
                        Console.WriteLine("=> Cadnomer: {0}", itemCadasterSearch.Cadnomer);
                        Console.WriteLine("=> Category: {0}", itemCadasterSearch.Category);
                        Console.WriteLine("=> Type: {0}", itemCadasterSearch.Type);

                        if (itemCadasterSearch.Type == "Здание")
                        {
                            _listCadasterNumber.Add(new string[] { _itemCityAddress, itemCadasterSearch.Cadnomer });
                        }
                    }
                    Console.WriteLine("====================");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: {0} - {1}", cadasterSearsh.Error.Code, cadasterSearsh.Error.Mess);
                    Console.ResetColor();
                }

                //EGRNResponseCadasterObjectInfoFull cadasterObjectInfoFull = helperWebRequestEGRN.WRCadasterObjectInfoFull(_cadnomer);
                //if (cadasterObjectInfoFull.Error.Code == null)
                //{
                //    Console.WriteLine("Region: {0}", cadasterObjectInfoFull.Region);
                //    Console.WriteLine("XZP Available: {0}",cadasterObjectInfoFull.Documents.XZP.Available);
                //    Console.WriteLine("XZP Label: {0}",cadasterObjectInfoFull.Documents.XZP.Label);
                //    Console.WriteLine("XZP Price: {0}",cadasterObjectInfoFull.Documents.XZP.Price);
                //    Console.WriteLine("SOPP Available: {0}", cadasterObjectInfoFull.Documents.SOPP.Available);
                //    Console.WriteLine("SOPP Label: {0}", cadasterObjectInfoFull.Documents.SOPP.Label);
                //    Console.WriteLine("SOPP Price: {0}", cadasterObjectInfoFull.Documents.SOPP.Price);
                //    Console.WriteLine("KPT Available: {0}", cadasterObjectInfoFull.Documents.KPT.Available);
                //    Console.WriteLine("KPT Label: {0}:", cadasterObjectInfoFull.Documents.KPT.Label);
                //    Console.WriteLine("KPT Price: {0}:", cadasterObjectInfoFull.Documents.KPT.Price);
                //    Console.WriteLine("----------");
                //    Console.WriteLine("Address: {0}", cadasterObjectInfoFull.EGRN.Object.Address);
                //    Console.WriteLine("Area: {0}", cadasterObjectInfoFull.EGRN.Object.Area);
                //    Console.WriteLine("Cadnomer: {0}", cadasterObjectInfoFull.EGRN.Object.Cadnomer);
                //    Console.WriteLine("Category: {0}", cadasterObjectInfoFull.EGRN.Object.Category);
                //    Console.WriteLine("Type: {0}", cadasterObjectInfoFull.EGRN.Object.Type);
                //    Console.WriteLine("----------");
                //    Console.WriteLine("Address: {0}", cadasterObjectInfoFull.EGRN.Details.Address);
                //    Console.WriteLine("Cadaster engineer: {0}", cadasterObjectInfoFull.EGRN.Details.CadasterEngineer);
                //    Console.WriteLine("Cadaster number: {0}", cadasterObjectInfoFull.EGRN.Details.CadasterNumber);
                //    Console.WriteLine("Cadaster value: {0}", cadasterObjectInfoFull.EGRN.Details.CadasterValue);
                //    Console.WriteLine("Date approval value: {0}", cadasterObjectInfoFull.EGRN.Details.DateApprovalValue);
                //    Console.WriteLine("Date cadaster state: {0}", cadasterObjectInfoFull.EGRN.Details.DateCadasterState);
                //    Console.WriteLine("Date deposit: {0}", cadasterObjectInfoFull.EGRN.Details.DateDeposit);
                //    Console.WriteLine("Date info update: {0}", cadasterObjectInfoFull.EGRN.Details.DateInfoUpdate);
                //    Console.WriteLine("Date value: {0}", cadasterObjectInfoFull.EGRN.Details.DateValue);
                //    Console.WriteLine("Land category: {0}", cadasterObjectInfoFull.EGRN.Details.LandCategory);
                //    Console.WriteLine("Measure unit: {0}", cadasterObjectInfoFull.EGRN.Details.MeasureUnit);
                //    Console.WriteLine("Object state: {0}", cadasterObjectInfoFull.EGRN.Details.ObjectState);
                //    Console.WriteLine("Object type: {0}", cadasterObjectInfoFull.EGRN.Details.ObjectType);
                //    Console.WriteLine("Owner type: {0}", cadasterObjectInfoFull.EGRN.Details.OwnerType);
                //    Console.WriteLine("Permitted use: {0}", cadasterObjectInfoFull.EGRN.Details.PermittedUse);
                //    Console.WriteLine("Rightholders count: {0}", cadasterObjectInfoFull.EGRN.Details.RightholdersCount);
                //    Console.WriteLine("Square: {0}", cadasterObjectInfoFull.EGRN.Details.Square);
                //
                //    Console.WriteLine("====================");
                //}
                //else
                //{
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    Console.WriteLine("Error: {0} - {1}", cadasterObjectInfoFull.Error.Code, cadasterObjectInfoFull.Error.Mess);
                //    Console.ResetColor();
                //}
            }

            foreach (var _itemCadasterNumber in _listCadasterNumber)
            {
                int i = 0;
                EGRNResponseReestrScan reestrScan = helperWebRequestEGRN.WRReestrScan(_itemCadasterNumber[1]);
                if (reestrScan.Error.Code == null)
                {
                    Console.WriteLine("Note: {0}", reestrScan.Note);
                    //do
                    //{
                    //    Console.Write("{0} sec left", i);
                    //    Console.SetCursorPosition(0, Console.CursorTop);
                    //    Thread.Sleep(1000);
                    //    i++;
                    //}
                    //while (reestrScan.Scan == "processing");

                    //Console.WriteLine("Scan: {0}", reestrScan.Scan);
                    //Console.WriteLine("Note: {0}", reestrScan.Note);
                    //Console.WriteLine("Id: {0}", reestrScan.Reestr.Id);
                    //Console.WriteLine("Count: {0}", reestrScan.Reestr.Count);
                    //Console.WriteLine("Apartments_residential: {0}", reestrScan.Reestr.Categories.Apartments_residential.Count);
                    //Console.WriteLine("Apartments_nonresidential: {0}", reestrScan.Reestr.Categories.Apartments_nonresidential.Count);
                    //Console.WriteLine("Others: {0}", reestrScan.Reestr.Categories.Others.Count);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: {0} - {1}", reestrScan.Error.Code, reestrScan.Error.Mess);
                    Console.ResetColor();
                }

                File.AppendAllText(@"LogResult.csv", DateTime.Now + _itemCadasterNumber[0] + ";" + _itemCadasterNumber[1] + ";" + reestrScan.Reestr.Categories.Apartments_residential.Count + ";" + reestrScan.Reestr.Categories.Apartments_nonresidential.Count + ";" + reestrScan.Reestr.Categories.Others.Count + ";" + Environment.NewLine, Encoding.Default);
            }

            Console.WriteLine("Scan completed success");
            Console.Read();
        }
    }
}
