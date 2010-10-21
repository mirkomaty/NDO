using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace ClassGenerator.SchemaAnalyzer
{
    class NamespaceWrapper
    {
        XmlQualifiedName[] qnames;
        XmlQualifiedName defaultNamespace;
        string targetNamespace;

        public string TargetNamespace
        {
            get { return targetNamespace; }
        }

        public XmlQualifiedName DefaultNamespace
        {
            get { return defaultNamespace; }
        }

        public XmlQualifiedName[] QualifiedNames
        {
            get { return qnames; }
        }


        public NamespaceWrapper(XmlQualifiedName[] qnames, string targetNamespace)
        {
            this.targetNamespace = targetNamespace;
            this.qnames = qnames;
            int i;
            for(i = 0; i < qnames.Length; i++)
            {
                XmlQualifiedName qn = qnames[i];
                if (qn.Name == string.Empty)
                    break;
            }

            if (i < qnames.Length)
            {
                this.defaultNamespace = qnames[i];

                string ns = qnames[i].Namespace;
                string prefix = ns[ns.LastIndexOf("/") + 1].ToString();
                if (PrefixExists(prefix))
                {
                    for (int ci = (int)'a'; ci <= (int)'z'; ci++)
                    {
                        char c = (char)ci;
                        prefix = c.ToString();
                        if (!PrefixExists(prefix))
                            break;
                    }
                }            
                qnames[i] = new XmlQualifiedName(prefix, ns);
            }
        }

        bool PrefixExists(string prefix)
        {
            foreach (XmlQualifiedName qn in this.qnames)
            {
                if (qn.Name == prefix)
                    return true;
            }
            return false;
        }

        string GetPrefix(string nameSpace)
        {
            foreach(XmlQualifiedName qn in this.qnames)
            {
                if (qn.Namespace == nameSpace)
                    return qn.Name;
            }
            return null;
        }

        public string GetXPath(XmlQualifiedName qn)
        {
            if (string.IsNullOrEmpty(qn.Namespace))
                return qn.Name;
            string prefix = GetPrefix(qn.Namespace);
            if (prefix == null)
                throw new Exception("Unknown namespace: " + qn.Namespace);
            return (prefix + ':' + qn.Name);
        }

        public string this[string prefix]
        {
            get
            {
                foreach (XmlQualifiedName qn in this.qnames)
                    if (qn.Name == prefix)
                        return qn.Namespace;
                return null;
            }
        }

        public static string GetPrefixFromQualifiedName(string qname)
        {
            int p = qname.IndexOf(':');
            if (p > 0)
                return (qname.Substring(0, p));
            return null;
        }

        public static string GetXpathFromQualifiedName(string qname)
        {
            int p = qname.IndexOf(':');
            if (p > 0)
                return (qname.Substring(p + 1));
            return qname;
        }

    }
}
