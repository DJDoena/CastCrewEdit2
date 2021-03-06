﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 
namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class IMDbToDVDProfilerCrewRoleTransformation
    {
        private CreditType[] creditTypeListField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("CreditType", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public CreditType[] CreditTypeList
        {
            get
            {
                return creditTypeListField;
            }
            set
            {
                creditTypeListField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CreditType
    {

        private string iMDbCreditTypeField;

        private string dVDProfilerCreditTypeField;

        private CreditSubtype[] creditSubtypeListField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IMDbCreditType
        {
            get
            {
                return iMDbCreditTypeField;
            }
            set
            {
                iMDbCreditTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DVDProfilerCreditType
        {
            get
            {
                return dVDProfilerCreditTypeField;
            }
            set
            {
                dVDProfilerCreditTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("CreditSubtype", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public CreditSubtype[] CreditSubtypeList
        {
            get
            {
                return creditSubtypeListField;
            }
            set
            {
                creditSubtypeListField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CreditSubtype
    {

        private IMDbCreditSubtype iMDbCreditSubtypeField;

        private string dVDProfilerCreditSubtypeField;

        private string dVDProfilerCustomRoleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IMDbCreditSubtype IMDbCreditSubtype
        {
            get
            {
                return iMDbCreditSubtypeField;
            }
            set
            {
                iMDbCreditSubtypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DVDProfilerCreditSubtype
        {
            get
            {
                return dVDProfilerCreditSubtypeField;
            }
            set
            {
                dVDProfilerCreditSubtypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DVDProfilerCustomRole
        {
            get
            {
                return dVDProfilerCustomRoleField;
            }
            set
            {
                dVDProfilerCustomRoleField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class IMDbCreditSubtype
    {

        private bool startsWithField;

        private bool startsWithFieldSpecified;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool StartsWith
        {
            get
            {
                return startsWithField;
            }
            set
            {
                startsWithField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StartsWithSpecified
        {
            get
            {
                return startsWithFieldSpecified;
            }
            set
            {
                startsWithFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }
    }
}