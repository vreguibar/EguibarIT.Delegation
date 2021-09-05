using System;
using System.Collections.Generic;

namespace EguibarIT.Delegation
{
    /// <summary>
    ///
    /// </summary>
    public static class Vars
    {
        // ---------- hashtables ----------
        /// <summary>
        /// Global Variable as Hashtable containing all classSchema Name=GUID
        /// </summary>
        public static Dictionary<string, Guid> AllclassSchemaGuids = new Dictionary<string, Guid>();

        /// <summary>
        /// Global Variable as Hashtable containing all AttributeSchema Name=GUID
        /// </summary>
        public static Dictionary<string, Guid> AllAttributeSchemaGuids = new Dictionary<string, Guid>();

        /// <summary>
        /// Global Variable as Hashtable containing all Extended Rights Name=GUID
        /// </summary>
        public static Dictionary<string, Guid> AllExtendedRightsGuids = new Dictionary<string, Guid>();

        // ---------- END ----------

        // ---------- INVERSE hashtables ----------
        /// <summary>
        /// Global Variable as Hashtable containing all classSchema GUID=Name
        /// </summary>
        public static Dictionary<Guid, string> InvAllclassSchemaGuids = new Dictionary<Guid, string>();

        /// <summary>
        /// Global Variable as Hashtable containing all AttributeSchema GUID=Name
        /// </summary>
        public static Dictionary<Guid, string> InvAllAttributeSchemaGuids = new Dictionary<Guid, string>();

        /// <summary>
        /// Global Variable as Hashtable containing all Extended Rights GUID=Name
        /// </summary>
        public static Dictionary<Guid, string> InvAllExtendedRightsGuids = new Dictionary<Guid, string>();

        // ---------- END INVERSE ----------

        /// <summary>
        ///
        /// </summary>
        public static bool IsClassSchema = new bool();

        /// <summary>
        ///
        /// </summary>
        public static bool IsAttributeSchema = new bool();

        /// <summary>
        ///
        /// </summary>
        public static bool IsExtendedRights = new bool();

        /// <summary>
        ///
        /// </summary>
        public static bool IsInverse = new bool();
    }

    /// <summary>
    /// Access Control Entry (ACE) class
    /// Used to return ACE from AD
    /// </summary>
    public class ACEarray
    {
        /// <summary>
        ///
        /// </summary>
        public int ACENumber { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string DistinguishedName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string IdentityReference { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ActiveDirectoryRightst { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string AccessControlType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string InheritanceType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string InheritedObjectType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string IsInherited { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ACEarray() { }

        /// <summary>
        /// Access Control Entry Array for AclAccessRule CMDlet
        /// </summary>
        /// <param name="number"></param>
        /// <param name="distinguishedName"></param>
        /// <param name="identityReference"></param>
        /// <param name="activeDirectoryRightst"></param>
        /// <param name="accessControlType"></param>
        /// <param name="objectType"></param>
        /// <param name="inheritanceType"></param>
        /// <param name="inheritedObjectType"></param>
        /// <param name="isInherited"></param>
        public ACEarray(int number,
                        string distinguishedName,
                        string identityReference,
                        string activeDirectoryRightst,
                        string accessControlType,
                        string objectType,
                        string inheritanceType,
                        string inheritedObjectType,
                        string isInherited)
        {
            ACENumber = number;
            DistinguishedName = distinguishedName;
            IdentityReference = identityReference;
            ActiveDirectoryRightst = activeDirectoryRightst;
            AccessControlType = accessControlType;
            ObjectType = objectType;
            InheritanceType = inheritanceType;
            InheritedObjectType = inheritedObjectType;
            IsInherited = isInherited;
        }
    }
}