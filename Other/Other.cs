using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Net.NetworkInformation;
using System.Text;

namespace EguibarIT.Delegation.Other
{
    /// <summary>
    ///
    /// </summary>
    public class Domains
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="objectPath"></param>
        /// <returns></returns>
        public static bool Exists(string objectPath)
        {
            bool found = false;
            if (DirectoryEntry.Exists("LDAP://" + objectPath))
            {
                found = true;
            }
            return found;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="friendlyDomainName"></param>
        /// <returns></returns>
        public static string FriendlyDomainToLdapDomain(string friendlyDomainName)
        {
            string ldapPath = null;

            try
            {
                DirectoryContext objContext = new DirectoryContext(DirectoryContextType.Domain, friendlyDomainName);
                Domain objDomain = Domain.GetDomain(objContext);
                ldapPath = objDomain.Name;
            }
            catch (DirectoryServicesCOMException e)
            {
                ldapPath = e.Message.ToString();
            }
            return ldapPath;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ForestName"></param>
        /// <returns></returns>
        public static ArrayList EnumerateDomains(string ForestName)
        {
            DirectoryContext objContext = new DirectoryContext(DirectoryContextType.Forest, ForestName);

            ArrayList allDomains = new ArrayList();
            Forest currentForest = Forest.GetForest(objContext);

            DomainCollection myDomains = currentForest.Domains;

            foreach (Domain objDomain in myDomains)
            {
                allDomains.Add(objDomain.Name);
            }
            return allDomains;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ArrayList EnumerateDomains()
        {
            return EnumerateDomains(GetAdFQDN());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ForestName"></param>
        /// <returns></returns>
        public static ArrayList EnumerateDomainsGC(string ForestName)
        {
            ArrayList alGCs = new ArrayList();

            DirectoryContext objContext = new DirectoryContext(DirectoryContextType.Forest, ForestName);
            Forest currentForest = Forest.GetForest(objContext);

            foreach (GlobalCatalog gc in currentForest.GlobalCatalogs)
            {
                alGCs.Add(gc.Name);
            }
            return alGCs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ArrayList EnumerateDomainsGC()
        {
            return EnumerateDomainsGC(GetAdFQDN());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="DomainName"></param>
        /// <returns></returns>
        public static ArrayList EnumerateDomainControllers(string DomainName)
        {
            DirectoryContext objContext = new DirectoryContext(DirectoryContextType.Domain, DomainName);

            ArrayList alDcs = new ArrayList();
            Domain domain = Domain.GetDomain(objContext);

            foreach (DomainController dc in domain.DomainControllers)
            {
                alDcs.Add(dc.Name);
            }
            return alDcs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ArrayList EnumerateDomainControllers()
        {
            return EnumerateDomainControllers(GetAdFQDN());
        }

        /// <summary>
        /// Gets the AD FQDS from current domain
        /// </summary>
        /// <returns>AD Fully Qualified Domain Name - FQDN</returns>
        public static string GetAdFQDN()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            return properties.DomainName;
        }

        /// <summary>
        /// Gets NetBIOS domain name from DNS Domain Name
        /// </summary>
        /// <param name="dnsDomainName"></param>
        /// <returns>AD NetBIOS domain Name</returns>
        public static string GetNetbiosDomainName(string dnsDomainName)
        {
            string netbiosDomainName = string.Empty;

            DirectoryEntry rootDSE = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", dnsDomainName));

            string configurationNamingContext = rootDSE.Properties["configurationNamingContext"][0].ToString();

            DirectoryEntry searchRoot = new DirectoryEntry("LDAP://cn=Partitions," + configurationNamingContext);

            DirectorySearcher searcher = new DirectorySearcher(searchRoot);
            searcher.SearchScope = SearchScope.OneLevel;
            searcher.PropertiesToLoad.Add("netbiosname");
            searcher.Filter = string.Format("(&(objectcategory=Crossref)(dnsRoot={0})(netBIOSName=*))", dnsDomainName);

            SearchResult result = searcher.FindOne();

            if (result != null)
            {
                netbiosDomainName = result.Properties["netbiosname"][0].ToString();
            }

            return netbiosDomainName;
        }

        /// <summary>
        /// Gets NetBIOS domain name finding the current AD FQDN
        /// </summary>
        /// <returns>AD NetBIOS domain Name</returns>
        public static string GetNetbiosDomainName()
        {
            return GetNetbiosDomainName(GetAdFQDN());
        }

        /// <summary>
        /// Get the "Default Naming Context" form the current Domain/Forest
        /// </summary>
        /// <returns>Default Naming Context</returns>
        public static string GetdefaultNamingContext()
        {
            DirectoryEntry ldapRoot = new DirectoryEntry(string.Format("LDAP://{0}/rootDSE", EguibarIT.Delegation.Other.Domains.GetAdFQDN()));

            return ldapRoot.Properties["defaultNamingContext"][0].ToString();
        }

        /// <summary>
        /// Get the "Configuration Naming Context" form the current Domain/Forest
        /// </summary>
        /// <returns>Configuration Naming Context</returns>
        public static string GetConfigurationNamingContext()
        {
            DirectoryEntry ldapRoot = new DirectoryEntry(string.Format("LDAP://{0}/rootDSE", EguibarIT.Delegation.Other.Domains.GetAdFQDN()));

            return ldapRoot.Properties["ConfigurationNamingContext"][0].ToString();
        }

        /// <summary>
        /// Get the "Schema Naming Context" form the current Domain/Forest
        /// </summary>
        /// <returns>Schema Naming Context</returns>
        public static string GetschemaNamingContext()
        {
            DirectoryEntry ldapRoot = new DirectoryEntry(string.Format("LDAP://{0}/rootDSE", EguibarIT.Delegation.Other.Domains.GetAdFQDN()));

            return ldapRoot.Properties["schemaNamingContext"][0].ToString();
        }

        /// <summary>
        /// Get the "Root Domain Naming Context" form the current Domain/Forest
        /// </summary>
        /// <returns>Root Domain Naming Context</returns>
        public static string GetrootDomainNamingContext()
        {
            DirectoryEntry ldapRoot = new DirectoryEntry(string.Format("LDAP://{0}/rootDSE", EguibarIT.Delegation.Other.Domains.GetAdFQDN()));

            return ldapRoot.Properties["rootDomainNamingContext"][0].ToString();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class Finding
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="LDAPUrl"></param>
        /// <returns></returns>
        public static List<string> GetUserLDAPProperties(string LDAPUrl)
        {
            List<string> properties = new List<string>();
            ActiveDirectorySchema adSchema = ActiveDirectorySchema.GetCurrentSchema();
            ActiveDirectorySchemaClass userSchema = default(ActiveDirectorySchemaClass);
            ActiveDirectorySchemaPropertyCollection propertiesCollection = default(ActiveDirectorySchemaPropertyCollection);
            userSchema = adSchema.FindClass("user");
            propertiesCollection = userSchema.MandatoryProperties;
            foreach (ActiveDirectorySchemaProperty prop in propertiesCollection)
            {
                properties.Add(prop.Name);
            }
            propertiesCollection = userSchema.OptionalProperties;
            foreach (ActiveDirectorySchemaProperty prop in propertiesCollection)
            {
                properties.Add(prop.Name);
            }
            properties.Sort();
            return properties;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="domain"></param>
        /// <param name="propertyToLoad"></param>
        /// <returns></returns>
        public static string GetNTAccountProperty(string sid, string domain, string propertyToLoad)
        {
            if (string.IsNullOrEmpty(sid)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(propertyToLoad)) throw new ArgumentNullException();
            string ldapDomainName = GetLDAPDomainName(domain);
            using (DirectoryEntry entries = new DirectoryEntry(ldapDomainName))
            {
                string filter = string.Format("(&amp;(objectCategory=person)(objectClass=user)(objectSID={0}))", sid);
                DirectorySearcher searcher = new DirectorySearcher(entries, filter);
                searcher.PropertiesToLoad.Add(propertyToLoad);
                searcher.PropertiesToLoad.Add("objectSID");
                SearchResult result = searcher.FindOne();
                if (!result.Properties.Contains(propertyToLoad))
                    throw new ActiveDirectoryObjectNotFoundException(string.Format("Property '{0}' not found on NTAccount '{1}'", propertyToLoad, sid));
                return result.Properties[propertyToLoad][0].ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string GetLDAPDomainName(string domainName)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(domainName)) throw new ArgumentNullException();
            string[] dcItems = domainName.Split(".".ToCharArray());
            sb.Append("LDAP://");
            foreach (string item in dcItems)
            {
                sb.AppendFormat("DC={0},", item);
            }
            return sb.ToString().Substring(0, sb.ToString().Length - 1);
        }
    }
}