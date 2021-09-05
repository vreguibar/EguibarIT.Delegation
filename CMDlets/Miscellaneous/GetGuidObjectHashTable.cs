using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


namespace EguibarIT.Delegation.CMDlets
{
    /// <summary>
    /// <para type="synopsis">Function to Read all GUID from Schema</para>
    /// <para type="description">Function that reads all objects GUID from the </para>
    /// <para type="description">Schema and returnsa Hashtable object.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "GuidObjectHashTable", ConfirmImpact = ConfirmImpact.Medium)]
    [OutputType(typeof(GuidObjectHashTable))]
    public class GetGuidObjectHashTable : Cmdlet
    {
        public Hashtable GUIDmap
        {
            get { return guidmap; }
            set { guidmap = value; }
        }
        private Hashtable guidmap;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            try
            {
                if (EguibarIT.Delegation.Other.Vars.AllclassSchemaGuids == null || EguibarIT.Delegation.Other.Vars.AllclassSchemaGuids.Count == 0)
                {
                    EguibarIT.Delegation.Other.Vars.AllclassSchemaGuids = EguibarIT.Delegation.Other.GetGuidObjects.GetGuidSchemaObjects();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred when accessing global variable AllSchemaGUIDs: '{0}'", ex);
            }
        }

        protected override void ProcessRecord()
        {
            try
            {
                guidmap = EguibarIT.Delegation.Other.Vars.AllclassSchemaGuids;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: '{0}'", ex.Message);
                throw new ApplicationException("An error occurred when returning AllSchemaGUIDs global variable: '{0}'", ex);
            }

            WriteObject(guidmap);
        }

        protected override void EndProcessing()
        {

        }

        protected override void StopProcessing()
        {

        }

    }

    public class GuidObjectHashTable
    {
        public Hashtable guidmap;
    }
}
