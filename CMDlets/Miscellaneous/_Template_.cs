using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    ///     <para type="synopsis"></para>
    ///     <para type="description"></para>
    ///     <example>
    ///         <code></code>
    ///     </example>
    ///     <remarks></remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Set, "AdAclXxxxx", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(int))]
    public class Xxxxx : Cmdlet
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
        /// <para type="inputType">STRING representing the Distinguished Name of the object (ej. OU=Users,OU=Good,OU=Sites,DC=EguibarIT,DC=local).</para>
        /// <para type="description">Distinguished Name of the object (or container) where the permissions are going to be configured.</para>
        /// </summary>
        [Parameter(
               Position = 1,
               Mandatory = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "Distinguished Name of the object (or container) where the permissions are going to be configured."
            )]
        [ValidateNotNullOrEmpty]
        public string LDAPpath
        {
            get { return _ldappath; }
            set { _ldappath = value; }
        }
        private string _ldappath;


        /// <summary>
        /// <para type="inputType">SWITCH parameter (true or false). If present the value becomes TRUE, and the access rule will be removed</para>
        /// <para type="description">Switch indicator to remove the access rule</para>
        /// </summary>
        [Parameter(
               Position = 2,
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


        #endregion

        #region Begin()
        /// <summary>
        /// 
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }
        #endregion Begin()

        #region Process()
        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            /*
             
             */

        }
        #endregion Process()

        #region End()
        /// <summary>
        /// 
        /// </summary>
        protected override void EndProcessing()
        {

        }
        #endregion End()

        /// <summary>
        /// 
        /// </summary>
        protected override void StopProcessing()
        {

        }

    } //END class Xxxxx
} // END namespace EguibarIT.Delegation
