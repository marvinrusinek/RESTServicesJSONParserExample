﻿using System;
using System.Net;
using System.Runtime.Serialization.Json;

namespace RESTServicesJSONParserExample
{
    class Program
    {
        static string BingMapsKey = "N5oTURq9iQ0pTUlaRu9k~-bD3uqx0mLPUoJfgYnI-Iw~AvvnTmI40bC6JhglP6YyIL6qAVF4_i59DOB_p-xuYLtDKe0mtPa9FAIM93NGuzPx";
        static void Main(string[] args)
        {
            try
            {
                string locationsRequest = CreateRequest("New%20York");
                Response locationsResponse = MakeRequest(locationsRequest);
                ProcessResponse(locationsResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }

        }

        //Create the request URL
        public static string CreateRequest(string queryString)
        {
            string UrlRequest = "http://dev.virtualearth.net/REST/v1/Locations/" +
                                           queryString +
                                           "?key=" + BingMapsKey;
            return (UrlRequest);
        }

        public static Response MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(requestUrl) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    System.Runtime.Serialization.Json.DataContractJsonSerializer jsonSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Response));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    Response jsonResponse
                    = objResponse as Response;
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        static public void ProcessResponse(Response locationsResponse)
        {

            int locNum = locationsResponse.ResourceSets[0].Resources.Length;

            //Get formatted addresses: Option 1
            //Get all locations in the response and then extract the formatted address for each location
            Console.WriteLine("Show all formatted addresses");
            for (int i = 0; i < locNum; i++)
            {
                Location location = (Location)locationsResponse.ResourceSets[0].Resources[i];
                Console.WriteLine(location.Address.FormattedAddress);
            }
            Console.WriteLine();

            //Get the Geocode Points for each Location
            for (int i = 0; i < locNum; i++)
            {
                Location location = (Location)locationsResponse.ResourceSets[0].Resources[i];
                Console.WriteLine("Geocode Points for " + location.Address.FormattedAddress);
                int geocodePointNum = location.GeocodePoints.Length;
                for (int j = 0; j < geocodePointNum; j++)
                {
                    Console.WriteLine("    Point: " + location.GeocodePoints[j].Coordinates[0].ToString() + "," +
                                                 location.GeocodePoints[j].Coordinates[1].ToString());
                    double test = location.GeocodePoints[j].Coordinates[1];
                    Console.Write("    Usage: ");
                    for (int k = 0; k < location.GeocodePoints[j].UsageTypes.Length; k++)
                    {
                        Console.Write(location.GeocodePoints[j].UsageTypes[k].ToString() + " ");
                    }
                    Console.WriteLine("\n\n");
                }
            }
            Console.WriteLine();


            //Get all locations that have a MatchCode=Good and Confidence=High
            Console.WriteLine("Locations that have a Confidence=High");
            for (int i = 0; i < locNum; i++)
            {
                Location location = (Location)locationsResponse.ResourceSets[0].Resources[i];
                if (location.Confidence == "High")
                    Console.WriteLine(location.Address.FormattedAddress);
            }
            Console.WriteLine();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();


        }
    }
}
