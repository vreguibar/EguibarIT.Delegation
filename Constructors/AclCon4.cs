using System;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace EguibarIT.Delegation.Constructors
{
    /// <summary>
    ///
    /// </summary>
    public class AclCon4
    {
        /// <summary>
        /// Constructor to add or remove Active Directory Access Rules with 4 parameters
        /// </summary>
        /// <param name="LDAPpath">STRING representing the Distinguished Name of the object (ej. OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local)</param>
        /// <param name="GroupID">STRING representing the group SamAccountName</param>
        /// <param name="adRights">The ActiveDirectoryRights enumeration specifies the access rights that are assigned to an Active Directory Domain Services object</param>
        /// <param name="acType">Access Control type to Allow or Deny</param>
        /// <param name="objectType">Object GUID defined on the Schema</param>
        /// <param name="RemoveRule">BOOL indicating if the given rule has to be removed</param>
        /// <returns>bool</returns>
        public static bool AclConstructor4(string LDAPpath,
                                            string GroupID,
                                            ActiveDirectoryRights adRights,
                                            AccessControlType acType,
                                            Guid objectType,
                                            bool RemoveRule
                                            )
        {
            bool returnValue = false;
            /*
            // https://msdn.microsoft.com/en-us/library/cawwkf0x(v=vs.110).aspx
            // https://github.com/rashiph/DecompliedDotNetLibraries/blob/master/System.DirectoryServices/System/DirectoryServices/ActiveDirectoryAccessRule.cs
             ActiveDirectoryAccessRule(IdentityReference identity,
                                       ActiveDirectoryRights adRights,
                                       AccessControlType acType,
                                       Guid objectType
                                       )
            */
            /*
            // https://github.com/rashiph/DecompliedDotNetLibraries/blob/master/System.DirectoryServices/System/DirectoryServices/ActiveDirectorySecurityInheritance.cs
            // http://stackoverflow.com/questions/9567708/programmatically-set-protection-from-accidental-deletion-on-an-ou-in-microsoft-a
            // http://stackoverflow.com/questions/4193909/how-do-i-add-permissions-to-an-ou-using-c

            // https://social.msdn.microsoft.com/Forums/en-US/1283a2d0-0c76-442c-af4b-db255bca3215/assign-rights-to-the-user-group-for-particular-ou-in-active-directory-using-c?forum=netfx64bit
            */

            try {
                // Check that object starts with LDAP:// - add if missing
                // "LDAP://OU=Admin,DC=EguibarIT,DC=local" or "OU=Admin,DC=EguibarIT,DC=local"
                if (!LDAPpath.StartsWith("LDAP://", System.StringComparison.CurrentCultureIgnoreCase)) {
                    LDAPpath = "LDAP://" + LDAPpath;
                }

                returnValue = true;
            } catch (Exception ex) {
                returnValue = false;

                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("An error occurred while verifying LDAP path: '{0}'. Message is {1}", ex, ex.Message));
            }

            // Get a reference to the Object we want to delegate
            DirectoryEntry myObject = new DirectoryEntry(LDAPpath);

            // Define the variable for Trustee
            IdentityReference identity;

            // Check if Identity is a WellKnownSID
            // If identity is NOT a WellKnownSID, the function will translate to existing Object SID.
            // WellKnownSid function will return null if SID is not well known.
            identity = EguibarIT.Delegation.SIDs.CheckIfNameIsWellKnownSID(GroupID);

            // Set what to do (AD Rights http://msdn.microsoft.com/en-us/library/system.directoryservices.activedirectoryrights(v=vs.110).aspx)
            //ActiveDirectoryRights adRights = ActiveDirectoryRights.GenericAll;

            // Define if allowed or denied (AccessControlType - Allow/Denied)
            //AccessControlType acType = AccessControlType.Allow;

            // Set the object GUID
            //Guid objectType = new Guid();

            // Set the scope (AdSecurityInheritance - http://msdn.microsoft.com/en-us/library/system.directoryservices.activedirectorysecurityinheritance(v=vs.110).aspx)
            //ActiveDirectorySecurityInheritance inheritanceType = ActiveDirectorySecurityInheritance.All;

            // Create the access rule
            ActiveDirectoryAccessRule newRule;

            try {
                newRule = new ActiveDirectoryAccessRule(identity, adRights, acType, objectType);

                returnValue = true;
            } catch (Exception ex) {
                returnValue = false;

                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("An error occurred while creating the access rule: '{0}'. Message is {1}", ex, ex.Message));
            }

            try {
                // If parameter RemoveRule is False (default when omitted) it will ADD the Access Rule
                // if TRUE then will REMOVE the access rule
                if (RemoveRule == true) {
                    myObject.ObjectSecurity.RemoveAccessRule(newRule);
                } else {
                    // if false (no remove)
                    //myOU.ObjectSecurity.SetAccessRule(newRule);
                    myObject.ObjectSecurity.AddAccessRule(newRule);
                }

                returnValue = true;
            } catch (Exception ex) {
                returnValue = false;

                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("An error occurred while adding OR removing the access rule: '{0}'. Message is {1}", ex, ex.Message));
            }

            try {
                // Re-apply the modified DACL to the OU
                // Now push these AccessRules to AD
                myObject.CommitChanges();

                returnValue = true;
            } catch (Exception ex) {
                returnValue = false;

                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("An error occurred while commiting changes to the access rule: '{0}'. Message is {1}", ex, ex.Message));
            }

            return returnValue;
        }

        /// <summary>
        /// Constructor to add or remove Active Directory Access Rules with 5 parameters
        /// </summary>
        /// <param name="LDAPpath">STRING representing the Distinguished Name of the object (ej. OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local)</param>
        /// <param name="GroupID">SID representing the group SamAccountName</param>
        /// <param name="adRights">The ActiveDirectoryRights enumeration specifies the access rights that are assigned to an Active Directory Domain Services object</param>
        /// <param name="acType">Access Control type to Allow or Deny</param>
        /// <param name="objectType">Object GUID defined on the Schema</param>
        /// <param name="RemoveRule">BOOL indicating if the given rule has to be removed</param>
        /// <returns>bool</returns>
        public static bool AclConstructor5(string LDAPpath,
                                            SecurityIdentifier GroupID,
                                            ActiveDirectoryRights adRights,
                                            AccessControlType acType,
                                            Guid objectType,
                                            bool RemoveRule
                                            )
        {
            return AclConstructor5(LDAPpath,
                                    GroupID,
                                    adRights,
                                    acType,
                                    objectType,
                                    RemoveRule
                                    );
        }
    }//end class AclCon4
}