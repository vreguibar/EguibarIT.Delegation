using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace EguibarIT.Delegation
{
    /// <summary>
    ///
    /// </summary>
    internal class GUIDs
    {
        /// <summary>
        /// get all schema classess and store it into a hashtable
        /// in the form "name" = "GUID" (Key=Value)
        /// </summary>
        /// <returns>Hashtable</returns>
        public static Dictionary<string, Guid> GetAllClassSchemaGUID()
        {
            try
            {
                if (EguibarIT.Delegation.Vars.AllclassSchemaGuids == null || EguibarIT.Delegation.Vars.AllclassSchemaGuids.Count == 0)
                {
                    ActiveDirectorySchema currentSchema = ActiveDirectorySchema.GetCurrentSchema();

                    // add a null guid
                    //Vars.AllclassSchemaGuids.Add("GuidNULL", System.Guid.Empty);

                    //SchemaClassType type = new SchemaClassType("classSchema");
                    foreach (ActiveDirectorySchemaClass schemaClass in currentSchema.FindAllClasses())
                    {
                        /*Console.WriteLine("Name = {0}", schemaClass.Name);
                        Console.WriteLine("GUID = {0}", schemaClass.SchemaGuid);*/
                        if (!Vars.AllAttributeSchemaGuids.TryGetValue(schemaClass.Name, out Guid value))
                        {
                            Vars.AllclassSchemaGuids.Add(schemaClass.Name, schemaClass.SchemaGuid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred while retriving ClassSchema ldapDisplayName & GUIDs: '{0}'", ex);
            }
            return Vars.AllclassSchemaGuids;
        }

        /// <summary>
        /// get all schema Attributes and store it into a hashtable
        /// in the form "name" = "GUID" (Key=Value)
        /// </summary>
        /// <returns>Hashtable</returns>
        public static Dictionary<string, Guid> GetAllAttributeSchemaGUID()
        {
            try
            {
                if (EguibarIT.Delegation.Vars.AllAttributeSchemaGuids == null || EguibarIT.Delegation.Vars.AllAttributeSchemaGuids.Count == 0)
                {
                    ActiveDirectorySchema currentSchema = ActiveDirectorySchema.GetCurrentSchema();

                    // add a null guid
                    //Vars.AllAttributeSchemaGuids.Add("GuidNULL", System.Guid.Empty);

                    foreach (ActiveDirectorySchemaProperty schemaProperty in currentSchema.FindAllProperties())
                    {
                        /*Console.WriteLine("Name = {0}", schemaProperty.Name);
                        Console.WriteLine("SchemaGuid = {0}", schemaProperty.SchemaGuid);*/
                        if (!Vars.AllAttributeSchemaGuids.TryGetValue(schemaProperty.Name, out Guid value))
                        {
                            Vars.AllAttributeSchemaGuids.Add(schemaProperty.Name, schemaProperty.SchemaGuid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred while retriving AttributeSchema ldapDisplayName & GUIDs: '{0}'", ex);
            }
            return Vars.AllAttributeSchemaGuids;
        }

        /// <summary>
        /// get all schema Extended-Rights and store it into a hashtable
        /// in the form "name" = "GUID" (Key=Value)
        /// </summary>
        /// <returns>Hashtable</returns>
        public static Dictionary<string, Guid> GetAllExtendedRightsGUID()
        {
            try
            {
                if (EguibarIT.Delegation.Vars.AllExtendedRightsGuids == null || EguibarIT.Delegation.Vars.AllExtendedRightsGuids.Count == 0)
                {
                    string ConfigurationNamingContext = string.Empty;

                    DirectoryEntry rootDSE = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", EguibarIT.Delegation.Other.Domains.GetAdFQDN()));

                    ConfigurationNamingContext = rootDSE.Properties["ConfigurationNamingContext"][0].ToString();

                    DirectoryEntry searchRoot = new DirectoryEntry("LDAP://" + ConfigurationNamingContext);

                    DirectorySearcher searcher = new DirectorySearcher(searchRoot)
                    {
                        SearchScope = SearchScope.Subtree
                    };
                    searcher.PropertiesToLoad.Add("displayName");
                    searcher.PropertiesToLoad.Add("rightsGuid");
                    searcher.Filter = string.Format("(&(objectclass=controlAccessRight)(rightsguid=*))");
                    searcher.PageSize = 3000;

                    // add a null guid
                    //Vars.AllExtendedRightsGuids.Add("GuidNULL", System.Guid.Empty);

                    foreach (SearchResult result in searcher.FindAll())
                    {
                        Guid guid = new Guid(result.Properties["rightsGuid"][0].ToString());

                        if (!Vars.AllExtendedRightsGuids.TryGetValue(result.Properties["displayName"][0].ToString(), out Guid value))
                        {
                            Vars.AllExtendedRightsGuids.Add(result.Properties["displayName"][0].ToString(), guid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred while retriving schema extended rights displayName & GUIDs: '{0}'", ex);
            }

            return Vars.AllExtendedRightsGuids;
        }

        // ---------- INVERSE hashtables ----------
        /// <summary>
        /// get all schema classess and store it into a hashtable
        /// in the form "GUID" = "name" (Value=Key)
        /// </summary>
        /// <returns>Dictionary</returns>
        public static Dictionary<Guid, string> GetInvAllClassSchemaGUID()
        {
            try
            {
                if (EguibarIT.Delegation.Vars.InvAllclassSchemaGuids == null || EguibarIT.Delegation.Vars.InvAllclassSchemaGuids.Count == 0)
                {
                    // add a null guid
                    //Vars.InvAllclassSchemaGuids.Add(System.Guid.Empty, "GuidNULL");

                    ActiveDirectorySchema currentSchema = ActiveDirectorySchema.GetCurrentSchema();

                    //SchemaClassType type = new SchemaClassType("classSchema");
                    foreach (ActiveDirectorySchemaClass schemaClass in currentSchema.FindAllClasses())
                    {
                        /*Console.WriteLine("Name = {0}", schemaClass.Name);
                        Console.WriteLine("GUID = {0}", schemaClass.SchemaGuid);*/
                        if (!Vars.InvAllAttributeSchemaGuids.TryGetValue(schemaClass.SchemaGuid, out string value))
                        {
                            Vars.InvAllclassSchemaGuids.Add(schemaClass.SchemaGuid, schemaClass.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred while retriving Inverse ClassSchema ldapDisplayName & GUIDs: '{0}'", ex);
            }
            return Vars.InvAllclassSchemaGuids;
        }

        // ---------- INVERSE hashtables ----------
        /// <summary>
        /// get all schema Attributes and store it into a hashtable
        /// in the form "GUID" = "name" (Value=Key)
        /// </summary>
        /// <returns>Dictionary</returns>
        public static Dictionary<Guid, string> GetInvAllAttributeSchemaGUID()
        {
            try
            {
                if (EguibarIT.Delegation.Vars.InvAllAttributeSchemaGuids == null || EguibarIT.Delegation.Vars.InvAllAttributeSchemaGuids.Count == 0)
                {
                    ActiveDirectorySchema currentSchema = ActiveDirectorySchema.GetCurrentSchema();

                    // add a null guid
                    //Vars.InvAllAttributeSchemaGuids.Add(System.Guid.Empty, "GuidNULL");

                    foreach (ActiveDirectorySchemaProperty schemaProperty in currentSchema.FindAllProperties())
                    {
                        /*Console.WriteLine("Name = {0}", schemaProperty.Name);
                        Console.WriteLine("SchemaGuid = {0}", schemaProperty.SchemaGuid);*/
                        if (!Vars.InvAllAttributeSchemaGuids.TryGetValue(schemaProperty.SchemaGuid, out string value))
                        {
                            Vars.InvAllAttributeSchemaGuids.Add(schemaProperty.SchemaGuid, schemaProperty.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred while retriving Inverse AttributeSchema ldapDisplayName & GUIDs: '{0}'", ex);
            }
            return Vars.InvAllAttributeSchemaGuids;
        }

        // ---------- INVERSE hashtables ----------
        /// <summary>
        /// get all schema Extended-Rights and store it into a hashtable
        /// in the form "name" = "GUID" (Key=Value)
        /// </summary>
        /// <returns>Dictionary</returns>
        public static Dictionary<Guid, string> GetInvAllExtendedRightsGUID()
        {
            try
            {
                if (EguibarIT.Delegation.Vars.InvAllExtendedRightsGuids == null || EguibarIT.Delegation.Vars.InvAllExtendedRightsGuids.Count == 0)
                {
                    string ConfigurationNamingContext = string.Empty;

                    DirectoryEntry rootDSE = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", EguibarIT.Delegation.Other.Domains.GetAdFQDN()));

                    ConfigurationNamingContext = rootDSE.Properties["ConfigurationNamingContext"][0].ToString();

                    DirectoryEntry searchRoot = new DirectoryEntry("LDAP://" + ConfigurationNamingContext);

                    DirectorySearcher searcher = new DirectorySearcher(searchRoot)
                    {
                        SearchScope = SearchScope.Subtree
                    };
                    searcher.PropertiesToLoad.Add("displayName");
                    searcher.PropertiesToLoad.Add("rightsGuid");
                    searcher.Filter = string.Format("(&(objectclass=controlAccessRight)(rightsguid=*))");
                    searcher.PageSize = 3000;

                    // add a null guid
                    //Vars.InvAllExtendedRightsGuids.Add(System.Guid.Empty, "GuidNULL");

                    foreach (SearchResult result in searcher.FindAll())
                    {
                        Guid guid = new Guid(result.Properties["rightsGuid"][0].ToString());

                        if (!Vars.InvAllExtendedRightsGuids.TryGetValue(guid, out string value))
                        {
                            Vars.InvAllExtendedRightsGuids.Add(guid, result.Properties["displayName"][0].ToString());
                        }
                    }//end foreach
                }//end if
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred while retriving schema extended inverse rights displayName & GUIDs: '{0}'", ex);
            }

            return Vars.InvAllExtendedRightsGuids;
        }

        /// <summary>
        /// Translate the name into the Schema GUID
        /// </summary>
        /// <param name="_name"></param>
        /// <returns>Schema GUID of the given LDAP display name</returns>
        public static Guid ConvertNameToSchemaGUID(string _name)
        {
            //get the ClassSchema hashtable
            //Dictionary<string, Guid> ClassSchemaHT = EguibarIT.Delegation.GUIDs.GetAllClassSchemaGUID();

            //get the AttributeSchema hashtable
            //Dictionary<string, Guid> AttributeSchemaHT = EguibarIT.Delegation.GUIDs.GetAllAttributeSchemaGUID();

            //get the Extended Rights hashtable
            //Dictionary<string, Guid> ExtendedRightsHT = EguibarIT.Delegation.GUIDs.GetAllExtendedRightsGUID();

            /*
            // ---------- Class Schema
Get-NameToGUID -Name "computer" # "bf967a86-0de6-11d0-a285-00aa003049e2"
Get-NameToGUID -Name "group"    # "bf967a9c-0de6-11d0-a285-00aa003049e2"
Get-NameToGUID -Name "subnet"   # "b7b13124-b82e-11d0-afee-0000f80367c1"
Get-NameToGUID -Name "user"     # "bf967aba-0de6-11d0-a285-00aa003049e2"
            */
            if (EguibarIT.Delegation.GUIDs.GetAllClassSchemaGUID().ContainsKey(_name))
            {
                // Write the value
                return ((Guid)EguibarIT.Delegation.GUIDs.GetAllClassSchemaGUID()[_name]);
            }

            /*
            // ---------- Attribute Schema
Get-NameToGUID -Name "AccountExpires" # "bf967915-0de6-11d0-a285-00aa003049e2"
Get-NameToGUID -Name "StreetAddress"  # "f0f8ff84-1191-11d0-a060-00aa006c33ed"
Get-NameToGUID -Name "Comment"        # "bf96793e-0de6-11d0-a285-00aa003049e2"
Get-NameToGUID -Name "Description"    # "bf967950-0de6-11d0-a285-00aa003049e2"
Get-NameToGUID -Name "Employee-ID"    # "bf967962-0de6-11d0-a285-00aa003049e2"
Get-NameToGUID -Name "Manager"        # "bf9679b5-0de6-11d0-a285-00aa003049e2"
Get-NameToGUID -Name "Picture"        # "8d3bca50-1d7e-11d0-a081-00aa006c33ed"
Get-NameToGUID -Name "SamAccountName" # "3e0abfd0-126a-11d0-a060-00aa006c33ed"
             */
            if (EguibarIT.Delegation.GUIDs.GetAllAttributeSchemaGUID().ContainsKey(_name))
            {
                // Write the value
                return ((Guid)EguibarIT.Delegation.GUIDs.GetAllAttributeSchemaGUID()[_name]);
            }

            /*
            // ---------- Extended Rights
Get-NameToGUID -Name "Allowed to Authenticate" # "68b1d179-0d15-4d4f-ab71-46152e79a7bc"
Get-NameToGUID -Name "Migrate SID History"     # "ba33815a-4f93-4c76-87f3-57574bff8109"
Get-NameToGUID -Name "Reset Password"          # "00299570-246d-11d0-a768-00aa006e0529"
Get-NameToGUID -Name "Change Password"         # "ab721a53-1e2f-11d0-9819-00aa0040529b"
Get-NameToGUID -Name "General Information"     # "59ba2f42-79a2-11d0-9020-00c04fc2d3cf"
Get-NameToGUID -Name "Logon Information"       # "5f202010-79a5-11d0-9020-00c04fc2d4cf"
Get-NameToGUID -Name "Personal Information"    # "77b5b886-944a-11d1-aebd-0000f80367c1"
Get-NameToGUID -Name "Account Restrictions"    # "4c164200-20c0-11d0-a768-00aa006e0529"
            */
            if (EguibarIT.Delegation.GUIDs.GetAllExtendedRightsGUID().ContainsKey(_name))
            {
                // Write the value
                return ((Guid)EguibarIT.Delegation.GUIDs.GetAllExtendedRightsGUID()[_name]);
            }

            if (_name.Equals("GuidNULL"))
            {
                return System.Guid.Empty;
            }

            return System.Guid.Empty;
        }

        /*
        /// <summary>
        ///
        /// </summary>
        public Guid GUID
        {
            get { return _guid; }
            set { _guid = value; }
        }
        private Guid _guid;
        */

        /// <summary>
        /// Translate the GUID into the Schema Name
        /// </summary>
        /// <param name="_guid"></param>
        /// <returns>STRING representing the LDAP Display name of the GUID</returns>
        public static string ConvertNameToSchemaGUID(Guid _guid)
        {
            //get the Inverse ClassSchema hashtable
            //Dictionary<Guid, string> InvClassSchemaHT = EguibarIT.Delegation.GUIDs.GetInvAllClassSchemaGUID();

            //get the Inverse AttributeSchema hashtable
            //Dictionary<Guid, string> InvAttributeSchemaHT = EguibarIT.Delegation.GUIDs.GetInvAllAttributeSchemaGUID();

            //get the Inverse Extended Rights hashtable
            //Dictionary<Guid, string> InvExtendedRightsHT = EguibarIT.Delegation.GUIDs.GetInvAllExtendedRightsGUID();

            /*
            // ---------- Class Schema
Get-GUIDtoName -Guid "bf967a86-0de6-11d0-a285-00aa003049e2" # computer (classSchema)
Get-GUIDtoName -Guid "bf967a9c-0de6-11d0-a285-00aa003049e2" # Group (classSchema)
Get-GUIDtoName -Guid "b7b13124-b82e-11d0-afee-0000f80367c1" # Subnet (classSchema)
Get-GUIDtoName -Guid "bf967aba-0de6-11d0-a285-00aa003049e2" # User (classSchema)
            */
            if (EguibarIT.Delegation.GUIDs.GetInvAllClassSchemaGUID().ContainsKey(_guid))
            {
                // Write the value
                return ((string)EguibarIT.Delegation.GUIDs.GetInvAllClassSchemaGUID()[_guid] + " [ClassSchema]");
            }

            /*
            // ---------- Attribute Schema
Get-GUIDtoName -Guid "bf967915-0de6-11d0-a285-00aa003049e2" # AccountExpires
Get-GUIDtoName -Guid "f0f8ff84-1191-11d0-a060-00aa006c33ed" # StreetAddress (attributeSchema)
Get-GUIDtoName -Guid "bf96793e-0de6-11d0-a285-00aa003049e2" # Comment
Get-GUIDtoName -Guid "bf967950-0de6-11d0-a285-00aa003049e2" # Description
Get-GUIDtoName -Guid "bf967962-0de6-11d0-a285-00aa003049e2" # Employee-ID
Get-GUIDtoName -Guid "bf9679b5-0de6-11d0-a285-00aa003049e2" # Manager
Get-GUIDtoName -Guid "8d3bca50-1d7e-11d0-a081-00aa006c33ed" # Picture
Get-GUIDtoName -Guid "3e0abfd0-126a-11d0-a060-00aa006c33ed" # SamAccountName
             */
            if (EguibarIT.Delegation.GUIDs.GetInvAllAttributeSchemaGUID().ContainsKey(_guid))
            {
                // Write the value
                return ((string)EguibarIT.Delegation.GUIDs.GetInvAllAttributeSchemaGUID()[_guid] + " [AttributeSchema]");
            }

            /*
            // ---------- Extended Rights
Get-GUIDtoName -Guid "68b1d179-0d15-4d4f-ab71-46152e79a7bc" # Allowed to Authenticate [Extended Right]
Get-GUIDtoName -Guid "ba33815a-4f93-4c76-87f3-57574bff8109" # Migrate SID History [Extended Right]
Get-GUIDtoName -Guid "00299570-246d-11d0-a768-00aa006e0529" # Reset Password [Extended Right]
Get-GUIDtoName -Guid "ab721a53-1e2f-11d0-9819-00aa0040529b" # Change Password [Extended Right]
Get-GUIDtoName -Guid "59ba2f42-79a2-11d0-9020-00c04fc2d3cf" # General Information [Extended Right]
Get-GUIDtoName -Guid "5f202010-79a5-11d0-9020-00c04fc2d4cf" # Logon Information [Property Set]
Get-GUIDtoName -Guid "77b5b886-944a-11d1-aebd-0000f80367c1" # Personal Information [Property Set]
Get-GUIDtoName -Guid "4c164200-20c0-11d0-a768-00aa006e0529" # Account Restrictions [Property Set]
            */

            if (EguibarIT.Delegation.GUIDs.GetInvAllExtendedRightsGUID().ContainsKey(_guid))
            {
                // Write the value
                return ((string)EguibarIT.Delegation.GUIDs.GetInvAllExtendedRightsGUID()[_guid] + " [Extended Rights]");
            }

            if (_guid.Equals(System.Guid.Empty))
            {
                return "GuidNULL";
            }

            return _guid.ToString();
        }
    } // END Class GUIDs
}