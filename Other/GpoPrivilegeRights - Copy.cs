using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace EguibarIT.Delegation.Other
{
    /// <summary>
    /// 
    /// </summary>
    public class GpoPrivilegeRights
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurrentSection"></param>
        /// <param name="CurrentKey"></param>
        /// <param name="members"></param>
        /// <param name="GptTmpl"></param>
        /// <returns></returns>
        public static bool ConfigRestrictionsOnSection(string CurrentSection, string CurrentKey, List<string> members, IniData GptTmpl)
        {

            #region variables

            List<string> UserSIDs = new List<string>();
            List<string> TempMembers = new List<string>();
            List<string> NewMembers = new List<string>();

            string CurrentMember = null;

            // Define the variable for Trustee
            System.Security.Principal.IdentityReference identity;

            #endregion variables

            try
            {
                // Check if KEY already exists. TURUE if exist, then modify. FALSE if it doesn't exist (else) and create from scratch

                //This line gets the Value from the Key on the given section
                string directValue = GptTmpl[CurrentSection][CurrentKey];

                // Check if Key-Value pair exists
                if (directValue != null) // Key Exists
                {

                    // Get existing value and split it into a list
                    TempMembers = directValue.Split(',').ToList<string>();

                    //Check that existing values are still valid (Sid is valid)

                    foreach (string ExistingMember in TempMembers)
                    {
                        try
                        {
                            // Translate the SID to a SamAccountName string. If SID is not resolved, CurrentMember will be null
                            CurrentMember = new System.Security.Principal.SecurityIdentifier(ExistingMember.TrimStart('*')).Translate(typeof(System.Security.Principal.NTAccount)).ToString();
                        }
                        catch
                        {
                            CurrentMember = null;
                        }


                        // If SID is not resolved, CurrentMember will be null
                        // If not null, then add it to the new list
                        if (CurrentMember != null)
                        {
                            NewMembers.Add(ExistingMember);
                        }

                        // Set null to the variable for the next use.
                        CurrentMember = null;
                    }

                    // Iterate through all members
                    foreach (string item in members)
                    {
                        // Check if Identity is a WellKnownSID
                        // If identity is NOT a WellKnownSID, the function will translate to existing Object SID.
                        // WellKnownSid function will return null if SID is not well known.
                        identity = EguibarIT.Delegation.SIDs.CheckIfNameIsWellKnownSID(item);

                        // Check if new sid is already defined on value. Add it if NOT.
                        if (!NewMembers.Contains("*" + identity.ToString()))
                        {
                            NewMembers.Add("*" + identity.ToString());
                        }

                        // Add the resulting list of users to the Key
                        GptTmpl[CurrentSection][CurrentKey] = String.Join(",", NewMembers);
                    }

                }
                else //Key does not exists
                {
                    // Iterate through all members
                    foreach (string item in members)
                    {
                        // Define the variable for Trustee
                        //System.Security.Principal.IdentityReference identity;
                        // Check if Identity is a WellKnownSID
                        identity = EguibarIT.Delegation.SIDs.CheckIfNameIsWellKnownSID(item);

                        // WellKnownSid function will return null if SID is not well known.
                        if (identity == null)
                        {
                            // Retrive current SID
                            identity = new NTAccount((EguibarIT.Delegation.Other.Domains.GetNetbiosDomainName()), item).Translate(typeof(SecurityIdentifier));
                        }

                        //Add the new member to the List, adding * prefix
                        UserSIDs.Add("*" + identity.ToString());
                    }

                    // Add the resulting list of users to the Key
                    GptTmpl[CurrentSection].AddKey(CurrentKey, String.Join(",", UserSIDs));

                }
            } //Try
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("Either you are trying to add a group that does not exist, or the identity provided does not correspond to a Group object class : '{0}'. Message is {1}", ex, ex.Message));
            }
            finally
            {

            }

            return true;
        } // 
    } // GpoPrivilegeRights
} // Namespace
