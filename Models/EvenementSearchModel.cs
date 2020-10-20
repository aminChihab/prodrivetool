using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace postersopmetaal.Models
{
    public class EvenementSearchModel
    {
        public string PJmanager { get; set; }

        public string Debiteurnaam { get; set; }

        public DateTimeOffset? Cursusdatum { get; set; }

        public string Project { get; set; }
    }

    public class EvenementBusinessLogic
    {
        
        public static string AddFilter(string apiUrl, EvenementSearchModel evenement)
        {
            string filterFields = "?filterfieldids=";
            string filterValues = "&filtervalues=";
            string filterOperators = "&operatortypes=";
            int counter = 0;

            if (evenement != null)
            {

                if (!String.IsNullOrEmpty(evenement.PJmanager))
                {
                    filterFields += "PJmanager";
                    filterValues += evenement.PJmanager;
                    filterOperators += "6";
                    counter++;
                }
                if (!String.IsNullOrEmpty(evenement.Debiteurnaam))
                {
                    if (counter == 0)
                    {
                        filterFields += "Debiteurnaam";
                        filterValues += evenement.Debiteurnaam;
                        filterOperators += "6";
                    }
                    else
                    {
                        filterFields += "%2CDebiteurnaam";
                        filterValues += $"%2C{evenement.Debiteurnaam}";
                        filterOperators += "%2C6";
                    }
                    counter++;
                }
                if (!String.IsNullOrEmpty(evenement.Project))
                {
                    if (counter == 0)
                    {
                        filterFields += "Project";
                        filterValues += evenement.Project;
                        filterOperators += "6";
                    }
                    else
                    {
                        filterFields += "%2CProject";
                        filterValues += $"%2C{evenement.Project}";
                        filterOperators += "%2C6";
                    }
                    counter++;
                }
                if (evenement.Cursusdatum.HasValue)
                {
                    if (counter == 0)
                    {
                        filterFields += "Cursusdatum";
                        filterValues += evenement.Cursusdatum;
                        filterOperators += "2";
                    }
                    else
                    {
                        filterFields += "%2CCursusdatum";
                        filterValues += $"%2C{evenement.Cursusdatum}";
                        filterOperators += "%2C2";
                    }
                }

            }       

            return apiUrl + filterFields + filterValues + filterOperators;
        }

    }
}
