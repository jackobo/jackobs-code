﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GamesPortal.Service.Jackpot
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="JackPotInfo")]
	public partial class JackpotInfoDBDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertJackpot(Jackpot instance);
    partial void UpdateJackpot(Jackpot instance);
    partial void DeleteJackpot(Jackpot instance);
    partial void InsertJackpotGameType(JackpotGameType instance);
    partial void UpdateJackpotGameType(JackpotGameType instance);
    partial void DeleteJackpotGameType(JackpotGameType instance);
    #endregion
		
		public JackpotInfoDBDataContext() : 
				base(global::GamesPortal.Service.Properties.Settings.Default.JackPotInfoConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public JackpotInfoDBDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public JackpotInfoDBDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public JackpotInfoDBDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public JackpotInfoDBDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Jackpot> Jackpots
		{
			get
			{
				return this.GetTable<Jackpot>();
			}
		}
		
		public System.Data.Linq.Table<JackpotGameType> JackpotGameTypes
		{
			get
			{
				return this.GetTable<JackpotGameType>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="NJP.[_Jackpots]")]
	public partial class Jackpot : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private short _JP_ID;
		
		private string _JP_Name;
		
		private EntitySet<JackpotGameType> @__JackpotGameTypes;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnJP_IDChanging(short value);
    partial void OnJP_IDChanged();
    partial void OnJP_NameChanging(string value);
    partial void OnJP_NameChanged();
    #endregion
		
		public Jackpot()
		{
			this.@__JackpotGameTypes = new EntitySet<JackpotGameType>(new Action<JackpotGameType>(this.attach__JackpotGameTypes), new Action<JackpotGameType>(this.detach__JackpotGameTypes));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_JP_ID", DbType="SmallInt NOT NULL", IsPrimaryKey=true)]
		public short JP_ID
		{
			get
			{
				return this._JP_ID;
			}
			set
			{
				if ((this._JP_ID != value))
				{
					this.OnJP_IDChanging(value);
					this.SendPropertyChanging();
					this._JP_ID = value;
					this.SendPropertyChanged("JP_ID");
					this.OnJP_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_JP_Name", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string JP_Name
		{
			get
			{
				return this._JP_Name;
			}
			set
			{
				if ((this._JP_Name != value))
				{
					this.OnJP_NameChanging(value);
					this.SendPropertyChanging();
					this._JP_Name = value;
					this.SendPropertyChanged("JP_Name");
					this.OnJP_NameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Jackpot__JackpotGameType", Storage="__JackpotGameTypes", ThisKey="JP_ID", OtherKey="JGT_JP_ID")]
		public EntitySet<JackpotGameType> JackpotGameTypes
		{
			get
			{
				return this.@__JackpotGameTypes;
			}
			set
			{
				this.@__JackpotGameTypes.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach__JackpotGameTypes(JackpotGameType entity)
		{
			this.SendPropertyChanging();
			entity.Jackpot = this;
		}
		
		private void detach__JackpotGameTypes(JackpotGameType entity)
		{
			this.SendPropertyChanging();
			entity.Jackpot = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="NJP.[_JackpotGameTypes]")]
	public partial class JackpotGameType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private short _JGT_JP_ID;
		
		private int _JGT_GT_ID;
		
		private EntityRef<Jackpot> _Jackpot;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnJGT_JP_IDChanging(short value);
    partial void OnJGT_JP_IDChanged();
    partial void OnJGT_GT_IDChanging(int value);
    partial void OnJGT_GT_IDChanged();
    #endregion
		
		public JackpotGameType()
		{
			this._Jackpot = default(EntityRef<Jackpot>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_JGT_JP_ID", DbType="SmallInt NOT NULL", IsPrimaryKey=true)]
		public short JGT_JP_ID
		{
			get
			{
				return this._JGT_JP_ID;
			}
			set
			{
				if ((this._JGT_JP_ID != value))
				{
					if (this._Jackpot.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnJGT_JP_IDChanging(value);
					this.SendPropertyChanging();
					this._JGT_JP_ID = value;
					this.SendPropertyChanged("JGT_JP_ID");
					this.OnJGT_JP_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_JGT_GT_ID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int JGT_GT_ID
		{
			get
			{
				return this._JGT_GT_ID;
			}
			set
			{
				if ((this._JGT_GT_ID != value))
				{
					this.OnJGT_GT_IDChanging(value);
					this.SendPropertyChanging();
					this._JGT_GT_ID = value;
					this.SendPropertyChanged("JGT_GT_ID");
					this.OnJGT_GT_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Jackpot__JackpotGameType", Storage="_Jackpot", ThisKey="JGT_JP_ID", OtherKey="JP_ID", IsForeignKey=true)]
		public Jackpot Jackpot
		{
			get
			{
				return this._Jackpot.Entity;
			}
			set
			{
				Jackpot previousValue = this._Jackpot.Entity;
				if (((previousValue != value) 
							|| (this._Jackpot.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Jackpot.Entity = null;
						previousValue.JackpotGameTypes.Remove(this);
					}
					this._Jackpot.Entity = value;
					if ((value != null))
					{
						value.JackpotGameTypes.Add(this);
						this._JGT_JP_ID = value.JP_ID;
					}
					else
					{
						this._JGT_JP_ID = default(short);
					}
					this.SendPropertyChanged("Jackpot");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591