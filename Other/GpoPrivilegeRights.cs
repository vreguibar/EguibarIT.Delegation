using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace EguibarIT.Delegation.Other
{
    /// <summary>
    /// Class to access GptTmpl.inf and modify its data.
    /// </summary>
    public class GPOs
    {
        /// <summary>
        /// Configure a Section with Key/Value of a GptTmpl.inf file.
        /// </summary>
        /// <param name="CurrentSection">Section name</param>
        /// <param name="CurrentKey">Key name</param>
        /// <param name="members">List of values to be configured</param>
        /// <param name="GptTmpl">GptTmpl.inf file as IniFile object</param>
        /// <returns></returns>
        public static bool ConfigRestrictionsOnSection(string CurrentSection, string CurrentKey, List<string> members, IniFile GptTmpl)
        {
            #region variables

            bool satus = false;

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

                // Check if Key-Value pair exists
                if (GptTmpl.Sections[CurrentSection].KeyValuePair.ContainsKey(CurrentKey)) // Key Exists
                {
                    //This line gets the Value from the Key on the given section
                    string directValue = GptTmpl.Sections[CurrentSection].KeyValuePair[CurrentKey];

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
                            //Only add the CurrentMember if not present on NewMembers
                            if (!NewMembers.Contains(CurrentMember))
                            {
                                NewMembers.Add(ExistingMember);
                            }
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
                    }
                    // Add the resulting list of users to the Key
                    GptTmpl.Sections[CurrentSection].KeyValuePair[CurrentKey] = String.Join(",", NewMembers);
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

                        //Check the current SID is not already on list
                        if (!UserSIDs.Contains("*" + identity.ToString()))
                        {
                            //Add the new member to the List, adding * prefix
                            UserSIDs.Add("*" + identity.ToString());
                        }
                    }

                    // Add the resulting list of users to the Key
                    //GptTmpl.AddSetting(CurrentSection, CurrentKey, String.Join(",", UserSIDs));
                    GptTmpl.Sections[CurrentSection].KeyValuePair.Add(CurrentKey, String.Join(",", UserSIDs));
                }

                GptTmpl.WriteAllText();
            } //end Try
            catch (Exception ex)
            {
                satus = false;

                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("Either you are trying to add a group that does not exist, or the identity provided does not correspond to a Group object class : '{0}'. Message is {1}", ex, ex.Message));
            }
            finally
            {
                satus = true;
            }

            return satus;
        } // end ConfigRestrictionsOnSection

        /// <summary>
        /// Get the GptTmpl.inf file from the given GPO.
        /// </summary>
        /// <param name="_GpoToModify">Name of the GPO to get the GptTmpl.inf file</param>
        /// <returns></returns>
        public static IniFile GptTemplate(string _GpoToModify)
        {
            #region variables

            // DefaultNamingContext DistinguishedName
            string AdDn;

            // Inizialize file variable for future use
            IniFile GptTmpl = new IniFile();

            //Variables hosting path to files
            string PathToGptTmpl;
            string SysVolPath;
            string GptTmplFile;

            // Use current user’s domain
            Microsoft.GroupPolicy.GPDomain domain = new Microsoft.GroupPolicy.GPDomain();

            // Define the GPO variable
            Microsoft.GroupPolicy.Gpo Gpo;

            // Define GPO ID string
            string GpoId;

            // Registry Key var
            RegistryKey rk;

            #endregion variables

            // Get AD DistinguishedName
            AdDn = EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext();

            // Get the GPO
            Gpo = domain.GetGpo(_GpoToModify);

            // Get the GPO Id and Include Brackets {}
            GpoId = "{" + Gpo.Id + "}";

            // Get the SysVol path from registry
            rk = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Netlogon\\Parameters", false);
            SysVolPath = (string)rk.GetValue("SysVol");
            rk.Close();

            // Get path where the GptTmpl.inf file should be stored
            PathToGptTmpl = string.Format("{0}\\{1}\\Policies\\{2}\\Machine\\microsoft\\windows nt\\SecEdit", SysVolPath, EguibarIT.Delegation.Other.Domains.GetAdFQDN(), GpoId);

            // If the folder does not exist yet, it will be created. If the folder exists already, the line will be ignored
            Directory.CreateDirectory(PathToGptTmpl);

            // Get full path + filename
            GptTmplFile = string.Format("{0}\\GptTmpl.inf", PathToGptTmpl);

            IniFile GptTemplate = new IniFile();

            GptTemplate.FilePath = GptTmplFile;

            if (File.Exists(GptTmplFile))
            {
                // Get content of GptTmpl.inf file
                // This load the INI file, reads the data contained in the file, and parses that data
                //GptTmpl = parser.ReadFile(GptTmplFile);

                GptTemplate.ReadFile(GptTmplFile);
            }

            return GptTemplate;
        }// end GptTemplate
    } // end GPOs

    /// <summary>
    /// Class to access Gpt.ini and modify its data.
    /// </summary>
    public class GPTs
    {
        /// <summary>
        /// Update Version number on both AD object and GPT.ini file
        /// </summary>
        /// <param name="_GpoToModify">Name of the GPO to get the Gpt.ini file</param>
        /// <returns></returns>
        public static bool UpdateGpt(string _GpoToModify)
        {
            // Modify the GPO versionNumber on the GPC
            // https://technet.microsoft.com/en-us/library/ff730972.aspx
            // https://blogs.technet.microsoft.com/grouppolicy/2008/01/08/understanding-the-domain-based-gpo-version-number-gpmc-script-included/

            #region Variables

            bool satus = false;

            // DefaultNamingContext DistinguishedName
            string AdDn;

            // Sysvol
            string SysVolPath;

            // Registry Key var
            RegistryKey rk;

            // Define the GPO variable
            Microsoft.GroupPolicy.Gpo Gpo;

            // Define GPO ID string
            string GpoId;

            // Inizialize file variable for future use
            IniFile Gpt = new Other.IniFile();

            //Vars for VersionObject
            DirectoryEntry de;
            Int64 VersionObject;
            string HexValue;
            string HexUserVN;
            string HexComputerVN;
            Int64 UserVN;
            Int64 ComputerVN;
            string NewHex;
            Int64 NewVersionObject;

            #endregion Variables

            // Get AD DistinguishedName
            AdDn = EguibarIT.Delegation.Other.Domains.GetdefaultNamingContext();

            // Use current user’s domain
            Microsoft.GroupPolicy.GPDomain domain = new Microsoft.GroupPolicy.GPDomain();

            // Get the SysVol path from registry
            rk = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Netlogon\\Parameters", false);
            SysVolPath = (string)rk.GetValue("SysVol");
            rk.Close();

            // Get the GPO
            Gpo = domain.GetGpo(_GpoToModify);

            // Get the GPO Id and Include Brackets {}
            GpoId = "{" + Gpo.Id + "}";

            // Get path to the GPTs.ini file. Increment to make changes.
            string PathToGpt = string.Format("{0}\\{1}\\Policies\\{2}\\gpt.ini", SysVolPath, EguibarIT.Delegation.Other.Domains.GetAdFQDN(), GpoId);

            try
            {
                // Get the GPO object
                de = new DirectoryEntry(string.Format("LDAP://CN={0},CN=Policies,CN=System,{1}", GpoId, AdDn));

                // Get the VersionObject of the DirectoryEntry (the GPO)
                VersionObject = Convert.ToInt64(de.Properties["VersionNumber"].Value.ToString());

                // Convert the value into a 8 digit HEX string
                HexValue = VersionObject.ToString("x8");

                // Top 16 bits HEX UserVersionNumber - first 4 characters (complete with zero to the left)
                // This is the UserVersion
                HexUserVN = HexValue.Substring(0, 4);

                // Lower 16 bits HEX ComputerVersionNumber - last 4 characters (complete with zero to the left)
                // This is the ComputerVersion
                HexComputerVN = HexValue.Substring(4);

                //Top 16 bits as Integer UserVersionNumber
                UserVN = Convert.ToInt64(HexUserVN, 16);

                //Lower 16 bits as Integer ComputerVersionNumber
                ComputerVN = Convert.ToInt64(HexComputerVN, 16);

                // Increment Computer Version Number by 3
                ComputerVN += 3;

                //Concatenate '0x' and 'HEX UserVersionNumber having 4 digits' and 'HEX ComputerVersionNumber having 4
                // digits' (0x must be added in order to indicate Hexadecimal number, otherwise fails)
                NewHex = "0x" + HexUserVN + ComputerVN.ToString("x4");

                // Convert the New Hex number to integer
                NewVersionObject = Convert.ToInt64(NewHex, 16);

                //Update the GPO VersionNumber with the new value
                de.Properties["VersionNumber"].Value = NewVersionObject.ToString();

                // Save the information on the DirectoryObject
                de.CommitChanges();

                //Close the DirectoryEntry
                de.Close();

                // Write new version value to GPT (Including Section Name)
                Gpt.Sections.Add(new IniSection("General"));
                Gpt.Sections["General"].KeyValuePair.Add("Version", NewVersionObject.ToString());

                // Save settings on GPT file
                Gpt.WriteAllText(@PathToGpt);
            }//end Try
            catch (Exception ex)
            {
                satus = false;

                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException(string.Format("The GPTs.ini file could not be modified: '{0}'. Message is {1}", ex, ex.Message));
            }
            finally
            {
                satus = true;
            }

            return satus;
        }
    }//end class
} // Namespace