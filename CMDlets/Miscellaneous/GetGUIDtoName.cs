using System;
using System.Management.Automation;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Helper function to translate GUID to display name</para>
    /// <para type="description">This function translates a GUID into a human readible Display Name</para>
    /// <example>
    /// This example shows how to use this CMDlet to get the name of the given GUID
    /// <code>Get-GUIDtoName -GUID "bf967a86-0de6-11d0-a285-00aa003049e2"</code>
    /// </example>
    /// <remarks>Get LDAP display name from GUID of an object.</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Get, "GUIDtoName", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(string))]
    public class GetGUIDtoName : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        /// <para type="outputType">GUID.</para>
        /// <para type="description">GUID of the object to translate to its display name.</para>
        /// </summary>
        [Parameter(
               Position = 0,
               Mandatory = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true,
               HelpMessage = "The GUID to translate."
            )]
        public Guid GUID
        {
            get { return _guid; }
            set { _guid = value; }
        }

        private Guid _guid;

        #endregion Parameters definition

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

        /// <returns>
        /// Returns the name of the object
        /// </returns>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            WriteObject(EguibarIT.Delegation.GUIDs.ConvertNameToSchemaGUID(_guid));
        }

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

        /// <summary>
        ///
        /// </summary>
        protected override void StopProcessing()
        {
        }
    }
}