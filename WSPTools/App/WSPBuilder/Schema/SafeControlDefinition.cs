using System;
using System.Collections.Generic;
using System.Text;

public partial class SafeControlDefinition
{

    public static bool IsMatch(SafeControlDefinition source, SafeControlDefinition target)
    {
        bool result =
            source.Assembly == target.Assembly &&
            source.Namespace == target.Namespace &&
            source.TypeName == target.TypeName;

        return result;
    }

}
