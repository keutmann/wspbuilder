/* Program : TestWSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Keutmann.Demo.Dummy
{
    public class DummyWebControl : WebControl
    {
        // Dummy WebControl
        // For testing, to be included in SafeControls
    }

    public class DummyControl : Control
    {
        // Dummy Control
        // For testing, to be included in SafeControls
    }

    abstract class AbstractDummyControl : Control
    {
        // Abstract Dummy Control
        // For testing, not to be included in SafeControls
    }

    interface InterfaceDummy
    {
        // Interface dummy 
        // For testing, not to be included in SafeControls
    }

}
