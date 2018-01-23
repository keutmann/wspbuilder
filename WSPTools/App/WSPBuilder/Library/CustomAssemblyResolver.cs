/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2008
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using System.Collections;
#endregion

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public class CustomAssemblyResolver : BaseAssemblyResolver
    {
        private IDictionary m_cache;

        public IDictionary Cache
        {
            get { return m_cache; }
            set { m_cache = value; }
        }

        public CustomAssemblyResolver()
        {
            m_cache = new Hashtable();
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            AssemblyDefinition asm = (AssemblyDefinition)Cache[name.FullName];

            if (asm == null)
            {
                asm = base.Resolve(name);
                Cache[name.FullName] = asm;

                AssemblyStore.Load(asm);
            }

            return asm;
        }

        public void Clear()
        {
            m_cache = new Hashtable();
        }

    }
}