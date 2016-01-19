using System;
using System.Net;
using System.Collections.Generic;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [TestClass]
    public class CityTests
    {
        [TestMethod]
        public void TestPredefinedLocations()
        {
            int usaCountryId = 1;
            int californiaId = 6;
            int bayAreaId = 5;

            string[] countryTexts = new string[] { "US", "USA", "United States", "United States of America" };
            string[] stateTexts = new string[] { "CA", "WA", "DC", "California", "Washington", "Washington DC", "District of Columbia"};
            string[] subRegionTexts = new string[] { "San Francisco Bay Area", "Bay Area", "Orange Co", "Sacramento County" };
            string[] cityTexts = new string[] { "San Francisco", "SFO", "Palo Alto", "Berkeley", "Oakland", "San Mateo" };

            using (SqlConnection conn = new SqlConnection(TestConstants.DBConnectionStringProduction))
            {
                conn.Open();

                List<Country> countries = Country.ReadAllFromDBase(conn, null);
                Dictionary<string, Country> countryMap = Country.MapItemsByNameAndCode(countries, true, true, true);

                #region VERIFY Country
                foreach (String countryText in countryTexts)
                {
                    if (!countryMap.ContainsKey(countryText))
                    {
                        throw new ApplicationException("Test failed for: " + countryText);
                    }
                }

                Dictionary<int, Country> countryMapById = Country.MapItemsById(countries, true);
                if (!countryMapById.ContainsKey(usaCountryId))
                {
                    throw new ApplicationException("Test failed for US ID=" + usaCountryId);
                }
                #endregion
                
                #region VERIFY Region
                List<Region> regions = Region.ReadAllFromDBase(conn, null, usaCountryId);
                Dictionary<string, Region> regionMap = Region.MapItemsByNameAndCode(regions, true, true, true);
                foreach (String regionText in stateTexts)
                {
                    if (!regionMap.ContainsKey(regionText))
                    {
                        throw new ApplicationException("Test failed for: " + regionText);
                    }
                }

                Dictionary<int, Region> regionMapById = Region.MapItemsById(regions, true);
                #endregion

                #region VERIFY SubRegion
                List<SubRegion> subRegions = SubRegion.ReadAllFromDBase(conn, null, usaCountryId, californiaId);
                Dictionary<string, SubRegion> subRegionMap = SubRegion.MapItemsByNameAndCode(subRegions, true, true, true);
                foreach (String subRegionText in subRegionTexts)
                {
                    if (!subRegionMap.ContainsKey(subRegionText))
                    {
                        throw new ApplicationException("Test failed for: " + subRegionText);
                    }
                }

                Dictionary<int, SubRegion> subRegionMapById = SubRegion.MapItemsById(subRegions, true);                
                #endregion

                #region VERIFY City
                List<City> cities = City.ReadAllFromDBase(conn, null, usaCountryId, californiaId, bayAreaId);
                Dictionary<string, City> citiesMap = City.MapItemsByNameAndCode(cities, true, true, true);
                foreach (String cityText in cityTexts)
                {
                    if (!citiesMap.ContainsKey(cityText))
                    {
                        throw new ApplicationException("Test failed for: " + cityText);
                    }
                }

                Dictionary<int, City> citiesById = City.MapItemsById(cities, true);
                #endregion
            }
        }
    }
}
