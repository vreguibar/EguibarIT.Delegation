using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    ///     <para type="synopsis">Function to Read all Class GUID from Schema</para>
    ///     <para type="description">Function that reads all Class GUID from the Schema and returns a Hashtable object</para>
    ///         <example>
    ///             <para>This example shows how to use this CMDlet to get the Hashtable</para>
    ///             <code>Get-ClassSchemaHashTable</code>
    ///         </example>
    ///     <remarks>Get all object class and its corresponding GUID from Schema, returning a hashtable</remarks>
    /// </summary>
    /// <para type="link" uri="(http://EguibarIT.eu)">[Eguibar Information Technology S.L. web site]</para>
    [Cmdlet(VerbsCommon.Get, "ClassSchemaHashTable", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(ClassSchemaHashTable))]
    public class GetClassSchemaHashTable : PSCmdlet
    {
        #region Parameters definition

        /// <summary>
        ///     <para type="outputType">Hashtable.</para>
        ///     <para type="description">Hashtable containing object class and its corresponding GUID.</para>
        /// </summary>
        public Dictionary<string, Guid> ClassSchemaMap
        {
            get { return classschemamap; }
            set { classschemamap = value; }
        }

        private Dictionary<string, Guid> classschemamap;

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

            try
            {
                if (EguibarIT.Delegation.Vars.AllclassSchemaGuids == null || EguibarIT.Delegation.Vars.AllclassSchemaGuids.Count == 0)
                {
                    EguibarIT.Delegation.Vars.AllclassSchemaGuids = EguibarIT.Delegation.GUIDs.GetAllClassSchemaGUID();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred when accessing global variable AllclassSchemaGuids: '{0}'", ex);
            }
        }

        /// <returns>
        /// Returns the Hashtable
        /// </returns>
        protected override void ProcessRecord()
        {
            try
            {
                classschemamap = EguibarIT.Delegation.Vars.AllclassSchemaGuids;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred when returning AllclassSchemaGuids global variable: '{0}'", ex);
            }

            WriteObject(classschemamap);
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

    /// <summary>
    ///     <para type="outputType">Hashtable.</para>
    ///     <para type="description">Hashtable containing object class and its corresponding GUID.</para>
    /// </summary>
    public class ClassSchemaHashTable
    {
        /// <summary>
        ///     <para type="outputType">Hashtable.</para>
        ///     <para type="description">Hashtable containing object class and its corresponding GUID.</para>
        /// </summary>
        public Dictionary<string, Guid> ClassSchemaMap;
    }
}