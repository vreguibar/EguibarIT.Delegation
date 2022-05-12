using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Sets the premission for a group to Replicate the directory</para>
    /// <para type="description">Configures the configuration container to delegate the permissions to a group so it can replicate the directory</para>
    /// <example>
    /// This example shows how to use this CMDlet to add permissions
    /// <code>Set-AdDirectoryReplication "SL_InfraRight"</code>
    ///
    /// This example shows how to use this CMDlet to add permissions using named parameters
    /// <code>Set-AdDirectoryReplication -Group "SL_InfraRight" </code>
    ///
    /// This example shows how to use this CMDlet to remove permissions using named parameters
    /// <code>Set-AdDirectoryReplication -Group "SL_InfraRight" -RemoveRule</code>
    /// </example>
    /// <remarks>Set the permissions on a container to create/delete Site objects</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdDirectoryReplication", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class AdDirectoryReplication : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="inputType">STRING representing the group SamAccountName.</para>
        /// <para type="description">Identity of the group getting the delegation.</para>
        /// </summary>
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Identity of the group getting the delegation."
            )]
        [ValidateNotNullOrEmpty]
        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        private string _group;

        /// <summary>
        /// <para type="inputType">SWITCH parameter (true or false). If present the value becomes TRUE, and the access rule will be removed</para>
        /// <para type="description">Switch indicator to remove the access rule</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = false,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Switch indicator. If present (TRUE), the access rule will be removed."
            )]
        public SwitchParameter RemoveRule
        {
            get { return _removerule; }
            set { _removerule = value; }
        }

        private bool _removerule;

        #endregion Parameters definition

        #region Begin()

        /// <summary>
        ///
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (this.MyInvocation.BoundParameters.ContainsKey("Verbose"))
            {
                WriteVerbose("|=> ************************************************************************ <=|");
                WriteVerbose(DateTime.Today.ToShortDateString());
                WriteVerbose(string.Format("  Starting: {0}", this.MyInvocation.MyCommand));

                string paramVerbose;
                paramVerbose = "Parameters:\n";
                paramVerbose += string.Format("{0,-12}{1,-30}{2,-30}\n", null, " Key", " Value");
                paramVerbose += string.Format("{0,-12}{1,-30}{2,-30}\n", null, "----------", "----------");

                // display PSBoundparameters formatted nicely for Verbose output
                // var is iDictionary
                var pb = this.MyInvocation.BoundParameters; // | Format - Table - AutoSize | Out - String).TrimEnd()

                foreach (var item in pb)
                {
                    paramVerbose += string.Format("{0,-12}{1,-30}{2,-30}\n", null, item.Key, item.Value);
                }
                WriteVerbose(string.Format("{0}\n", paramVerbose));
            }
        }

        #endregion Begin()

        #region Process()

        /// <summary>
        ///
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            // Get all Naming Contexts
            PropertyValueCollection AllContexts = EguibarIT.Delegation.Other.Domains.getAllNamingContexts();

            /*
                Iterate through each existing Naming Context
                and delegate:
                + Monitor Active Directory Replication
                + Replicating Directory Changes
                + Replicating Directory Changes All
                + Replicating Directory Changes In Filtered Set
                + Manage Replication Topology
                + Replication Synchronization
             */
            foreach (var item in AllContexts)
            {
                ////////////////////////////////////////////////
                // Monitor Active Directory Replication
                /*
                      ACENumber              : 
                            DistinguishedName: Current Naming Context
                      IdentityReference      : EguibarIT\SL_DirReplRight
                      ActiveDirectoryRightst : ExtendedRight
                      AccessControlType      : Allow
                      ObjectType             : Monitor Active Directory Replication[Extended Rights]
                              InheritanceType: None
                      InheritedObjectType    : GuidNULL
                      IsInherited            : False
                */
                if (
                   EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                       item.ToString(),
                       _group,
                       ActiveDirectoryRights.ExtendedRight,
                       AccessControlType.Allow,
                       EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Monitor Active Directory Replication"),
                       _removerule
               ))
                {
                    if (_removerule)
                    {
                        WriteVerbose(String.Format("\"Monitor Active Directory Replication\" was revoked from {0} naming context.", item.ToString()));
                    } 
                    else
                    {
                        WriteVerbose(String.Format("Delegate \"Monitor Active Directory Replication\" completed succesfully on {0} naming context.", item.ToString()));
                    }
                    
                } 
                else
                {
                    WriteWarning(String.Format("Something went wrong when delegating \"Monitor Active Directory Replicationon\" {0} naming context.", item.ToString()));
                }

                ////////////////////////////////////////////////
                // Replicating Directory Changes
                /*
                    ACENumber              : 
                            DistinguishedName: Current Naming Context
                      IdentityReference      : EguibarIT\SL_DirReplRight
                      ActiveDirectoryRightst : ExtendedRight
                      AccessControlType      : Allow
                      ObjectType             : Replicating Directory Changes[Extended Rights]
                              InheritanceType: None
                      InheritedObjectType    : GuidNULL
                      IsInherited            : False
                */
                if (
                   EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                       item.ToString(),
                       _group,
                       ActiveDirectoryRights.ExtendedRight,
                       AccessControlType.Allow,
                       EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes"),
                       _removerule
               ))
                {
                    WriteVerbose(String.Format("Delegate \"Replicating Directory Changes\" completed succesfully on {0} naming context.", item.ToString()));
                }
                else
                {
                    WriteWarning(String.Format("Something went wrong when delegating \"Replicating Directory Changes\" {0} naming context.", item.ToString()));
                }

                ////////////////////////////////////////////////
                // Replicating Directory Changes All
                /*
                      ACENumber              : 
                            DistinguishedName: Current Naming Context
                      IdentityReference      : EguibarIT\SL_DirReplRight
                      ActiveDirectoryRightst : ExtendedRight
                      AccessControlType      : Allow
                      ObjectType             : Replicating Directory Changes All[Extended Rights]
                              InheritanceType: None
                      InheritedObjectType    : GuidNULL
                      IsInherited            : False
                */
                if (
                   EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                       item.ToString(),
                       _group,
                       ActiveDirectoryRights.ExtendedRight,
                       AccessControlType.Allow,
                       EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes All"),
                       _removerule
               ))
                {
                    WriteVerbose(String.Format("Delegate \"Replicating Directory Changes All\" completed succesfully on {0} naming context.", item.ToString()));
                }
                else
                {
                    WriteWarning(String.Format("Something went wrong when delegating \"Replicating Directory Changes All\" {0} naming context.", item.ToString()));
                }

                ////////////////////////////////////////////////
                // Replicating Directory Changes In Filtered Set
                /*
                      ACENumber              : 
                            DistinguishedName: Current Naming Context
                      IdentityReference      : EguibarIT\SL_DirReplRight
                      ActiveDirectoryRightst : ExtendedRight
                      AccessControlType      : Allow
                      ObjectType             : Replicating Directory Changes In Filtered Set[Extended Rights]
                              InheritanceType: None
                      InheritedObjectType    : GuidNULL
                      IsInherited            : False
                */
                if (
                   EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                       item.ToString(),
                       _group,
                       ActiveDirectoryRights.ExtendedRight,
                       AccessControlType.Allow,
                       EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replicating Directory Changes In Filtered Set"),
                       _removerule
               ))
                {
                    WriteVerbose(String.Format("Delegate \"Replicating Directory Changes In Filtered Set\" completed succesfully on {0} naming context.", item.ToString()));
                }
                else
                {
                    WriteWarning(String.Format("Something went wrong when delegating \"Replicating Directory Changes In Filtered Set\" {0} naming context.", item.ToString()));
                }

                ////////////////////////////////////////////////
                // Manage Replication Topology
                /*
                      ACENumber              : 
                            DistinguishedName: Current Naming Context
                      IdentityReference      : EguibarIT\SL_DirReplRight
                      ActiveDirectoryRightst : ExtendedRight
                      AccessControlType      : Allow
                      ObjectType             : Manage Replication Topology[Extended Rights]
                              InheritanceType: None
                      InheritedObjectType    : GuidNULL
                      IsInherited            : False
                */
                if (
                   EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                       item.ToString(),
                       _group,
                       ActiveDirectoryRights.ExtendedRight,
                       AccessControlType.Allow,
                       EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Manage Replication Topology"),
                       _removerule
               ))
                {
                    WriteVerbose(String.Format("Delegate \"Manage Replication Topology\" completed succesfully on {0} naming context.", item.ToString()));
                }
                else
                {
                    WriteWarning(String.Format("Something went wrong when delegating \"Manage Replication Topology\" {0} naming context.", item.ToString()));
                }

                ////////////////////////////////////////////////
                // Replication Synchronization
                /*
                      ACENumber              : 
                            DistinguishedName: Current Naming Context
                      IdentityReference      : EguibarIT\SL_DirReplRight
                      ActiveDirectoryRightst : ExtendedRight
                      AccessControlType      : Allow
                      ObjectType             : Replication Synchronization[Extended Rights]
                              InheritanceType: All
                      InheritedObjectType    : GuidNULL
                      IsInherited            : False
                */
                if (
                   EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                       item.ToString(),
                       _group,
                       ActiveDirectoryRights.ExtendedRight,
                       AccessControlType.Allow,
                       EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("Replication Synchronization"),
                       _removerule
               ))
                {
                    WriteVerbose(String.Format("Delegate \"Replication Synchronization\" on {0} naming context.", item.ToString()));
                }
                else
                {
                    WriteWarning(String.Format("Something went wrong when delegating \"Replication Synchronization\" {0} naming context.", item.ToString()));
                }

            } //end Foreach

            // Get Forest Partition Container
            SearchResultCollection partitions = EguibarIT.Delegation.Other.Domains.GetPartitionsReplicaLocations();

            string CurrentConfigurationNamingContext = EguibarIT.Delegation.Other.Domains.GetConfigurationNamingContext();

            //Iterate through all partitions found
            foreach (SearchResult part in partitions)
            {
                // if Replica-Locations is populated
                if (part.Properties["msDS-NC-Replica-Locations"].Count > 0)
                {
                    var NameProperty = part.Properties["name"];
                    string GuidNameString = NameProperty[0].ToString();

                    ////////////////////////////////////////////////
                    // Partition Replica Locations
                    /*
                          ACENumber              : 1
                                DistinguishedName: CN=2bdcf0cf-6755-4d80-a0a6-2977da70d57f,CN=Partitions,CN=Configuration,DC=EguibarIT,DC=local
                          IdentityReference      : EguibarIT\SL_DirReplRight
                          ActiveDirectoryRightst : ReadProperty
                          AccessControlType      : Allow
                          ObjectType             : msDS-NC-Replica-Locations[Extended Rights]
                                  InheritanceType: None
                          InheritedObjectType    : GuidNULL
                          IsInherited            : False
                    */
                    if (
                       EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                           String.Format("CN={0},CN=Partitions,{1}", GuidNameString, CurrentConfigurationNamingContext),
                           _group,
                           ActiveDirectoryRights.ReadProperty,
                           AccessControlType.Allow,
                           EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("msDS-NC-Replica-Locations"),
                           _removerule
                   ))
                    {
                        WriteVerbose(String.Format("Delegate \"Read Property msDS-NC-Replica-Locations\" completed succesfully on {0} naming context.", String.Format("CN={0},CN=Partitions,{1}", GuidNameString, CurrentConfigurationNamingContext)));
                    }
                    else
                    {
                        WriteWarning(String.Format("Something went wrong when delegating \"Read Property msDS-NC-Replica-Locations\" {0} naming context.", String.Format("CN={0},CN=Partitions,{1}", GuidNameString, CurrentConfigurationNamingContext)));
                    }

                    /*
                          ACENumber              : 1
                                DistinguishedName: CN=2bdcf0cf-6755-4d80-a0a6-2977da70d57f,CN=Partitions,CN=Configuration,DC=EguibarIT,DC=local
                          IdentityReference      : EguibarIT\SL_DirReplRight
                          ActiveDirectoryRightst : ReadProperty
                          AccessControlType      : Allow
                          ObjectType             : msDS-NC-Replica-Locations[Extended Rights]
                                  InheritanceType: None
                          InheritedObjectType    : GuidNULL
                          IsInherited            : False
                    */
                    if (
                       EguibarIT.Delegation.Constructors.AclCon4.AclConstructor4(
                           String.Format("CN={0},CN=Partitions,{1}", GuidNameString, CurrentConfigurationNamingContext),
                           _group,
                           ActiveDirectoryRights.WriteProperty,
                           AccessControlType.Allow,
                           EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID("msDS-NC-Replica-Locations"),
                           _removerule
                   ))
                    {
                        WriteVerbose(String.Format("Delegate \"Write Property msDS-NC-Replica-Locations\" completed succesfully on {0} naming context.", String.Format("CN={0},CN=Partitions,{1}", GuidNameString, CurrentConfigurationNamingContext)));
                    }
                    else
                    {
                        WriteWarning(String.Format("Something went wrong when delegating \"Write Property msDS-NC-Replica-Locations\" {0} naming context.", String.Format("CN={0},CN=Partitions,{1}", GuidNameString, CurrentConfigurationNamingContext)));
                    }
                }
            }

        }

        #endregion Process()

        #region End()

        /// <summary>
        ///
        /// </summary>
        protected override void EndProcessing()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("Verbose"))
            {
                string paramVerboseEnd;
                paramVerboseEnd = string.Format("\n         Function {0} finished.\n", this.MyInvocation.MyCommand);
                paramVerboseEnd += "-------------------------------------------------------------------------------\n\n";

                WriteVerbose(paramVerboseEnd);
            }
        }

        #endregion End()

        /// <summary>
        ///
        /// </summary>
        protected override void StopProcessing()
        {
        }
    } //END class AdDirectoryReplication
} // END namespace EguibarIT.Delegation.DirectoryReplication