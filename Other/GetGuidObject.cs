using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EguibarIT.Delegation.Other
{
    public class GetGuidObjects
    {
        public static Hashtable GetGuidSchemaObjects()
        {

            try
            {
                if (EguibarIT.Delegation.Other.Vars.AllclassSchemaGuids == null || EguibarIT.Delegation.Other.Vars.AllclassSchemaGuids.Count == 0)
                {
                    string SchemaNamingContext = string.Empty;

                    DirectoryEntry rootDSE = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", EguibarIT.Delegation.Other.Domains.GetAdFQDN()));

                    SchemaNamingContext = rootDSE.Properties["SchemaNamingContext"][0].ToString();

                    DirectoryEntry searchRoot = new DirectoryEntry("LDAP://" + SchemaNamingContext);

                    DirectorySearcher searcher = new DirectorySearcher(searchRoot);
                    searcher.SearchScope = SearchScope.Subtree;
                    searcher.PropertiesToLoad.Add("lDAPDisplayName");
                    searcher.PropertiesToLoad.Add("schemaIDGUID");
                    searcher.Filter = string.Format("(schemaidguid=*)");
                    searcher.PageSize = 3000;

                    foreach (SearchResult result in searcher.FindAll())
                    {
                        Byte[] bArr = (Byte[])result.Properties["schemaIDGUID"][0];
                        Guid guid = new Guid(bArr);
                        Vars.AllclassSchemaGuids.Add(result.Properties["lDAPDisplayName"][0].ToString(), guid.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred while retriving schema ldapDisplayName & GUIDs: '{0}'", ex);
            }

            return Vars.AllclassSchemaGuids;
        }
    }
}
