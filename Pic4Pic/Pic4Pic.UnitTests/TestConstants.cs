using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    public class TestConstants
    {
        public const string DBConnectionStringProduction = "SERVER=cl2yszx8kw.database.windows.net;UID=ginger-bizspark-db-admin;PWD=sUphangile-yedi-K;database=ginger-dbase";
        public const string DBConnectionString = "SERVER=cl2yszx8kw.database.windows.net;UID=ginger-bizspark-db-admin;PWD=sUphangile-yedi-K;database=ginger-test-dbase";
        public const string FacebookToken = "CAAHFE0c9Er0BAAN36LExpUZC5fh5d9ZCsqEsAgdj993GSft72n3uvRy1lIPk4esVMC5yAzUysKT7YmubOrgpNAS1V1DpgtEefbDwcrWxTgSKfmEkh2L94FRuIwBEXd8h9aAk4Y85YRsNMuOAogrNx5H9VBFVRrHI4VIOEkvfEkv59WZA8f7vqosSoZBbC3M3S5FHjMrcWbEJbQxXdJLf2SDhMYtXZAYQvjFEfzEWoPAZDZD";
        public static Guid testUserId = new Guid("00000001-0001-0001-0001-000000000001");

        public static BlobStorageAccount GetTestBlobStorageAccount()
        {
            BlobStorageAccount bsa = new BlobStorageAccount();
            bsa.AccountKey = "3u5eL8jRgxu2mjmiVNH5D7JSP+zZqqtmjo15oo3miOY0KwKqffNhVFXTkNxFNiLaTl6tmSKSvDRs5Ilaeke5Ng==";
            bsa.AccountName = "gingertest";
            bsa.ContainerName = "photos";
            bsa.UriTemplate = "http://{0}.blob.core.windows.net";
            return bsa;
        }
    }
}
