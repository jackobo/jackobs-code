﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GGPMockBootstrapper.InstallationProgressFeedbackService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BeginInstallationRequest", Namespace="http://schemas.datacontract.org/2004/07/GGPGameServer.Installer.Models")]
    [System.SerializableAttribute()]
    public partial class BeginInstallationRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int ActionsCountField;
        
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
        public int ActionsCount {
            get {
                return this.ActionsCountField;
            }
            set {
                if ((this.ActionsCountField.Equals(value) != true)) {
                    this.ActionsCountField = value;
                    this.RaisePropertyChanged("ActionsCount");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="BeginExecuteActionRequest", Namespace="http://schemas.datacontract.org/2004/07/GGPGameServer.Installer.Models")]
    [System.SerializableAttribute()]
    public partial class BeginExecuteActionRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int ActionsCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CurrentActionDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CurrentActionIndexField;
        
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
        public int ActionsCount {
            get {
                return this.ActionsCountField;
            }
            set {
                if ((this.ActionsCountField.Equals(value) != true)) {
                    this.ActionsCountField = value;
                    this.RaisePropertyChanged("ActionsCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CurrentActionDescription {
            get {
                return this.CurrentActionDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.CurrentActionDescriptionField, value) != true)) {
                    this.CurrentActionDescriptionField = value;
                    this.RaisePropertyChanged("CurrentActionDescription");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CurrentActionIndex {
            get {
                return this.CurrentActionIndexField;
            }
            set {
                if ((this.CurrentActionIndexField.Equals(value) != true)) {
                    this.CurrentActionIndexField = value;
                    this.RaisePropertyChanged("CurrentActionIndex");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="EndExecuteActionRequest", Namespace="http://schemas.datacontract.org/2004/07/GGPGameServer.Installer.Models")]
    [System.SerializableAttribute()]
    public partial class EndExecuteActionRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int ActionsCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CurrentActionDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CurrentActionIndexField;
        
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
        public int ActionsCount {
            get {
                return this.ActionsCountField;
            }
            set {
                if ((this.ActionsCountField.Equals(value) != true)) {
                    this.ActionsCountField = value;
                    this.RaisePropertyChanged("ActionsCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CurrentActionDescription {
            get {
                return this.CurrentActionDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.CurrentActionDescriptionField, value) != true)) {
                    this.CurrentActionDescriptionField = value;
                    this.RaisePropertyChanged("CurrentActionDescription");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CurrentActionIndex {
            get {
                return this.CurrentActionIndexField;
            }
            set {
                if ((this.CurrentActionIndexField.Equals(value) != true)) {
                    this.CurrentActionIndexField = value;
                    this.RaisePropertyChanged("CurrentActionIndex");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="EndInstallationRequest", Namespace="http://schemas.datacontract.org/2004/07/GGPGameServer.Installer.Models")]
    [System.SerializableAttribute()]
    public partial class EndInstallationRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool SuccessField;
        
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
        public bool Success {
            get {
                return this.SuccessField;
            }
            set {
                if ((this.SuccessField.Equals(value) != true)) {
                    this.SuccessField = value;
                    this.RaisePropertyChanged("Success");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="InstallationProgressFeedbackService.IInstallationProgressFeedbackService", CallbackContract=typeof(GGPMockBootstrapper.InstallationProgressFeedbackService.IInstallationProgressFeedbackServiceCallback))]
    public interface IInstallationProgressFeedbackService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstallationProgressFeedbackService/Subscribe", ReplyAction="http://tempuri.org/IInstallationProgressFeedbackService/SubscribeResponse")]
        void Subscribe();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IInstallationProgressFeedbackServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IInstallationProgressFeedbackService/BeginInstallation")]
        void BeginInstallation(GGPMockBootstrapper.InstallationProgressFeedbackService.BeginInstallationRequest request);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IInstallationProgressFeedbackService/BeginExecuteAction")]
        void BeginExecuteAction(GGPMockBootstrapper.InstallationProgressFeedbackService.BeginExecuteActionRequest request);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IInstallationProgressFeedbackService/EndExecuteAction")]
        void EndExecuteAction(GGPMockBootstrapper.InstallationProgressFeedbackService.EndExecuteActionRequest request);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IInstallationProgressFeedbackService/EndInstallation")]
        void EndInstallation(GGPMockBootstrapper.InstallationProgressFeedbackService.EndInstallationRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IInstallationProgressFeedbackServiceChannel : GGPMockBootstrapper.InstallationProgressFeedbackService.IInstallationProgressFeedbackService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class InstallationProgressFeedbackServiceClient : System.ServiceModel.DuplexClientBase<GGPMockBootstrapper.InstallationProgressFeedbackService.IInstallationProgressFeedbackService>, GGPMockBootstrapper.InstallationProgressFeedbackService.IInstallationProgressFeedbackService {
        
        public InstallationProgressFeedbackServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public InstallationProgressFeedbackServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public InstallationProgressFeedbackServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public InstallationProgressFeedbackServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public InstallationProgressFeedbackServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Subscribe() {
            base.Channel.Subscribe();
        }
    }
}
