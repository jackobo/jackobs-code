﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tools.BuildMachineAdapterService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ExplicitForceGameSynchronizationRequest", Namespace="http://schemas.datacontract.org/2004/07/GamesPortal.Service")]
    [System.SerializableAttribute()]
    public partial class ExplicitForceGameSynchronizationRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int GameTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GamesFolderNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RepositoryNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VersionFolderField;
        
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
        public int GameType {
            get {
                return this.GameTypeField;
            }
            set {
                if ((this.GameTypeField.Equals(value) != true)) {
                    this.GameTypeField = value;
                    this.RaisePropertyChanged("GameType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GamesFolderName {
            get {
                return this.GamesFolderNameField;
            }
            set {
                if ((object.ReferenceEquals(this.GamesFolderNameField, value) != true)) {
                    this.GamesFolderNameField = value;
                    this.RaisePropertyChanged("GamesFolderName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RepositoryName {
            get {
                return this.RepositoryNameField;
            }
            set {
                if ((object.ReferenceEquals(this.RepositoryNameField, value) != true)) {
                    this.RepositoryNameField = value;
                    this.RaisePropertyChanged("RepositoryName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string VersionFolder {
            get {
                return this.VersionFolderField;
            }
            set {
                if ((object.ReferenceEquals(this.VersionFolderField, value) != true)) {
                    this.VersionFolderField = value;
                    this.RaisePropertyChanged("VersionFolder");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BuildMachineAdapterService.IGamesPortalToBuildMachineAdapter")]
    public interface IGamesPortalToBuildMachineAdapter {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGamesPortalToBuildMachineAdapter/ForceGameSynchronization", ReplyAction="http://tempuri.org/IGamesPortalToBuildMachineAdapter/ForceGameSynchronizationResp" +
            "onse")]
        void ForceGameSynchronization(Tools.BuildMachineAdapterService.ExplicitForceGameSynchronizationRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGamesPortalToBuildMachineAdapterChannel : Tools.BuildMachineAdapterService.IGamesPortalToBuildMachineAdapter, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GamesPortalToBuildMachineAdapterClient : System.ServiceModel.ClientBase<Tools.BuildMachineAdapterService.IGamesPortalToBuildMachineAdapter>, Tools.BuildMachineAdapterService.IGamesPortalToBuildMachineAdapter {
        
        public GamesPortalToBuildMachineAdapterClient() {
        }
        
        public GamesPortalToBuildMachineAdapterClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GamesPortalToBuildMachineAdapterClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GamesPortalToBuildMachineAdapterClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GamesPortalToBuildMachineAdapterClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void ForceGameSynchronization(Tools.BuildMachineAdapterService.ExplicitForceGameSynchronizationRequest request) {
            base.Channel.ForceGameSynchronization(request);
        }
    }
}
