using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Security.Principal;

namespace EguibarIT.Delegation
{
    /// <summary>
    ///
    /// </summary>
    public class SIDs
    {
        /// <summary>
        /// Convert SID to Name
        /// </summary>
        /// <param name="_sid">Security Identifier SID of the object to be translated</param>
        /// <returns></returns>
        public static string ConvertSidToName(string _sid)
        {
            return ConvertSidToName(new SecurityIdentifier(_sid));
        }

        /// <summary>
        /// Convert SID to Name
        /// </summary>
        /// <param name="_sid">Security Identifier SID of the object to be translated</param>
        /// <returns></returns>
        public static string ConvertSidToName(SecurityIdentifier _sid)
        {
            string foundName = null;

            /*
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            // find a group
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, _sid);
            if (group != null)
            {
                foundName = group.SamAccountName;
            }
            */
            try
            {
                foundName = new System.Security.Principal.SecurityIdentifier(_sid.ToString()).Translate(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            catch (IdentityNotMappedException)
            {
                if (IsWellKnownSID(new SecurityIdentifier(_sid.ToString())))
                {
                    foundName = GetNameOfWellKnownSID(new SecurityIdentifier(_sid.ToString()));
                }
                else
                {
                    WarningException myEx = new WarningException("Identity Not Mapped Exception");
                    foundName = null;
                }
            }

            return foundName;
        }

        /// <summary>
        /// Verify if SID exists. If IdentityNotMappedException is raised, means the SID does not exists and returns false.
        /// </summary>
        /// <param name="_sid">STRING representing the SID to be translated.</param>
        /// <returns>BOOL. True if SID exists. False if it does not exist.</returns>
        public static bool SidExists(string _sid)
        {
            if (ConvertSidToName(_sid) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check the given sid against the System.Security.Principal.WellKnownSidType. Return true if a match is found.
        /// </summary>
        /// <param name="_sid"></param>
        /// <returns>BOOL indicating if SID is a WellKnownSid</returns>
        public static bool IsWellKnownSID(SecurityIdentifier _sid)
        {
            bool result = false;

            foreach (WellKnownSidType v in Enum.GetValues(typeof(WellKnownSidType)))
            {
                //Console.WriteLine("XXXXX...: " + _sid.IsWellKnownSID(v));
                if (_sid.IsWellKnown(v))
                {
                    //Console.WriteLine("Name...: " + v);
                    //Console.WriteLine("Sid....: " + _sid);

                    result = true;
                }
            }
            //Console.WriteLine("----- Bool...: " + result);
            return result;
        }

        /// <summary>
        /// Check the given sid against the System.Security.Principal.WellKnownSidType. Return the name if a match is found.
        /// </summary>
        /// <param name="_sid"></param>
        /// <returns>Name of the WellKnownSid</returns>
        public static string GetNameOfWellKnownSID(SecurityIdentifier _sid)
        {
            string result = string.Empty;

            foreach (WellKnownSidType name in Enum.GetValues(typeof(WellKnownSidType)))
            {
                //Console.WriteLine("XXXXX...: " + _sid.IsWellKnownSID(name));
                if (_sid.IsWellKnown(name))
                {
                    //Console.WriteLine("Name...: " + name);
                    //Console.WriteLine("Sid....: " + _sid);

                    //
                    switch (name.ToString())
                    {
                        case "NullSid": { result = "Null"; break; };
                        case "WorldSid": { result = "World"; break; };
                        case "LocalSid": { result = "Local"; break; };
                        case "CreatorOwnerSid": { result = "Creator Owner"; break; };
                        case "CreatorGroupSid": { result = "Creator Group"; break; };
                        case "CreatorOwnerServerSid": { result = "Creator Owner Server"; break; };
                        case "CreatorGroupServerSid": { result = "Creator Group Server"; break; };
                        case "NTAuthoritySid": { result = "NT AUTHORITY"; break; };
                        case "DialupSid": { result = "Dialup"; break; };
                        case "NetworkSid": { result = "Network"; break; };
                        case "BatchSid": { result = "Batch"; break; };
                        case "InteractiveSid": { result = "Interactive"; break; };
                        case "ServiceSid": { result = "Service"; break; };
                        case "AnonymousSid": { result = "ANONYMOUS"; break; };
                        case "ProxySid": { result = "Proxy"; break; };
                        case "EnterpriseControllersSid": { result = "Enterprise Controllers"; break; };
                        case "SelfSid": { result = "SELF"; break; };
                        case "AuthenticatedUserSid": { result = "Authenticated User"; break; };
                        case "RestrictedCodeSid": { result = "Restricted Code"; break; };
                        case "TerminalServerSid": { result = "Terminal Server"; break; };
                        case "RemoteLogonIdSid": { result = "Remote Logon"; break; };
                        case "LogonIdsSid": { result = "Logon Ids"; break; };
                        case "LocalSystemSid": { result = "Local System"; break; };
                        case "LocalServiceSid": { result = "Local Service"; break; };
                        case "NetworkServiceSid": { result = "Network Service"; break; };
                        case "BuiltinDomainSid": { result = "BUILTIN\\Domain"; break; };
                        case "BuiltinAdministratorsSid": { result = "BUILTIN\\Administrators"; break; };
                        case "BuiltinUsersSid": { result = "BUILTIN\\Users"; break; };
                        case "BuiltinGuestsSid": { result = "BUILTIN\\Guests"; break; };
                        case "BuiltinPowerUsersSid": { result = "BUILTIN\\Power Users"; break; };
                        case "BuiltinAccountOperatorsSid": { result = "BUILTIN\\Account Operators"; break; };
                        case "BuiltinSystemOperatorsSid": { result = "BUILTIN\\System Operators"; break; };
                        case "BuiltinPrintOperatorsSid": { result = "BUILTIN\\Print Operators"; break; };
                        case "BuiltinBackupOperatorsSid": { result = "BUILTIN\\Backup Operators"; break; };
                        case "BuiltinReplicatorSid": { result = "BUILTIN\\Replicator"; break; };
                        case "BuiltinPreWindows2000CompatibleAccessSid": { result = "BUILTIN\\Pre–Windows 2000 Compatible Access"; break; };
                        case "BuiltinRemoteDesktopUsersSid": { result = "BUILTIN\\Remote Desktop Users"; break; };
                        case "BuiltinNetworkConfigurationOperatorsSid": { result = "BUILTIN\\Network Configuration Operators"; break; };
                        case "AccountAdministratorSid": { result = "Administrator"; break; };
                        case "AccountGuestSid": { result = "Guest"; break; };
                        case "AccountKrbtgtSid": { result = "Krbtgt"; break; };
                        case "AccountDomainAdminsSid": { result = "Domain Admins"; break; };
                        case "AccountDomainUsersSid": { result = "Domain Users"; break; };
                        case "AccountDomainGuestsSid": { result = "Domain Guests"; break; };
                        case "AccountComputersSid": { result = "Computer"; break; };
                        case "AccountControllersSid": { result = "Controllers"; break; };
                        case "AccountCertAdminsSid": { result = "Cert Admins"; break; };
                        case "AccountSchemaAdminsSid": { result = "Schema Admins"; break; };
                        case "AccountEnterpriseAdminsSid": { result = "Enterprise Admins"; break; };
                        case "AccountPolicyAdminsSid": { result = "Policy Admins"; break; };
                        case "AccountRasAndIasServersSid": { result = "Ras And Ias Servers"; break; };
                        case "NtlmAuthenticationSid": { result = "Ntlm Authentication"; break; };
                        case "DigestAuthenticationSid": { result = "Digest Authentication"; break; };
                        case "SChannelAuthenticationSid": { result = "SChannel Authentication"; break; };
                        case "ThisOrganizationSid": { result = "This Organization"; break; };
                        case "OtherOrganizationSid": { result = "Other Organization"; break; };
                        case "BuiltinIncomingForestTrustBuildersSid": { result = "BUILTIN\\Incoming Forest Trust Builders"; break; };
                        case "BuiltinPerformanceMonitoringUsersSid": { result = "BUILTIN\\Performance Monitoring Users"; break; };
                        case "BuiltinPerformanceLoggingUsersSid": { result = "BUILTIN\\Performance Logging Users"; break; };
                        case "BuiltinAuthorizationAccessSid": { result = "BUILTIN\\Authorization Access"; break; };
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Return the WellKnownSID of the given SamAccountName
        /// </summary>
        /// <param name="_name"></param>
        /// <returns>SID</returns>
        public static System.Security.Principal.IdentityReference CheckIfNameIsWellKnownSID(string _name)
        {
            // Define the variable for Trustee
            //System.Security.Principal.SecurityIdentifier identity;
            System.Security.Principal.IdentityReference identity;

            try
            {
                // Check if group is a Well-Known group
                // https://technet.microsoft.com/en-us/library/cc978401.aspx
                // https://msdn.microsoft.com/en-us/library/cc980032.aspx
                // https://msdn.microsoft.com/en-us/library/system.security.principal.wellknownsidtype(v=vs.110).aspx
                // https://support.microsoft.com/en-us/help/243330/well-known-security-identifiers-in-windows-operating-systems
                // https://adsecurity.org/?p=1001

                _name = _name.ToLower();

                _name = _name.Replace("built-in\\", "");
                _name = _name.Replace("builtin\\", "");
                _name = _name.Replace("built in\\", "");
                _name = _name.Replace("nt authority\\", "");
                _name = _name.Replace("ntauthority\\", "");
                _name = _name.Replace("nt service\\", "");
                _name = _name.Replace("ntservice\\", "");

                switch (_name)
                {
                    case "null nuthority":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-0");
                        break;

                    case "nobody":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-0-0");
                        break;

                    case "world authority":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-1");
                        break;

                    case "everyone":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-1-0");
                        break;

                    case "untrusted mandatory level":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-16-0");
                        break;

                    case "high mandatory level":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-16-12288");
                        break;

                    case "system mandatory level":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-16-16384");
                        break;

                    case "protected process mandatory level":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-16-20480");
                        break;

                    case "secure process mandatory level":
                        identity = new System.Security.Principal.SecurityIdentifier("*S-1-16-28672");
                        break;

                    case "low mandatory level":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-16-4096");
                        break;

                    case "medium mandatory level":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-16-8192");
                        break;

                    case "medium plus mandatory level":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-16-8448");
                        break;

                    case "local authority":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-2");
                        break;

                    case "creator authority":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-3");
                        break;

                    case "creator owner":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-3-0");
                        break;

                    case "creator group":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-3-1");
                        break;

                    case "creator owner server":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-3-2");
                        break;

                    case "creator group server":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-3-3");
                        break;

                    case "nonunique authority":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-4");
                        break;

                    case "nt authority":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5");
                        break;

                    case "dialup":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-1");
                        break;

                    case "network":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-2");
                        break;

                    case "batch":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-3");
                        break;

                    case "nt authority (localservice)":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-19");
                        break;

                    case "local service":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-19");
                        break;

                    case "network service":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-20");
                        break;

                    case "key admins":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-21-4195037842-338827918-94892514-526");
                        break;

                    case "interactive":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-4");
                        break;

                    case "service":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-6");
                        break;

                    case "anonymous":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-7");
                        break;

                    case "anonymous logon":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-7");
                        break;

                    case "proxy":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-8");
                        break;

                    case "enterprise controllers":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-9");
                        break;

                    case "principal self":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-10");
                        break;

                    case "self":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-10");
                        break;

                    case "authenticated users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-11");
                        break;

                    case "local account":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-113");
                        break;

                    case "local account and member of administrators group":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-114");
                        break;

                    case "restricted code":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-12");
                        break;

                    case "terminal server users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-13");
                        break;

                    case "remote interactive logon":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-14");
                        break;

                    case "this organization":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-15");
                        break;

                    case "iis_usrs":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-17");
                        break;

                    case "local system":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-18");
                        break;

                    case "system":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-18");
                        break;

                    case "administrators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-544");
                        break;

                    case "'users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-545");
                        break;

                    case "guests":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-546");
                        break;

                    case "power users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-547");
                        break;

                    case "account operators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-548");
                        break;

                    case "server operators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-549");
                        break;

                    case "print operators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-550");
                        break;

                    case "backup operators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-551");
                        break;

                    case "replicators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-552");
                        break;

                    case "pre–windows 2000 compatible access":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-554");
                        break;

                    case "remote desktop users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-555");
                        break;

                    case "network configuration operators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-556");
                        break;

                    case "incoming forest trust builders":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-557");
                        break;

                    case "performance monitor users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-558");
                        break;

                    case "performance log users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-559");
                        break;

                    case "windows authorization access group":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-560");
                        break;

                    case "terminal server license servers":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-561");
                        break;

                    case "distributed com users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-562");
                        break;

                    case "iis_iusrs":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-568");
                        break;

                    case "cryptographic operators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-569");
                        break;

                    case "event log readers":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-573");
                        break;

                    case "rds remote access servers":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-575");
                        break;

                    case "rds management servers":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-577");
                        break;

                    case "hyper-v administrators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-578");
                        break;

                    case "access control assistance operators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-579");
                        break;

                    case "remote management users":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-580");
                        break;

                    case "system managed accounts group":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-581");
                        break;

                    case "storage replica administrators":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-582");
                        break;

                    case "ntlm authentication":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-64-10");
                        break;

                    case "schannel authentication":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-64-14");
                        break;

                    case "digest authentication":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-64-21");
                        break;

                    case "nt service":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-80");
                        break;

                    case "all services":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-80-0");
                        break;

                    case "nt virtual machine":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-83-0");
                        break;

                    case "virtual machines":
                        identity = new System.Security.Principal.SecurityIdentifier("S-1-5-83-0");
                        break;

                    default:
                        // Provide the trustee identity (Group who gets the permissions) by using current domain
                        identity = new NTAccount((EguibarIT.Delegation.Other.Domains.GetNetbiosDomainName()), _name).Translate(typeof(SecurityIdentifier));
                        break;
                } //Switch
            } //Try
            catch (Exception ex)
            {
                identity = null;

                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("An error occurred while retriving the identity: '{0}'. Message is {1}", ex, ex.Message));
            }

            return identity;
        } // CheckIfNameIsWellKnownSID

        /// <summary>
        /// Return the current Domain SID
        /// </summary>
        /// <returns>Domain SID</returns>
        public static System.Security.Principal.SecurityIdentifier DomainSid()
        {
            DirectoryEntry ldapRoot = new DirectoryEntry(String.Format("LDAP://{0}", EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext()));

            SecurityIdentifier domainSid = new SecurityIdentifier((byte[])ldapRoot.Properties["objectSid"].Value, 0);

            return domainSid;
        }
    } // END Class SIDs
}