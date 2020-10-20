using postersopmetaal.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace postersopmetaal.Services
{
    public class AfasService
    {
        // TODO: Extraheer client en request functionaliteit in aparte functie

        public async static Task<List<Evenement>> GetEvenementenById(string apiUrl, List<long> idList)
        {
            List<Evenement> evenementen = new List<Evenement>();

            foreach (var id in idList)
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(apiUrl += $"?filterfieldids=CrId&filtervalues={id}&operatortypes=1"),
                        Method = HttpMethod.Get
                    };

                    request.Headers.Clear();
                    request.Headers.Authorization = new AuthenticationHeaderValue("AfasToken", Auth.BASE_64_TOKEN);

                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        JObject jsonResponse = JObject.Parse(jsonString);
                        JArray objResponse = (JArray)jsonResponse["rows"];
                        evenementen.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject<List<Evenement>>(objResponse.ToString()));
                    }
                }
            }
            return evenementen;


        }

        public async static Task<List<Evenement>> GetEvenementen(string apiUrl)
        {

            List<Evenement> evenementen = new List<Evenement>();

            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(apiUrl),
                    Method = HttpMethod.Get
                };

                request.Headers.Clear();
                request.Headers.Authorization = new AuthenticationHeaderValue("AfasToken", Auth.BASE_64_TOKEN);

                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(jsonString);
                    JArray objResponse = (JArray)jsonResponse["rows"];
                    evenementen = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Evenement>>(objResponse.ToString());
                }
            }
            return evenementen;
        }

        private async static Task<Dictionary<List<Evenement>, List<EvenementFactuur>>> GetEvenementFactuur(List<Evenement> evenementen)
        {
            string apiUrl = Environment.GetEnvironmentVariable("AFAS_EVENEMENT_FACTUUR_API"); 
            List<EvenementFactuur> evenementFacturen = new List<EvenementFactuur>();
            Dictionary<List<Evenement>, List<EvenementFactuur>> facturenGegevens = new Dictionary<List<Evenement>, List<EvenementFactuur>>();
                        
            foreach (var evenement in evenementen)
            {
                for (int i = 0; i < evenement.Beschikbare_plaatsen; i++)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = new HttpRequestMessage()
                        {
                            RequestUri = new Uri(apiUrl += $"?filterfieldids=FILTER_MP%2CBcCo&filtervalues=MP0{i+1}%2C{evenement.BcCo}&operatortypes=1%2C1"),
                            Method = HttpMethod.Get
                        };

                        request.Headers.Clear();
                        request.Headers.Authorization = new AuthenticationHeaderValue("AfasToken", Auth.BASE_64_TOKEN);

                        HttpResponseMessage response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            JObject jsonResponse = JObject.Parse(jsonString);
                            JArray objResponse = (JArray)jsonResponse["rows"];
                            evenementFacturen.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject<List<EvenementFactuur>>(objResponse.ToString()));
                        }
                    }
                }                
            }
            facturenGegevens.Add(evenementen, evenementFacturen);    
            return facturenGegevens;

        }
    
        public async static Task<string> PostFactuur(List<Evenement> evenementen)
        {
            string apiUrl = Environment.GetEnvironmentVariable("AFAS_FACTUUR_API"); ;
            Dictionary<List<Evenement>, List<EvenementFactuur>> facturenGegevens = await GetEvenementFactuur(evenementen);
            List<Factuur> facturen = new List<Factuur>();
            string responsestring = "";

            
            foreach (var factuurGegevens in facturenGegevens)
            {
                if (factuurGegevens.Key.Count == 0 || factuurGegevens.Value.Count == 0)
                {
                    return "Niet genoeg gegevens om een factuur te maken.";
                }

                Factuur factuur = new Factuur();
                KnCourseMember knmember = new KnCourseMember();

                Element element = new Element();                
                element.CrId = factuurGegevens.Key[0].CrId;
                element.CdId = factuurGegevens.Value[0].CdId;

                Fields fields = new Fields();
                fields.BcCo = factuurGegevens.Key[0].BcCo;
                fields.SuDa = DateTime.Now.ToString("yyyy-MM-dd");
                fields.Invo = factuurGegevens.Key[0].Invo;
                fields.DeId = factuurGegevens.Key[0].DeId;
                fields.DfPr = factuurGegevens.Key[0].DfPr;

                element.Fields = fields;
                knmember.Element = element;
                factuur.KnCourseMember = knmember;
                facturen.Add(factuur);
            }


            foreach (var factuur in facturen)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(factuur);
                json = json.Replace("CrId", "@CrId").Replace("CdId", "@CdId");
                Debug.Write(json);

                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(apiUrl),
                        Method = HttpMethod.Put,
                        Content = new StringContent(json)
                    };

                    request.Headers.Clear();
                    request.Headers.Authorization = new AuthenticationHeaderValue("AfasToken", Auth.BASE_64_TOKEN);

                    HttpResponseMessage response = await client.SendAsync(request);
                    responsestring += await response.Content.ReadAsStringAsync();
                }
            }
            
            
            return responsestring;
        }
    }
}
