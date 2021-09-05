using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DirectoryServices
{
    internal sealed class ActiveDirectoryRightsTranslator
    {
        internal static int AccessMaskFromRights(ActiveDirectoryRights adRights)
        {
            return (int)adRights;
        }



        internal static ActiveDirectoryRights RightsFromAccessMask(int accessMask)
        {
            return (ActiveDirectoryRights)accessMask;
        }
    }
}
