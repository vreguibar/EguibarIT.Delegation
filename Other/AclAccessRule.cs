using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.Other
{
    /// <summary>
    ///
    /// </summary>
    public class AccessRule
    {
        /// <summary>
        /// Returns a List of Access Control Rules of a given object, being able to filter if identity is present
        /// </summary>
        /// <param name="_distinguishedName">LDAPpath to the object to get the list from</param>
        /// <param name="_searchby">If present, list only the Control Rules matching this identity</param>
        /// <returns>List of Arrays</returns>
        //public static List<string> AclAccessRule(string _distinguishedName, [Optional] string _searchby)
        public static List<ACEarray> AclAccessRule(string _distinguishedName, [Optional] string _searchby)
        {
            #region variables

            List<ACEarray> arrayList = new List<ACEarray>();

            int i = 0;

            #endregion variables

            // Get DirectoryEntry to the object
            DirectoryEntry userEntry = new DirectoryEntry("LDAP://" + _distinguishedName);

            // Get the accessRules from the given object
            AuthorizationRuleCollection rules = userEntry.ObjectSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

            // Iterate through all rules
            foreach (System.Security.AccessControl.AuthorizationRule rule in rules)
            {
                i++;
                var ACElist = new ACEarray();
                string _strIdentityReference;
                //Dictionary<string, string> values = new Dictionary<string, string>();

                ActiveDirectoryAccessRule adar = rule as ActiveDirectoryAccessRule;

                if (_searchby != null)
                {
                    try
                    {
                        _searchby = _searchby.ToLower();
                        _searchby = _searchby.Replace(String.Format("{0}\\", EguibarIT.Delegation.Other.Domains.GetNetbiosDomainName()), "");

                        _strIdentityReference = EguibarIT.Delegation.SIDs.ConvertSidToName(adar.IdentityReference.ToString());

                        _strIdentityReference = _strIdentityReference.ToLower();

                        _strIdentityReference = _strIdentityReference.Replace(String.Format("{0}\\", EguibarIT.Delegation.Other.Domains.GetNetbiosDomainName()).ToLower(), "");
                        _strIdentityReference = _strIdentityReference.Replace("built-in\\", "");
                        _strIdentityReference = _strIdentityReference.Replace("built in\\", "");
                        _strIdentityReference = _strIdentityReference.Replace("builtin\\", "");
                        _strIdentityReference = _strIdentityReference.Replace("nt authority\\", "");
                        _strIdentityReference = _strIdentityReference.Replace("ntauthority\\", "");
                        _strIdentityReference = _strIdentityReference.Replace("nt service\\", "");
                        _strIdentityReference = _strIdentityReference.Replace("ntservice\\", "");

                        //Compare 2 strings providing invariant culture.
                        if ((String.Compare(_searchby, _strIdentityReference, StringComparison.CurrentCultureIgnoreCase)) == 0)
                        {
                            ACElist.ACENumber = i;
                            ACElist.DistinguishedName = _distinguishedName;
                            ACElist.IdentityReference = EguibarIT.Delegation.SIDs.ConvertSidToName(adar.IdentityReference.ToString());
                            ACElist.ActiveDirectoryRightst = adar.ActiveDirectoryRights.ToString(); // System.DirectoryServices.ActiveDirectoryRights
                            ACElist.AccessControlType = adar.AccessControlType.ToString();// System.Security.AccessControl.AccessControlType
                            ACElist.ObjectType = EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID(adar.ObjectType); // System.Guid
                            ACElist.InheritanceType = adar.InheritanceType.ToString(); // System.DirectoryServices.ActiveDirectorySecurityInheritance
                            ACElist.InheritedObjectType = EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID(adar.InheritedObjectType); // System.Guid
                            ACElist.IsInherited = adar.IsInherited.ToString();

                            arrayList.Add(ACElist);
                        }
                        else
                        {
                            i--;
                        }
                    }//end try
                    catch (Exception ex)
                    {
                        throw new ApplicationException(string.Format("An error occurred while processing the Access Rule: '{0}'. Message is {1}", ex, ex.Message));
                    }
                }//end if
                else
                {
                    try
                    {
                        ACElist.ACENumber = i;
                        ACElist.DistinguishedName = _distinguishedName;
                        ACElist.IdentityReference = EguibarIT.Delegation.SIDs.ConvertSidToName(adar.IdentityReference.ToString());
                        ACElist.ActiveDirectoryRightst = adar.ActiveDirectoryRights.ToString(); // System.DirectoryServices.ActiveDirectoryRights
                        ACElist.AccessControlType = adar.AccessControlType.ToString();// System.Security.AccessControl.AccessControlType
                        ACElist.ObjectType = EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID(adar.ObjectType); // System.Guid
                        ACElist.InheritanceType = adar.InheritanceType.ToString(); // System.DirectoryServices.ActiveDirectorySecurityInheritance
                        ACElist.InheritedObjectType = EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID(adar.InheritedObjectType); // System.Guid
                        ACElist.IsInherited = adar.IsInherited.ToString();

                        arrayList.Add(ACElist);
                    }//end try
                    catch (Exception ex)
                    {
                        throw new ApplicationException(string.Format("An error occurred while processing the Access Rule: '{0}'. Message is {1}", ex, ex.Message));
                    }
                }//end else/if
            }//end foreach

            //return List;
            return arrayList;
        }//end AclAccessRule
    }//end class AccessRule
}