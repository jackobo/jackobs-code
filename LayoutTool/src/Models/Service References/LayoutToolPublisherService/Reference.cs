﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LayoutTool.Models.LayoutToolPublisherService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PublishSkinRequest", Namespace="http://schemas.datacontract.org/2004/07/GamesPortal.Service")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(LayoutTool.Models.LayoutToolPublisherService.PublishSkinToProductionRequest))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(LayoutTool.Models.LayoutToolPublisherService.PublishSkinToQARequest))]
    public partial class PublishSkinRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int BrandIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ClientVersionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool HasWarningsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SkinContentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int SkinIdField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BrandId {
            get {
                return this.BrandIdField;
            }
            set {
                if ((this.BrandIdField.Equals(value) != true)) {
                    this.BrandIdField = value;
                    this.RaisePropertyChanged("BrandId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ClientVersion {
            get {
                return this.ClientVersionField;
            }
            set {
                if ((object.ReferenceEquals(this.ClientVersionField, value) != true)) {
                    this.ClientVersionField = value;
                    this.RaisePropertyChanged("ClientVersion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool HasWarnings {
            get {
                return this.HasWarningsField;
            }
            set {
                if ((this.HasWarningsField.Equals(value) != true)) {
                    this.HasWarningsField = value;
                    this.RaisePropertyChanged("HasWarnings");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SkinContent {
            get {
                return this.SkinContentField;
            }
            set {
                if ((object.ReferenceEquals(this.SkinContentField, value) != true)) {
                    this.SkinContentField = value;
                    this.RaisePropertyChanged("SkinContent");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SkinId {
            get {
                return this.SkinIdField;
            }
            set {
                if ((this.SkinIdField.Equals(value) != true)) {
                    this.SkinIdField = value;
                    this.RaisePropertyChanged("SkinId");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PublishSkinToProductionRequest", Namespace="http://schemas.datacontract.org/2004/07/GamesPortal.Service")]
    [System.SerializableAttribute()]
    public partial class PublishSkinToProductionRequest : LayoutTool.Models.LayoutToolPublisherService.PublishSkinRequest {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NavigationPlanContentField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NavigationPlanContent {
            get {
                return this.NavigationPlanContentField;
            }
            set {
                if ((object.ReferenceEquals(this.NavigationPlanContentField, value) != true)) {
                    this.NavigationPlanContentField = value;
                    this.RaisePropertyChanged("NavigationPlanContent");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PublishSkinToQARequest", Namespace="http://schemas.datacontract.org/2004/07/GamesPortal.Service")]
    [System.SerializableAttribute()]
    public partial class PublishSkinToQARequest : LayoutTool.Models.LayoutToolPublisherService.PublishSkinRequest {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EnvironmentField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Environment {
            get {
                return this.EnvironmentField;
            }
            set {
                if ((object.ReferenceEquals(this.EnvironmentField, value) != true)) {
                    this.EnvironmentField = value;
                    this.RaisePropertyChanged("Environment");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GetCurrentProductionNavigationPlanRequest", Namespace="http://schemas.datacontract.org/2004/07/GamesPortal.Service")]
    [System.SerializableAttribute()]
    public partial class GetCurrentProductionNavigationPlanRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int BrandIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ClientVersionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ClientVersionJobNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int SkinIdField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BrandId {
            get {
                return this.BrandIdField;
            }
            set {
                if ((this.BrandIdField.Equals(value) != true)) {
                    this.BrandIdField = value;
                    this.RaisePropertyChanged("BrandId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ClientVersion {
            get {
                return this.ClientVersionField;
            }
            set {
                if ((object.ReferenceEquals(this.ClientVersionField, value) != true)) {
                    this.ClientVersionField = value;
                    this.RaisePropertyChanged("ClientVersion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ClientVersionJobNumber {
            get {
                return this.ClientVersionJobNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.ClientVersionJobNumberField, value) != true)) {
                    this.ClientVersionJobNumberField = value;
                    this.RaisePropertyChanged("ClientVersionJobNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SkinId {
            get {
                return this.SkinIdField;
            }
            set {
                if ((this.SkinIdField.Equals(value) != true)) {
                    this.SkinIdField = value;
                    this.RaisePropertyChanged("SkinId");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GetCurrentProductionNavigationPlanResponse", Namespace="http://schemas.datacontract.org/2004/07/GamesPortal.Service")]
    [System.SerializableAttribute()]
    public partial class GetCurrentProductionNavigationPlanResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NavigationPlanContentField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NavigationPlanContent {
            get {
                return this.NavigationPlanContentField;
            }
            set {
                if ((object.ReferenceEquals(this.NavigationPlanContentField, value) != true)) {
                    this.NavigationPlanContentField = value;
                    this.RaisePropertyChanged("NavigationPlanContent");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LayoutToolPublisherService.ILayoutToolPublisher")]
    public interface ILayoutToolPublisher {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILayoutToolPublisher/PublishSkinForQA", ReplyAction="http://tempuri.org/ILayoutToolPublisher/PublishSkinForQAResponse")]
        void PublishSkinForQA(LayoutTool.Models.LayoutToolPublisherService.PublishSkinToQARequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILayoutToolPublisher/PublishSkinForProduction", ReplyAction="http://tempuri.org/ILayoutToolPublisher/PublishSkinForProductionResponse")]
        void PublishSkinForProduction(LayoutTool.Models.LayoutToolPublisherService.PublishSkinToProductionRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILayoutToolPublisher/GetCurrentProductionNavigationPlan", ReplyAction="http://tempuri.org/ILayoutToolPublisher/GetCurrentProductionNavigationPlanRespons" +
            "e")]
        LayoutTool.Models.LayoutToolPublisherService.GetCurrentProductionNavigationPlanResponse GetCurrentProductionNavigationPlan(LayoutTool.Models.LayoutToolPublisherService.GetCurrentProductionNavigationPlanRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILayoutToolPublisherChannel : LayoutTool.Models.LayoutToolPublisherService.ILayoutToolPublisher, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LayoutToolPublisherClient : System.ServiceModel.ClientBase<LayoutTool.Models.LayoutToolPublisherService.ILayoutToolPublisher>, LayoutTool.Models.LayoutToolPublisherService.ILayoutToolPublisher {
        
        public LayoutToolPublisherClient() {
        }
        
        public LayoutToolPublisherClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LayoutToolPublisherClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LayoutToolPublisherClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LayoutToolPublisherClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void PublishSkinForQA(LayoutTool.Models.LayoutToolPublisherService.PublishSkinToQARequest request) {
            base.Channel.PublishSkinForQA(request);
        }
        
        public void PublishSkinForProduction(LayoutTool.Models.LayoutToolPublisherService.PublishSkinToProductionRequest request) {
            base.Channel.PublishSkinForProduction(request);
        }
        
        public LayoutTool.Models.LayoutToolPublisherService.GetCurrentProductionNavigationPlanResponse GetCurrentProductionNavigationPlan(LayoutTool.Models.LayoutToolPublisherService.GetCurrentProductionNavigationPlanRequest request) {
            return base.Channel.GetCurrentProductionNavigationPlan(request);
        }
    }
}